#include <settings.h>

#include <algorithm>
#include <filesystem>
#include <fstream>
#include <xstring>

#include <assert.h>
#include <windows.h>
#include <ext.h>

void load_settings_from_file(IniSettings& settings)
{
    if (!std::filesystem::exists(settings.path))
        return;

    for (auto& option : settings.options)
    {
        switch (option.type)
        {
        case IniVarType::Bool:
        {
            auto length = GetPrivateProfileString(
                option.section.c_str(),
                option.name.c_str(),
                option.value.b ? "true" : "false",
                option.value.s.data(),
                option.value.s.size(),
                settings.path.c_str()
            );

            std::string value{ option.value.s.data(), length };
            std::transform(value.begin(), value.end(), value.begin(), [](auto c) { return std::tolower(c); });
            option.value.b = value == std::string{ "true" };
            break;
        }
        case IniVarType::Int:
        {
            option.value.i = GetPrivateProfileInt(
                option.section.c_str(),
                option.name.c_str(),
                option.value.i,
                settings.path.c_str()
            );
            break;
        }
        case IniVarType::Float:
        {
            auto length = GetPrivateProfileString(
                option.section.c_str(),
                option.name.c_str(),
                std::to_string(option.value.f).c_str(),
                option.value.s.data(),
                option.value.s.size(),
                settings.path.c_str()
            );

            std::string value{ option.value.s.data(), length };
            option.value.f = std::stof(value);
            break;
        }
        case IniVarType::String:
        {
            auto length = GetPrivateProfileString(
                option.section.c_str(),
                option.name.c_str(),
                option.value.s.data(),
                option.value.s.data(),
                option.value.s.size(),
                settings.path.c_str()
            );

            break;
        }
        default:
            assert(false);
        }
    }
}

void save_settings_to_file(IniSettings& settings)
{
    if (!std::filesystem::exists(settings.path))
        std::fstream().open(settings.path);

    for (auto& option : settings.options)
    {
        std::string value;
        switch (option.type)
        {
        case IniVarType::Bool:
            value = option.value.b ? "true" : "false";
            break;
        case IniVarType::Int:
            value = std::to_string(option.value.i);
            break;
        case IniVarType::Float:
            value = std::to_string(option.value.f);
            break;
        case IniVarType::String:
            value = option.value.s.data();
            break;
        default:
            assert(false);
        }

        WritePrivateProfileString(
            option.section.c_str(),
            option.name.c_str(),
            value.c_str(),
            settings.path.c_str()
        );
    }
}

IniOption* find_option(IniSettings& settings, std::string const& section, std::string const& name)
{
    auto it = std::find_if(settings.options.begin(), settings.options.end(), [&](auto const& option) -> bool {
        return option.section == section && option.name == name;
        });

    if (it != settings.options.end())
        return &*it;

    return nullptr;
}


template<>
bool check_option(IniSettings& settings, std::string const& section, std::string const& name, bool default_value)
{
    auto option = find_option(settings, section, name);
    if (option == nullptr)
        return default_value;

    switch (option->type)
    {
    case IniVarType::Bool:
        return static_cast<float>(option->value.b);
    case IniVarType::Float:
        return !eps_equals(option->value.f, 0.0f);
    case IniVarType::Int:
        return option->value.i > 0;
    case IniVarType::String:
    {
        std::string value = option->value.s.data();
        std::transform(value.begin(), value.end(), value.begin(),
            [](char c) { return std::tolower(c); });

        return value == "true" || value == "1" || value == "t";
    }
    default:
        return default_value;
    }
}

template<>
float check_option(IniSettings& settings, std::string const& section, std::string const& name, float default_value)
{
    auto option = find_option(settings, section, name);
    if (option == nullptr)
        return default_value;

    switch (option->type)
    {
    case IniVarType::Bool:
        return static_cast<float>(option->value.b);
    case IniVarType::Float:
        return option->value.f;
    case IniVarType::Int:
        return static_cast<float>(option->value.i);
    case IniVarType::String:
        return std::atof(option->value.s.data());
    default:
        return default_value;
    }
}

template<>
int check_option(IniSettings& settings, std::string const& section, std::string const& name, int default_value)
{
    auto option = find_option(settings, section, name);
    if (option == nullptr)
        return default_value;

    switch (option->type)
    {
    case IniVarType::Bool:
        return static_cast<int>(option->value.b);
    case IniVarType::Float:
        return static_cast<int>(option->value.f);
    case IniVarType::Int:
        return option->value.i;
    case IniVarType::String:
        return std::atoi(option->value.s.data());
    default:
        return default_value;
    }
}

template<>
std::string check_option(IniSettings& settings, std::string const& section, std::string const& name, std::string default_value)
{
    auto option = find_option(settings, section, name);
    if (option == nullptr)
        return default_value;

    switch (option->type)
    {
    case IniVarType::Bool:
        return std::to_string(option->value.b);
    case IniVarType::Float:
        return std::to_string(option->value.f);
    case IniVarType::Int:
        return std::to_string(option->value.i);
    case IniVarType::String:
        return option->value.s.data();
    default:
        return default_value;
    }
}

IniSettings create_editor_settings(const std::string& path)
{
    IniSettings file;
    file.path = path + "settings.ini";

    IniOption option;
    option.section = "Paths";
    option.type = IniVarType::String;

    option.name = "Steam";
    strcpy_s(option.value.s.data(), option.value.s.size(), "C:\\Program Files (x86)\\Steam\\steam.exe");
    file.options.push_back(option);

    option.section = "Flags";
    option.type = IniVarType::Bool;

    option.name = "UseWinStore";
    option.value.b = false;
    file.options.push_back(option);

    return file;
}
