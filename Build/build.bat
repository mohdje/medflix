@echo off

set params=%1

set DESKTOP_WINFORMS_CSPROJ_PATH="..\Desktop Application\WinFormsApp\MedflixWinForms\MedflixWinForms.csproj"
set WINDOWS_BUILD_PATH="..\Desktop Application\WinFormsApp\MedflixWinForms\bin\Release\net7.0-windows\win-x64\publish"
set WEBHOST_CSPROJ_PATH="..\WebHostStreaming\WebHostStreaming\WebHostStreaming.csproj"
set WEBHOST_MACOS_BUILD_PATH="..\WebHostStreaming\WebHostStreaming\bin\Release\net7.0\osx-x64\publish"
set FRONT_END_APP_PATH="..\Front\react-streaming-client"
set EXTRACT_UPDATE_APP_PATH="..\Desktop Application\extract-update-package-electron-app"
set MACOS_APP_BUNDLE_BUILD_PATH=".\MacOS App bundle\macos"

@REM Build Windows App
if not "%params:windows=%"=="%params%" (
    if exist %WINDOWS_BUILD_PATH% (
        rmdir %WINDOWS_BUILD_PATH% /s /q
    )
    
    dotnet publish %DESKTOP_WINFORMS_CSPROJ_PATH% -c Release -r win-x64 --self-contained

    move %WINDOWS_BUILD_PATH%\MedflixWinForms.exe %WINDOWS_BUILD_PATH%\Medflix.exe
    
    @REM Build Frontend
    npm run build --prefix %FRONT_END_APP_PATH%

    xcopy %FRONT_END_APP_PATH%"\build\"  %WINDOWS_BUILD_PATH%"\view\" /E

    @REM @REM Build Exract Update App
    npm run make --prefix %EXTRACT_UPDATE_APP_PATH%

    xcopy %EXTRACT_UPDATE_APP_PATH%"\out\Extract Medflix Package-win32-x64\" %WINDOWS_BUILD_PATH%"\extract-update\" /E

    echo Windows application is ready
)

@REM Build Webhost and view for Macos App
if not "%params:macos=%"=="%params%" (
    if exist %WEBHOST_MACOS_BUILD_PATH% (
        rmdir %WEBHOST_MACOS_BUILD_PATH% /s /q
    )

    dotnet publish %WEBHOST_CSPROJ_PATH% -c Release -r osx-x64 --self-contained

    @REM Build Frontend
    npm run build --prefix %FRONT_END_APP_PATH%

    if exist %MACOS_APP_BUNDLE_BUILD_PATH% (
        rmdir %MACOS_APP_BUNDLE_BUILD_PATH% /s /q
    )

    xcopy %WEBHOST_MACOS_BUILD_PATH% %MACOS_APP_BUNDLE_BUILD_PATH%"\" /E
    xcopy %FRONT_END_APP_PATH%"\build\" %MACOS_APP_BUNDLE_BUILD_PATH%"\view\" /E

    echo Macos application is ready
)

