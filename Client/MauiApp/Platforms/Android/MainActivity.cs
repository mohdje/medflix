using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Medflix.Services;

namespace Medflix
{
   
    [Activity(Theme = "@style/Maui.SplashTheme",MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]

    [IntentFilter(new[] { "android.intent.action.MAIN" }, Categories = new[] { "android.intent.category.LEANBACK_LAUNCHER" })]
    // "@style/Maui.SplashTheme"
    public class MainActivity : MauiAppCompatActivity
    {

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {

            if (keyCode == Keycode.Back && RemoteCommandActionNotifier.Instance.PreventBackButton) 
            {
                RemoteCommandActionNotifier.Instance.NotifyRemoteCommandBackButtonPressed();
                return true;
            }
            else if (keyCode == Keycode.DpadLeft && RemoteCommandActionNotifier.Instance.PreventLeftButton)
            {
                RemoteCommandActionNotifier.Instance.NotifyRemoteCommandLeftButtonPressed();
                return true;
            }
            else if (keyCode == Keycode.DpadRight && RemoteCommandActionNotifier.Instance.PreventRightButton)
            {
                RemoteCommandActionNotifier.Instance.NotifyRemoteCommandRightButtonPressed();
                return true;
            }
            else
            {
                RemoteCommandActionNotifier.Instance.NotifyRemoteCommandButtonPressed();
            }
            return base.OnKeyDown(keyCode, e);
        }
    }
}