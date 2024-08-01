using AmigoSecreto.Entidades;
using AmigoSecreto.Classes.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using System.Linq;
using System.Reactive.Subjects;
using AmigoSecreto.Entidades.Enums;
using System.Diagnostics.Contracts;


namespace AmigoSecreto.Classes.Services
{
    public class ServicosPrincipais
    {
        public static string firebase_key = "CKPEchlczobO577cnfG0qTDG4CmCDW5dPN218Oda";
        FirebaseClient Client = new FirebaseClient("https://amigo-secreto-5c6da-default-rtdb.firebaseio.com/", new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(firebase_key) });

        public async Task<string> CriacaoSorteio(string usuario, string nome)
        {
            try
            {
                Guid guid = Guid.NewGuid();
                var sorteio = new Sorteio()
                {
                    Criador = usuario,
                    Nome = nome,
                    IdSorteio = guid.ToString(),
                    DataCriacao = DateTime.Now,
                    StatusSorteio = 0,
                    Participantes = new List<Participantes>()
                };
                await Client.Child("Sorteios")
                    .PostAsync(sorteio);
                return sorteio.IdSorteio;
            }
            catch
            {
                return null;
            }
        }
        public async Task<bool> AdicionarParticipanteSorteio(string idSorteio, Participantes participante)
        {
            var sorteioKey = (await Client.Child("Sorteios")
                .OnceAsync<Sorteio>())
                .FirstOrDefault(f => f.Object.IdSorteio == idSorteio)?.Key;

            if (sorteioKey == null)
            {
                return false;
            }

            try
            {
                var sorteio = await BuscadorSorteio(idSorteio);

                if (sorteio == null)
                {
                    return false;
                }

                sorteio.AdicionarParticipante(participante);

                await Client.Child("Sorteios")
                    .Child(sorteioKey)
                    .PutAsync(sorteio);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AtualizarParticipante(string idSorteio, Participantes participante, int participanteIndex)
        {
            var sorteioKey = (await Client.Child("Sorteios")
        .OnceAsync<Sorteio>())
        .FirstOrDefault(f => f.Object.IdSorteio == idSorteio)?.Key;

            if (sorteioKey == null)
            {
                return false;
            }

            try
            {
                var sorteio = await BuscadorSorteio(idSorteio);
                if (sorteio.Participantes != null && participanteIndex >= 0 && participanteIndex < sorteio.Participantes.Count)
                {
                    sorteio.Participantes[participanteIndex] = participante;

                    await Client.Child("Sorteios")
                        .Child(sorteioKey)
                        .PutAsync(sorteio);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoverParticipante(string idSorteio, int participanteIndex)
        {
            var sorteioKey = (await Client.Child("Sorteios")
                .OnceAsync<Sorteio>())
                .FirstOrDefault(f => f.Object.IdSorteio == idSorteio)?.Key;

            if (sorteioKey == null)
            {
                return false;
            }

            try
            {
                var sorteio = await BuscadorSorteio(idSorteio);
                if (sorteio.Participantes != null && participanteIndex >= 0 && participanteIndex < sorteio.Participantes.Count)
                {
                    sorteio.Participantes.RemoveAt(participanteIndex);

                    await Client.Child("Sorteios")
                        .Child(sorteioKey)
                        .PutAsync(sorteio);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }


        public async Task RealizarSorteio(string idSorteio)
        {
            var sorteioKey = (await Client.Child("Sorteios")
                .OnceAsync<Sorteio>())
                .FirstOrDefault(f => f.Object.IdSorteio == idSorteio)?.Key;

            if (sorteioKey == null)
            {
                return;
            }

            try
            {
                var sorteio = await BuscadorSorteio(idSorteio);
                if (sorteio == null)
                {
                    return;
                }
                var pares = sorteio.RealizarSorteio();

                foreach (var par in pares)
                {
                    var participante = sorteio.Participantes.FirstOrDefault(p => p.Nome == par.Key);
                    var escolhido = sorteio.Participantes.FirstOrDefault(p => p.Nome == par.Value);

                    if (participante != null && escolhido != null)
                    {
                        var envioSMTP = new EnvioSMTP();
                        envioSMTP.EnvioEmailSorteio(participante.Email, participante.Nome, escolhido.Nome, escolhido.SugestaoPresentes);
                    }
                }

                sorteio.StatusSorteio = StatusSorteio.Ativo;
                await Client.Child("Sorteios")
                    .Child(sorteioKey)
                    .PutAsync(sorteio);
            }
            catch
            {
                return;
            }
        }

        public async Task<bool> AtualizarSorteio(string idSorteio, Sorteio sorteioAtualizado)
        {
            try
            {
                var sorteioKey = (await Client.Child("Sorteios")
                .OnceAsync<Sorteio>())
                .FirstOrDefault(f => f.Object.IdSorteio == idSorteio)?.Key;

                if (sorteioKey == null)
                {
                    return false;
                }

                try
                {
                    var sorteio = await BuscadorSorteio(idSorteio);
                    if (sorteio == null)
                    {
                        return false;
                    }
                    await Client
                        .Child("Sorteios")
                        .Child(sorteioKey)
                        .PutAsync(sorteioAtualizado);

                    return true;
                }
                catch
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<Sorteio> BuscadorSorteio(string idSorteio)
        {
            var todosSorteios = await Client
                .Child("Sorteios")
                .OnceAsync<Sorteio>();

            var sorteio = todosSorteios
                .FirstOrDefault(s => s.Object.IdSorteio == idSorteio);

            return sorteio?.Object;
        }

        public async Task<List<Sorteio>> ListadorSorteios(string usuario)
        {
            var listagem = (await Client.Child("Sorteios")
                .OnceAsync<Sorteio>())
                .Where(u => u.Object.Criador == usuario)
                .Select(u => new Sorteio()
                {
                    Nome = u.Object.Nome,
                    IdSorteio = u.Object.IdSorteio,
                    DataCriacao = u.Object.DataCriacao,
                    StatusSorteio = u.Object.StatusSorteio,
                })
                .OrderByDescending(u => u.DataCriacao)
                .ToList();
            return listagem;
        }

        public async Task<List<Participantes>> ListadorParticipantes(string idSorteio)
        {
            var sorteioKey = (await Client.Child("Sorteios")
                .OnceAsync<Sorteio>())
                .FirstOrDefault(f => f.Object.IdSorteio == idSorteio)?.Key;

            if (sorteioKey == null)
            {
                return null;
            }

            try
            {
                var sorteio = await BuscadorSorteio(idSorteio);

                if (sorteio != null)
                {
                    return sorteio.Participantes;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
