﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace youtube_center.Classes
{
    // http://brianlagunas.com/a-better-way-to-data-bind-enums-in-wpf/
    public class EnumDescriptionTypeConverter : EnumConverter
    {
        public EnumDescriptionTypeConverter(Type type)
            : base(type)
        {
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value != null)
                {
                    var fi = value.GetType().GetField(value.ToString());
                    if (fi != null)
                    {
                        var attributes =
                            (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        return attributes.Length > 0 && !string.IsNullOrEmpty(attributes[0].Description)
                            ? attributes[0].Description
                            : value.ToString();
                    }
                }

                return string.Empty;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class ViewsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var number = System.Convert.ToInt32(value);

            if (number >= 1_000_000)
            {
                var million = Math.DivRem(number, 1_000_000, out var remainder);
                return $"{million}.{remainder / 1000}m views";
            }

            if (number >= 1000)
                return $"{number / 1000}k views";

            return $"{number} views";
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

    public class WatchedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToBoolean(value) ? "Mark as unwatched" : "Mark as watched";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}