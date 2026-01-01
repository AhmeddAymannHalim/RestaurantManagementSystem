using System.Text.RegularExpressions;

namespace RestaurantManageSystem.Application.Utilities
{
    public static class InputSanitizer
    {
        /// <summary>
        /// Sanitizes input to prevent XSS attacks
        /// </summary>
        public static string SanitizeXss(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Remove dangerous HTML/script tags
            var sanitized = Regex.Replace(input, @"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>", "", RegexOptions.IgnoreCase);
            sanitized = Regex.Replace(sanitized, @"<iframe\b[^<]*(?:(?!<\/iframe>)<[^<]*)*<\/iframe>", "", RegexOptions.IgnoreCase);
            sanitized = Regex.Replace(sanitized, @"on\w+\s*=", "", RegexOptions.IgnoreCase);

            return sanitized;
        }

        /// <summary>
        /// Sanitizes SQL input to reduce SQL injection risk
        /// Note: Use parameterized queries primarily; this is a secondary defense
        /// </summary>
        public static string SanitizeSql(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Replace dangerous SQL characters
            return input
                .Replace("'", "''")
                .Replace(";", "")
                .Replace("--", "")
                .Replace("/*", "")
                .Replace("*/", "");
        }

        /// <summary>
        /// Validates email format
        /// </summary>
        public static bool IsValidEmail(string? email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Sanitizes phone number (removes invalid characters)
        /// </summary>
        public static string SanitizePhoneNumber(string? phone)
        {
            if (string.IsNullOrEmpty(phone))
                return string.Empty;

            return Regex.Replace(phone, @"[^0-9\+\-\(\)\s]", "");
        }

        /// <summary>
        /// Removes potentially dangerous characters from filenames
        /// </summary>
        public static string SanitizeFileName(string? fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            var invalidChars = System.IO.Path.GetInvalidFileNameChars();
            return new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());
        }
    }
}
