using System;
using System.Web;
using NHibernate;
using Goleak.Infra.Banco;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System.IO;

namespace Goleak.Infra
{
    public class SessaoPorRequisicaoModule : IHttpModule
    {
        public void Dispose()
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public void Init(HttpApplication context)
        {
            context.BeginRequest += OnBeginRequest;
            context.EndRequest += OnEndRequest;
        }

        private static void OnBeginRequest(object sender, EventArgs e)
        {
            ConstruirSessionFactory();
            AbrirSessao();
        }

        private static void OnEndRequest(object sender, EventArgs e)
        {
            FecharSessao();
        }

        private static ISessionFactory sessionFactory;

        private static void ConstruirSessionFactory()
        {
            if (sessionFactory != null)
                return;

            sessionFactory = BancoConfiguracaoFactory.CriarBancoConfiguracao().ConstruirSessionFactory();

            HabilitarLog();
        }

        private static void HabilitarLog()
        {
            if (!HttpContext.Current.IsDebuggingEnabled)
                return;

            log4net.Config.XmlConfigurator.Configure();
        }

        public static ISession Session
        {
            get { return (ISession) HttpContext.Current.Items["hibernate.current.session"]; }
            set { HttpContext.Current.Items["hibernate.current.session"] = value; }
        }

        private static void AbrirSessao()
        {
            Session = sessionFactory.OpenSession();
        }

        private static void FecharSessao()
        {
            var session = Session;

            if (session == null)
                return;

            session.Dispose();
        }
    }
}