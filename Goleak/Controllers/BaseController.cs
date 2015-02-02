using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Goleak.Helpers;
using Goleak.Infra.Models;

namespace Goleak.Controllers
{
    public class BaseController : Controller
    {

        public class Notificacao
        {
            public Notificacao(String texto, TipoNotificacao tipo)
            {
                Texto = texto;
                Tipo = tipo;
            }
            public enum TipoNotificacao
            {
                Alerta = 1,
                Erro = 2,
                Sucesso = 3
            }
            public String Texto { get; set; }
            public TipoNotificacao Tipo { get; set; }
        }

        protected override void ExecuteCore()
        {
            string cultureName = null;
            // Attempt to read the culture cookie from Request
            HttpCookie cultureCookie = Request.Cookies["_culture"];
            if (cultureCookie != null)
                cultureName = cultureCookie.Value;
            else
                cultureName = Request.UserLanguages[0]; // obtain it from HTTP header AcceptLanguages

            // Validate culture name
            cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe


            // Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            base.ExecuteCore();
        }


        public virtual User UserLogged
        {
            get 
            { 
                return (User)(Session["UserLogged"]); 
            }
            set 
            {
                Session["UserLogged"] = value;
            }
        }

        protected void ExibirNotificacao(Notificacao msg)
        {
            if (!TempData.ContainsKey("msg"))
            {
                TempData["msg"] = new List<Notificacao>();
            }
            TempData.Keep("msg");
            ((IList<Notificacao>)TempData["msg"]).Add(msg);

            if (msg.Tipo == Notificacao.TipoNotificacao.Erro)
                ModelState.AddModelError("", msg.Texto);
        }

        protected void ExibirNotificacao(String texto, Notificacao.TipoNotificacao tipo)
        {
            ExibirNotificacao(new Notificacao(texto, tipo));
        }


        public string GetUserIp()
        {
            try
            {
                string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ip))
                {
                    ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                return ip;
            }
            catch
            {
                return string.Empty;
            }
        }

        protected void ExibirNotificacaoModelState()
        {
            var erros = new List<string>();

            foreach (var item in ModelState.Where(p => p.Value.Errors.Count > 0 ).Select(p => p.Value))
            {
                foreach (var erro in item.Errors)
                    erros.Add(erro.ErrorMessage);
            }

            if (erros.Count > 0)
                ExibirNotificacao(erros.Distinct().Aggregate((a, b) => a + " <br> " + b), Notificacao.TipoNotificacao.Erro);
        }

        public RedirectToRouteResult RedirectToActionWithNotification(string actionName, string notificationMessage, Notificacao.TipoNotificacao type)
        {
            ExibirNotificacao(notificationMessage, type);
            var routeValues = new RouteValueDictionary {
                {"message", notificationMessage},
                {"NotificationType", type}
            };

            return RedirectToAction(actionName, routeValues);
        }

        public RedirectToRouteResult RedirectToActionWithNotification(string actionName, string controller, string notificationMessage, Notificacao.TipoNotificacao type)
        {
            ExibirNotificacao(notificationMessage, type);
            var routeValues = new RouteValueDictionary {
                {"message", notificationMessage},
                {"NotificationType", type}
            };

            return RedirectToAction(actionName, controller, routeValues);
        }


    }
}
