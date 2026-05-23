using Microsoft.Maui.ApplicationModel.Communication;
//using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using CoursePlanner.Data;

namespace CoursePlanner.Utilities
{
    class ValidationResult
    {
        // ===== Properties =====
        public bool IsValid { get; }
        public string ErrorMessage { get; }
        public string ErrorTitle { get; }
        public string ErrorCancel { get; }

        // ===== Constructor =====
        private ValidationResult(bool isValid, string errorTitle, string errorMessage, string errorCancel)
        {
            IsValid = isValid;

            ErrorTitle = errorTitle;
            ErrorMessage = errorMessage;
            ErrorCancel = errorCancel;
        }

        public static ValidationResult Success()
            => new(true, "", "", "");

        public static ValidationResult Fail(string errorTitle, string errorMessage, string errorCancel)
            => new(false, errorTitle, errorMessage, errorCancel);

        // ===== Input Validators ======
        // USER VALIDATORS
        public static ValidationResult ValidateUserName(string name)
        {
            Regex validCharacters = new Regex(@"^[a-zA-Z0-9\s]+$");

            if (string.IsNullOrEmpty(name))
                return Fail("Error", "Please enter a username.", "OK");
            if (name.Length < 3)
                return Fail("Error", "Username must be at least 3 characters long.", "OK");
            else if (!validCharacters.IsMatch(name))
                return Fail("Error", "Username can only contain letters, numbers, and spaces.", "OK");

            return Success();
        }

        public static ValidationResult ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return Fail("Error", "Please enter a password.", "OK");
            if (password.Length < 6)
                return Fail("Error", "Password must be at least 6 characters long.", "OK");

            return Success();
        }


        // TERM VALIDATORS
        public static ValidationResult ValidateTermTitle(string title)
        {
            Regex validCharacters = new Regex(@"^[a-zA-Z0-9\s]+$");

            if (string.IsNullOrEmpty(title))
                return Fail("Error", "Please enter a term title.", "OK");

            if (title.Length < 3)
                return Fail("Error", "Term title must be at least 3 characters long.", "OK");

            else if (!validCharacters.IsMatch(title))
                return Fail("Error", "Term title can only contain letters, numbers, and spaces.", "OK");

            return Success();
        }

        public static ValidationResult ValidateStartEndDates(DateTime startDate, DateTime endDate)
        {
            if (startDate == endDate)
                return Fail("Error", "Start Date cannot be the same as End Date.", "OK");

            if (startDate > endDate)
                return Fail("Error", "Start Date cannot be after End Date.", "OK");

            if (endDate < DateTime.Today)
                return Fail("Error", "End Date cannot be in the past.", "OK ");

            return Success();
        }

        // COURSE VALIDATORS
        public static ValidationResult ValidateCourseName(string name)
        {
            Regex lettersOnly = new Regex(@"^[a-zA-Z0-9 ]+$");

            if (string.IsNullOrEmpty(name))
                return Fail("Error", "Please enter a course name.", "OK");

            if (!lettersOnly.IsMatch(name))
                return Fail("Error", "Course Name must contain only letters, numbers, and spaces.", "OK");

            return Success();
        }

        public static ValidationResult ValidateCourseStatus(string status)
        {
            string[] validStatuses = { "In Progress", "Completed", "Dropped", "Plan to Take" };

            if (string.IsNullOrEmpty(status))
                return Fail("Error", "Please select a course status.", "OK");

            if (!validStatuses.Contains(status))
                return Fail("Error", "Please select a valid course status.", "OK");
            return Success();
        }

        public static ValidationResult ValidateInstructorName(string name)
        {
            Regex lettersOnly = new Regex(@"^[a-zA-Z ]+$");

            if (string.IsNullOrEmpty(name))
                return Fail("Error", "Please enter an instructor name.", "OK");

            if (!lettersOnly.IsMatch(name))
                return Fail("Error", "Instructor Name must contain only letters.", "OK");

            return Success();
        }

        public static ValidationResult ValidateInstructorEmail(string email)
        {
            Regex emailPattern = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            if (string.IsNullOrEmpty(email))
                return Fail("Error", "Please enter an instructor email.", "OK");

            if (!emailPattern.IsMatch(email))
                return Fail("Error", "Please enter a valid instructor email.", "OK");

            return Success();
        }

        public static ValidationResult ValidateInstructorPhone(string phone)
        {
            Regex validCharacters = new Regex(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$");

            if (string.IsNullOrEmpty(phone))
                return Fail("Error", "Please enter an instructor phone number.", "OK");

            if (!validCharacters.IsMatch(phone))
                return Fail("Error", "Please enter a valid instructor phone number.", "OK");

            return Success();
        }

        public static ValidationResult ValidateCourseLoad(int courses)
        {
            if (courses >= 6)
                return Fail("Error", "Only 6 courses are allowed per term.", "OK");

            return Success();
        }

        // ASSESSMENT VALIDATORS
        public static ValidationResult ValidateAssessmentTitle(string title)
        {
            Regex validCharacters = new Regex(@"^[a-zA-Z0-9\s]+$");

            if (string.IsNullOrEmpty(title))
                return Fail("Error", "Please enter an assessment title.", "OK");

            if (!validCharacters.IsMatch(title))
                return Fail("Error", "Please enter a valid assessment title.", "OK");

            return Success();
        }

        public static ValidationResult ValidateAssessmentDates(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                return Fail("Error", "Start Date cannot be after End Date.", "OK");

            if (endDate < DateTime.Today)
                return Fail("Error", "End Date cannot be in the past.", "OK ");

            return Success();
        }

        public static async Task<ValidationResult> ValidateAssessmentType(
            AppDatabase database,
            string type,
            Models.Course selectedCourse,
            int currentAssessmentId = 0)
        {
            var courseAssessments = await database.GetAssessmentsByCourseAsync(selectedCourse.CourseId);

            bool duplicateExists = courseAssessments.Any(a => a.Type == type && a.AssessmentId != currentAssessmentId);

            if (duplicateExists)
                return Fail("Error", "This course already has an assessment of the selected type. Each course can only have one Performance and one Objective assessment.", "OK");

            return Success();
        }
    }
}
