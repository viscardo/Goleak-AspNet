using System.Collections.Generic;

namespace Goleak.Infra.Models
{
    public class FacebookAlbumModel
    {
        public List<FacebookAlbum> albumListing { get; set; }
    }

    public class FacebookAlbum
    {
        public string name { get; set; }
        public string id { get; set; }
        public string cover_photo { get; set; }
        public string source { get; set; }
    }
}