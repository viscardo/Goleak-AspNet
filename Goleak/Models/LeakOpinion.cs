using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Goleak.Models
{
    public class LeakOpinion
    {
        [Key]
        public int Id { get; set; }


        public int LeakId { get; set; }
        [ForeignKey("LeakId")]
        public virtual Leak Leak { get; set; }
        

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
       
 
        public bool Opinion { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}
