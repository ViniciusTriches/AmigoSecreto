using System;
using System.Collections.Generic;
using System.Text;

namespace AmigoSecreto.Entidades
{
    public class Participantes
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string SugestaoPresentes { get; set; }

        public Participantes() { }

        public Participantes(string nome, string email, string sugestaoPresentes)
        {
            Nome = nome;
            Email = email;
            SugestaoPresentes = sugestaoPresentes;
        }
    }
}
