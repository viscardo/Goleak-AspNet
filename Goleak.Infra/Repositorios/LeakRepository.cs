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
    public class LeakRepository : ILeakRepository
    {
        public ISession Session { get; set; }

        public LeakRepository()
        {
            Session = SessaoPorRequisicaoModule.Session;
        }


        public Leak Find(int id)
        {
            return Session.Get<Leak>(id);
        }

  
        public void Update(Leak leak)
        {
            Session.Update(leak);
            Session.Flush();
        }

        public void Save(Leak entity)
        {
            Session.Save(entity);
        }

        public void Remove(Leak entity)
        {
            Session.Delete(entity);
            Session.Flush();
        }
    }

    public interface ILeakRepository 
    {

        Leak Find(int id);
        void Update(Leak leak);
        void Save(Leak leak);

        void Remove(Leak leak);
    }
}