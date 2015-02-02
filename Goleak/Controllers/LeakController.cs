using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Goleak.Infra.Models;
using Goleak.Infra.Repositorios;
using Goleak.Infra.Email;
using System.Threading;

namespace Goleak.Controllers
{   
    public class LeakController : BaseController
    {
		private readonly IUserRepository userRepository;
		private readonly ILeakRepository leakRepository;

		// If you are using Dependency Injection, you can delete the following constructor
        public LeakController() : this(new UserRepository(), new LeakRepository())
        {
        }

        public LeakController(IUserRepository userRepository, ILeakRepository leakRepository)
        {
			this.userRepository = userRepository;
			this.leakRepository = leakRepository;
        }

        //
        // POST: /Leak/Create

        [HttpPost]
        public PartialViewResult Create(string LeakText, int UserLeakedId)
        {
            try
            {
                Leak leak = new Leak();
                leak.CreatedOn = DateTime.Now;
                leak.LeakText = LeakText;

                leak.UserWrote = base.UserLogged;

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
                return PartialView("_LeakPartial", leak);
            }
            catch
            {
                return null;
            }
        }


        public ViewResult Index(int id)
        {
            Leak leak = leakRepository.Find(id);

            if (leak != null)
            {
                return View(leak);
            }
            if(UserLogged   != null)
                RedirectToAction("Feed", "User");
            else
                RedirectToAction("Login", "Account");
            
            return View(new Leak());
        }

        /*
        //
        // GET: /Leak/

        public ViewResult Index()
        {
            return View(leakRepository.AllIncluding(leak => leak.UserWrote, leak => leak.UserLeaked));
        }

        //
        // GET: /Leak/Details/5

        public ViewResult Details(int id)
        {
            return View(leakRepository.Find(id));
        }

        //
        // GET: /Leak/Create

        public ActionResult Create()
        {
            ViewBag.PossibleUserWrote = userRepository.All;
            ViewBag.PossibleUserLeaked = userRepository.All;
            return View();
        } 

        //
        // GET: /Leak/Edit/5
 
        public ActionResult Edit(int id)
        {
			ViewBag.PossibleUserWrote = userRepository.All;
			ViewBag.PossibleUserLeaked = userRepository.All;
             return View(leakRepository.Find(id));
        }

        //
        // POST: /Leak/Edit/5

        [HttpPost]
        public ActionResult Edit(Leak leak)
        {
            if (ModelState.IsValid) {
                leakRepository.InsertOrUpdate(leak);
                leakRepository.Save();
                return RedirectToAction("Index");
            } else {
				ViewBag.PossibleUserWrote = userRepository.All;
				ViewBag.PossibleUserLeaked = userRepository.All;
				return View();
			}
        }

        //
        // GET: /Leak/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View(leakRepository.Find(id));
        }

        //
        // POST: /Leak/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            leakRepository.Delete(id);
            leakRepository.Save();

            return RedirectToAction("Index");
        }
        */


    }
}

