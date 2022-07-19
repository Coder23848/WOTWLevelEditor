#pragma once

#include <Il2CppModLoader/interception_macros.h>

void on_fixed_update(app::GameController* this_ptr, float delta);

app::Vector3 world_mouse_position();
app::Camera* get_camera();
app::GameController* get_game_controller();
app::SeinCharacter* get_sein();
app::ScenesManager* get_scenes_manager();