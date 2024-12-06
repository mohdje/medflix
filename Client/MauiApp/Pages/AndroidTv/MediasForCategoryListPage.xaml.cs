using Medflix.Controls.VideoPlayer;
using Medflix.Models.Media;
using Medflix.Services;
using Medflix.Views.AndroidTv;
using System.Runtime.CompilerServices;

namespace Medflix.Pages.AndroidTv;

public partial class MediasForCategoryListPage : ContentPage
{
    int categoryId;
    int currentPage = 1;
    int lastMediasFetchPage = 0;

    CategoryType categoryType;
    List<MediaDetails> medias;
    const int mediasPerPage = 12;
	public MediasForCategoryListPage()
	{
		InitializeComponent();
	}

    public MediasForCategoryListPage(Category category, CategoryType categoryType) : this()
    {
        categoryId = category.Id;
        this.categoryType = categoryType;
        CategoryName.Text = category.Name;
        medias = new List<MediaDetails>();

        LoadNextMediasPageAsync();
    }

    private async Task LoadNextMediasPageAsync()
    {
        LockUI();

        lastMediasFetchPage = currentPage;
        var result = categoryType == CategoryType.Platform ?
            await MedflixApiService.Instance.GetMediasForPlatform(categoryId, currentPage) :
            await MedflixApiService.Instance.GetMediasForGenre(categoryId, currentPage);

        if (result != null && result.Any())
            medias.AddRange(result);

        await UpdateMediasListAsync();
    }

  

    private async Task UpdateMediasListAsync()
    {
        LockUI();

        await Task.Delay(500);

        var mediasToDisplay = GetMediasForPage(currentPage);

        MediaList.Clear();
        foreach (var media in mediasToDisplay)
            MediaList.Add(new MediaLitePresentationView(media));

        PreviousButton.Button.IsEnabled = currentPage > 1;
        NextButton.Button.IsEnabled = GetMediasForPage(currentPage + 1).Any();
        Spinner.IsVisible = false;
        MediaList.IsVisible = true;
    }

    private IEnumerable<MediaDetails> GetMediasForPage(int page)
    {
        var startIndex = (currentPage - 1) * mediasPerPage;
        return medias.Take(new Range(startIndex, startIndex + mediasPerPage));
    }

    private void LockUI()
    {
        PreviousButton.Button.IsEnabled = false;
        NextButton.Button.IsEnabled = false;
        Spinner.IsVisible = true;
        MediaList.IsVisible = false;
    }

    private async void NextButtonClicked(object sender, EventArgs e)
    {
        currentPage++;
        if (currentPage > lastMediasFetchPage)
            await LoadNextMediasPageAsync();
        else
            await UpdateMediasListAsync();
    }

    private async void PreviousButtonClicked(object sender, EventArgs e)
    {
        currentPage--;
        await UpdateMediasListAsync();
    }
}