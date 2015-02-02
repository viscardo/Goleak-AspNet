using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Goleak.Models
{ 
    public class UserRepository : IUserRepository
    {
        GoleakContext context = new GoleakContext();

        public IQueryable<User> All
        {
            get { return context.User; }
        }

        public IQueryable<User> AllIncluding(params Expression<Func<User, object>>[] includeProperties)
        {
            IQueryable<User> query = context.User;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public User SearchForFacebookId(string uid)
        {
           return context.User.Where(p => p.Fb == uid).SingleOrDefault();
        }

        public User SearchForUsername(string username)
        {
            return context.User.Where(p => p.Username == username).SingleOrDefault();
        }



        public IList<Leak> LeakFeed(int UserId)
        {
            User usuario = context.User.Find(UserId);
            List< int> listaIds = usuario.Friends.Select(q=>q.Id).ToList();
            listaIds.Add(UserId);
            return context.Leak.Where(p => listaIds.Contains(p.UserLeaked.Id)).OrderByDescending(p => p.CreatedOn).ToList();
        }

        public User Find(int id)
        {
            return context.User.Find(id);
        }

        public void InsertOrUpdate(User user)
        {
            if (user.Id == default(int)) {
                // New entity
                context.User.Add(user);
            } else {
                // Existing entity
                context.Entry(user).State = EntityState.Modified;
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

    public interface IUserRepository : IDisposable
    {
        IQueryable<User> All { get; }
        IQueryable<User> AllIncluding(params Expression<Func<User, object>>[] includeProperties);
        User Find(int id);
        void InsertOrUpdate(User user);
        void Save();
        User SearchForFacebookId(string uid);
        User SearchForUsername(string username);

        IList<Leak> LeakFeed(int UserId);
    }
}