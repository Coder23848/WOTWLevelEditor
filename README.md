# WOTW Level Editor
```
Very incomplete, currently can move GameObjects around and delete them.

controls (command line, definitely subject to change):
listall -> list all objects in the level
selectid <id> -> select an object, currently only GameObjects/Transforms are meaningfully supported
select <name> <index, usually 0> -> select and object by its name. 0 as index gives the first object with that name, 1 gives the second, etc.
move <x> <y> <z> -> move the currently selected object by the given amount
delete -> delete the selected object, also deletes components and children
save -> saves the changes to the file
exit -> exits the program

Make sure to back up your level files, if you forget and mess something up you have to reinstall WOTW.
I know my code is messy lol
```
