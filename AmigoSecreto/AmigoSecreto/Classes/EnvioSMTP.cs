using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using Xamarin.Forms.Xaml;

namespace AmigoSecreto.Classes
{
    public class EnvioSMTP
    {
        public EnvioSMTP() { }

        public void EnvioEmailSorteio(string toAddres, string participante, string escolhido, string presentes)
        {
            try
            {
                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.Credentials = new NetworkCredential("amigosecretosilches@gmail.com", "laqf yqly puys epxq");
                    smtpClient.EnableSsl = true;

                    MailMessage message = new MailMessage();
                    message.From = new MailAddress("amigosecretosilches@gmail.com");
                    message.To.Add(toAddres);
                    message.Subject = "Resultado do Sorteio do Amigo Secreto!🥳";
                    message.Body = $"Olá, {participante}!<br><br>" +
                        "É com grande entusiasmo que anunciamos os resultados do nosso sorteio de Amigo Secreto!🎉<br><br><b>" +
                        $"Seu Amigo Secreto é: {escolhido}</b><br><br>" +
                        $"Lembramos que seu amigo deu as seguinte sugestões de presente {presentes}." +
                        $"A data da revelação será no nosso encontro, certifique-se de ter seu presente pronto até lá!<br><br>" +
                        "Aqui vão algumas dicas para garantir que todos se divirtam e aproveitem ao máximo:<br>- " +
                        "Pense nas preferências e hobbies do seu amigo secreto.<br>- " +
                        "Use a criatividade para escolher um presente especial.<br>- " +
                        "Mantenha o segredo até o momento da revelação!<br><br>" +
                        "Vamos tornar este Amigo Secreto inesquecível!<br><br>";
                    message.IsBodyHtml = true;

                    smtpClient.Send(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao enviar o email: " + ex.Message);
            }
        }

    }
}
