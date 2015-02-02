using System.Collections.Generic;

namespace Goleak.Models
{
    public class FacebookPhotosModel
    {
        public List<FacebookPhoto> photosListing { get; set; }
    }

    public class FacebookPhoto
    {
        public string name { get; set; }
        public string id { get; set; }
        public string source { get; set; }
    }
}