using System;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Goleak.Infra.Models
{


    public class LeakOpinion
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual Leak Leak { get; set; }
        public virtual User User { get; set; }
        public virtual bool Opinion { get; set; }
        public virtual DateTime CreatedOn { get; set; }

    }
}
