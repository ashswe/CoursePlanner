using CoursePlanner.Data;
using CoursePlanner.Models;
using CoursePlanner.Utilities;
using System.Runtime.CompilerServices;
using static CoursePlanner.Services.NotificationService;

namespace CoursePlanner.Pages;

public partial class AssessmentDetails : ContentPage
{
    // ===== Data =====
    private readonly AppDatabase _database;
    private readonly Models.Course _selectedCourse;
    private readonly Models.Term _selectedTerm;
    private readonly Models.Assessment? _selectedAssessment;

    // ===== Constructors =====
    public AssessmentDetails(AppDatabase database, Models.Course selectedCourse, Models.Term selectedTerm, Models.Assessment selectedAssessment)
	{
		InitializeComponent();
        _database = database;
        _selectedCourse = selectedCourse;
        _selectedTerm = selectedTerm;
        _selectedAssessment = selectedAssessment;
    }

    public AssessmentDetails(AppDatabase database, Models.Course selectedCourse, Models.Term selectedTerm)
    {
        InitializeComponent();
        _database = database;
        _selectedCourse = selectedCourse;
        _selectedTerm = selectedTerm;
    }

    // ===== Helper Methods =====
    private async Task LoadAssessmentDetails()
    {
        if (_selectedAssessment != null)
        {
            AssessmentTitleEntry.Text = _selectedAssessment.Title;
            AssessmentTypePicker.SelectedItem = _selectedAssessment.Type;
            StartDatePicker.Date = _selectedAssessment.StartDate;
            EndDatePicker.Date = _selectedAssessment.EndDate;

            AssessmentStartNotifSwitch.IsToggled = _selectedAssessment.StartDateNotif;
            AssessmentEndNotifSwitch.IsToggled = _selectedAssessment.EndDateNotif;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAssessmentDetails();
    }

    private async Task<bool> ValidateInput()
    {
        try
        {
            var title = ValidationResult.ValidateAssessmentTitle(AssessmentTitleEntry?.Text?.Trim() ?? string.Empty);
            if (!title.IsValid)
            {
                await DisplayAlert(title.ErrorTitle, title.ErrorMessage, title.ErrorCancel);
                return false;
            }

            var type = await ValidationResult.ValidateAssessmentType(_database, AssessmentTypePicker?.SelectedItem?.ToString() ?? string.Empty, _selectedCourse, _selectedAssessment?.AssessmentId ?? 0);
            if (!type.IsValid)
            {
                await DisplayAlert(type.ErrorTitle, type.ErrorMessage, type.ErrorCancel);
                return false;
            }

            var dates = ValidationResult.ValidateAssessmentDates(StartDatePicker.Date, EndDatePicker.Date);
            if (!dates.IsValid)
            {
                await DisplayAlert(dates.ErrorTitle, dates.ErrorMessage, dates.ErrorCancel);
                return false;
            }
        }
        catch
        {
            await DisplayAlert("Error", "An unexpected error occurred.", "OK");
            return false;
        }

        return true;
    }

    private async Task UpdateAssessmentFromInput()
    {
        if (_selectedAssessment != null)
        {
            _selectedAssessment.Title = AssessmentTitleEntry.Text?.Trim() ?? string.Empty;
            _selectedAssessment.Type = AssessmentTypePicker.SelectedItem?.ToString() ?? string.Empty;
            _selectedAssessment.StartDate = StartDatePicker.Date;
            _selectedAssessment.EndDate = EndDatePicker.Date;
            _selectedAssessment.StartDateNotif = AssessmentStartNotifSwitch.IsToggled;
            _selectedAssessment.EndDateNotif = AssessmentEndNotifSwitch.IsToggled;
        }
    }

    private async Task ScheduleAssessmentNotifications(Assessment selectedAssessment)
    {
        var title = $"Assessment: {selectedAssessment.Title}";
        var notifStartMessage = $"Course: {_selectedAssessment.Title} starts on {selectedAssessment.StartDate:MM/dd}.";
        var notifEndMessage = $"Course: {_selectedAssessment.Title} ends on {selectedAssessment.EndDate:MM/dd}.";

        if (AssessmentStartNotifSwitch?.IsToggled == true)
        {
            await ScheduleNotificationAsync(
            selectedAssessment.CourseId,
            title,
            notifStartMessage,
            StartDatePicker.Date.AddHours(9),
            AlertType.CourseStart);
        }

        if (AssessmentEndNotifSwitch?.IsToggled == true)
        {
            await ScheduleNotificationAsync(
            selectedAssessment.CourseId,
            title,
            notifEndMessage,
            EndDatePicker.Date.AddHours(9),
            AlertType.CourseEnd);
        }
    }

    // ==== Events =====
    private async void UpdateButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!await ValidateInput())
                return;

            await UpdateAssessmentFromInput();

            await _database.SaveAssessmentAsync(_selectedAssessment);
            await DisplayAlert("Success", "Assessment updated successfully.", "OK");

            await ScheduleAssessmentNotifications(_selectedAssessment);
            await Navigation.PopAsync();
        }
        catch
        {
            await DisplayAlert("Error", "An unexpected error occurred while saving the assessment.", "OK");
        }
    }

    private async void DeleteButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            var confirm = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this assessment?", "Yes", "No");
            if (!confirm)
                return;

            await _database.DeleteAssessmentAsync(_selectedAssessment);
            await DisplayAlert("Deleted", "Assessment deleted successfully.", "OK");

            await Navigation.PopAsync();
        }
        catch
        {
            await DisplayAlert("Error", "An unexpected error occurred while deleting the assessment.", "OK");
        }
    }
}