using AmigoSecreto.Classes;
using AmigoSecreto.Classes.Services;
using AmigoSecreto.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AmigoSecreto.Paginas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Sorteio : ContentPage
    {
        string userEmail = Application.Current.Properties["UserEmail"].ToString();
        Participantes ParticipanteSelecionado;
        int ParticipanteSelecionadoIndex = -1;

        public Sorteio()
        {
            InitializeComponent();
            ListLoad();
        }

        public async void ListLoad()
        {
            if (Application.Current.Properties.ContainsKey("SorteioId"))
            {
                string idSorteio = Application.Current.Properties["SorteioId"].ToString();
                var servicos = new ServicosPrincipais();


                var sorteio = await servicos.BuscadorSorteio(idSorteio);
                if (sorteio != null)
                {
                    SorteioNome_Entry.Text = sorteio.Nome;
                    ParticipantesSorteio_CollectionView.ItemsSource = sorteio.Participantes;
                }
            }
        }

        public void LimpadorCampos()
        {
            ParticipanteNome_Entry.Text = string.Empty;
            ParticipanteEmail_Entry.Text = string.Empty;
            ParticipantePresente_Entry.Text = string.Empty;
            SorteioNome_Entry.Text = string.Empty;
            ParticipanteSelecionado = null;
            ParticipanteSelecionadoIndex = -1;
            AdicionarParticipante_Button.Text = "Adicionar Participante";
            
        }

        private async void RegistrarSorteio_Button_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SorteioNome_Entry.Text))
            {
                await DisplayAlert("Erro", "É obrigatorio informar nome do sorteio", "Ok");
                return;
            }

            var servico = new ServicosPrincipais();
            var sorteioId = await servico.CriacaoSorteio(userEmail, SorteioNome_Entry.Text);

            if (sorteioId != null)
            {
                DisplayAlert("✅ Sucesso", "Sorteio Criado", "Ok");
                Application.Current.Properties["SorteioId"] = sorteioId;

                ParticipanteButtons_Grid.IsVisible = true;
                ParticipanteDados_Frame.IsVisible = true;
                FinalizarSorteio_Button.IsVisible = true;
                FinalizarSorteio_Button.IsEnabled = true;

                SorteioNome_Entry.IsEnabled = false;
                RegistrarSorteio_Button.IsVisible = false;
            }
            else
            {
                await DisplayAlert("Erro", "Falha ao criar sorteio! Tente novamente!", "Ok");
            }
        }

        private async void AdicionarParticipante_Button_Clicked(object sender, EventArgs e)
        {
            if (!Application.Current.Properties.ContainsKey("SorteioId"))
            {
                await DisplayAlert("Erro", "Nenhum sorteio foi criado.", "Ok");
                return;
            }

            string idSorteio = Application.Current.Properties["SorteioId"].ToString();

            if (string.IsNullOrEmpty(ParticipanteEmail_Entry.Text)
                || string.IsNullOrEmpty(ParticipanteNome_Entry.Text)
                || string.IsNullOrEmpty(ParticipantePresente_Entry.Text))
            {
                await DisplayAlert("Erro", "É obrigatório informar os dados do participante", "Ok");
                return;
            }

            var validarEmail = new ValidadorEmail().IsValidEmail(ParticipanteEmail_Entry.Text, out string erro);
            if (!validarEmail)
            {
                await DisplayAlert("Erro", erro, "Ok");
                return;
            }

            var novoParticipante = new Participantes()
            {
                Nome = ParticipanteNome_Entry.Text,
                Email = ParticipanteEmail_Entry.Text,
                SugestaoPresentes = ParticipantePresente_Entry.Text
            };

            var servico = new ServicosPrincipais();
            bool result;

            if (ParticipanteSelecionadoIndex == -1)
            {
                result = await servico.AdicionarParticipanteSorteio(idSorteio, novoParticipante);
            }
            else
            {
                result = await servico.AtualizarParticipante(idSorteio, novoParticipante, ParticipanteSelecionadoIndex);
            }

            if (result)
            {
                await DisplayAlert("✅ Sucesso", "Participante salvo com sucesso", "Ok");
                LimpadorCampos();
                ListLoad();
            }
            else
            {
                await DisplayAlert("Erro", "Falha ao salvar participante", "Ok");
            }
        }

        private async void RemoverParticipante_Button_Clicked(object sender, EventArgs e)
        {
            if (ParticipanteSelecionadoIndex == -1)
            {
                await DisplayAlert("Erro", "Nenhum participante selecionado.", "Ok");
                return;
            }

            var confirmacao = await DisplayAlert("Confirmação", "Você tem certeza que deseja remover este participante?", "Sim", "Não");
            if (confirmacao)
            {
                string idSorteio = Application.Current.Properties["SorteioId"].ToString();
                var servico = new ServicosPrincipais();

                bool result = await servico.RemoverParticipante(idSorteio, ParticipanteSelecionadoIndex);

                if (result)
                {
                    await DisplayAlert("✅ Sucesso", "Participante removido com sucesso", "Ok");
                    LimpadorCampos();
                    ListLoad();
                }
                else
                {
                    await DisplayAlert("Erro", "Falha ao remover participante", "Ok");
                }
            }
        }

        private async void FinalizarSorteio_Button_Clicked(object sender, EventArgs e)
        {
            string idSorteio = Application.Current.Properties["SorteioId"].ToString();
            var servico = new ServicosPrincipais();
            var sorteio = await servico.BuscadorSorteio(idSorteio);

            if (sorteio == null || sorteio.Participantes.Count < 3)
            {
                await DisplayAlert("Ops", "O sorteio precisa de ao menos 3 participantes para ser finalizado", "Ok");
                return;
            }

            await servico.RealizarSorteio(idSorteio);
            await DisplayAlert("✅ Sucesso", "Sorteio realizado com sucesso!", "Ok");

            ParticipanteButtons_Grid.IsVisible = false;
            ParticipanteDados_Frame.IsVisible = false;
            FinalizarSorteio_Button.IsVisible = false;
            FinalizarSorteio_Button.IsEnabled = false;

            SorteioNome_Entry.IsEnabled = true;
            RegistrarSorteio_Button.IsVisible = true;
        }

        private void ParticipantesSorteio_CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Participantes participante)
            {
                ParticipanteNome_Entry.Text = participante.Nome;
                ParticipanteEmail_Entry.Text = participante.Email;
                ParticipantePresente_Entry.Text = participante.SugestaoPresentes;

                ParticipanteSelecionado = participante;
                ParticipanteSelecionadoIndex = (ParticipantesSorteio_CollectionView.ItemsSource as List<Participantes>).IndexOf(participante);
                AdicionarParticipante_Button.Text = "Atualizar Cadastro";
            }
        }
    }
}