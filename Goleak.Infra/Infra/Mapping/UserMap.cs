using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping.ByCode.Conformist;
using Goleak.Infra.Models;
using NHibernate.Mapping.ByCode;
using Goleak.Infra.TipoGenerico;

namespace Goleak.Infra.Mapping
{
    public class UserMap : ClassMapping<User>
    {
        public UserMap()
        {
            Table("Users");

            Id(k => k.Id, a => a.Generator(Generators.Native));

            Property(p => p.Fb, a =>
            {
                a.NotNullable(true);
            });

            Property(p => p.CreatedOn, a =>
            {
                a.NotNullable(true);
            });

            Property(p => p.FirstName, a => { a.NotNullable(false); });
            Property(p => p.LastName, a => { a.NotNullable(false); });
            Property(p => p.Username, a => { a.NotNullable(false); });
            Property(p => p.PicUrl, a => { a.NotNullable(false); });
            Property(p => p.Email, a => { a.NotNullable(false); });
            Property(p => p.ReceiveNotification, a => { a.NotNullable(false); });
            Property(p => p.LastIp, a => { a.NotNullable(false); });
            Property(p => p.Gender, a => { a.NotNullable(false); });
            Property(p => p.LastLogin, a => { a.NotNullable(false); });
            Property(p => p.Active, a => { a.NotNullable(false); });
            Property(x => x.LeaksCount, map =>
            {
                map.Formula("( SELECT count(*) FROM Leaks L  WHERE L.UserLeakedId=Id)");
            });

            Bag(x => x.Friends,
               map =>
               {

                   map.Table("user_friend");
                   map.Cascade(Cascade.All);
                   map.Key(km => km.Column("USER_ID"));
               },
               action => action.ManyToMany(map => map.Column("FRIEND_ID")));


            Bag(p => p.LeaksWrote, a =>
            {
                a.Inverse(true);
                a.Key(k => k.Column("UserWroteId"));
            }, a => a.OneToMany());

            Bag(p => p.Leaks, a =>
            {
                a.Inverse(true);
                a.Key(k => k.Column("UserLeakedId"));
            }, a => a.OneToMany());

        }
    }
}
