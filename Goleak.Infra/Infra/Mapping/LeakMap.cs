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
    public class LeakMap: ClassMapping<Leak>
    {
        public LeakMap()
        {
            Table("Leaks");

            Id(k => k.Id, a => a.Generator(Generators.Native));

            Property( p=> p.LeakText, a=> 
            {
                a.NotNullable(true);
            });

            Property( p=> p.CreatedOn, a=> 
            {
                a.NotNullable(true);
            });

            ManyToOne(p => p.UserWrote, a =>
            {
                a.Column("UserWroteId");
                a.NotNullable(true);
            });

            ManyToOne(p => p.UserLeaked, a =>
            {
                a.Column("UserLeakedId");
                a.NotNullable(true);
            });


            Bag(p => p.LeakOpinions, a =>
            {
                a.Inverse(true);
                a.Key(k => k.Column("LeakId"));
            }, a => a.OneToMany());


        }
    }
}
