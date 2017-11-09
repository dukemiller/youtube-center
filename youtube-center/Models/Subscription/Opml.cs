using System.Xml.Serialization;

namespace youtube_center.Models.Subscription
{
    [XmlRoot(ElementName = "opml")]
    public class Opml
    {
        [XmlElement(ElementName = "body")]
        public Body Body { get; set; }

        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
    }
}