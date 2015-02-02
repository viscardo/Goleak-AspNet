using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using SendGridMail;
using SendGridMail.Transport;

namespace Goleak.Infra.Email
{
    public class ServicoDeEmail
    {
        public ServicoDeEmail() { }


        public void EnviarEmailLeaked(string to, string nomeUsuario, string leak)
        {
            string mensagem = String.Format(this.TemplateGotLeaked(), nomeUsuario, leak);
            this.EnviarEmail(to, mensagem, "GoLeak - A friend said something about you.");
        }

        public void EnviarEmail(string to, string mensagem, string titulo)
        {

            var username = "azure_be720be7f98baf0e75f77038009cfcc6@azure.com";
            var pswd = "sm1zv6vl";

            // Create the email object first, then add the properties.
            SendGrid myMessage = SendGrid.GetInstance();
            myMessage.AddTo(to);
            myMessage.From = new MailAddress("igotleaked@goleak.com", "GoLeak");
            myMessage.Subject = titulo;
            myMessage.Html = mensagem;

            // Create credentials, specifying your user name and password.
            var credentials = new NetworkCredential(username, pswd);

            // Create an SMTP transport for sending email.
            var transportSMTP = SMTP.GetInstance(credentials);

            // Send the email.
            transportSMTP.Deliver(myMessage);
            
        }

  

        private string TemplateGotLeaked()
        {
            string template = @"<p>{0},<br />
                                  A friend wrote on your wall this message: <br />
                                  <b>
                                  {1}
                                  </b>
                                  <br />
                                  Go now at <a href=""http://www.goleak.com""> GoLeak </a> and try to discover who wrote.</p>
                                  ";

            return template;
        }


    }
}
