using System.ComponentModel;
using youtube_center.Classes;

namespace youtube_center.Enums
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum DoubleClickAction
    {
        [Description("Open browser page to youtube")]
        Youtube,

        [Description("Open in streamlink")]
        Streamlink,

        [Description("Copy to clipboard")]
        CopyToClipboard
    }
}