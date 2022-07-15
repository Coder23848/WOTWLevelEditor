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

    namespace unity
    {
        IL2CPP_MODLOADER_DLLEXPORT void destroy_object(void* object);
        IL2CPP_MODLOADER_DLLEXPORT app::Transform* get_parent(app::Transform* object);
        IL2CPP_MODLOADER_DLLEXPORT app::Transform* get_transform(void* object);
        IL2CPP_MODLOADER_DLLEXPORT app::GameObject* get_game_object(void* component);
        IL2CPP_MODLOADER_DLLEXPORT std::vector<app::GameObject*> get_children(app::GameObject* game_object);
        IL2CPP_MODLOADER_DLLEXPORT app::GameObject* find_child(app::GameObject* game_object, std::string_view name);
        IL2CPP_MODLOADER_DLLEXPORT app::GameObject* find_child(app::GameObject* game_object, std::vector<std::string_view> const& path);
        IL2CPP_MODLOADER_DLLEXPORT int32_t get_scene_count();
        IL2CPP_MODLOADER_DLLEXPORT app::Scene get_scene_at(int32_t i);
        IL2CPP_MODLOADER_DLLEXPORT app::Scene get_active_scene();
        IL2CPP_MODLOADER_DLLEXPORT std::vector<app::GameObject*> get_root_game_objects(app::Scene& scene);
        IL2CPP_MODLOADER_DLLEXPORT std::string get_scene_name(app::Scene& scene);
        IL2CPP_MODLOADER_DLLEXPORT std::string get_scene_path(app::Scene& scene);
    }

    namespace untyped
    {
        IL2CPP_MODLOADER_DLLEXPORT Il2CppObject* create_object(std::string_view namezpace, std::string_view klass, std::string_view nested);
        IL2CPP_MODLOADER_DLLEXPORT Il2CppObject* create_object(std::string_view namezpace, std::string_view name);
        IL2CPP_MODLOADER_DLLEXPORT Il2CppObject* create_object(Il2CppClass* klass);
        IL2CPP_MODLOADER_DLLEXPORT Il2CppObject* box_value(Il2CppClass* klass, void* value);
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

    IL2CPP_MODLOADER_DLLEXPORT app::String* string_new(std::string_view str);
    IL2CPP_MODLOADER_DLLEXPORT app::String* string_new(std::string_view str, uint32_t len);
    IL2CPP_MODLOADER_DLLEXPORT app::String* string_new(std::wstring_view str);

    IL2CPP_MODLOADER_DLLEXPORT Il2CppObject* invoke_v(void* obj, std::string_view method, std::vector<void*> params = {});
    IL2CPP_MODLOADER_DLLEXPORT Il2CppObject* invoke_virtual_v(void* obj, Il2CppClass* base, std::string_view method, std::vector<void*> params = {});

    IL2CPP_MODLOADER_DLLEXPORT bool is_assignable(Il2CppClass* klass, std::string_view namezpace, std::string_view name);
    IL2CPP_MODLOADER_DLLEXPORT bool is_assignable(Il2CppClass* klass, Il2CppClass* iklass);
    IL2CPP_MODLOADER_DLLEXPORT bool is_assignable(void* obj, std::string_view namezpace, std::string_view name);
    IL2CPP_MODLOADER_DLLEXPORT bool is_assignable(void* obj, Il2CppClass* iklass);

    // Templates

    template<typename T>
    T* create_object(std::string_view namezpace, std::string_view klass, std::string_view nested)
    {
        return reinterpret_cast<T*>(untyped::create_object(namezpace, klass, nested));
    }

    template<typename T>
    T* create_object(std::string_view namezpace, std::string_view name)
    {
        return reinterpret_cast<T*>(untyped::create_object(namezpace, name));
    }

    template<typename T>
    T* create_object(Il2CppClass* klass)
    {
        return reinterpret_cast<T*>(untyped::create_object(klass));
    }

    template<typename Return = Il2CppObject>
    Return* invoke(void* obj, std::string_view method)
    {
        std::vector<void*> collected_params;
        return reinterpret_cast<Return*>(invoke_v(obj, method, collected_params));
    }

    template<typename Return = Il2CppObject, typename ...Args>
    Return* invoke(void* obj, std::string_view method, Args ... params)
    {
        std::vector<void*> collected_params{ { reinterpret_cast<void*>(params)... } };
        return reinterpret_cast<Return*>(invoke_v(obj, method, collected_params));
    }

    template<typename Return = Il2CppObject>
    Return* invoke_virtual(void* obj, Il2CppClass* base, std::string_view method)
    {
        std::vector<void*> collected_params{};
        return reinterpret_cast<Return*>(invoke_virtual_v(obj, base, method, collected_params));
    }

    template<typename Return = Il2CppObject, typename ...Args>
    Return* invoke_virtual(void* obj, Il2CppClass* base, std::string_view method, Args ... params)
    {
        std::vector<void*> collected_params{ { reinterpret_cast<void*>(params)... } };
        return reinterpret_cast<Return*>(invoke_virtual_v(obj, base, method, collected_params));
    }

    constexpr bool use_internal_box_function = false;
    template<typename Return, typename Input, typename InputKlass>
    Return* box_value(InputKlass* klass, Input value)
    {
        if (use_internal_box_function)
        {
            return reinterpret_cast<Return*>(untyped::box_value(
                reinterpret_cast<Il2CppClass*>(klass), reinterpret_cast<void*>(&value)));
        }
        else
        {
            auto boxed_value = create_object<Return>(klass);
            boxed_value->fields = value;
            return boxed_value;
        }
    }

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