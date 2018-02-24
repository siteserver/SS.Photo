namespace SS.Photo.Model
{
    public class PhotoInfo
    {
        public const string TableName = "ss_Photo";

        public PhotoInfo()
        {
            Id = 0;
            SiteId = 0;
            ContentId = 0;
            SmallUrl = string.Empty;
            MiddleUrl = string.Empty;
            LargeUrl = string.Empty;
            Taxis = 0;
            Description = string.Empty;
        }

        public PhotoInfo(int id, int siteId, int contentId, string smallUrl, string middleUrl, string largeUrl, int taxis, string description)
        {
            Id = id;
            SiteId = siteId;
            ContentId = contentId;
            SmallUrl = smallUrl;
            MiddleUrl = middleUrl;
            LargeUrl = largeUrl;
            Taxis = taxis;
            Description = description;
        }

        public int Id { get; set; }

        public int SiteId { get; set; }

        public int ContentId { get; set; }

        public string SmallUrl { get; set; }

        public string MiddleUrl { get; set; }

        public string LargeUrl { get; set; }

        public int Taxis { get; set; }

        public string Description { get; set; }
    }
}
