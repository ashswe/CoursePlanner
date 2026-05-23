using CoursePlanner.Data;
using CoursePlanner.Models;
using CoursePlanner.Utilities;

namespace CoursePlanner.Pages;

public partial class AddAssessment : ContentPage
{
    // ===== Data =====
    private readonly AppDatabase _database;
    private readonly Models.Course _selectedCourse;
    readonly Models.Term _selectedTerm;

    // ===== Constructor =====
    public AddAssessment(AppDatabase database, Models.Course selectedCourse, Models.Term selectedTerm)
	{
		InitializeComponent();
        _database = database;
        _selectedCourse = selectedCourse;
        _selectedTerm = selectedTerm;
    }

    // ===== Helper Methods =====
    private Assessment CreateAssessmentFromInput()
    {
        var newAssessment = new Models.Assessment
        {
            CourseId = _selectedCourse.CourseId,
            Title = AssessmentTitleEntry.Text.Trim(),
            Type = AssessmentTypePicker.SelectedItem.ToString(),
            StartDate = AssessmentStartDatePicker.Date,
            EndDate = AssessmentEndDatePicker.Date,
            StartDateNotif = AssessmentStartAlertSwitch.IsToggled,
            EndDateNotif = AssessmentEndAlertSwitch.IsToggled
        };

        return newAssessment;
    }

    private async Task<bool> ValidateInput()
    {
        try
        {
            var assessmentTitle = ValidationResult.ValidateAssessmentTitle(AssessmentTitleEntry?.Text?.Trim() ?? string.Empty);
            if (!assessmentTitle.IsValid)
            {
                await DisplayAlert(assessmentTitle.ErrorTitle, assessmentTitle.ErrorMessage, assessmentTitle.ErrorCancel);
                return false;
            }

            var dates = ValidationResult.ValidateAssessmentDates(
                AssessmentStartDatePicker?.Date ?? DateTime.Today,
                AssessmentEndDatePicker?.Date ?? DateTime.Today);
            if (!dates.IsValid)
            {
                await DisplayAlert(dates.ErrorTitle, dates.ErrorMessage, dates.ErrorCancel);
                return false;
            }

            var assessmentType = await ValidationResult.ValidateAssessmentType(_database, AssessmentTypePicker.SelectedItem.ToString()!, _selectedCourse);
            if (!assessmentType.IsValid)
            {
                await DisplayAlert(assessmentType.ErrorTitle, assessmentType.ErrorMessage, assessmentType.ErrorCancel);
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


    // ===== Event Handlers =====
    private async void AddAssessmentButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!await ValidateInput())
                return;
            var newAssessment = CreateAssessmentFromInput();

            await _database.SaveAssessmentAsync(newAssessment);
            await DisplayAlert("Success", "Assessment added successfully.", "OK");
            await Navigation.PopAsync();
        }
        catch
        {
            await DisplayAlert("Error", "An unexpected error occurred while saving the assessment.", "OK");
        }
    }
}