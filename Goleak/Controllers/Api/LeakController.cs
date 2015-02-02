using Goleak.Infra.Models;
using Goleak.Infra.Repositorios;
using System;
using System.Collections.Generic;
//using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;

using System.Linq;
using Facebook;
using Newtonsoft.Json;
using System.Threading;
using Goleak.Infra.Email;
using Newtonsoft.Json.Linq;
using System.Transactions;


namespace Goleak.Controllers.Api
{
    public class ReturnMessage
    {
        public bool Sucess { get; set; }
        public string Message { get; set; }

        public string Tipo { get; set; }
    }
    public class LeakController : ApiController
    {

        private readonly IUserRepository userRepository;
        private readonly ILeakRepository leakRepository;
        private readonly ILeakOpinionRepository leakOpinionRepository;

        // If you are using Dependency Injection, you can delete the following constructor
        public LeakController()
            : this(new UserRepository(), new LeakRepository(), new LeakOpinionRepository())
        {
        }

        public LeakController(IUserRepository userRepository, ILeakRepository leakRepository, ILeakOpinionRepository leakOpinionRepository)
        {
            this.userRepository = userRepository;
            this.leakRepository = leakRepository;
            this.leakOpinionRepository = leakOpinionRepository;

        }

        // GET api/default1

        public IEnumerable<Leak> GetMyLeakFedd(int userId)
        {

            var lista = userRepository.LeakFeed(userId).Select(p =>
                new Leak
                {
                    Id = p.Id,
                    CreatedOn = p.CreatedOn,
                    LeakText = p.LeakText,
                    UserLeaked = new User() { Id = p.UserLeaked.Id, FirstName = p.UserLeaked.FirstName, LastName = p.UserLeaked.LastName, PicUrl = p.UserLeaked.PicUrl, Active = p.UserLeaked.Active, Fb = p.UserLeaked.Fb },
                    UserWrote = new User() { Gender = p.UserWrote.Gender },
                    TrueLeaks = p.TrueLeaks,
                    FalseLeaks = p.FalseLeaks
                });

            return lista;
        }


        public IEnumerable<Leak> GetLeaksOnMe(int userId)
        {

            var lista = userRepository.Find(userId).Leaks.OrderByDescending(p => p.CreatedOn).Select(p =>
                new Leak
                {
                    Id = p.Id,
                    CreatedOn = p.CreatedOn,
                    LeakText = p.LeakText,
                    UserLeaked = new User() { Id = p.UserLeaked.Id, FirstName = p.UserLeaked.FirstName, LastName = p.UserLeaked.LastName, PicUrl = p.UserLeaked.PicUrl, Active = p.UserLeaked.Active, Fb = p.UserLeaked.Fb },
                    UserWrote = new User() { Gender = p.UserWrote.Gender },
                    TrueLeaks = p.TrueLeaks,
                    FalseLeaks = p.FalseLeaks
                });

            return lista;
        }

        public User GetUserByFacebookId(string id)
        {
            var user = userRepository.SearchForFacebookId(id);
            User novoUsuario = new User()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Gender = user.Gender,
                Fb = user.Fb
            };

            return novoUsuario;
        }

        public User GetLoginByFacebook(string fbId, string accesstoken)
        {
            User user = userRepository.SearchForFacebookId(fbId);
            if (user != null && user.Email != null)
            {
                user.LastLogin = DateTime.Now;
                user.Active = true;

                userRepository.Update(user);

            }
            else
            {
                FacebookUserModel facebookUser = new FacebookUserModel();
                try
                {

                    var client = new FacebookClient(accesstoken);
                    dynamic fbresult = client.Get("me?fields=id,email,first_name,last_name,gender,locale,link,username,timezone,location,picture");
                    facebookUser = Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookUserModel>(fbresult.ToString());


                    if (user == null)
                        user = new User();

                    #region Amigos
                    /*
                    FacebookFriendsModel friends = new FacebookFriendsModel();
                    fbresult = client.Get("me/friends");
                    var data = fbresult["data"].ToString();

                    friends.friendsListing = JsonConvert.DeserializeObject<List<FacebookFriend>>(data);

                    FacebookUserModel facebookFriend = new FacebookUserModel();


                    user.Friends = new List<User>();

                    foreach (FacebookFriend item in friends.friendsListing)
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
                    */
                    #endregion Amigos

                    //Grava no banco 
                    user.Fb = fbId;
                    user.FirstName = facebookUser.first_name;
                    user.LastName = facebookUser.last_name;
                    user.Gender = facebookUser.gender != null ? facebookUser.gender.Substring(0, 1) : string.Empty;
                    user.Email = facebookUser.email;
                    user.Username = facebookUser.username;
                    user.PicUrl = facebookUser.picture.data.url;
                    user.CreatedOn = DateTime.Now;
                    user.LastLogin = DateTime.Now;
                    //user.LastIp = base.GetUserIp();
                    user.ReceiveNotification = true;
                    user.Active = true;

                    userRepository.Save(user);
                    //base.UserLogged = user;
                }
                catch
                {
                    return null;
                }

            }


            User novoUsuario = new User()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Gender = user.Gender,
                Fb = user.Fb,
                FriendsCount = user.Friends == null ? 0 : user.Friends.Count
            };

            return novoUsuario;

        }

        public IEnumerable<User> GetFriends(int id)
        {
            return userRepository.Find(id).Friends.OrderBy(x => x.FirstName).Select(p =>
                new User()
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Email = p.Email,
                    Gender = p.Gender,
                    Fb = p.Fb,
                    LeaksCount = p.LeaksCount
                });

        }

        public ReturnMessage DeleteLeak(int leakId, int userId, string accesstoken)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            returnMessage.Tipo = "DELETE_LEAK";
            var myLeak = leakRepository.Find(leakId);
            if (myLeak == null)
            {
                returnMessage.Sucess = false;
                returnMessage.Message = "This leak was not found, please try again.";
                return returnMessage;
            }
            if (myLeak.UserLeaked.Id != userId)
            {
                returnMessage.Sucess = false;
                returnMessage.Message = "Hey this leak does not belong to you. You can be banish.";
                try
                {
                    Thread emailThread = new Thread(delegate()
                    {
                        ServicoDeEmail email = new ServicoDeEmail();
                        email.EnviarEmail("goleak.com@gmail.com", "The user " + userId + " try to remove the leak " + leakId, "Someone try to delete other user leak");
                    });

                    emailThread.IsBackground = true;
                    emailThread.Start();

                }
                catch { }
                return returnMessage;
            }

            try
            {
                leakRepository.Remove(myLeak);
                returnMessage.Sucess = false;
                returnMessage.Message = "Hey this leak does not belong to you. You can be banish.";
                return returnMessage;
            }
            catch
            {
                returnMessage.Sucess = false;
                returnMessage.Message = "There was an error when we try to delete this leak.";
                return returnMessage;
            }
        }

        public ReturnMessage PostCheckLeakOnwer(int leakId, int userId)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            returnMessage.Tipo = "LEAK_ONWER";

            var myLeak = leakRepository.Find(leakId);
            if (myLeak == null)
            {
                returnMessage.Sucess = false;
                returnMessage.Message = "This leak was not found, please try again.";
                return returnMessage;
            }
            if (myLeak.UserLeaked.Id != userId)
            {
                returnMessage.Sucess = false;
            }
            else
            {
                returnMessage.Sucess = true;
            }
            return returnMessage;

        }

        public ReturnMessage PostReportSpam(int leakId, int userId)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            returnMessage.Tipo = "REPORT_SPAM";
            try
            {
                Thread emailThread = new Thread(delegate()
                {
                    ServicoDeEmail email = new ServicoDeEmail();
                    email.EnviarEmail("goleak.com@gmail.com", "The user " + userId + " report the leak " + leakId + " as offensive", "Ofensive spam report.");
                });

                emailThread.IsBackground = true;
                emailThread.Start();

            }
            catch { }
            returnMessage.Sucess = true;
            returnMessage.Message = "Your report was sent to our team, thank you";
            return returnMessage;

        }

        public ReturnMessage PostRemoveProfile(string id)
        {
            var UserLogged = userRepository.Find(int.Parse(id));
            ReturnMessage returnMessage = new ReturnMessage();
            if (UserLogged != null)
            {
                UserLogged.Active = false;
                userRepository.Update(UserLogged);

                returnMessage.Sucess = true;
                returnMessage.Message = "Your profile was removed from Goleak. If you want to come back log in again.";
                return returnMessage;
            }
            else
            {
                returnMessage.Sucess = false;
                returnMessage.Message = "Ops something got wrong.";
                return returnMessage;
            }

        }

        public ReturnMessage GetLike(int LeakId, int userId)
        {
            LeakOpinion leakOpinion = new LeakOpinion();
            leakOpinion.CreatedOn = DateTime.Now;
            leakOpinion.Leak = leakRepository.Find(LeakId);
            leakOpinion.Opinion = true;
            leakOpinion.User = userRepository.Find(userId);

            ReturnMessage returnMessage = new ReturnMessage();
            if (leakOpinion.Leak.LeakOpinions.Any(x => x.User.Id == userId))
            {
                returnMessage.Sucess = false;
                returnMessage.Message = "You already gave your opnion.";
                return returnMessage;
            }

            if (this.Create(leakOpinion))
            {
                returnMessage.Sucess = true;
                returnMessage.Message = "You like the leak";
                return returnMessage;
            }
            else
            {
                returnMessage.Sucess = false;
                returnMessage.Message = "Ops something got wrong.";
                return returnMessage;
            }
        }


        public ReturnMessage PostUpdateFriends(string id, string accesstoken, string facebookFriends)
        {
            User user = userRepository.Find(int.Parse(id));
            ReturnMessage returnMessage = new ReturnMessage();
            if (user != null && user.Email != null)
            {
                FacebookUserModel facebookUser = new FacebookUserModel();
                try
                {

                    var client = new FacebookClient(accesstoken);
                    dynamic fbresult = client.Get("me?fields=id,email,first_name,last_name,gender,locale,link,username,timezone,location,picture");
                    facebookUser = Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookUserModel>(fbresult.ToString());

                    //Amigos
                    //FacebookFriendsModel friends = new FacebookFriendsModel();
                    //fbresult = client.Get("me/friends");
                    //var data = fbresult["data"].ToString();

                    //friends.friendsListing = JsonConvert.DeserializeObject<List<FacebookFriend>>(data);

                    FacebookUserModel facebookFriend = new FacebookUserModel();

                    if (user.Friends == null)
                        user.Friends = new List<User>();

                    List<string> facebookFriendsId = user.Friends.Select(p => p.Fb).ToList();
                    List<string> notFriendsYet = new List<string>();
                    foreach (string item in facebookFriends.Split(','))
                    {
                        if (!facebookFriendsId.Contains(item))
                            notFriendsYet.Add(item);
                    }

                    foreach (string item in notFriendsYet)
                    {
                        dynamic fbresult2 = client.Get(item + "?fields=id,email,first_name,last_name,gender,locale,link,username,timezone,location,picture");
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
                    returnMessage.Sucess = false;
                    returnMessage.Message = "An error ocurred on the connection on facebook. Please try again.";
                    return returnMessage;
                }

                using (var scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    userRepository.Update(user);
                    scope.Complete();
                }

                returnMessage.Sucess = true;
                returnMessage.Message = "Friends are updated.";


            }
            return returnMessage; ;

        }


        public ReturnMessage GetDislike(int LeakId, int userId)
        {
            LeakOpinion leakOpinion = new LeakOpinion();
            leakOpinion.CreatedOn = DateTime.Now;
            leakOpinion.Leak = leakRepository.Find(LeakId);
            leakOpinion.Opinion = false;
            leakOpinion.User = userRepository.Find(userId);

            ReturnMessage returnMessage = new ReturnMessage();

            if (leakOpinion.Leak.LeakOpinions.Any(x => x.User.Id == userId))
            {
                returnMessage.Sucess = false;
                returnMessage.Message = "You already gave your opnion.";
                return returnMessage;
            }

            if (this.Create(leakOpinion))
            {
                returnMessage.Sucess = true;
                returnMessage.Message = "You don't like the leak";
                return returnMessage;
            }
            else
            {
                returnMessage.Sucess = false;
                returnMessage.Message = "Ops something got wrong.";
                return returnMessage;
            }


        }



        public ReturnMessage GetCreateLeak(string LeakText, int UserLeakedId, int UserId)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            try
            {
                Leak leak = new Leak();
                leak.CreatedOn = DateTime.Now;
                leak.LeakText = LeakText;

                leak.UserWrote = userRepository.Find(UserId);

                leak.UserLeaked = new Infra.Models.User();
                leak.UserLeaked = userRepository.Find(UserLeakedId);

                leakRepository.Save(leak);

                if (leak.UserLeaked.Email != null)
                {
                    try
                    {
                        Thread emailThread = new Thread(delegate()
                        {
                            ServicoDeEmail email = new ServicoDeEmail();
                            email.EnviarEmailLeaked(leak.UserLeaked.Email, leak.UserLeaked.FirstName, leak.LeakText);
                        });

                        emailThread.IsBackground = true;
                        emailThread.Start();

                    }
                    catch { }
                }
                returnMessage.Sucess = true;
                returnMessage.Message = "Ops something got wrong.";
                return returnMessage;
            }
            catch
            {
                returnMessage.Sucess = false;
                returnMessage.Message = "Ops something got wrong.";
                return returnMessage;

            }
        }

        public HttpResponseMessage PostCreateLeak(string LeakText, int UserLeakedId, int UserId)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            try
            {
                Leak leak = new Leak();
                leak.CreatedOn = DateTime.Now;
                leak.LeakText = LeakText;

                leak.UserWrote = userRepository.Find(UserId);

                leak.UserLeaked = new Infra.Models.User();
                leak.UserLeaked = userRepository.Find(UserLeakedId);

                leakRepository.Save(leak);

                if (leak.UserLeaked.Email != null)
                {
                    try
                    {
                        Thread emailThread = new Thread(delegate()
                        {
                            ServicoDeEmail email = new ServicoDeEmail();
                            email.EnviarEmailLeaked(leak.UserLeaked.Email, leak.UserLeaked.FirstName, leak.LeakText);
                        });

                        emailThread.IsBackground = true;
                        emailThread.Start();

                    }
                    catch { }
                }
                returnMessage.Sucess = true;
                returnMessage.Message = "You leaked your friend.";
                var response = Request.CreateResponse<ReturnMessage>(HttpStatusCode.Created, returnMessage);
                return response;
            }
            catch
            {
                returnMessage.Sucess = false;
                returnMessage.Message = "Ops something got wrong.";
                var response = Request.CreateResponse<ReturnMessage>(HttpStatusCode.Created, returnMessage);
                return response;

            }
        }


        public bool Create(LeakOpinion leakopinion)
        {
            if (ModelState.IsValid)
            {

                if (!leakopinion.Leak.LeakOpinions.Where(p => p.User.Id == leakopinion.User.Id).Any())
                {
                    leakOpinionRepository.Save(leakopinion);
                    return true;
                }
                else
                    return false;

            }
            return false;
        }


        public ReturnMessage PostCreateLeakOld(string LeakText, int UserLeakedId, int UserId)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            try
            {
                Leak leak = new Leak();
                leak.CreatedOn = DateTime.Now;
                leak.LeakText = LeakText;

                leak.UserWrote = userRepository.Find(UserId);

                leak.UserLeaked = new Infra.Models.User();
                leak.UserLeaked = userRepository.Find(UserLeakedId);

                leakRepository.Save(leak);

                if (leak.UserLeaked.Email != null)
                {
                    try
                    {
                        Thread emailThread = new Thread(delegate()
                        {
                            ServicoDeEmail email = new ServicoDeEmail();
                            email.EnviarEmailLeaked(leak.UserLeaked.Email, leak.UserLeaked.FirstName, leak.LeakText);
                        });

                        emailThread.IsBackground = true;
                        emailThread.Start();

                    }
                    catch { }
                }
                returnMessage.Sucess = true;
                returnMessage.Message = "You just leaked your friend.";
                return returnMessage;
            }
            catch
            {
                returnMessage.Sucess = false;
                returnMessage.Message = "Ops something got wrong.";
                return returnMessage;

            }
        }




        // POST api/default1
        public void Post([FromBody]string value)
        {
        }

        // PUT api/default1/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/default1/5
        public void Delete(int id)
        {
        }
    }
}
