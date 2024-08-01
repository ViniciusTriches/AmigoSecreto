using System;
using System.Net.Mail;
using Xamarin.Essentials;

namespace AmigoSecreto.Classes
{
    public class ValidadorEmail
    {
        public string email { get; set; }

        public ValidadorEmail() { }

        public bool IsValidEmail(string email, out string mensagemDeErro)
        {
            try
            {
                if (!string.IsNullOrEmpty(email))
                {
                    MailAddress emailAddress = new MailAddress(email);
                    mensagemDeErro = string.Empty;
                    return true;
                }
                mensagemDeErro = "O Email informado é invalido!";
                return false;
            }
            catch
            {
                mensagemDeErro = "O Email informado é invalido!";
                return false;
            }
        }
    }
}