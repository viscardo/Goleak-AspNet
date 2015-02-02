using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Goleak.Models
{ 
    public class LeakRepository : ILeakRepository
    {
        GoleakContext context = new GoleakContext();

        public IQueryable<Leak> All
        {
            get { return context.Leak; }
        }

        public IQueryable<Leak> AllIncluding(params Expression<Func<Leak, object>>[] includeProperties)
        {
            IQueryable<Leak> query = context.Leak;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Leak Find(int id)
        {
            return context.Leak.Find(id);
        }



        public void InsertOrUpdate(Leak leak)
        {
            if (leak.Id == default(int)) {
                // New entity
                context.Leak.Add(leak);
            } else {
                // Existing entity
                context.Entry(leak).State = EntityState.Modified;
            }
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

    public interface ILeakRepository : IDisposable
    {
        IQueryable<Leak> All { get; }
        IQueryable<Leak> AllIncluding(params Expression<Func<Leak, object>>[] includeProperties);
        Leak Find(int id);
        void InsertOrUpdate(Leak leak);
        void Save();
    }
}