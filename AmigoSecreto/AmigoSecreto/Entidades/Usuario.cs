using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using AmigoSecreto.Classes.Exceptions;

namespace AmigoSecreto.Entidades
{
    public class Usuario
    {
        public string nome { get; set; }
        public string email { get; set; }
        public string senha { get; set; }
        public int numeroSorteios { get; set; }

        public Usuario() { }

        public Usuario(string nome, string email, string senha, int numeroSorteio)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new RegistrationException("Email não pode ser vazio");
            }
            
            this.nome = nome;
            this.email = email;
            this.senha = senha;
            this.numeroSorteios = numeroSorteio;
        }
    }
}
