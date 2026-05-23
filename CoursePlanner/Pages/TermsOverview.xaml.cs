using CoursePlanner.Data;
using CoursePlanner.Models;
using CoursePlanner.Utilities;
using CoursePlanner.Services;

namespace CoursePlanner.Pages;

public partial class TermsOverview : ContentPage
{
    // ===== Data =====
    private readonly AppDatabase _database;
    private readonly IShareService _shareService;

    internal List<Term> Terms { get; set; } = new List<Term>();

    // ===== Constructor =====
    public TermsOverview(AppDatabase database, IShareService shareService)
    {
        InitializeComponent();
        _database = database;
        _shareService = shareService;

        BindingContext = this;
    }

    // ===== Helper Methods =====
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Terms = await _database.GetTermsAsync();
        TermsCollectionView.ItemsSource = null;
        TermsCollectionView.ItemsSource = Terms;
    }

    // ===== Events =====
    private async void AddTermButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddTerm(_database));
    }

    private async void ReportButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProgressReport(_database, _shareService));
    }

    private async void TermsCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (e?.CurrentSelection == null || e.CurrentSelection.Count == 0)
                return;

            if (e.CurrentSelection.FirstOrDefault() is not Term selectedTerm)
                return;

            TermsCollectionView.SelectedItem = null;
            TermsCollectionView.IsEnabled = false;

            try
            {
                await Navigation.PushAsync(new TermDetails(_database, selectedTerm, _shareService));
            }
            finally
            {
                TermsCollectionView.IsEnabled = true;
            }
        }
        catch
        {
                await DisplayAlert("Error", "An error occurred while navigating to term details.", "OK");
        }
    }

}