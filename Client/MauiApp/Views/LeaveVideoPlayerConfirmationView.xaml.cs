namespace Medflix.Views;

public partial class LeaveVideoPlayerConfirmationView : ContentView
{
    public event EventHandler OnConfirm;
    public event EventHandler OnCancel;
    public LeaveVideoPlayerConfirmationView()
    {
        InitializeComponent();

        CancelButton.Button.Unfocused += OnUnfocus;
        ConfirmButton.Button.Unfocused += OnUnfocus;
    }

    private async void OnUnfocus(object? sender, FocusEventArgs e)
    {
        await Task.Delay(150);

       if(IsVisible && !CancelButton.Button.IsFocused && !ConfirmButton.Button.IsFocused) 
            CancelButton.Focus();
    }

    private void OnConfirmationButtonClicked(object sender, EventArgs e)
    {
        OnConfirm?.Invoke(this, EventArgs.Empty);
    }

    private void OnCancelButtonClicked(object sender, EventArgs e)
    {
        OnCancel?.Invoke(this, EventArgs.Empty);
    }

    public void Show()
    {
        IsVisible = true;
        CancelButton.Focus();
    }

    public void Hide()
    {
        IsVisible = false;
        CancelButton.Unfocus();
        ConfirmButton.Unfocus();
    }
}