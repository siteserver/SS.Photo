using Datory;

namespace SS.Photo.Core
{
    [Table("ss_Photo")]
    public class PhotoInfo : Entity
    {
        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn]
        public int ChannelId { get; set; }

        [TableColumn]
        public int ContentId { get; set; }

        [TableColumn]
        public string SmallUrl { get; set; }

        [TableColumn]
        public string MiddleUrl { get; set; }

        [TableColumn]
        public string LargeUrl { get; set; }

        [TableColumn]
        public int Taxis { get; set; }

        [TableColumn(Length = 2000)]
        public string Description { get; set; }
    }
}
