using System.Xml.Serialization;

namespace youtube_center.Models.Subscription
{
    [XmlRoot(ElementName = "body")]
    public class Body
    {
        [XmlElement(ElementName = "outline")]
        public Outline Outline { get; set; }
    }
}