using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
//using Goleak.Filters;
using Goleak.Infra.Models;
using Goleak.Infra.Repositorios;
using Facebook;
using Newtonsoft.Json;
using System.Threading;


namespace Goleak.Controllers
{
    [Authorize]
    //[InitializeSimpleMembership]
    public class AccountController : BaseController
    {
        private readonly IUserRepository userRepository;

        // If you are using Dependency Injection, you can delete the following constructor
        public AccountController()
            : this(new UserRepository())
        {
        }

        public AccountController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;

        }
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (base.UserLogged != null)
            {
                return RedirectToAction("Feed", "User");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    WebSecurity.Login(model.UserName, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // UserWrote does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }
            User user = userRepository.SearchForFacebookId(result.ProviderUserId);
            if (user != null && user.Email != null)
            {
                user.LastLogin = DateTime.Now;
                user.Active = true;
                user.LastIp = base.GetUserIp();
                base.UserLogged = user;

                userRepository.Update(user);
                return RedirectToAction("Feed", "User");
            }
            else
            {
                FacebookUserModel facebookUser = new FacebookUserModel();
                try
                {

                    var client = new FacebookClient(result.ExtraData["accesstoken"]);
                    dynamic fbresult = client.Get("me?fields=id,email,first_name,last_name,gender,locale,link,username,timezone,location,picture");
                    facebookUser = Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookUserModel>(fbresult.ToString());

                    //Amigos
                    FacebookFriendsModel friends = new FacebookFriendsModel();
                    fbresult = client.Get("me/friends");
                    var data = fbresult["data"].ToString();

                    friends.friendsListing = JsonConvert.DeserializeObject<List<FacebookFriend>>(data);

                    FacebookUserModel facebookFriend = new FacebookUserModel();
                    if (user == null)
                        user = new User();

                    user.Friends = new List<User>();
                    //Grava no banco 
                    user.Fb = result.ProviderUserId;
                    user.FirstName = facebookUser.first_name;
                    user.LastName = facebookUser.last_name;
                    user.Gender = facebookUser.gender != null ? facebookUser.gender.Substring(0, 1) : string.Empty;
                    user.Email = facebookUser.email;
                    user.Username = facebookUser.username;
                    user.PicUrl = facebookUser.picture.data.url;
                    user.CreatedOn = DateTime.Now;
                    user.LastLogin = DateTime.Now;
                    user.LastIp = base.GetUserIp();
                    user.ReceiveNotification = true;
                    user.Active = true;

                    #region Salvando o Usuario

                    using (var scope = new TransactionScope(TransactionScopeOption.Required))
                    {
                        userRepository.Save(user);
                        scope.Complete();
                    }

                    #endregion

                    #region Criando relacionamento com os amigos que ja estao no banco

                    List<User> alreadyFriends = userRepository.SearchForFacebookId(friends.friendsListing.Select(p => p.id).ToList());
                    if (alreadyFriends != null)
                    {
                        user.Friends = alreadyFriends;
                        using (var scope = new TransactionScope(TransactionScopeOption.Required))
                        {
                            userRepository.Update(user);
                            scope.Complete();
                        }

                    }
                    #endregion

                    #region Adcionando usuarios novos




                    List<FacebookFriend> newUsers = new List<FacebookFriend>();
                    newUsers = friends.friendsListing.Where(p => !alreadyFriends.Select(x => x.Fb).Contains(p.id)).ToList();



                    foreach (FacebookFriend item in newUsers)
                    {
                        dynamic fbresult2 = client.Get(item.id + "?fields=id,email,first_name,last_name,gender,locale,link,username,timezone,location,picture");
                        facebookFriend = Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookUserModel>(fbresult2.ToString());

                        //Grava no banco 
                        User amigo = new User();

                        amigo.FirstName = facebookFriend.first_name;
                        amigo.LastName = facebookFriend.last_name;
                        amigo.Gender = facebookFriend.gender != null ? facebookFriend.gender.Substring(0, 1) : string.Empty;
                        amigo.PicUrl = facebookFriend.picture.data.url;
                        amigo.Username = facebookFriend.username;
                        amigo.Fb = facebookFriend.id;
                        amigo.CreatedOn = DateTime.Now;
                        userRepository.Save(amigo);

                        user.Friends.Add(amigo);
                    }
                    using (var scope = new TransactionScope(TransactionScopeOption.Required))
                    {
                        userRepository.Update(user);
                        scope.Complete();
                    }




                    #endregion

                }
                catch
                {
                    ExibirNotificacao("An error ocurred on the connection on facebook. Please try again.", Notificacao.TipoNotificacao.Alerta);
                    return RedirectToAction("Login", "Account");
                }



                base.UserLogged = user;
                return RedirectToAction("Feed", "User");
            }

        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalUpdate(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("UpdateFacebookFriend", new { ReturnUrl = returnUrl }));
        }

        [AllowAnonymous]
        public ActionResult UpdateFacebookFriend(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("UpdateFacebookFriend", new { ReturnUrl = returnUrl }));

            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }
            User user = userRepository.SearchForFacebookId(result.ProviderUserId);
            if (user != null && user.Email != null)
            {
                FacebookUserModel facebookUser = new FacebookUserModel();
                try
                {

                    var client = new FacebookClient(result.ExtraData["accesstoken"]);
                    dynamic fbresult = client.Get("me?fields=id,email,first_name,last_name,gender,locale,link,username,timezone,location,picture");
                    facebookUser = Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookUserModel>(fbresult.ToString());

                    //Amigos
                    FacebookFriendsModel friends = new FacebookFriendsModel();
                    fbresult = client.Get("me/friends");
                    var data = fbresult["data"].ToString();

                    friends.friendsListing = JsonConvert.DeserializeObject<List<FacebookFriend>>(data);

                    FacebookUserModel facebookFriend = new FacebookUserModel();

                    if (user.Friends == null)
                        user.Friends = new List<User>();

                    List<string> facebookFriendsId = user.Friends.Select(p => p.Fb).ToList();
                    List<FacebookFriend> notFriendsYet = new List<FacebookFriend>();
                    foreach (FacebookFriend item in friends.friendsListing)
                    {
                        if (!facebookFriendsId.Contains(item.id))
                            notFriendsYet.Add(item);
                    }

                    foreach (FacebookFriend item in notFriendsYet)
                    {
                        dynamic fbresult2 = client.Get(item.id + "?fields=id,email,first_name,last_name,gender,locale,link,username,timezone,location,picture");
                        facebookFriend = Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookUserModel>(fbresult2.ToString());

                        //Grava no banco 
                        User amigo = userRepository.SearchForFacebookId(facebookFriend.id);
                        if (amigo == null)
                        {
                            amigo = new User();
                            amigo.FirstName = facebookFriend.first_name;
                            amigo.LastName = facebookFriend.last_name;
                            amigo.Gender = facebookFriend.gender != null ? facebookFriend.gender.Substring(0, 1) : string.Empty;
                            amigo.PicUrl = facebookFriend.picture.data.url;
                            amigo.Username = facebookFriend.username;
                            amigo.Fb = facebookFriend.id;
                            amigo.CreatedOn = DateTime.Now;
                        }
                        user.Friends.Add(amigo);
                    }
                }
                catch
                {
                    ExibirNotificacao("An error ocurred on the connection on facebook. Please try again.", Notificacao.TipoNotificacao.Alerta);
                    return View();
                }


                userRepository.Save(user);
                base.UserLogged = user;
                ExibirNotificacao("Friends are updated.", Notificacao.TipoNotificacao.Alerta);

                return RedirectToAction("Feed", "User");
            }
            return RedirectToAction("Feed", "User");

        }



        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            //if (ModelState.IsValid)
            //{
            //    // Insert a new user into the database
            //    using (UsersContext db = new UsersContext())
            //    {
            //        UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
            //        // Check if user already exists
            //        if (user == null)
            //        {
            //            // Insert name into the profile table
            //            db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
            //            db.SaveChanges();

            //            OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
            //            OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

            //            return RedirectToLocal(returnUrl);
            //        }
            //        else
            //        {
            //            ModelState.AddModelError("UserName", "UserWrote name already exists. Please enter a different user name.");
            //        }
            //    }
            //}

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult UpdateExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_UpdateExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "UserWrote name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
