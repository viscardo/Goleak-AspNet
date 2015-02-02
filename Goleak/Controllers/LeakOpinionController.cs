using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Goleak.Infra.Models;
using Goleak.Infra.Repositorios;

namespace Goleak.Controllers
{   
    public class LeakOpinionController : BaseController
    {
		private readonly ILeakRepository leakRepository;
		private readonly IUserRepository userRepository;
		private readonly ILeakOpinionRepository leakopinionRepository;

		// If you are using Dependency Injection, you can delete the following constructor
        public LeakOpinionController() : this(new LeakRepository(), new UserRepository(), new LeakOpinionRepository())
        {
        }

        public LeakOpinionController(ILeakRepository leakRepository, IUserRepository userRepository, ILeakOpinionRepository leakopinionRepository)
        {
			this.leakRepository = leakRepository;
			this.userRepository = userRepository;
			this.leakopinionRepository = leakopinionRepository;
        }
        
        [HttpPost]
        public JsonResult Like(int LeakId)
        {
            LeakOpinion leakOpinion = new LeakOpinion();
            leakOpinion.CreatedOn = DateTime.Now;
            leakOpinion.Leak = leakRepository.Find(LeakId);
            leakOpinion.Opinion = true;
            leakOpinion.User = UserLogged;

            if (this.Create(leakOpinion))
                return Json(new { success = true });
            else
                return Json(new { success = false });
        }


        [HttpPost]
        public JsonResult Dislike(int LeakId)
        {
            LeakOpinion leakOpinion = new LeakOpinion();
            leakOpinion.CreatedOn = DateTime.Now;
            leakOpinion.Leak = leakRepository.Find(LeakId);
            leakOpinion.Opinion = false;
            leakOpinion.User = UserLogged;

            if (this.Create(leakOpinion))
                return Json(new { success = true });
            else
                return Json(new { success = false });

            
        }
            
        
        public bool Create(LeakOpinion leakopinion)
        {
            if (ModelState.IsValid)
            {

                if (!leakopinion.Leak.LeakOpinions.Where(p => p.User.Id == leakopinion.User.Id).Any())
                {
                    leakopinionRepository.Save(leakopinion);
                    return true;
                }
                else
                    return false;
              
            }
            return false;
        }

        /*

        //
        // GET: /LeakOpinion/

        public ViewResult Index()
        {
            return View(leakopinionRepository.AllIncluding(leakopinion => leakopinion.Leak, leakopinion => leakopinion.User));
        }

        //
        // GET: /LeakOpinion/Details/5

        public ViewResult Details(int id)
        {
            return View(leakopinionRepository.Find(id));
        }

        //
        // GET: /LeakOpinion/Create

        public ActionResult Create()
        {
			ViewBag.PossibleLeak = leakRepository.All;
			ViewBag.PossibleUser = userRepository.All;
            return View();
        } 

        //
        // POST: /LeakOpinion/Create


        
        //
        // GET: /LeakOpinion/Edit/5
 
        public ActionResult Edit(int id)
        {
			ViewBag.PossibleLeak = leakRepository.All;
			ViewBag.PossibleUser = userRepository.All;
             return View(leakopinionRepository.Find(id));
        }

        //
        // POST: /LeakOpinion/Edit/5

        [HttpPost]
        public ActionResult Edit(LeakOpinion leakopinion)
        {
            if (ModelState.IsValid) {
                leakopinionRepository.InsertOrUpdate(leakopinion);
                leakopinionRepository.Save();
                return RedirectToAction("Index");
            } else {
				ViewBag.PossibleLeak = leakRepository.All;
				ViewBag.PossibleUser = userRepository.All;
				return View();
			}
        }

        //
        // GET: /LeakOpinion/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View(leakopinionRepository.Find(id));
        }

        //
        // POST: /LeakOpinion/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            leakopinionRepository.Delete(id);
            leakopinionRepository.Save();

            return RedirectToAction("Index");
        }
        */

    }
}

