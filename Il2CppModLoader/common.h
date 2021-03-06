#pragma once

#include <string>

#include <Il2CppModLoader/constants.h>
#include <Il2CppModLoader/macros.h>

namespace modloader
{
	using shutdown_handler = void(*)();

    struct IL2CPP_MODLOADER_DLLEXPORT Initialization
    {
        using init = void (*)();
        Initialization(init p_call);

        init call;
        Initialization* next;
    };

	IL2CPP_MODLOADER_DLLEXPORT void trace(MessageType type, int level, std::string const& group, std::string const& message);
    IL2CPP_MODLOADER_DLLEXPORT void shutdown();
}

#define CALL_ON_INIT(func) modloader::Initialization func##_init_struct(&func)