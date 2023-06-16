@echo off

set params=%1
set BACKEND_END_PATH="..\WebHostStreaming\WebHostStreaming"
set APP_SETTINGS_PATH=%BACKEND_END_PATH%\appsettings.json
set BACKEND_CSPROJ_PATH=%BACKEND_END_PATH%\WebHostStreaming.csproj
set BACKEND_BIN_PATH=%BACKEND_END_PATH%\bin
set BACKEND_SOLUTION_PATH="..\WebHostStreaming\WebHostStreaming.sln"
set FRONT_END_APP_PATH="..\Front\react-streaming-client"
set MSBUILD_PATH="C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
set DESKTOP_APP_PATH="..\Front\desktop-electron-app"
set DESKTOP_VIEW_APP_PATH=%DESKTOP_APP_PATH%"\views\app\"
set EXTRACT_UPDATE_APP_PATH="..\Front\extract-update-package-electron-app"

if not "%params:web=%"=="%params%" (  
    @REM Build Backend
    if exist %BACKEND_BIN_PATH%"\Release\net6.0" (
        rmdir  %BACKEND_BIN_PATH%"\Release\net6.0" /s /q
    )

   ".\OverrideJSON\OverrideJSON.exe" %APP_SETTINGS_PATH% "Platform" "web"

    %MSBUILD_PATH% %BACKEND_SOLUTION_PATH% /p:Configuration="Release" /p:Platform="Any CPU"

    @REM Build Frontend
    npm run build --prefix %FRONT_END_APP_PATH%

    xcopy %FRONT_END_APP_PATH%"\build\" %BACKEND_BIN_PATH%"\Release\net6.0\view\" /E

    echo Web application is ready
)

if not "%params:windows=%"=="%params%" (
    @REM Build Backend
    if exist %BACKEND_BIN_PATH%"\Release Background\net6.0" (
        rmdir %BACKEND_BIN_PATH%"\Release Background\net6.0" /s /q
    )

    ".\OverrideJSON\OverrideJSON.exe" %APP_SETTINGS_PATH% "Platform" "windows"
    
    %MSBUILD_PATH%  %BACKEND_SOLUTION_PATH% /p:Configuration="Release Background" /p:Platform="Any CPU"

    @REM Build Frontend
    npm run build --prefix %FRONT_END_APP_PATH%

    @REM Build Desktop App
    if exist %DESKTOP_VIEW_APP_PATH% (
        rmdir %DESKTOP_VIEW_APP_PATH% /s /q
    )
    
    xcopy %FRONT_END_APP_PATH%"\build\"  %DESKTOP_VIEW_APP_PATH%  /E

    npm run make --prefix %DESKTOP_APP_PATH%

    xcopy %DESKTOP_APP_PATH%"\out\Medflix-win32-x64\"  %BACKEND_BIN_PATH%"\Release Background\net6.0\windows-app\" /E

    @REM Build Exract Update App
    npm run make --prefix %EXTRACT_UPDATE_APP_PATH%

    xcopy %EXTRACT_UPDATE_APP_PATH%"\out\Extract Medflix Package-win32-x64\" %BACKEND_BIN_PATH%"\Release Background\net6.0\extract-update\" /E

    echo Windows application is ready
)

if not "%params:macos=%"=="%params%" (
 @REM Build Backend
    if exist %BACKEND_BIN_PATH%"\Release Background\macos" (
        rmdir %BACKEND_BIN_PATH%"\Release Background\macos" /s /q
    )

    ".\OverrideJSON\OverrideJSON.exe" %APP_SETTINGS_PATH% "Platform" "macos"

    dotnet publish "%BACKEND_CSPROJ_PATH%" --output %BACKEND_BIN_PATH%"\Release Background\macos"  -p:PublishProfile=FolderProfile

    xcopy  %BACKEND_BIN_PATH%"\Release Background\macos" ".\MacOS App bundle\macos\" /E

    echo Macos backend is ready
)

