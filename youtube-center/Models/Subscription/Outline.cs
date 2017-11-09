using System.Xml.Serialization;

namespace youtube_center.Models.Subscription
{
    [XmlRoot(ElementName = "outline")]
    public class Outline
    {
        [XmlAttribute(AttributeName = "text")]
        public string Text { get; set; }

        [XmlAttribute(AttributeName = "title")]
        public string Title { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "xmlUrl")]
        public string XmlUrl { get; set; }
    }
}