#include <dll_main.h>
#include <macros.h>

#include <Il2CppModLoader/common.h>
#include <Il2CppModLoader/il2cpp_helpers.h>
#include <Il2CppModLoader/interception_macros.h>

#include <string>

#include <windows.h>

using namespace modloader;

IL2CPP_BINDING(, GameController, bool, get_InputLocked, (app::GameController* thisPtr));
IL2CPP_BINDING(, GameController, bool, get_LockInput, (app::GameController* thisPtr));
IL2CPP_BINDING(, GameController, bool, get_IsSuspended, (app::GameController* thisPtr));
IL2CPP_BINDING(, GameController, bool, get_SecondaryMapAndInventoryCanBeOpened, (app::GameController* thisPtr));

STATIC_IL2CPP_BINDING(, TimeUtility, float, get_fixedDeltaTime, ());
IL2CPP_INTERCEPT(, GameController, void, FixedUpdate, (app::GameController* this_ptr))
{
    GameController::FixedUpdate(this_ptr);
    //ipc::update_pipe();
    on_fixed_update(this_ptr, TimeUtility::get_fixedDeltaTime());
}

INJECT_C_DLLEXPORT bool player_can_move()
{
    auto gcip = get_game_controller();
    return !(GameController::get_InputLocked(gcip) ||
        GameController::get_LockInput(gcip) ||
        GameController::get_IsSuspended(gcip)) &&
        GameController::get_SecondaryMapAndInventoryCanBeOpened(gcip);
}

void reload_scenes()
{
    auto scenes_manager = get_scenes_manager();
    il2cpp::invoke(scenes_manager, "UnloadAllScenes"); // The game will reload them automatically, since Ori's still in them.
}

bool paused_for_editing = false;

void open_editor()
{
    if (paused_for_editing)
    {
        trace(MessageType::Warning, 0, "editor", "Tried to open the editor, but it's already open.");
        return;
    }

    auto game_controller = get_game_controller();
    il2cpp::invoke(game_controller, "SuspendGameplayForUI");

    //reload_scenes();

    paused_for_editing = true;
}
void close_editor()
{
    if (!paused_for_editing)
    {
        trace(MessageType::Warning, 0, "editor", "Tried to close the editor, but it's already closed.");
        return;
    }

    auto game_controller = get_game_controller();
    il2cpp::invoke(game_controller, "ResumeGameplayForUI");

    //reload_scenes();

    paused_for_editing = false;
}

STATIC_IL2CPP_BINDING(UnityEngine, Input, bool, GetKeyInt, (app::KeyCode__Enum keyCode));
STATIC_IL2CPP_BINDING(UnityEngine, Input, bool, GetKeyDownInt, (app::KeyCode__Enum keyCode));
STATIC_IL2CPP_BINDING(UnityEngine, Input, bool, GetMouseButtonDown, (int32_t button));
STATIC_IL2CPP_BINDING(UnityEngine, Input, app::Vector3, get_mousePosition, ());
STATIC_IL2CPP_BINDING(UnityEngine, Camera, app::Camera*, get_main, ());
STATIC_IL2CPP_BINDING_OVERLOAD(UnityEngine, Resources, app::Object*, Load, (app::String* path), (System:String));
STATIC_IL2CPP_BINDING_OVERLOAD(UnityEngine, Object, app::Object*, Instantiate, (app::Object* original), (UnityEngine:Object));
STATIC_IL2CPP_BINDING(UnityEngine, Quaternion, app::Quaternion, Euler, (float x, float y, float z));
IL2CPP_BINDING(UnityEngine, Behaviour, bool, get_enabled, (app::Camera* this_ptr));
IL2CPP_BINDING(UnityEngine, Transform, app::Vector3, get_position, (app::Transform* this_ptr));
IL2CPP_BINDING(UnityEngine, Camera, app::Vector3, ScreenToWorldPoint, (app::Camera* this_ptr, app::Vector3 position));
IL2CPP_BINDING(, CollectablePlaceholder, void, set_IsSuspended, (app::CollectablePlaceholder* this_ptr, bool value));

app::Camera* camera = nullptr;
app::Camera* get_camera()
{
    if (camera == nullptr || !Behaviour::get_enabled(camera))
    {
        camera = Camera::get_main();
    }
    return camera;
}

app::Vector3 world_mouse_position()
{
    auto screen_mouse_position = Input::get_mousePosition();
    auto camera = get_camera();
    auto camera_position = Transform::get_position(il2cpp::unity::get_transform(il2cpp::unity::get_game_object(camera)));
    //trace(MessageType::Warning, 0, "editor", std::to_string(camera_position.x) + ", " + std::to_string(camera_position.y) + ", " + std::to_string(camera_position.z));
    screen_mouse_position.z = camera_position.z * -1;
    return Camera::ScreenToWorldPoint(camera, screen_mouse_position);
}

void on_fixed_update(app::GameController* this_ptr, float delta)
{
    if (Input::GetKeyInt(app::KeyCode__Enum::KeyCode__Enum_LeftAlt) && Input::GetKeyDownInt(app::KeyCode__Enum::KeyCode__Enum_Alpha1))
    {
        if (paused_for_editing)
        {
            close_editor();
        }
        else
        {
            open_editor();
        }
    }
    
    if (paused_for_editing && Input::GetMouseButtonDown(0))
    {
        auto pos = world_mouse_position();
        pos.z = 0;
        auto scenes_manager = get_scenes_manager();
        auto current_scene = il2cpp::invoke<app::SceneManagerScene>(scenes_manager, "get_CurrentSceneManagerScene");
        auto scene_root = current_scene->fields.SceneRoot;
        auto root_transform = il2cpp::unity::get_transform(il2cpp::unity::get_game_object(scene_root));
        auto prefab = Object::Instantiate(Resources::Load(il2cpp::string_new("pickups/energydroppickup")));
        auto this_transform = il2cpp::unity::get_transform(prefab);
        il2cpp::invoke(this_transform, "set_position", &pos);
        il2cpp::invoke(this_transform, "SetParent", root_transform);
    }
}

app::GameController* get_game_controller()
{
    return il2cpp::get_class<app::GameController__Class>("", "GameController")->static_fields->Instance;
}

app::SeinCharacter* get_sein()
{
    return il2cpp::get_class<app::Characters__Class>("Game", "Characters")->static_fields->m_sein;
}

app::ScenesManager* get_scenes_manager()
{
    return il2cpp::get_class<app::Scenes__Class>("Core", "Scenes")->static_fields->Manager;
}

void initialize_main()
{
}

CALL_ON_INIT(initialize_main);