using System;
using System.Collections.Generic;
using System.Linq;

namespace DicomModifier.Services
{
    public static class PasswordValidation
    {
        public static int MinimumLength { get; set; } = 8;
        public static bool RequireUppercase { get; set; } = true;
        public static bool RequireLowercase { get; set; } = true;
        public static bool RequireDigit { get; set; } = true;
        public static bool RequireSpecialCharacter { get; set; } = true;

        public static bool ValidatePassword(string password, out string errorMessage)
        {
            var errors = new List<string>();
            if (password.Length < 8)
                errors.Add("La password deve essere lunga almeno 8 caratteri.");
            if (!password.Any(char.IsUpper))
                errors.Add("La password deve contenere almeno una lettera maiuscola.");
            if (!password.Any(char.IsLower))
                errors.Add("La password deve contenere almeno una lettera minuscola.");
            if (!password.Any(char.IsDigit))
                errors.Add("La password deve contenere almeno una cifra.");
            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
                errors.Add("La password deve contenere almeno un carattere speciale.");

            errorMessage = string.Join(Environment.NewLine, errors);
            return errors.Count == 0;
        }

    }
}
