using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace youtube_center.Classes
{
    public class ViewsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var number = System.Convert.ToInt32(value);
            return number >= 1000 ? $"{number / 1000}k views" : $"{number} views";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DateConverter : IValueConverter
    {
        // https://www.codeproject.com/Articles/770323/How-to-Convert-a-Date-Time-to-X-minutes-ago-in-Csh
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dt = System.Convert.ToDateTime(value);

            var span = DateTime.Now - dt;

            if (span.Days > 365)
            {
                int years = (span.Days / 365);
                if (span.Days % 365 != 0)
                    years += 1;
                return $"~{years} {(years == 1 ? "year" : "years")} ago";
            }

            if (span.Days > 30)
            {
                var months = span.Days / 30;
                if (span.Days % 31 != 0)
                    months += 1;
                return $"~{months} {(months == 1 ? "month" : "months")} ago";
            }

            if (span.Days > 0)
                return $"~{span.Days} {(span.Days == 1 ? "day" : "days")} ago";

            if (span.Hours > 0)
                return $"~{span.Hours} {(span.Hours == 1 ? "hour" : "hours")} ago";

            if (span.Minutes > 0)
                return $"~{span.Minutes} {(span.Minutes == 1 ? "minute" : "minutes")} ago";

            if (span.Seconds > 5)
                return $"~{span.Seconds} seconds ago";

            return span.Seconds <= 5 ? "just now" : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToBoolean(value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}