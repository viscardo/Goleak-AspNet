using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Goleak.Infra.Models;
using Goleak.Infra.Repositorios;
using Newtonsoft.Json;

namespace Goleak.Controllers
{   
    public class UserController : BaseController
    {
		private readonly IUserRepository userRepository;

		// If you are using Dependency Injection, you can delete the following constructor
        public UserController() : this(new UserRepository())
        {
        }

        public UserController(IUserRepository userRepository)
        {
			this.userRepository = userRepository;
        }

        public ActionResult Feed()
        {
            if (UserLogged != null)
            {
                return View(userRepository.LeakFeed(UserLogged.Id));
            }
            ExibirNotificacao("You must be logged into Goleak Network.", Notificacao.TipoNotificacao.Alerta);
            return RedirectToAction("Login", "Account");
        }

        public ActionResult Profile(string id)
        {

            if (UserLogged != null)
            {
                UserLogged = userRepository.Find(UserLogged.Id);
                User usuario = new User();
                if (id != null && id != string.Empty)
                {
                    if (UserLogged.Friends.Any(p => p.UserLink == id))
                    {
                        usuario = userRepository.SearchForUsername(id);
                        if (usuario == null)
                            usuario = userRepository.SearchForFacebookId(id);
                    }
                    else
                    {
                        if (UserLogged.UserLink != id)
                        {
                            ExibirNotificacao("Opss. This person is not your facebook friend.", Notificacao.TipoNotificacao.Alerta);
                            return View(UserLogged);
                        }
                        else
                            usuario = userRepository.Find(UserLogged.Id);
                    }
                }
                else
                    usuario = userRepository.Find(UserLogged.Id);

                ViewBag.ItsMe = usuario.Id == UserLogged.Id;
               
                return View(usuario);
            }
            ExibirNotificacao("You must be logged into Goleak Network.", Notificacao.TipoNotificacao.Alerta);
            return RedirectToAction("Login", "Account");
        }


        public ActionResult RemoveProfile()
        {
            if (UserLogged != null)
            {
                return View(base.UserLogged);
            }
            ExibirNotificacao("You must be logged into Goleak Network.", Notificacao.TipoNotificacao.Alerta);
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public ActionResult RemoveProfile(User user)
        {
            if (UserLogged != null)
            {
                UserLogged.Active = false;
                userRepository.Update(UserLogged);
                ExibirNotificacao("We hope you come back soon.", Notificacao.TipoNotificacao.Alerta);
            }
            else
                ExibirNotificacao("You must be logged into Goleak Network.", Notificacao.TipoNotificacao.Alerta);

            return RedirectToAction("Login", "Account");
        }

        public ActionResult Friends()
        {
            if (UserLogged != null)
            {
                return View(UserLogged);
            }
            ExibirNotificacao("You must be logged into Goleak Network.", Notificacao.TipoNotificacao.Alerta);
            return RedirectToAction("Login", "Account");

        }

        //public ActionResult Edit()
        //{
        //    return View(UserLogged);
        //}

        //[HttpPost]
        //public ActionResult Edit(string Password, string PasswordConfirm, bool ReceiveNotification)
        //{
        //    if (UserLogged != null)
        //    {
        //        if (Password != PasswordConfirm)
        //        {
        //            base.ExibirNotificacao("User or password does not match", Notificacao.TipoNotificacao.Alerta);
        //            return View(UserLogged);
        //        }

        //        var usuario = userRepository.Find(UserLogged.Id);
        //        usuario.Password = Password;
        //        usuario.ReceiveNotification = ReceiveNotification;
        //        userRepository.InsertOrUpdate(usuario);
        //        userRepository.Save();
        //        base.UserLogged = usuario;
        //        ExibirNotificacao("Your preferences was changed.", Notificacao.TipoNotificacao.Alerta);
        //        return RedirectToAction("Feed", "User");
        //    }
        //    ExibirNotificacao("You must be logged into Goleak Network.", Notificacao.TipoNotificacao.Alerta);
        //    return RedirectToAction("Login", "Account");
        //}
        
        public JsonResult SelectedFriendFilter(string partName)
        {
            //base.UserLogged = userRepository.Find(base.UserLogged.Id);
            //List<User> lista = UserLogged.Friends.Where(p => p.FirstName.ToUpper().Contains(partName.ToUpper())).ToList();
            IList<User> lista = userRepository.SearchFriends(base.UserLogged.Id, partName);
            return Json(lista.Select(p=> new { UserLink = p.UserLink, PicUrl = p.Fb, FirstName = p.FirstName, LastName = p.LastName, Leaks = p.Leaks.Count } ), JsonRequestBehavior.AllowGet);
        }

        /*
        //
        // GET: /User/

        public ViewResult Index()
        {
            return View(userRepository.AllIncluding(user => user.Friends, user => user.LeaksWrote, user => user.Leaks));
        }




        //
        // GET: /User/Details/5

        public ViewResult Details(int id)
        {
            return View(userRepository.Find(id));
        }

        //
        // GET: /User/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /User/Create

        [HttpPost]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid) {
                userRepository.InsertOrUpdate(user);
                userRepository.Save();
                return RedirectToAction("Index");
            } else {
				return View();
			}
        }
        
        //
        // GET: /User/Edit/5
 
        public ActionResult Edit(int id)
        {
             return View(userRepository.Find(id));
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid) {
                userRepository.InsertOrUpdate(user);
                userRepository.Save();
                return RedirectToAction("Index");
            } else {
				return View();
			}
        }

        //
        // GET: /User/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View(userRepository.Find(id));
        }

        //
        // POST: /User/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            userRepository.Delete(id);
            userRepository.Save();

            return RedirectToAction("Index");
        }
         * */


    }
}

