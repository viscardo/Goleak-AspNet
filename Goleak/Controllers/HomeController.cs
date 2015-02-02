using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Goleak.Infra.Models;
using Goleak.Infra.Repositorios;
using Facebook;
using Newtonsoft.Json;



namespace Goleak.Controllers
{
    public class HomeController : BaseController
    {

        private readonly IUserRepository userRepository;


		// If you are using Dependency Injection, you can delete the following constructor
        public HomeController() : this(new UserRepository())
        {
        }

        public HomeController(IUserRepository userRepository)
        {
			this.userRepository = userRepository;

        }

        public ActionResult About()
        {
            return View();
        }


        public ActionResult Contact()
        {
            return View();
        }


        public ActionResult Index(string id)
        {
            return View();
        }

        //public ActionResult Login()
        //{
        //    if(base.UserLogged == null)
        //        return View();
        //    else
        //        return RedirectToAction("Profile", "User");
        //}

        [HttpGet]
        public ActionResult LogOut()
        {
            base.UserLogged = null;
            return RedirectToAction("Login", "Account");
        }

        //[HttpPost]
        //public ActionResult Login(string email, string password)
        //{
        //    User usuario = userRepository.SearchForUsernameAndPassword(email, password);
        //    if (usuario != null)
        //    {
        //        usuario.LastLogin = DateTime.Now;
        //        usuario.LastIp = base.GetUserIp();
        //        base.UserLogged = usuario;
                
        //        userRepository.InsertOrUpdate(usuario);
        //        userRepository.Save();
        //        return RedirectToAction("Feed", "User");
        //    }
        //    else
        //    {
        //        base.ExibirNotificacao("User or password does not match", Notificacao.TipoNotificacao.Alerta);
        //    }
        //    return View();
        //}

        //public ActionResult Signin()
        //{
        //    if (Session["userSign"] != null)
        //    {
        //        return View((User)Session["userSign"]);
        //    }
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Signin(User user)
        //{
        //    if (user.Password != user.PasswordConfirm || user.Password == null)
        //        base.ExibirNotificacao("Password does not match", Notificacao.TipoNotificacao.Alerta);
        //    else
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            user.Fb = Session["uid"].ToString();

        //            User verifyUser = userRepository.SearchForFacebookId(user.Fb);

        //            if (verifyUser != null)
        //            {
        //                if (verifyUser.Fb == user.Fb)
        //                {
        //                    if (verifyUser.Email != null)
        //                    {
        //                        ExibirNotificacao("User already exists on Goleak NetWork", Notificacao.TipoNotificacao.Alerta);
        //                        return View();
        //                    }
        //                    verifyUser.Email = user.Email;
        //                    verifyUser.Password = user.Password;
        //                    verifyUser.PasswordConfirm = user.PasswordConfirm;
        //                    user = verifyUser;
        //                }

        //            }

        //            FacebookUserModel facebookUser = new FacebookUserModel();
        //            try
        //            {

        //                var client = new FacebookClient(Session["accessToken"].ToString());
        //                dynamic fbresult = client.Get("me?fields=id,email,first_name,last_name,gender,locale,link,username,timezone,location,picture");
        //                facebookUser = Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookUserModel>(fbresult.ToString());

        //                //Amigos
        //                FacebookFriendsModel friends = new FacebookFriendsModel();
        //                fbresult = client.Get("me/friends");
        //                var data = fbresult["data"].ToString();

        //                friends.friendsListing = JsonConvert.DeserializeObject<List<FacebookFriend>>(data);

        //                FacebookUserModel facebookFriend = new FacebookUserModel();
        //                user.Friends = new List<User>();

        //                foreach (FacebookFriend item in friends.friendsListing)
        //                {
        //                    dynamic fbresult2 = client.Get(item.id + "?fields=id,email,first_name,last_name,gender,locale,link,username,timezone,location,picture");
        //                    facebookFriend = Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookUserModel>(fbresult2.ToString());

        //                    //Grava no banco 
        //                    Models.User amigo = userRepository.SearchForFacebookId(facebookFriend.id);
        //                    if (amigo == null)
        //                    {
        //                        amigo = new Models.User();
        //                        amigo.FirstName = facebookFriend.first_name;
        //                        amigo.LastName = facebookFriend.last_name;
        //                        amigo.Gender = facebookFriend.gender != null ? facebookFriend.gender.Substring(0, 1) : string.Empty;
        //                        amigo.PicUrl = facebookFriend.picture.data.url;
        //                        amigo.Username = facebookFriend.username;
        //                        amigo.Fb = facebookFriend.id;
        //                        amigo.CreatedOn = DateTime.Now;
        //                    }
        //                    user.Friends.Add(amigo);
        //                }
        //            }
        //            catch
        //            {
        //                ExibirNotificacao("An error ocurred on the connection on facebook. Please try again.", Notificacao.TipoNotificacao.Alerta);
        //                return View("Signin");
        //            }


        //            //Grava no banco 
        //            user.FirstName = facebookUser.first_name;
        //            user.LastName = facebookUser.last_name;
        //            user.Gender = facebookUser.gender != null ? facebookUser.gender.Substring(0, 1) : string.Empty;
        //            user.Email = facebookUser.email;
        //            user.Username = facebookUser.username;
        //            user.PicUrl = facebookUser.picture.data.url;
        //            user.CreatedOn = DateTime.Now;
        //            user.LastLogin = DateTime.Now;
        //            user.LastIp = base.GetUserIp();
        //            user.ReceiveNotification = true;


        //            userRepository.InsertOrUpdate(user);
        //            userRepository.Save();
        //            ExibirNotificacao("Welcome to the Goleak NetWork. Now You can Login.", Notificacao.TipoNotificacao.Alerta);
        //            return View("Login");
        //        }
        //        else
        //        {
        //            base.ExibirNotificacaoModelState();
        //        }
        //    }

        //    return View();
        //}

      


        //[HttpPost]
        //public JsonResult FacebookCredentials(FacebookLoginModel model)
        //{
        //    Session["uid"] = model.uid;
        //    Session["accessToken"] = model.accessToken;

        //    FacebookUserModel facebookUser = new FacebookUserModel();
        //    var client = new FacebookClient(Session["accessToken"].ToString());
        //    dynamic fbresult = client.Get("me?fields=id,email,first_name,last_name,gender,locale,link,username,timezone,location,picture");
        //    facebookUser = Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookUserModel>(fbresult.ToString());


        //    return Json(new { FirstName = facebookUser.first_name, 
        //        LastName = facebookUser.last_name,
        //                      Email = facebookUser.email,
        //                      Gender = facebookUser.gender,
        //                      Fb = model.uid
        //    }, JsonRequestBehavior.AllowGet);
        //}


        
    }
}
