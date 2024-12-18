const { icon } = require('./consts');

module.exports = {
  packagerConfig: {
    name: "Extract Medflix Package",
    productName: "Extract Medflix Package",
    executableName: "extract_package",
    icon: icon
  },
  rebuildConfig: {},
  makers: [
    {
      name: '@electron-forge/maker-squirrel',
      config: {},
    },
    {
      name: '@electron-forge/maker-zip',
      platforms: ['darwin'],
    },
    {
      name: '@electron-forge/maker-deb',
      config: {},
    },
    {
      name: '@electron-forge/maker-rpm',
      config: {},
    },
  ],
};
