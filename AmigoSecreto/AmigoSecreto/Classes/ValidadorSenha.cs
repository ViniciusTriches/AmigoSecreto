using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AmigoSecreto.Classes
{
    public class ValidadorSenha
    {
        public string senha { get; set; }
        public ValidadorSenha() { }

        public ValidadorSenha(string senha)
        {
            this.senha = senha;
        }

        public static bool IsValidPassword(string password, out string mensagemDeErro)
        {
            var minCaracteres = 8;
            var hasUpperCase = new Regex(@"[A-Z]+");
            var hasLowerCase = new Regex(@"[a-z]+");
            var hasDigit = new Regex(@"[0-9]+");

            if (password.Length < minCaracteres)
            {
                mensagemDeErro = $"A senha deve conter no minimo {minCaracteres} caracteres.";
                return false;
            }

            /*if (!hasUpperCase.IsMatch(password))
            {
                mensagemDeErro = "A senha deve conter no minimo uma letra maiuscula";
                return false;
            }

            if (!hasLowerCase.IsMatch(password))
            {
                mensagemDeErro = "A senha deve conter no minimo uma letra miniscula";
                return false;
            }

            if (!hasDigit.IsMatch(password))
            {
                mensagemDeErro = "A senha deve conter no minimo um digito.";
                return false;
            }*/

            mensagemDeErro = string.Empty;
            return true;
        }
    }
}
