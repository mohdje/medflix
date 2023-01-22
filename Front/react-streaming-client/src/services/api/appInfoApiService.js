import BaseApiService from "./baseApiService";

class AppInfoApiService extends BaseApiService {
    buildAppInfoUrl(url){
        return this.buildUrl('application/' + (url ? url : ''));
    }

    isDesktopApplication(onSuccess, onFail) {
        if (this.isDesktopApp) onSuccess(this.isDesktopApp);
        else {
            const self = this;
            this.getRequest(this.buildAppInfoUrl('platform'), null, true, (response) => {
                self.isDesktopApp = response.isDesktopApplication;
                onSuccess(response.isDesktopApplication);
            }, onFail);
        }
    }

    playWithVlc(streamUrl, onSuccess, onFail){
        var parameters = [
            {
                name: 'data',
                value: btoa(streamUrl)
            }
        ];

        this.getRequest(this.buildAppInfoUrl('startvlc'), parameters, false, onSuccess, onFail);
    }
}

export default AppInfoApiService;
  