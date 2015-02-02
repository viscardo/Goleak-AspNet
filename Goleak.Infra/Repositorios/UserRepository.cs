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
    public class UserRepository : IUserRepository
    {
        public ISession Session { get; set; }

        public UserRepository()
        {
            Session = SessaoPorRequisicaoModule.Session;
        }



        public User SearchForFacebookId(string uid)
        {
            return Session.QueryOver<User>().Where(p => p.Fb == uid).SingleOrDefault();
        }

        public List<User> SearchForFacebookId(List<string> uid)
        {
            return Session.QueryOver<User>()
            .AndRestrictionOn(x => x.Fb).IsIn(uid).List().ToList();
        }

        public User SearchForUsername(string username)
        {
            return Session.QueryOver<User>().Where(p => p.Username == username).SingleOrDefault();
        }

        public IList<User> SearchFriends(int userId, string partName)
        {
            return this.Find(userId).Friends.Where(p => p.FirstName.ToUpper().Contains(partName.ToUpper())).ToList();

        }

        public IQueryable<Leak> LeakFeed(int UserId)
        {
            User usuario = this.Find(UserId);
            List< int> listaIds = usuario.Friends.Select(q=>q.Id).ToList();
            listaIds.Add(UserId);
            return Session.QueryOver<Leak>()
                .JoinQueryOver<User>(leak => leak.UserLeaked)
                .AndRestrictionOn(x => x.Id).IsIn(listaIds).Where(p=> p.Active == true).List().OrderByDescending(p => p.CreatedOn).AsQueryable();
        }

        public User Find(int id)
        {
            return Session.Get<User>(id);
        }

        public void Update(User user)
        {
            Session.Update(user);
            Session.Flush();
        }


        public void Save(User user)
        {
            Session.Save(user);
        }


    }

    public interface IUserRepository 
    {
        User Find(int id);
        void Update(User user);
        void Save(User user);
        User SearchForFacebookId(string uid);
        List<User> SearchForFacebookId(List<string> uid);
        User SearchForUsername(string username);

        IQueryable<Leak> LeakFeed(int UserId);

        IList<User> SearchFriends(int userId, string partName);
    }
}