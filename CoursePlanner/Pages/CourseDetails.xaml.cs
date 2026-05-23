using CoursePlanner.Data;
using CoursePlanner.Models;
using CoursePlanner.Utilities;
using static CoursePlanner.Services.NotificationService;
using CoursePlanner.Services;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace CoursePlanner.Pages;

public partial class CourseDetails : ContentPage
{

    // ===== Data =====
    private readonly AppDatabase _database;
    private readonly Models.Course _selectedCourse;
    private readonly Models.Term _selectedTerm;
    private readonly IShareService _shareService;

    // ===== Constructor =====
    public CourseDetails(AppDatabase database, Models.Course selectedCourse, Models.Term selectedTerm, IShareService shareService)
    {
        InitializeComponent();

        _database = database;
        _selectedCourse = selectedCourse;
        _shareService = shareService;
        _selectedTerm = selectedTerm;
        BindingContext = this;
    }

    // ===== Helper Methods =====
    private async Task<List<Assessment>> GetAssessmentsForCourse()
    {
        try
        {
            return await _database.GetAssessmentsByCourseAsync(_selectedCourse.CourseId);
        }
        catch
        {
            await DisplayAlert("Error", "An unexpected error occurred while loading assessments.", "OK");
            return new List<Assessment>();
        }
    }

    private async Task LoadCourseDetails()
    {
        CourseNameEntry.Text = _selectedCourse.Name;
        CourseStartDatePicker.Date = _selectedCourse.StartDate;
        CourseEndDatePicker.Date = _selectedCourse.DueDate;

        StatusPicker.SelectedItem = _selectedCourse.Status;

        InstructorNameEntry.Text = _selectedCourse.InstructorName;
        InstructorEmailEntry.Text = _selectedCourse.InstructorEmail;
        InstructorPhoneEntry.Text = _selectedCourse.InstructorPhone;

        NotesEditor.Text = _selectedCourse.Notes;

        CourseStartAlertSwitch.IsToggled = _selectedCourse.StartDateNotif;
        CourseEndAlertSwitch.IsToggled = _selectedCourse.EndDateNotif;

        AssessmentsCollectionView.ItemsSource = await GetAssessmentsForCourse();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCourseDetails();
    }
    private async Task<bool> ValidateInput()
    {
        try
        {
            var courseTitle = ValidationResult.ValidateCourseName(CourseNameEntry?.Text?.Trim() ?? string.Empty);
            if (!courseTitle.IsValid)
            {
                await DisplayAlert(courseTitle.ErrorTitle, courseTitle.ErrorMessage, courseTitle.ErrorCancel);
                return false;
            }

            var dates = ValidationResult.ValidateStartEndDates(
                CourseStartDatePicker?.Date ?? DateTime.Today,
                CourseEndDatePicker?.Date ?? DateTime.Today);
            if (!dates.IsValid)
            {
                await DisplayAlert(dates.ErrorTitle, dates.ErrorMessage, dates.ErrorCancel);
                return false;
            }

            var statusString = StatusPicker?.SelectedItem?.ToString() ?? string.Empty;
            var status = ValidationResult.ValidateCourseStatus(statusString);
            if (!status.IsValid)
            {
                await DisplayAlert(status.ErrorTitle, status.ErrorMessage, status.ErrorCancel);
                return false;
            }

            var instructorName = ValidationResult.ValidateInstructorName(InstructorNameEntry?.Text?.Trim() ?? string.Empty);
            if (!instructorName.IsValid)
            {
                await DisplayAlert(instructorName.ErrorTitle, instructorName.ErrorMessage, instructorName.ErrorCancel);
                return false;
            }

            var instructorEmail = ValidationResult.ValidateInstructorEmail(InstructorEmailEntry?.Text?.Trim() ?? string.Empty);
            if (!instructorEmail.IsValid)
            {
                await DisplayAlert(instructorEmail.ErrorTitle, instructorEmail.ErrorMessage, instructorEmail.ErrorCancel);
                return false;
            }

            var instructorPhone = ValidationResult.ValidateInstructorPhone(InstructorPhoneEntry?.Text?.Trim() ?? string.Empty);
            if (!instructorPhone.IsValid)
            {
                await DisplayAlert(instructorPhone.ErrorTitle, instructorPhone.ErrorMessage, instructorPhone.ErrorCancel);
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

    private void UpdateCourseFromInput()
    {
        _selectedCourse.Name = CourseNameEntry.Text?.Trim() ?? string.Empty;
        _selectedCourse.StartDate = CourseStartDatePicker.Date;
        _selectedCourse.DueDate = CourseEndDatePicker.Date;

        _selectedCourse.Status = StatusPicker.SelectedItem?.ToString() ?? string.Empty;

        _selectedCourse.InstructorName = InstructorNameEntry.Text?.Trim() ?? string.Empty;
        _selectedCourse.InstructorPhone = InstructorPhoneEntry.Text?.Trim() ?? string.Empty;
        _selectedCourse.InstructorEmail = InstructorEmailEntry.Text?.Trim() ?? string.Empty;

        _selectedCourse.Notes = NotesEditor.Text?.Trim();

        _selectedCourse.StartDateNotif = CourseStartAlertSwitch.IsToggled;
        _selectedCourse.EndDateNotif = CourseEndAlertSwitch.IsToggled;
    }

    private async Task ScheduleCourseNotifications(Course selectedCourse)
    {
        var title = $"Course: {selectedCourse.Name}";
        var notifStartMessage = $"Course '{selectedCourse.Name}' starts on {selectedCourse.StartDate:MM/dd/yyyy}.";
        var notifEndMessage = $"Course '{selectedCourse.Name}' ends on {selectedCourse.DueDate:MM/dd/yyyy}.";

        if (CourseStartAlertSwitch?.IsToggled == true)
        {
            await ScheduleNotificationAsync(
            selectedCourse.CourseId,
            title,
            notifStartMessage,
            CourseStartDatePicker.Date.AddHours(9),
            AlertType.CourseStart);
        }

        if (CourseEndAlertSwitch?.IsToggled == true)
        {
            await ScheduleNotificationAsync(
            selectedCourse.CourseId,
            title,
            notifEndMessage,
            CourseEndDatePicker.Date.AddHours(9),
            AlertType.CourseEnd);
        }
    }

    // ===== Events =====
    private void DisplayNotesButton_Clicked(object sender, EventArgs e)
    {
        NotesSection.IsVisible = !NotesSection.IsVisible;

        DisplayNotesButton.Text = NotesSection.IsVisible ? "-" : "+";
    }

    private async void ShareNotesButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            var notes = _selectedCourse.Notes ?? string.Empty;
            var notesTitle = $"Course: {_selectedCourse.Name} Notes";

            await _shareService.ShareCourseNotesAsync(notesTitle, notes);
        }
        catch
        {
            await DisplayAlert("Share Notes", "An unexpected error occurred while trying to share the notes.", "OK");
        }
    }

    private async void UpdateCourseButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!await ValidateInput())
                return;

            UpdateCourseFromInput();

            await _database.SaveCourseAsync(_selectedCourse);
            await DisplayAlert("Success", "Course updated successfully.", "OK");

            await ScheduleCourseNotifications(_selectedCourse);
            await Navigation.PopAsync();
        }
        catch
        {
            await DisplayAlert("Error", $"An unexpected error occurred.", "OK");
        }
    }

    private async void DeleteCourseButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            var confirm = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this course?", "Yes", "No");
            if (!confirm)
                return;

            await _database.DeleteCourseAsync(_selectedCourse);
            await DisplayAlert("Deleted", "Course deleted successfully.", "OK");

            await Navigation.PopAsync();
        }
        catch
        {
            await DisplayAlert("Error", $"An unexpected error occurred.", "OK");
        }
    }

    private async void AddAssessmentButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new AddAssessment(_database, _selectedCourse, _selectedTerm));
        }
        catch
        {
            await DisplayAlert("Error", $"An unexpected error occurred.", "OK");
        }
    }

    private async void AssessmentsCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (e.CurrentSelection.FirstOrDefault() is not Models.Assessment selectedAssessment)
                return;
            await Navigation.PushAsync(new AssessmentDetails(_database, _selectedCourse, _selectedTerm, selectedAssessment));
        }
        catch
        {
            await DisplayAlert("Error", $"An unexpected error occurred.", "OK");
        }
    }
}