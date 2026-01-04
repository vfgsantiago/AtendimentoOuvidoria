using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using AtendimentoOuvidoria.UI.Web.Helpers;

namespace AtendimentoOuvidoria.UI.Web.Services
{
    public class EmailService : IEmailService
    {
        #region DI
        private EmailSettings _emailSettings { get; }
        private ILogger<EmailService> _logger;
        IConfiguration _configuration;

        public EmailService(IOptions<EmailSettings> emailSettings,
            IConfiguration configuration,
            ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _configuration = configuration;
            _logger = logger;
        }
        #endregion

        #region Email
        public async Task EnviarEmail(string email, string nome, string subject, string titulo, string message)
        {
            try
            {
                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.FromEmail, "Atendimento")
                };

                mail.To.Add(new MailAddress(email));

                //copia oculta padrao
                mail.Bcc.Add(new MailAddress(_emailSettings.BccEmail));
                mail.Subject = subject;
                mail.Body = PopulateBody(nome, titulo, message);
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                using (SmtpClient smtp = new SmtpClient(_emailSettings.PrimaryDomain))
                {
                    smtp.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Falha ao envia email - {ex.Message}");
            }
        }


        public async Task EnviarEmailAnexo(string email, string nome, string subject, string titulo, string message, byte[] anexo)
        {
            try
            {
                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.FromEmail, "Atendimento")
                };

                mail.To.Add(new MailAddress(email));

                MemoryStream stream = new MemoryStream();
                stream.Write(anexo, 0, anexo.Length);
                stream.Position = 0;
                Attachment attachment = new Attachment(stream, "atendimento.png", mediaType: MediaTypeNames.Image.Png);
                attachment.ContentDisposition.Inline = false;
                attachment.TransferEncoding = TransferEncoding.Base64;
                mail.Attachments.Add(attachment);

                //copia oculta padrao
                mail.Bcc.Add(new MailAddress(_emailSettings.BccEmail));
                mail.Subject = subject;
                mail.Body = PopulateBody(nome, titulo, message);
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                using (SmtpClient smtp = new SmtpClient(_emailSettings.PrimaryDomain))
                {
                    smtp.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Falha ao envia email - {ex.Message}");
            }
        }


        /// <summary>
        /// Repalce dos valores no template padrão
        /// </summary>
        /// <param name="Nome"></param>
        /// <param name="Titulo"></param>
        /// <param name="Url"></param>
        /// <param name="Texto"></param>
        /// <returns></returns>
        private string PopulateBody(string Nome, string Titulo, string Texto)
        {
            string body = string.Empty;

            string path = "wwwroot/Template/Email.html";

            using (StreamReader reader = new StreamReader(path))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{NOME}", Nome);
            body = body.Replace("{TITULO}", Titulo);
            body = body.Replace("{TEXTO}", Texto);
            return body;
        }

        #endregion
    }

    #region Interface
    public interface IEmailService
    {
        /// <summary>
        /// Envia um email e preenche os parametros no template padrão
        /// </summary>
        /// <param name="id"></param>
        /// <param name="subject"></param>
        /// <param name="titulo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task EnviarEmailAnexo(string email, string nome,  string subject, string titulo, string message, byte[] atachment);

        Task EnviarEmail(string email, string nome, string subject, string titulo, string message);
    }
    #endregion
}
