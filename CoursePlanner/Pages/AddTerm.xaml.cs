using CoursePlanner.Data;
using CoursePlanner.Models;
using CoursePlanner.Utilities;

namespace CoursePlanner.Pages;

public partial class AddTerm : ContentPage
{
    // ===== Data =====
    private readonly AppDatabase _database;

    // ===== Constructor =====
    public AddTerm(AppDatabase database)
    {
        InitializeComponent();
        _database = database;
    }

    // ===== Helper Methods =====
    private bool ValidateInput()
    {
        try
        {
            var termTitle = ValidationResult.ValidateTermTitle(TitleEntry.Text.Trim());
            if (!termTitle.IsValid)
            {
                DisplayAlert(termTitle.ErrorTitle, termTitle.ErrorMessage, termTitle.ErrorCancel);
                return false;
            }

            var dates = ValidationResult.ValidateStartEndDates(StartDatePicker.Date, EndDatePicker.Date);
            if (!dates.IsValid)
            {
                DisplayAlert(dates.ErrorTitle, dates.ErrorMessage, dates.ErrorCancel);
                return false;
            }
        }
        catch
        {
            DisplayAlert("Error", $"An unexpected error occurred.", "OK");
            return false;
        }

        return true;
    }

    private Term CreateTermFromInput()
    {
        Term newTerm = new Term
        {
            Title = TitleEntry.Text.Trim(),
            StartDate = StartDatePicker.Date,
            EndDate = EndDatePicker.Date
        };

        return newTerm;
    }   

    // ===== Events =====
    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!ValidateInput())
                return;

            var newTerm = CreateTermFromInput();

            await _database.SaveTermAsync(newTerm);
            await DisplayAlert("Success", $"Term '{newTerm.Title}' added successfully.", "OK");

            await Navigation.PopAsync();
        }
        catch
        {
            await DisplayAlert("Error", $"An unexpected error occurred.", "OK");
            return;
        }
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}