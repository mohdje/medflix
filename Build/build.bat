@echo off

set params=%1

set DESKTOP_AVALONIA_CSPROJ_PATH="..\Front\desktop-avalonia-ui-app\Medflix.Desktop\Medflix.Desktop.csproj"
set WINDOWS_BUILD_PATH="..\Front\desktop-avalonia-ui-app\Medflix.Desktop\bin\release\net7.0\win-x64\publish"
set MACOS_BUILD_PATH="..\Front\desktop-avalonia-ui-app\Medflix.Desktop\bin\release\net7.0\osx-x64\publish"
set FRONT_END_APP_PATH="..\Front\react-streaming-client"
set EXTRACT_UPDATE_APP_PATH="..\Front\extract-update-package-electron-app"

@REM Build Windows Avlonia App
if not "%params:windows=%"=="%params%" (
    if exist %WINDOWS_BUILD_PATH% (
        rmdir %WINDOWS_BUILD_PATH% /s /q
    )

    dotnet publish %DESKTOP_AVALONIA_CSPROJ_PATH% -c Release -f net7.0 -r win-x64 --self-contained

    move "%WINDOWS_BUILD_PATH%\Medflix.Desktop.exe" "%WINDOWS_BUILD_PATH%\Medflix.exe"
    
    @REM Build Frontend
    npm run build --prefix %FRONT_END_APP_PATH%

    xcopy %FRONT_END_APP_PATH%"\build\"  %WINDOWS_BUILD_PATH%"\view\" /E

    @REM Build Exract Update App
    npm run make --prefix %EXTRACT_UPDATE_APP_PATH%

    xcopy %EXTRACT_UPDATE_APP_PATH%"\out\Extract Medflix Package-win32-x64\" %WINDOWS_BUILD_PATH%"\extract-update\" /E

    echo Windows application is ready
)

@REM Build Macos Avlonia App
if not "%params:macos=%"=="%params%" (
    if exist %MACOS_BUILD_PATH% (
        rmdir %MACOS_BUILD_PATH% /s /q
    )

    dotnet publish %DESKTOP_AVALONIA_CSPROJ_PATH% -c Release -f net7.0 -r osx-x64 --self-contained

    @REM Build Frontend
    npm run build --prefix %FRONT_END_APP_PATH%

    xcopy %FRONT_END_APP_PATH%"\build\"  %MACOS_BUILD_PATH%"\view\" /E

    echo Macos backend is ready
)

