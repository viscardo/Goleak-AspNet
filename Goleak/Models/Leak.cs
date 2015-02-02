using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;

namespace Goleak.Models
{
    public class Leak
    {
        [Key]
        public int Id{ get; set; }

        public int UserWroteId { get; set; }
        //[ForeignKey("UserWroteId")]
        public virtual User UserWrote { get; set; }

        public int UserLeakedId { get; set; }
        //[ForeignKey("UserLeakedId")]
        public virtual User UserLeaked{ get; set; }

        public string LeakText { get; set; }

        public virtual ICollection<LeakOpinion> LeakOpinions { get; set; }

        public DateTime CreatedOn { get; set; }

        public int TrueLeaks
        {
            get
            {
                if (LeakOpinions != null)
                    return LeakOpinions.Where(p => p.Opinion).Count();
                else
                    return 0;
            }
        }

        public int FalseLeaks
        {
            get
            {
                if (LeakOpinions != null)
                    return LeakOpinions.Where(p => !p.Opinion).Count();
                else
                    return 0;
            }
        }

    }
}
