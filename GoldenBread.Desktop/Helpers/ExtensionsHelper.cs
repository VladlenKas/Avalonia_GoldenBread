using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Helpers
{
    public static class DateExtensions
    {
        private const string DateFormat = "dd.MM.yyyy";

        /// <summary>
        /// Convert DateOnly to String format dd.MM.yyyy
        /// </summary>
        public static string ToDateString(this DateOnly? date)
        {
            return date?.ToString(DateFormat, CultureInfo.InvariantCulture) ?? string.Empty;
        }

        /// <summary>
        /// Convert String to DateOnly. Retyrn null if format invalid
        /// </summary>
        public static DateOnly? ToDateOnly(this string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;

            if (DateOnly.TryParseExact(dateString, DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateOnly date))
            {
                return date;
            }

            return null;
        }

        /// <summary>
        /// Convert String to DateOnly with default value 
        /// </summary>
        public static DateOnly ToDateOnlyOrDefault(this string dateString, DateOnly defaultValue = default)
        {
            return dateString.ToDateOnly() ?? defaultValue;
        }

    }
}
