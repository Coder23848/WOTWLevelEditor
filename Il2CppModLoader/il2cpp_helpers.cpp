#include <il2cpp_internals/il2cpp_internals.h>

#include <il2cpp_helpers.h>
#include <interception_macros.h>
#include <common.h>

#include <Common/ext.h>

#include <string_view>
#include <unordered_map>
#include <vector>
#include <codecvt>

namespace il2cpp
{
    namespace
    {
        std::unordered_map<std::string, Il2CppClass*> resolved_classes;
        std::unordered_map<Il2CppClass*, std::vector<MethodOverloadInfo>> resolved_klass_overloads;

        IL2CPP_BINDING(System, String, app::Char__Array*, ToCharArray, (app::String* this_ptr));

        // Internal il2cpp methods.
        INTERNAL_BINDING(0x262560, Il2CppClass*, il2cpp_class_from_name, (Il2CppImage* image, const char* namezpace, const char* name));
        INTERNAL_BINDING(0x262CA0, Il2CppDomain*, il2cpp_domain_get, ());
        INTERNAL_BINDING(0x262CE0, Il2CppAssembly**, il2cpp_domain_get_assemblies, (Il2CppDomain* domain, size_t* size));
        INTERNAL_BINDING(0x101220, Il2CppImage*, il2cpp_assembly_get_image, (Il2CppAssembly* assembly));
        INTERNAL_BINDING(0x263870, Il2CppObject*, il2cpp_object_new, (Il2CppClass* klass));
        INTERNAL_BINDING(0x2625B0, PropertyInfo*, il2cpp_class_get_properties, (Il2CppClass* klass, void** iter));
        INTERNAL_BINDING(0x2626E0, MethodInfo*, il2cpp_class_get_methods, (Il2CppClass* klass, void** iter));
        // We use get_methods instead and build a lookup cache.
        //INTERNAL_BINDING(0x2626F0, MethodInfo*, il2cpp_class_get_method_from_name, (Il2CppClass* klass, const char* name, int argsCount));
        INTERNAL_BINDING(0x263860, MethodInfo*, il2cpp_object_get_virtual_method, (Il2CppObject* obj, const MethodInfo* method));
        INTERNAL_BINDING(0x263A70, Il2CppObject*, il2cpp_runtime_invoke, (const MethodInfo* method, void* obj, void** params, Il2CppException** exc));
        INTERNAL_BINDING(0x263D20, void*, il2cpp_thread_attach, (Il2CppDomain* domain));
        INTERNAL_BINDING(0x263B50, Il2CppString*, il2cpp_string_new_wrapper, (const char* str));
        INTERNAL_BINDING(0x263BC0, Il2CppString*, il2cpp_string_new_len, (const char* str, uint32_t len));
        INTERNAL_BINDING(0x263B70, Il2CppString*, il2cpp_string_new_utf16, (const Il2CppChar* str, uint32_t len));
        INTERNAL_BINDING(0x10F120, Il2CppType*, il2cpp_class_get_type, (Il2CppClass* klass));
        INTERNAL_BINDING(0x2643A0, char*, il2cpp_type_get_assembly_qualified_name, (const Il2CppType* type));
        INTERNAL_BINDING(0x002460, Il2CppObject*, il2cpp_value_box, (Il2CppClass* klass, void* value));
        INTERNAL_BINDING(0x101380, bool, il2cpp_class_is_assignable_from, (Il2CppClass* klass, Il2CppClass* oklass));
        INTERNAL_BINDING(0x262540, bool, il2cpp_class_is_subclass_of, (Il2CppClass* klass, Il2CppClass* klassc, bool check_interfaces));
        INTERNAL_BINDING(0x238110, bool, il2cpp_class_has_parent, (Il2CppClass* klass, Il2CppClass* klassc));
        INTERNAL_BINDING(0x262400, void, il2cpp_free, (void* obj));
        INTERNAL_BINDING(0x262590, Il2CppClass*, il2cpp_class_get_nested_types, (Il2CppClass* klass, void** iter));
        INTERNAL_BINDING(0x262810, bool, il2cpp_class_is_enum, (Il2CppClass* klass));
        INTERNAL_BINDING(0x262550, Il2CppClass*, il2cpp_class_from_type, (Il2CppType const* type));
        INTERNAL_BINDING(0x263100, uint32_t, il2cpp_gchandle_new, (Il2CppObject* obj, bool pinned));
        INTERNAL_BINDING(0x263120, uint32_t, il2cpp_gchandle_new_weakref, (Il2CppObject* obj, bool track_resurrection));
        INTERNAL_BINDING(0x263160, void, il2cpp_gchandle_free, (uint32_t handle));
        INTERNAL_BINDING(0x263140, Il2CppObject*, il2cpp_gchandle_get_target, (uint32_t handle));
        INTERNAL_BINDING(0x262470, Il2CppArraySize*, il2cpp_array_new, (Il2CppClass* element, il2cpp_array_size_t length));
        INTERNAL_BINDING(0x2624A0, Il2CppArraySize*, il2cpp_array_new_specific, (Il2CppClass* array_klass, il2cpp_array_size_t length));
        INTERNAL_BINDING(0x2624B0, Il2CppArraySize*, il2cpp_array_new_full, (Il2CppClass* array_klass, il2cpp_array_size_t* lengths, il2cpp_array_size_t* lower_bounds));

        thread_local std::string buffer;
        std::string const& get_full_name(std::string_view namezpace, std::string_view name, std::string_view nested = "")
        {
            buffer.clear();
            buffer.reserve(32);
            if (!namezpace.empty())
            {
                buffer += namezpace;
                buffer += ".";
            }

            buffer += name;

            if (!nested.empty())
            {
                buffer += ".";
                buffer += nested;
            }

            return buffer;
        }

        void resolve_overloads(Il2CppClass* klass)
        {
            std::vector<MethodOverloadInfo> overloads;
            void* it = nullptr;
            for (auto i = 0; i < klass->method_count; ++i)
            {
                auto method = il2cpp_class_get_methods(klass, &it);
                auto method_overload_info = std::find_if(overloads.begin(), overloads.end(), [method](MethodOverloadInfo const& info) -> bool {
                    return info.name == method->name && info.param_count == method->parameters_count;
                    });

                if (method_overload_info == overloads.end())
                {
                    MethodOverloadInfo info;
                    info.name = method->name;
                    info.param_count = method->parameters_count;
                    info.methods.push_back(method);
                    overloads.push_back(std::move(info));
                }
                else
                    method_overload_info->methods.push_back(method);
            }

            it = nullptr;
            for (auto i = 0; i < klass->property_count; ++i)
            {
                auto prop = il2cpp_class_get_properties(klass, &it);
                if (prop->get != nullptr)
                {
                    MethodOverloadInfo info;
                    info.name = prop->get->name;
                    info.param_count = 0;
                    info.methods.push_back(prop->get);
                    overloads.push_back(std::move(info));
                }

                if (prop->set != nullptr)
                {
                    MethodOverloadInfo info;
                    info.name = prop->set->name;
                    info.param_count = 1;
                    info.methods.push_back(prop->set);
                    overloads.push_back(std::move(info));
                }
            }

            resolved_klass_overloads[klass] = overloads;
        }
    }

    namespace untyped
    {
        Il2CppClass* get_class(std::string_view namezpace, std::string_view name)
        {
            auto const& full_name = get_full_name(namezpace, name);
            auto it = resolved_classes.find(full_name);
            if (it != resolved_classes.end())
                return it->second;

            Il2CppClass* klass = nullptr;
            size_t i = 0;
            size_t size = 0;
            auto domain = il2cpp_domain_get();
            auto assemblies = il2cpp_domain_get_assemblies(domain, &size);
            while (klass == nullptr && i < size)
            {
                auto image = il2cpp_assembly_get_image(assemblies[i]);
                klass = il2cpp_class_from_name(image, namezpace.data(), name.data());
                ++i;
            }

            if (klass == nullptr)
                trace(modloader::MessageType::Error, 1, "il2cpp", format("Failed to find klass %s", full_name.c_str()));

            // Add it to resolved classes anyway to prevent trace spam and future lookups.
            resolved_classes[full_name] = klass;

            return klass;
        }

        Il2CppClass* get_nested_class(std::string_view namezpace, std::string_view name, std::string_view nested)
        {
            {
                auto const& full_name = get_full_name(namezpace, name, nested);
                auto it = resolved_classes.find(full_name);
                if (it != resolved_classes.end())
                    return it->second;
            }

            Il2CppClass* output = nullptr;
            auto klass = get_class(namezpace, name);
            if (klass != nullptr)
            {
                void* iter = nullptr;
                il2cpp_class_get_nested_types(klass, &iter);
                auto typed_iter = reinterpret_cast<Il2CppClass**>(iter);
                for (auto i = 0; i < klass->nested_type_count && output == nullptr; ++i)
                {
                    if (typed_iter[i]->name == nested)
                        output = typed_iter[i];
                }
            }

            auto const& full_name = get_full_name(namezpace, name, nested);
            resolved_classes[full_name] = output;
            if (output == nullptr)
                trace(modloader::MessageType::Error, 1, "il2cpp", format("Failed to find klass %s", full_name.c_str()));

            return output;
        }
    }

    void trace_overloads(Il2CppClass* klass)
    {
        if (klass->parent != nullptr)
            trace_overloads(klass->parent);

        auto method_overloads = resolved_klass_overloads.find(klass);
        for (const auto& info : method_overloads->second)
            trace(modloader::MessageType::Error, 5, "il2cpp", format(" - %s.%s:%d", klass->name, info.name.data(), info.param_count));
    }

    MethodOverloadInfo const* get_method_info_internal(Il2CppClass* klass, std::string_view method, int param_count)
    {
        auto method_overloads = resolved_klass_overloads.find(klass);
        if (method_overloads == resolved_klass_overloads.end())
        {
            resolve_overloads(klass);
            method_overloads = resolved_klass_overloads.find(klass);
        }

        std::vector<MethodOverloadInfo> const& methods = method_overloads->second;
        auto method_overload_info = std::find_if(methods.begin(), methods.end(), [&method, &param_count](MethodOverloadInfo const& info) -> bool {
            return info.name == method && info.param_count == param_count;
            });

        if (method_overload_info == methods.end())
        {
            if (klass->parent != nullptr)
            {
                auto info = get_method_info_internal(klass->parent, method, param_count);
                if (info != nullptr)
                    return info;
            }

            trace(modloader::MessageType::Error, 1, "il2cpp", format("Could not find method '%s:%d' in klass '%s'", method.data(), param_count, klass->name));
            trace_overloads(klass);
            return nullptr;
        }

        return (&*method_overload_info);
    }

    int get_method_overload_count(Il2CppClass* klass, std::string_view method, int param_count)
    {
        auto method_overloads = resolved_klass_overloads.find(klass);
        if (method_overloads == resolved_klass_overloads.end())
        {
            resolve_overloads(klass);
            method_overloads = resolved_klass_overloads.find(klass);
        }

        std::vector<MethodOverloadInfo> const& methods = method_overloads->second;
        auto method_overload_info = std::find_if(methods.begin(), methods.end(), [&method, &param_count](MethodOverloadInfo const& info) -> bool {
            return info.name == method && info.param_count == param_count;
            });

        if (method_overload_info == methods.end())
        {
            trace(modloader::MessageType::Error, 1, "il2cpp", format("Method '%s' with params count %d in klass '%s.%s' does not exist",
                method.data(), param_count, klass->namespaze, klass->name));
            for (auto const& method_info : methods)
                trace(modloader::MessageType::Info, 3, "il2cpp", format("- %s(%d)", method_info.name.c_str(), method_info.param_count));

            return 0;
        }

        return method_overload_info->methods.size();
    }

    MethodInfo const* get_method_from_name_overloaded(Il2CppClass* klass, std::string_view method, int param_count, int overload)
    {
        auto info = get_method_info_internal(klass, method, param_count);
        if (info == nullptr)
            return nullptr;

        if (info->methods.size() <= overload)
        {
            trace(modloader::MessageType::Error, 1, "il2cpp", format("Overload '%d' for '%s:%d' in klass '%s' does not exist",
                overload, method.data(), param_count, klass->name));
        }

        return info->methods.at(overload);
    }

    MethodInfo const* get_method_from_name(Il2CppClass* klass, std::string_view method, std::vector<Il2CppClass*> const& params)
    {
        auto info = get_method_info_internal(klass, method, params.size());
        if (info == nullptr)
            return nullptr;

        if (info->methods.size() == 1)
            return info->methods.front();
        else
        {
            bool first = true;
            for (auto method_info : info->methods)
            {
                auto valid = true;
                for (auto i = 0; valid && i < method_info->parameters_count; ++i)
                {
                    auto& param = method_info->parameters[i];
                    Il2CppClass *klass_1 = il2cpp_class_from_type(param.parameter_type);
                    Il2CppClass *klass_2 = params.at(param.position);
                    if (klass_1 != klass_2)
                        valid = false;
                }

                if (valid)
                    return method_info;
            }
        }

        trace(modloader::MessageType::Error, 3, "il2cpp", format("could not find a method overload for '%s:%d'in klass '%s' that matched parameters",
            method.data(), params.size(), klass->name));

        trace(modloader::MessageType::Info, 3, "il2cpp", "valid parameters are:");
        for (auto method_info : info->methods)
        {
            std::string params = " - ";
            for (auto i = 0; i < method_info->parameters_count; ++i)
            {
                auto& param = method_info->parameters[i];
                auto klass = il2cpp_class_from_type(param.parameter_type);
                params += get_full_name(klass->namespaze, klass->name);
                params += ", ";
            }

            trace(modloader::MessageType::Info, 3, "il2cpp", params);
        }

        return nullptr;
    }

    MethodInfo const* get_method_from_name(Il2CppClass* klass, std::string_view method, std::vector<KlassDescriptor> const& params)
    {
        auto info = get_method_info_internal(klass, method, params.size());
        if (info == nullptr)
            return nullptr;

        if (info->methods.size() == 1)
            return info->methods.front();
        else
        {
            bool first = true;
            for (auto method_info : info->methods)
            {
                auto valid = true;
                for (auto i = 0; valid && i < method_info->parameters_count; ++i)
                {
                    auto& param = method_info->parameters[i];
                    Il2CppClass *klass_1 = il2cpp_class_from_type(param.parameter_type);
                    KlassDescriptor klass_2 = params.at(param.position);
                    if (klass_2.klass == nullptr)
                    {
                        if (klass_1->namespaze != klass_2.namezpace || klass_1->name != klass_2.name)
                            valid = false;
                    }
                    else if (klass_1 != klass_2.klass)
                        valid = false;

                }

                if (valid)
                    return method_info;
            }
        }

        trace(modloader::MessageType::Error, 3, "il2cpp", format("could not find a method overload for '%s:%d'in klass '%s' that matched parameters",
            method.data(), params.size(), klass->name));

        trace(modloader::MessageType::Info, 3, "il2cpp", "valid parameters are:");
        for (auto method_info : info->methods)
        {
            std::string params = " - ";
            for (auto i = 0; i < method_info->parameters_count; ++i)
            {
                auto& param = method_info->parameters[i];
                auto klass = il2cpp_class_from_type(param.parameter_type);
                params += get_full_name(klass->namespaze, klass->name);
                params += ", ";
            }

            trace(modloader::MessageType::Info, 3, "il2cpp", params);
        }

        return nullptr;
    }

    std::string convert_csstring(app::String* str)
    {
        std::string cppstr;
        if (str == nullptr)
            return cppstr;

        auto chars = String::ToCharArray(str);
        if (chars == nullptr)
            return cppstr;

        std::wstring wstr(reinterpret_cast<wchar_t*>(chars->vector), str->fields.m_stringLength);
#pragma warning(disable : 4996)
        using convert_type = std::codecvt_utf8<wchar_t>;
        std::wstring_convert<convert_type, wchar_t> converter;
        cppstr = converter.to_bytes(wstr);
#pragma warning(default : 4996)

        return cppstr;
    }
}