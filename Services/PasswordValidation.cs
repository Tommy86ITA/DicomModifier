// Interfaces/PasswordValidation.cs

namespace DicomModifier.Services
{
    // This static class contains methods and properties for validating passwords based on specific requirements.
    public static class PasswordValidation
    {
        // Minimum length required for the password.
        public static int MinimumLength { get; set; } = 8;
        // Flag to determine if at least one uppercase letter is required.
        public static bool RequireUppercase { get; set; } = true;
        // Flag to determine if at least one lowercase letter is required.
        public static bool RequireLowercase { get; set; } = true;
        // Flag to determine if at least one digit is required.
        public static bool RequireDigit { get; set; } = true;
        // Flag to determine if at least one special character is required.
        public static bool RequireSpecialCharacter { get; set; } = true;

        // This method validates the password against the set requirements and returns an error message if validation fails.
        public static bool ValidatePassword(string password, out string errorMessage)
        {
            // List to hold error messages.
            var errors = new List<string>();

            // Check if the password meets the minimum length requirement.
            if (password.Length < MinimumLength)
                errors.Add("The password must be at least 8 characters long.");
            // Check if the password contains at least one uppercase letter.
            if (RequireUppercase && !password.Any(char.IsUpper))
                errors.Add("The password must contain at least one uppercase letter.");
            // Check if the password contains at least one lowercase letter.
            if (RequireLowercase && !password.Any(char.IsLower))
                errors.Add("The password must contain at least one lowercase letter.");
            // Check if the password contains at least one digit.
            if (RequireDigit && !password.Any(char.IsDigit))
                errors.Add("The password must contain at least one digit.");
            // Check if the password contains at least one special character.
            if (RequireSpecialCharacter && !password.Any(ch => !char.IsLetterOrDigit(ch)))
                errors.Add("The password must contain at least one special character.");

            // Combine all error messages into a single string.
            errorMessage = string.Join(Environment.NewLine, errors);
            // Return true if no errors were found, otherwise false.
            return errors.Count == 0;
        }
    }
}
