using AmigoSecreto.Classes.Services;
using AmigoSecreto.Classes.Services.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AmigoSecreto
{
    public partial class MainPage : ContentPage
    {
        bool carregamento;
        public MainPage()
        {
            InitializeComponent();
        }

        public void TelaCarregamento()
        {
            if (carregamento)
            {
                RodaDeCarregamento.IsVisible = false;
                RodaDeCarregamento.IsRunning = false;

                carregamento = false;
            }
            else
            {
                RodaDeCarregamento.IsVisible = true;
                RodaDeCarregamento.IsRunning = true;

                carregamento = true;
            }
        }
        private async void Login_Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                TelaCarregamento();
                var logar = new ServicosUsuario();
                bool loginverification = await logar.Login(LoginEmail_Entry.Text, LoginSenha_Entry.Text);

                if (loginverification)
                {
                    Application.Current.Properties["UserEmail"] = LoginEmail_Entry.Text;
                    await Application.Current.SavePropertiesAsync();

                    LoginEmail_Entry.Text = string.Empty;
                    LoginSenha_Entry.Text = string.Empty;
                    Navigation.PushAsync(new Paginas.Home());

                    TelaCarregamento();
                }
                else
                {
                    await DisplayAlert("Ops", "Usuário e senha não correspondem", "Ok");
                    TelaCarregamento();
                }


            }
            catch
            {
                await DisplayAlert("Ops", "Ocorreu um erro!", "Ok");
                TelaCarregamento();
            }

        }

        private void Registro_Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Paginas.Registro());
        }

        private void Sobre_Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Paginas.Sobre());
        }
    }
}
