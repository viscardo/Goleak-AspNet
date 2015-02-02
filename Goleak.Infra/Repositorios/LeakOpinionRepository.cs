using System;
using System.Collections.Generic;
using System.Data;
using NHibernate;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Goleak.Infra.Models;

namespace Goleak.Infra.Repositorios
{ 
    public class LeakOpinionRepository : ILeakOpinionRepository
    {
        public ISession Session { get; set; }

        public LeakOpinionRepository()
        {
            Session = SessaoPorRequisicaoModule.Session;
        }


        public LeakOpinion Find(int id)
        {
            return Session.Get<LeakOpinion>(id);
        }

        public void Update(LeakOpinion leakopinion)
        {
            Session.Update(leakopinion);
            Session.Flush();
        }

        public void Save(LeakOpinion entity)
        {
            Session.Save(entity);
        }


    }

    public interface ILeakOpinionRepository 
    {

        LeakOpinion Find(int id);
        void Update(LeakOpinion leakopinion);
        void Save(LeakOpinion leak);
    }
}