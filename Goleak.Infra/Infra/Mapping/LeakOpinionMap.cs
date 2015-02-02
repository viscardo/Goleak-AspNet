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
    public class LeakOpinionMap: ClassMapping<LeakOpinion>
    {
        public LeakOpinionMap()
        {
            Table("LeakOpinions");

            Id(k => k.Id, a => a.Generator(Generators.Native));

            Property( p=> p.Opinion, a=> 
            {
                a.NotNullable(true);
            });

            Property( p=> p.CreatedOn, a=> 
            {
                a.NotNullable(true);
            });

            ManyToOne(p => p.User, a =>
            {
                a.Column("UserId");
                a.NotNullable(true);
            });

            ManyToOne(p => p.Leak, a =>
            {
                a.Column("LeakId");
                a.NotNullable(true);
            });


        }
    }
}
