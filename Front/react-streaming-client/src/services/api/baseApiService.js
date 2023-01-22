class BaseApiService {
    constructor() {
        this._apiBaseUrl = 'http://localhost:5000/';
      }
    
    buildUrl(relativePath){
        return this._apiBaseUrl + relativePath;
    }

    getRequest(url, parameters, deserializeResult, onSuccess, onFail) {
        const xhttp = new XMLHttpRequest();
        xhttp.onreadystatechange = function () {
            if (this.readyState === 4) {
                if (this.status === 200) {
                    var result = deserializeResult && this.response ? JSON.parse(this.response) : this.response;
                    if (onSuccess)
                        onSuccess(result);
                }
                else {
                    if (onFail)
                        onFail();
                }
            }
        };

        if (parameters && parameters.length > 0) {
            url += '?';
            for (let i = 0; i < parameters.length; i++) {
                const parameter = parameters[i];
                url += parameter.name + '=' + parameter.value;
                if (i !== parameters.length - 1)
                    url += '&'
            }
        }

        xhttp.open("GET", url, true);
        xhttp.send();
    }

    postRequest(url, parameters, onSuccess, onFail) {
        const xhttp = new XMLHttpRequest();
        xhttp.onreadystatechange = function () {
            if (this.readyState === 4) {
                if (this.status === 200) {
                    if (onSuccess)
                        onSuccess();
                }
                else {
                    if (onFail)
                        onFail();
                }
            }
        };
        xhttp.open("POST", url, true);
        xhttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        xhttp.send(parameters);
    }

    putRequest(url, obj, onSuccess){
        const xhttp = new XMLHttpRequest();
        xhttp.open("PUT", url, true);
        xhttp.setRequestHeader("Content-Type", "application/json");
        xhttp.send(JSON.stringify(obj));

        xhttp.onreadystatechange = function () {
            if (this.readyState === 4 && this.status === 200 && Boolean(onSuccess))
                onSuccess();
        }
    }

    deleteRequest(url, obj, onSuccess) {
        var xhttp = new XMLHttpRequest();
        xhttp.open("DELETE", url, true);
        xhttp.setRequestHeader("Content-Type", "application/json");
        xhttp.send(JSON.stringify(obj));

        xhttp.onreadystatechange = function () {
            if (this.readyState === 4)
                if (this.status === 200)
                    onSuccess();
        }
    }
  }

export default BaseApiService;