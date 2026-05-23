using CoursePlanner.Data;
using CoursePlanner.Models;
using CoursePlanner.Utilities;
using System.Collections.ObjectModel;
using CoursePlanner.Services;

namespace CoursePlanner.Pages;

public partial class TermDetails : ContentPage
{
    // ===== Data =====
    private readonly AppDatabase _database;
    private readonly Models.Term _selectedTerm;
    private readonly IShareService _shareService;

    internal List<Course> Courses { get; set; } = new List<Course>();

    // ===== Constructor =====
    public TermDetails(AppDatabase database, Models.Term selectedTerm, IShareService shareService)
    {
        InitializeComponent();
        _database = database;
        _selectedTerm = selectedTerm;
        _shareService = shareService;
        BindingContext = this;
    }

    // ===== Helper Methods =====
    private async Task LoadTermDetails()
    {
        TermTitleEntry.Text = _selectedTerm.Title.Trim();
        TermStartDatePicker.Date = _selectedTerm.StartDate;
        TermEndDatePicker.Date = _selectedTerm.EndDate;

        Courses = await _database.GetCoursesByTermAsync(_selectedTerm.TermId);

        CoursesCollectionView.ItemsSource = null;
        CoursesCollectionView.ItemsSource = Courses;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadTermDetails();
    }

    private bool ValidateInput()
    {
        try
        {
            _selectedTerm.Title = TermTitleEntry.Text.Trim();
            _selectedTerm.StartDate = TermStartDatePicker.Date;
            _selectedTerm.EndDate = TermEndDatePicker.Date;

            var termTitle = ValidationResult.ValidateTermTitle(_selectedTerm.Title);
            if (!termTitle.IsValid)
            {
                DisplayAlert(termTitle.ErrorTitle, termTitle.ErrorMessage, termTitle.ErrorCancel);
                return false;
            }

            var dates = ValidationResult.ValidateStartEndDates(_selectedTerm.StartDate, _selectedTerm.EndDate);
            if (!dates.IsValid)
            {
                DisplayAlert(dates.ErrorTitle, dates.ErrorMessage, dates.ErrorCancel);
                return false;
            }

            return true;
        }
        catch 
        {
            DisplayAlert("Error", $"An unexpected error occurred.", "OK");
            return false;
        }
    }

    // ===== Events =====
    private async void UpdateTermButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!ValidateInput())
                return;

            await _database.SaveTermAsync(_selectedTerm);
            await DisplayAlert("Success", "Term updated successfully.", "OK");

            await Navigation.PopAsync();
        }
        catch
        {
            await DisplayAlert("Error", $"An unexpected error occurred while updating the term.", "OK");
        }
    }

    private async void DeleteTermButton_Clicked(object sender, EventArgs e)
    {
        var confirm = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this term?", "Yes", "No");
        if (confirm)
        {
            await _database.DeleteTermAsync(_selectedTerm);
            await DisplayAlert("Success", "Term deleted successfully.", "OK");

            await Navigation.PopAsync();
        }
    }

    private async void AddCourseButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddCourse(_database, _selectedTerm));
    }

    private async void CoursesCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (e?.CurrentSelection == null || e.CurrentSelection.Count == 0)
                return;

            if (e.CurrentSelection.FirstOrDefault() is not Course selectedCourse)
                return;

            CoursesCollectionView.SelectedItem = null;
            CoursesCollectionView.IsEnabled = false;

            try
            {
                await Navigation.PushAsync(new CourseDetails(_database, selectedCourse, _selectedTerm, _shareService));
            }
            finally
            {
                CoursesCollectionView.IsEnabled = true;
            }
        }
        catch
        {
            await DisplayAlert("Error", $"An unexpected error occurred while opening the course.", "OK");
        }
    }
}