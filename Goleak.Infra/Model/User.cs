using System;
using System.Web;
//using System.Web.Services;
//using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
//using System.ComponentModel.DataAnnotations.Schema;

namespace Goleak.Infra.Models
{

    public class User
    {
        [Key]
        public virtual int Id { get; set; }

        [Required(ErrorMessageResourceName = "YouMustFacebookConect", ErrorMessageResourceType = typeof(Resources.Resources))]
        public virtual string Fb { get; set; }

        [Display(Name = "ModelFirstName", ResourceType = typeof(Resources.Resources))]
        public virtual string FirstName { get; set; }

        [Display(Name = "ModelLastName", ResourceType = typeof(Resources.Resources))]
        public virtual string LastName { get; set; }

        public virtual string Username { get; set; }

        public virtual int FriendsCount { get; set; }

        public virtual string UserLink
        {
            get
            {
                if (Username != null && Username != string.Empty)
                    return Username;
                else
                    return Fb;
            }
        }

        public virtual string PicUrl { get; set; }

        public virtual bool Active { get; set; }

        public virtual IList<User> Friends { get; set; }

        public virtual IList<Leak> LeaksWrote { get; set; }

        public virtual IList<Leak> Leaks { get; set; }

        public virtual string Email { get; set; }

        public virtual bool? ReceiveNotification { get; set; }

        public virtual string LastIp { get; set; }

        public virtual int LeaksCount
        {
            get;
            set;
        }

        [Display(Name = "ModelGender", ResourceType = typeof(Resources.Resources))]
        public virtual string Gender { get; set; }

        public virtual DateTime CreatedOn { get; set; }
        public virtual DateTime? LastLogin { get; set; }

    }
}
