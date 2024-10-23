
namespace Medflix.Services
{
    public class RemoteCommandActionNotifier 
    {
        public event EventHandler OnBackButtonPressed;
        public event EventHandler OnLeftButtonPressed;
        public event EventHandler OnRightButtonPressed;
        public event EventHandler OnButtonPressed;

        public bool PreventBackButton { get; set; }
        public bool PreventLeftButton { get; set; }
        public bool PreventRightButton { get; set; }

        private static RemoteCommandActionNotifier _instance;
        public static RemoteCommandActionNotifier Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new RemoteCommandActionNotifier();

                return _instance;
            }
        }

        private RemoteCommandActionNotifier()
        {
            
        }

        public void NotifyRemoteCommandBackButtonPressed()
        {
            OnBackButtonPressed?.Invoke(this, EventArgs.Empty);
        }

        public void NotifyRemoteCommandLeftButtonPressed()
        {
            OnLeftButtonPressed?.Invoke(this, EventArgs.Empty);
        }

        public void NotifyRemoteCommandRightButtonPressed()
        {
            OnRightButtonPressed?.Invoke(this, EventArgs.Empty);
        }

        public void NotifyRemoteCommandButtonPressed()
        {
            OnButtonPressed?.Invoke(this, EventArgs.Empty);
        }

    }
}
