using AmigoSecreto.Classes;
using AmigoSecreto.Classes.Services;
using AmigoSecreto.Classes.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AmigoSecreto.Paginas
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Registro : ContentPage
	{
        bool carregamento;
		public Registro ()
		{
			InitializeComponent ();
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

        private async void ConfirmRegistro_Button_Clicked(object sender, EventArgs e)
        {
            TelaCarregamento();
            if (SenhaRegistro_Entry.Text != ConfirmSenhaRegistro_Entry.Text)
            {
                await DisplayAlert("Ops", "As senhas não correspondem", "Ok");
                TelaCarregamento();
                return;
            }
            string erro;
            bool emailValido = new ValidadorEmail().IsValidEmail(EmailRegistro_Entry.Text, out erro);
            if (!emailValido)
            {
                await DisplayAlert("Ops", erro, "Ok");
                TelaCarregamento();
                return;
            }

            bool senhaValida = ValidadorSenha.IsValidPassword(SenhaRegistro_Entry.Text, out erro);

            if (!senhaValida)
            {
                await DisplayAlert("Ops", erro, "Ok");
                TelaCarregamento();
                return;
            }

            var cadastro = new ServicosUsuario();
            bool cadastroDuplicado = await cadastro.ValidadorCadastroDuplicado(EmailRegistro_Entry.Text);
            if (cadastroDuplicado)
            {
                await DisplayAlert("Ops", "Já existe cadastro para email encontrado!", "Ok");
                TelaCarregamento();
                return;
            }

            try
            {
                var acesso = new ServicosUsuario();
                bool cadastroAcesso = await acesso.CadastroUsuario(
                    NomeRegistro_Entry.Text,
                    EmailRegistro_Entry.Text,
                    SenhaRegistro_Entry.Text);

                if (cadastroAcesso)
                {
                    await DisplayAlert("Sucesso", "Usuário cadastrado!", "Ok");
                    TelaCarregamento();
                    return;
                }
                else
                {
                    await DisplayAlert("Ops", "Não foi possível cadastrar o usuario! Tente novamente!", "Ok");
                    TelaCarregamento();
                }

            }
            catch
            {
                await DisplayAlert("Ops", "Houve um erro ao efetuar o cadatro!\nTente novamente!", "Ok");
                TelaCarregamento();
            }
        }

        private void CancelarRegistro_Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MainPage());
        }
    }
}