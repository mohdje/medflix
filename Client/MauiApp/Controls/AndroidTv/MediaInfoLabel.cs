using Microsoft.Maui.Controls.Shapes;

namespace Medflix.Controls.AndroidTv
{
	public class MediaInfoLabel : ContentView
	{
		public string Text
		{
			get
			{
				return Label.Text;
			}
			set
			{
				IsVisible = !string.IsNullOrEmpty(value);
				Label.Text = value;
			}
		}
		private Label Label;
		public MediaInfoLabel()
		{
			Label = new Label { FontAttributes = FontAttributes.Bold, TextColor = Brush.White.Color };
			Content = new Border
			{
				BackgroundColor = Color.FromArgb("#505050"),
				Padding = new Thickness(5),
				Opacity = 0.7,
				StrokeShape = new RoundRectangle
				{
					CornerRadius = new CornerRadius(7)
				},
				StrokeThickness = 0,
				Content = Label
			};
		}
	}
}