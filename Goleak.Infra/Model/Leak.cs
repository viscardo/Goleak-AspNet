using System;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Goleak.Infra.Models
{

    public class Leak
    {
        private int _trueLeaks;
        private int _falseLeaks;

        [Key]
        public virtual int Id { get; set; }

        public virtual User UserWrote { get; set; }

        public virtual User UserLeaked{ get; set; }

        public virtual  string LeakText { get; set; }

        public virtual ICollection<LeakOpinion> LeakOpinions { get; set; }

        public virtual DateTime CreatedOn { get; set; }

        public virtual int TrueLeaks
        {
            get
            {
                if (LeakOpinions != null)
                    return LeakOpinions.Where(p => p.Opinion).Count();
                else
                    return _trueLeaks;
            }
            set
            {
                _trueLeaks = value;
            }
        }

        public virtual int FalseLeaks
        {
            get
            {
                if (LeakOpinions != null)
                    return LeakOpinions.Where(p => !p.Opinion).Count();
                else
                    return _falseLeaks;
            }
            set
            {
                _falseLeaks = value;
            }
        }

    }
}
