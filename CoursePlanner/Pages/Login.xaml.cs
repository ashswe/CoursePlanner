using CoursePlanner.Data;
using CoursePlanner.Services;
using CoursePlanner.Utilities;
using CoursePlanner.Models;
using CoursePlanner.Pages;

namespace CoursePlanner.Pages;

public partial class Login : ContentPage
{
    // ===== Fields =====
    private readonly AppDatabase _database;
    private readonly AuthService _authService;
    private readonly IShareService _shareService;

    // ===== Constructor =====
    public Login(AppDatabase database, AuthService authService, IShareService shareService)
	{
		InitializeComponent();
        _database = database;
        _authService = authService;
        _shareService = shareService;
    }

    // ===== Helper Methods =====
    private async Task<bool> ValidateInput()
    {
        try
        {
            var username = UsernameEntry.Text?.Trim();
            var password = PasswordEntry.Text.Trim();

            var usernameValidation = ValidationResult.ValidateUserName(username);
            if (!usernameValidation.IsValid)
            {
                await DisplayAlert(usernameValidation.ErrorTitle, usernameValidation.ErrorMessage, usernameValidation.ErrorCancel);
                return false;
            }

            var passwordValidation = ValidationResult.ValidatePassword(password);
            if (!passwordValidation.IsValid)
            {
                await DisplayAlert(passwordValidation.ErrorTitle, passwordValidation.ErrorMessage, passwordValidation.ErrorCancel);
                return false;
            }

            return true;
        }
        catch
        {
            await DisplayAlert("Validation Error", "An unexpected error occurred during validation. Please try again.", "OK");
            return false;
        }
    }



    // ===== Events =====
    private async void RegisterButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!await ValidateInput())
                return;

            var username = UsernameEntry.Text.Trim();
            var password = PasswordEntry.Text.Trim();

            bool registrationSuccess = await _authService.RegisterUserAsync(username, password);

            await DisplayAlert(
                registrationSuccess ? "Registration Successful" : "Registration Failed",
                registrationSuccess ? "Your account has been created. You can now log in." : "Username already exists. Please choose a different username.",
                "OK"
            );
        }
        catch
        {
            await DisplayAlert("Registration Error", "An unexpected error occurred during registration. Please try again.", "OK");
        }
    }

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!await ValidateInput())
                return;

            var username = UsernameEntry.Text.Trim();
            var password = PasswordEntry.Text.Trim();

            var user = await _authService.LoginUserAsync(username, password);

            if (user == null)
            {
                await DisplayAlert("Login Failed", "Invalid username or password. Please try again.", "OK");
                return;
            }

            Preferences.Set("LoggedInUserId", user.UserId);
            Preferences.Set("LoggedInUsername", user.Username);

            await DisplayAlert("Login Successful", $"Welcome back, {user.Username}!", "OK");
             
            await Navigation.PushAsync(new TermsOverview(_database, _shareService));
        }
        catch
        {
            await DisplayAlert("Login Error", "An unexpected error occurred during login. Please try again.", "OK");
        }
    }

}