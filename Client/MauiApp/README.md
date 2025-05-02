## How to publish client applications

# Windows
```bash
dotnet publish -f net8.0-windows10.0.26100.0 -c Release -p:WindowsPackageType=None -p:WindowsAppSDKSelfContained=true --self-contained 
```

# Macos 
```bash
chdir ~/.dotnet
./dotnet publish "/Users/mohamed/Documents/Medflix macos/development/Client/MauiApp/Medflix.csproj" -f net8.0-maccatalyst -c Release --self-contained
```

# Android
```bash
dotnet publish -f net8.0-android -c Release -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=medflix.keystore -p:AndroidSigningKeyAlias=medflix -p:AndroidSigningKeyPass={password} -p:AndroidSigningStorePass={password}
```