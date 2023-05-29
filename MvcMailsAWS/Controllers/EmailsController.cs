using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace MvcMailsAWS.Controllers
{
    public class EmailsController : Controller
    {
        private IConfiguration configuration;

        public EmailsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index
            (string email, string subject, string body)
        {
            string user = this.configuration.GetValue<string>
                ("AWS:EmailCredentials:User");
            string emailSender =
                this.configuration.GetValue<string>
                ("AWS:EmailCredentials:Email");
            string server = this.configuration.GetValue<string>
                ("AWS:EmailCredentials:Server");
            string password = this.configuration.GetValue<string>
               ("AWS:EmailCredentials:Password");
            MailMessage message = new MailMessage ();
            //FROM: CUENTA DEL SENDER DE AWS
            message.From = new MailAddress (emailSender);
            message.To.Add(new MailAddress (email));
            message.Subject = subject;
            message.Body = body;
            //CONFIGURAMOS LAS CREDENCIALES DE NUESTRO SERVICIO
            NetworkCredential credentials = 
                new NetworkCredential (user, password);
            //CONFIGURAMOS EL SERVIDOR SMTP
            SmtpClient smtpClient = new SmtpClient ();
            smtpClient.Host = server;
            smtpClient.Port = 25;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = true;
            smtpClient.Credentials = credentials;
            await smtpClient.SendMailAsync (message);
            ViewData["MENSAJE"] = "Mail enviado correctamente";
            return View();
        
        }

        public IActionResult MailAWS()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MailAWS
            (string email, string subject, string body)
        {
            string emailSender =
                this.configuration.GetValue<string>
                ("AWS:EmailCredentials:Email");
            AmazonSimpleEmailServiceClient client = 
                new AmazonSimpleEmailServiceClient (RegionEndpoint.USEast1);

            Destination destination = new Destination();
            //CREAMOS UNA COLECCION DONDE ENVIAREMOS LOS MAILS
            destination.ToAddresses = new List<string> { email };
            Message message = new Message ();
            message.Subject = new Amazon.SimpleEmail.Model.Content(subject);
            message.Body =
                new Body(new Amazon.SimpleEmail.Model.Content(body));
            //TODOS LOS SERVICIOS AWS SON IGUALES. 
            //TENDREMOS UN REQUEST Y UN RESPONSE
            SendEmailRequest request = new SendEmailRequest ();
            //DEBEMOS INDICAR TRES DATOS
            request.Destination = destination;
            request.Message = message;
            //NECESITA EL SOURCE, QUE ES EL EMAIL DE USER VERIFIED
            //DE NUESTRO SERVICIO
            request.Source = emailSender;   
            
            SendEmailResponse response=
                await client.SendEmailAsync (request);
            ViewData["MENSAJE"] = "Email enviado correctamente AWS";
            return View();

        }




    }
}
