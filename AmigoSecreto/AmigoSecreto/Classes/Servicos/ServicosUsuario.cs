using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AmigoSecreto.Entidades;
using System.Linq;
using AmigoSecreto.Paginas;
using static System.Net.Mime.MediaTypeNames;

namespace AmigoSecreto.Classes.Services.Services
{
    public class ServicosUsuario
    {
        public static string firebase_key = "CKPEchlczobO577cnfG0qTDG4CmCDW5dPN218Oda";
        FirebaseClient Client = new FirebaseClient("https://amigo-secreto-5c6da-default-rtdb.firebaseio.com/", new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(firebase_key) });

        public async Task<bool> CadastroUsuario(string nome, string email, string senha)
        {
            try
            {
                await Client.Child("Usuarios")
                    .PostAsync(new Usuario()
                    {
                        nome = nome,
                        email = email,
                        senha = senha
                    });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ValidadorCadastroDuplicado(string email)
        {
            var consult = (await Client.Child("Usuarios")
                .OnceAsync<Usuario>())
                .Where(u => u.Object.email == email)
                .FirstOrDefault();

            return consult != null;
        }

        public async Task<bool> Login(string email, string senha)
        {
            var consult = (await Client.Child("Usuarios")
                .OnceAsync<Usuario>())
                .Where(u => u.Object.email == email)
                .Where(u => u.Object.senha == senha)
                .FirstOrDefault();

            return consult != null;
        }

        public async Task<Usuario> PegarInfomacoesPerfil(string email)
        {
            var consult = (await Client.Child("Usuarios")
                .OnceAsync<Usuario>())
                .Where(u => u.Object.email == email)
                .Select(u => u.Object)
                .FirstOrDefault();

            return consult;
        } 
    }
}
