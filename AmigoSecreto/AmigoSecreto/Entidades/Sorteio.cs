using AmigoSecreto.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmigoSecreto.Entidades
{
    public class Sorteio
    {
        // 1 - Atributos
        public string Criador { get; set; }
        public string Nome { get; set; }
        public string IdSorteio { get; set; }
        public DateTime DataCriacao { get; set; }
        public StatusSorteio StatusSorteio { get; set; }
        public List<Participantes> Participantes { get; set; } = new List<Participantes>();

        // 2 - Construtores

        public Sorteio() { }

        public Sorteio(string criador, string nome, string idSorteio, DateTime dataCriação, StatusSorteio statusSorteio)
        {
            Criador = criador;
            Nome = nome;
            IdSorteio = idSorteio;
            DataCriacao = dataCriação;
            StatusSorteio = statusSorteio;
            Participantes = new List<Participantes>();
        }



        // 3 - Propriedades Customizadas

        // 4 - Outros Metodos

        public void AdicionarParticipante(Participantes participantes)
        {
            Participantes.Add(participantes);
        }

        public void RemoverParticipante(Participantes participantes)
        {
            Participantes.Remove(participantes);
        }

        public Dictionary<string, string> RealizarSorteio()
        {
            var sorteio = new Dictionary<string, string>();
            var sorteados = new List<Participantes>(Participantes);

            Random random = new Random();

            foreach (var participante in Participantes)
            {
                var participantesId = sorteados.Where(s => s != participante).ToList();

                if (participantesId.Count == 0)
                {
                    return RealizarSorteio();
                }

                int indiceSorteado = random.Next(participantesId.Count);
                var sorteado = participantesId[indiceSorteado];

                sorteio[participante.Nome] = sorteado.Nome;
                sorteados.Remove(sorteado);
            }
            return sorteio;
        }
    }
}
