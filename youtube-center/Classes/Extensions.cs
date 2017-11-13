using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace youtube_center.Classes
{
    public static class Extensions
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        // http://www.pavey.me/2015/04/aspnet-c-extension-method-to-get-enum.html
        public static string Description(this Enum value)
        {
            // variables  
            var enumType = value.GetType();
            var field = enumType.GetField(value.ToString());
            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

            // return  
            return attributes.Length == 0 ? value.ToString() : ((DescriptionAttribute)attributes[0]).Description;
        }
    }
}