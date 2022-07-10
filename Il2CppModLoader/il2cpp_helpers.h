#pragma once

#include <Il2CppModLoader/il2cpp_internals/il2cpp_internals.h>
#include <Il2CppModLoader/macros.h>

#include <vector>
#include <string_view>

namespace il2cpp
{
    struct MethodOverloadInfo
    {
        std::string name;
        int param_count;
        std::vector<MethodInfo const*> methods;
    };

    namespace untyped
    {
        IL2CPP_MODLOADER_DLLEXPORT Il2CppClass* get_class(std::string_view namezpace, std::string_view name);
        IL2CPP_MODLOADER_DLLEXPORT Il2CppClass* get_nested_class(std::string_view namezpace, std::string_view name, std::string_view nested);
    }

    struct KlassDescriptor
    {
        std::string namezpace;
        std::string name;
        Il2CppClass* klass;
    };

    IL2CPP_MODLOADER_DLLEXPORT std::string convert_csstring(app::String* str);

    IL2CPP_MODLOADER_DLLEXPORT int get_method_overload_count(Il2CppClass* klass, std::string_view method, int param_count);
    IL2CPP_MODLOADER_DLLEXPORT MethodInfo const* get_method_from_name_overloaded(Il2CppClass* klass, std::string_view method, int param_count, int overload);
    IL2CPP_MODLOADER_DLLEXPORT MethodInfo const* get_method_from_name(Il2CppClass* klass, std::string_view method, std::vector<Il2CppClass*> const& params);
    IL2CPP_MODLOADER_DLLEXPORT MethodInfo const* get_method_from_name(Il2CppClass* klass, std::string_view method, std::vector<KlassDescriptor> const& params);

    // Templates

    template<typename Return = Il2CppClass>
    Return* get_class(std::string_view namezpace, std::string_view name)
    {
        return reinterpret_cast<Return*>(untyped::get_class(namezpace, name));
    }

    template<typename Return = Il2CppClass>
    Return* get_nested_class(std::string_view namezpace, std::string_view name, std::string_view nested)
    {
        return reinterpret_cast<Return*>(untyped::get_nested_class(namezpace, name, nested));
    }
}