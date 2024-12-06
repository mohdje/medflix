using Medflix.Controls.AndroidTv;
using Medflix.Models.Media;
using Medflix.Services;
using Medflix.Pages.AndroidTv;

namespace Medflix.Views.AndroidTv;

public partial class CategoriesView : ContentView
{
    public CategoriesView()
    {
        InitializeComponent();
    }

    private async void OnLoaded(object sender, EventArgs e)
    {
        if (Platforms.Children.Any() || Genres.Children.Any())
            return;

        LoadingSpinner.IsVisible = true;

        Category[] platforms = null;
        Category[] genres = null;
        var tasks = new Task[]
        {
            MedflixApiService.Instance.GetAvailablePlatforms().ContinueWith(t => platforms = t.Result.ToArray()),
            MedflixApiService.Instance.GetAvailableGenres().ContinueWith(t => genres = t.Result.ToArray())
        };

        await Task.WhenAll(tasks);

        if (platforms != null)
        {
            foreach (var platform in platforms)
                Platforms.Children.Add(CreateButton(platform, CategoryType.Platform));
        }

        if (genres != null)
        {
            foreach (var genre in genres)
                Genres.Children.Add(CreateButton(genre, CategoryType.Genre));
        }

        LoadingSpinner.IsVisible = false;
        Categories.IsVisible = true;
    }

    private BorderedButton CreateButton(Category category, CategoryType categoryType)
    {
        var button = new Button
        {
            Text = category.Name,
            BackgroundColor = Color.FromArgb("#332c2a"),
            WidthRequest = 150,
            TextColor = Colors.White
        };
        button.Clicked += async (s, e) =>
        {
            await Navigation.PushAsync(new MediasForCategoryListPage(category, categoryType));
        };

        return new BorderedButton { Button = button };
    }
}