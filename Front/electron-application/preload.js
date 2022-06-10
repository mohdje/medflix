const customTitlebar = require('custom-electron-titlebar');

const stopBackgroundApplication = () => {
  var xhttp = new XMLHttpRequest();
  xhttp.open("GET", 'https://localhost:5001/application/stop', false);
  xhttp.send();
}

window.addEventListener('DOMContentLoaded', () => {
  new customTitlebar.Titlebar({
    menu: null,
    backgroundColor: customTitlebar.Color.fromHex('#000000'),
    onClose: ()=>{
        stopBackgroundApplication();
        window.close();
    }
  });
  document.getElementsByClassName('cet-container')[0].style.overflow = 'hidden';
  document.getElementsByClassName('cet-window-title')[0].innerHTML = '';
  document.getElementsByClassName('cet-window-icon')[0].style.display = 'none';
});