// Pretty much all of this backend is from the randomizer, so credit goes to the devs there.

#include <constants.h>
#include <macros.h>
#include <interception.h>
#include <il2cpp_helpers.h>
#include <interception_macros.h>
#include <common.h>

#include <windows.h>
#include <string>
#include <fstream>
#include <mutex>

#include <Common/csv.h>
#include <Common/ext.h>
#include <Common/settings.h>

namespace modloader
{
    // Have this here so it is included in the assembly and can be used to examine thrown exceptions.
    Il2CppExceptionWrapper ex;

    std::string base_path = "C:\\moon\\";
    std::string modloader_path = "modloader_config.json";
    std::string csv_path = "inject_log.csv";
    bool shutdown_thread = false;

    namespace
    {
        bool write_to_csv = true;
        bool flush_after_every_line = true;
        std::ofstream csv_file;
        std::mutex csv_mutex;

        void initialize_trace_file()
        {
            if (!write_to_csv)
                return;

            csv_file.open(base_path + csv_path);
            write_to_csv = csv_file.is_open();

            if (write_to_csv)
            {
                csv_mutex.lock();
                if (flush_after_every_line)
                    csv_file << "type, group, level, message," << std::endl;
                else
                    csv_file << "type, group, level, message,\n";
                csv_mutex.unlock();
            }
        }

        Initialization* start = nullptr;
        void initialization_callbacks()
        {
            auto it = start;
            while (it != nullptr)
            {
                it->call();
                it = it->next;
            }
        }
    }

    void write_trace(MessageType type, int level, std::string const& group, std::string const& message)
    {
        if (!write_to_csv)
            return;

        std::string sanitized_group = csv::sanitize_csv_field(group);
        std::string sanitized_message = csv::sanitize_csv_field(message);

        std::string line = format(
            "%d, [%s], %d, %s,",
            type,
            sanitized_group.c_str(),
            level,
            sanitized_message.c_str()
        );

        csv_mutex.lock();
        if (flush_after_every_line)
            csv_file << line << std::endl;
        else
            csv_file << line << "\n";
        csv_mutex.unlock();
    }

    IL2CPP_MODLOADER_DLLEXPORT void trace(MessageType type, int level, std::string const& group, std::string const& message)
    {
        write_trace(type, level, group, message);
    }

    Initialization::Initialization(Initialization::init p_call)
        : next(start)
        , call(p_call)
    {
        start = this;
    }

    extern bool bootstrap();
    extern void bootstrap_shutdown();

    std::vector<shutdown_handler> shutdown_handlers;

    STATIC_IL2CPP_BINDING(UnityEngine, Application, app::String*, get_version, ());
    STATIC_IL2CPP_BINDING(UnityEngine, Application, app::String*, get_unityVersion, ());
    STATIC_IL2CPP_BINDING(UnityEngine, Application, app::String*, get_productName, ());

    IL2CPP_MODLOADER_C_DLLEXPORT void injection_entry(std::string path)
    {
        base_path = path;
        trace(MessageType::Info, 5, "initialize", "Loading settings.");
        auto settings = create_editor_settings(base_path);
        load_settings_from_file(settings);
        //auto wait_for_debugger = check_option(settings, "Flags", "WaitForDebugger", false);
        //while (wait_for_debugger && !::IsDebuggerPresent())
        //    ::Sleep(100); // to avoid 100% CPU load

        initialize_trace_file();
        trace(MessageType::Info, 5, "initialize", "Mod Loader initialization.");

        trace(MessageType::Info, 5, "initialize", "Loading mods.");
        if (!bootstrap())
        {
            trace(MessageType::Info, 5, "initialize", "Failed to bootstrap, shutting down.");
            csv_file.close();
            shutdown_thread = true;
            FreeLibraryAndExitThread(GetModuleHandleA("Il2CppModLoader.dll"), 0);
        }

        trace(MessageType::Info, 5, "initialize", "Performing intercepts.");
        intercept::interception_init();

        auto product = il2cpp::convert_csstring(Application::get_productName());
        auto version = il2cpp::convert_csstring(Application::get_version());
        auto unity_version = il2cpp::convert_csstring(Application::get_unityVersion());
        trace(MessageType::Info, 5, "initialize", format(
            "Application %s injected (%s)[%s].", product.c_str(), version.c_str(), unity_version.c_str()));

        while (!shutdown_thread)
        {
            //console::console_poll();
        }

        for (auto handler : shutdown_handlers)
            handler();

        //console::console_free();

        if (write_to_csv)
            csv_file.close();

        intercept::interception_detach();
        bootstrap_shutdown();
        FreeLibraryAndExitThread(GetModuleHandleA("Il2CppModLoader.dll"), 0);
    }

    IL2CPP_MODLOADER_DLLEXPORT void shutdown()
    {
        shutdown_thread = true;
    }

    bool initialized = false;
    IL2CPP_INTERCEPT(, GameController, void, FixedUpdate, (app::GameController* this_ptr)) {
        if (!initialized) {
            trace(MessageType::Info, 5, "initialize", "Calling initialization callbacks.");
            initialization_callbacks();
            initialized = true;
        }

        GameController::FixedUpdate(this_ptr);
    }
}