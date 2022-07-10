#pragma once

#include <Il2CppModLoader/interception_macros.h>

void on_fixed_update(app::GameController* this_ptr, float delta);

app::GameController* get_game_controller();
app::SeinCharacter* get_sein();