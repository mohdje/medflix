using System.Collections.Generic;
using System.Xml.Serialization;

namespace MoviesAPI.Services.Torrent
{

    [XmlRoot("rss")]
    public class TorrentDownloadRssDto
    {
        [XmlElement("channel")]
        public Channel Channel { get; set; }
    }

    public class Channel
    {
        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("link")]
        public string Link { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("language")]
        public string Language { get; set; }

        [XmlElement("pubDate")]
        public string PubDate { get; set; }

        [XmlElement("lastBuildDate")]
        public string LastBuildDate { get; set; }

        [XmlElement("docs")]
        public string Docs { get; set; }

        [XmlElement("generator")]
        public string Generator { get; set; }

        [XmlElement("item")]
        public List<TorrentRssItem> Items { get; set; }
    }

    public class TorrentRssItem
    {
        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("link")]
        public string Link { get; set; }

        [XmlElement("guid")]
        public string Guid { get; set; }

        [XmlElement("pubDate")]
        public string PubDate { get; set; }

        [XmlElement("category")]
        public string Category { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }
    }
}