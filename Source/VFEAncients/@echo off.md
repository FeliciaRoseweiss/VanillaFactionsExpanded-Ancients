@echo off
cd /d "D:\!Проекты\rimworld\VanillaFactionsExpanded-Ancients"

echo Копирование контента из оригинального мода...
xcopy "VanillaFactionsExpanded-Ancients-main\Languages" "Languages" /E /I /Y
xcopy "VanillaFactionsExpanded-Ancients-main\Sounds" "Sounds" /E /I /Y  
xcopy "VanillaFactionsExpanded-Ancients-main\Textures" "Textures" /E /I /Y
xcopy "VanillaFactionsExpanded-Ancients-main\1.5\Defs" "1.6\Defs" /E /I /Y
xcopy "VanillaFactionsExpanded-Ancients-main\1.5\Patches" "1.6\Patches" /E /I /Y

echo Сборка проекта...
cd "Source\VFEAncients"
dotnet clean
dotnet build --configuration Release

echo Готово!
pause

