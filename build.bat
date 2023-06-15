@echo off

set params=%1

if not "%params:web=%"=="%params%" (  
   npm run build --prefix ".\Front\react-streaming-client"
    
    if exist ".\WebHostStreaming\WebHostStreaming\bin\Release\net6.0" (
        rmdir ".\WebHostStreaming\WebHostStreaming\bin\Release\net6.0" /s /q
    )

     "D:\Projets\ReplaceLine\ReplaceLine\bin\Release\net6.0\ReplaceLine.exe" ".\WebHostStreaming\WebHostStreaming\appsettings.json" 15 """Platform"":""web"""

    "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" ".\WebHostStreaming\WebHostStreaming.sln" /p:Configuration="Release" /p:Platform="Any CPU"

    xcopy ".\Front\react-streaming-client\build\" ".\WebHostStreaming\WebHostStreaming\bin\Release\net6.0\view\" /E

    echo Web application is ready
)

if not "%params:win=%"=="%params%" (  
    npm run build --prefix ".\Front\react-streaming-client"

    if exist "Front\desktop-electron-app\views\app" (
        rmdir "Front\desktop-electron-app\views\app" /s /q
    )
    
    xcopy "Front\react-streaming-client\build\" "Front\desktop-electron-app\views\app\" /E

    npm run make --prefix ".\Front\desktop-electron-app"

    npm run make --prefix ".\Front\extract-update-package-electron-app"

    if exist ".\WebHostStreaming\WebHostStreaming\bin\Release Background\net6.0" (
        rmdir ".\WebHostStreaming\WebHostStreaming\bin\Release Background\net6.0" /s /q
    )

    "D:\Projets\ReplaceLine\ReplaceLine\bin\Release\net6.0\ReplaceLine.exe" "WebHostStreaming\WebHostStreaming\appsettings.json" 15 """Platform"":""windows"""

    "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" ".\WebHostStreaming\WebHostStreaming.sln" /p:Configuration="Release Background" /p:Platform="Any CPU"

    xcopy ".\Front\desktop-electron-app\out\Medflix-win32-x64\" ".\WebHostStreaming\WebHostStreaming\bin\Release Background\net6.0\windows-app\" /E
    xcopy ".\Front\extract-update-package-electron-app\out\Extract Medflix Package-win32-x64\" ".\WebHostStreaming\WebHostStreaming\bin\Release Background\net6.0\extract-update\" /E

    echo Windows application is ready
)


