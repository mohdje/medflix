#!/bin/bash  
parent_path=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )
cd "$parent_path"

#copy swift app 
rm -r './Medflix.app' 
cp -av '../../Desktop Application/Macos/Build/Medflix.app' './Medflix.app'

#copy webhost app in swift app Resources folder
chmod 755 './macos/MedflixWebHost'
cp -av './macos/.' './Medflix.app/Contents/Resources/MedflixWebHost'

#build extract-update-package-electron-app 
cd '../../Desktop Application/extract-update-package-electron-app'
npm run make

cd '../..'

#copy extract-update app in swift app Resources folder
cp -av '../../Desktop Application/extract-update-package-electron-app/out/Extract Medflix Package-darwin-x64/Extract Medflix Package.app' './Medflix.app/Contents/Resources/extract_medflix_package.app'

#create dmg
cd './Build/MacOS App bundle'
rm -r './Medflix.dmg' 
appdmg ./dmg.json ./Medflix.dmg