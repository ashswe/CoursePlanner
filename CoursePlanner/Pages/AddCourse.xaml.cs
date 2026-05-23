using CoursePlanner.Data;
using CoursePlanner.Models;
using CoursePlanner.Utilities;
using Plugin.LocalNotification;
using System.Runtime.CompilerServices;
using static CoursePlanner.Services.NotificationService;

namespace CoursePlanner.Pages;

public partial class AddCourse : ContentPage
{
    // ===== Data =====
    private readonly AppDatabase _database;
    private readonly Models.Term _selectedTerm;

    // ===== Constructor =====
    public AddCourse(AppDatabase database, Models.Term selectedTerm)
    {
        InitializeComponent();

        _database = database;
        _selectedTerm = selectedTerm;
    }

    // ===== Helper Methods =====
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

    private async Task<bool> ValidateCourseLoad()
    {
        var coursesInTerm = await _database.GetCoursesByTermAsync(_selectedTerm.TermId);

        var courseLoad = ValidationResult.ValidateCourseLoad(Convert.ToInt32(coursesInTerm.Count));
        if (courseLoad.IsValid)
            return true;
        await DisplayAlert(courseLoad.ErrorTitle, courseLoad.ErrorMessage, courseLoad.ErrorCancel);
        return false;
    }

    private Course CreateCourseFromInput()
    {
        Course newCourse = new()
        {
            TermId = _selectedTerm.TermId,
            Name = CourseNameEntry?.Text?.Trim() ?? string.Empty,
            StartDate = CourseStartDatePicker?.Date ?? DateTime.Today,
            DueDate = CourseEndDatePicker?.Date ?? DateTime.Today,
            Status = StatusPicker?.SelectedItem?.ToString() ?? string.Empty,

            InstructorName = InstructorNameEntry?.Text?.Trim(),
            InstructorPhone = InstructorPhoneEntry?.Text?.Trim(),
            InstructorEmail = InstructorEmailEntry?.Text?.Trim(),

            Notes = NotesEditor?.Text?.Trim(),

            StartDateNotif = CourseStartAlertSwitch?.IsToggled ?? false,
            EndDateNotif = CourseEndAlertSwitch?.IsToggled ?? false
        };

        return newCourse;
    }

    private async Task ScheduleCourseNotifications(Course newCourse)
    {
        string title = $"Course: {newCourse.Name} Notes";

        if (CourseStartAlertSwitch?.IsToggled == true)
        {
            await ScheduleNotificationAsync(
            newCourse.CourseId,
            title,
            NotesEditor.Text?.Trim() ?? string.Empty,
            CourseStartDatePicker.Date.AddHours(9),
            AlertType.CourseStart);
        }
        
        if (CourseEndAlertSwitch?.IsToggled == true)
        {
            await ScheduleNotificationAsync(
            newCourse.CourseId,
            title,
            NotesEditor.Text?.Trim() ?? string.Empty,
            CourseEndDatePicker.Date.AddHours(9),
            AlertType.CourseEnd);
        }
    }

    // ===== Events =====
    private async void AddCourseButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!await ValidateInput())
                return;

            if (!await ValidateCourseLoad())
                return;

            var newCourse = CreateCourseFromInput();

            await _database.SaveCourseAsync(newCourse);
            await DisplayAlert("Success", "Course added successfully.", "OK");

            await ScheduleCourseNotifications(newCourse);
            await Navigation.PopAsync();
        }
        catch
        {
            await DisplayAlert("Error", $"An unexpected error occurred.", "OK");
        }
    }
}