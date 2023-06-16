#!/bin/bash  
parent_path=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )
cd "$parent_path"

#re-create app bundle from scratch
rm -r './Medflix.app' 
mkdir './Medflix.app' 

#create app bundle folders
mkdir './Medflix.app/Contents'
mkdir './Medflix.app/Contents/MacOS'
mkdir './Medflix.app/Contents/Resources'

#copy app bundle info files
cp -av './Info.plist' './Medflix.app/Contents'
cp -av './PkgInfo' './Medflix.app/Contents'

#copy app icon in app bundle Resources folder 
cp -av './Medflix.icns' './Medflix.app/Contents/Resources'

#copy backend app in app bundle MacOS folder
cp -av './macos/.' './Medflix.app/Contents/MacOS'

#build desktop electron app
cd '../../Front/desktop-electron-app'
npm run make

cd '../..'

#copy desktop electron app in app bundle MacOS folder
cp -av './Front/desktop-electron-app/out/Medflix-darwin-x64/Medflix.app' './Build/MacOS App bundle/Medflix.app/Contents/MacOS'

#build extract-update-package-electron-app 
cd './Front/extract-update-package-electron-app'
npm run make

cd '../..'

#update extract-update app in MedflixFiles
mkdir './Medflix.app/Contents/MacOS/extract-update'
cp -av './Front/extract-update-package-electron-app/out/Extract Medflix Package-darwin-x64/Extract Medflix Package.app' './Build/MacOS App bundle/Medflix.app/Contents/MacOS/extract-update'

#create dmg
cd './Build/MacOS App bundle'
appdmg ./dmg.json ./Medflix.dmg