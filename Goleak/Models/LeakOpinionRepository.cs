using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Goleak.Models
{ 
    public class LeakOpinionRepository : ILeakOpinionRepository
    {
        GoleakContext context = new GoleakContext();

        public IQueryable<LeakOpinion> All
        {
            get { return context.LeakOpinion; }
        }

        public IQueryable<LeakOpinion> AllIncluding(params Expression<Func<LeakOpinion, object>>[] includeProperties)
        {
            IQueryable<LeakOpinion> query = context.LeakOpinion;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public LeakOpinion Find(int id)
        {
            return context.LeakOpinion.Find(id);
        }

        public void InsertOrUpdate(LeakOpinion leakopinion)
        {
            if (leakopinion.Id == default(int)) {
                // New entity
                context.LeakOpinion.Add(leakopinion);
            } else {
                // Existing entity
                context.Entry(leakopinion).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var leakopinion = context.LeakOpinion.Find(id);
            context.LeakOpinion.Remove(leakopinion);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Dispose() 
        {
            context.Dispose();
        }
    }

    public interface ILeakOpinionRepository : IDisposable
    {
        IQueryable<LeakOpinion> All { get; }
        IQueryable<LeakOpinion> AllIncluding(params Expression<Func<LeakOpinion, object>>[] includeProperties);
        LeakOpinion Find(int id);
        void InsertOrUpdate(LeakOpinion leakopinion);
        void Delete(int id);
        void Save();
    }
}