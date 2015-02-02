using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Goleak.Models
{
    [NotMapped]
    public class FacebookLocation
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}