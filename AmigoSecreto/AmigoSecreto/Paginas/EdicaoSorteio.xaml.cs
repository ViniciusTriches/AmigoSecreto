using AmigoSecreto.Classes;
using AmigoSecreto.Classes.Services;
using AmigoSecreto.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AmigoSecreto.Paginas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EdicaoSorteio : ContentPage
    {
        string userEmail = Application.Current.Properties["UserEmail"].ToString();
        Participantes ParticipanteSelecionado;
        int ParticipanteSelecionadoIndex = -1;
        public EdicaoSorteio()
        {
            InitializeComponent();
            ListLoad();
        }

        public void LimpadorCampos()
        {
            ParticipanteNome_Entry.Text = string.Empty;
            ParticipanteEmail_Entry.Text = string.Empty;
            ParticipantePresente_Entry.Text = string.Empty;
            ParticipanteSelecionado = null;
            ParticipanteSelecionadoIndex = -1;
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

        private async void FinalizarEdicao_Button_Clicked(object sender, EventArgs e)
        {
            if (!Application.Current.Properties.ContainsKey("SorteioId"))
            {
                await DisplayAlert("Erro", "Nenhum sorteio foi selecionado.", "Ok");
                return;
            }

            string idSorteio = Application.Current.Properties["SorteioId"].ToString();

            if (string.IsNullOrEmpty(SorteioNome_Entry.Text))
            {
                await DisplayAlert("Erro", "É obrigatório informar o nome do sorteio.", "Ok");
                return;
            }

            // Cria o objeto Sorteio atualizado
            var sorteioAtualizado = new AmigoSecreto.Entidades.Sorteio()
            {
                IdSorteio = idSorteio,
                Nome = SorteioNome_Entry.Text,
                Participantes = (ParticipantesSorteio_CollectionView.ItemsSource as List<Participantes>) ?? new List<Participantes>()
            };

            var servico = new ServicosPrincipais();
            bool result = await servico.AtualizarSorteio(idSorteio, sorteioAtualizado);

            if (result)
            {
                await DisplayAlert("✅ Sucesso", "Sorteio atualizado com sucesso", "Ok");
                await Navigation.PopAsync(); // Retorna à página anterior
            }
            else
            {
                await DisplayAlert("Erro", "Falha ao atualizar o sorteio", "Ok");
            }
        }

        private async void AtualizarCadastro_Button_Clicked(object sender, EventArgs e)
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

        private void ParticipantesSorteio_CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Participantes participante)
            {
                ParticipanteNome_Entry.Text = participante.Nome;
                ParticipanteEmail_Entry.Text = participante.Email;
                ParticipantePresente_Entry.Text = participante.SugestaoPresentes;

                ParticipanteSelecionado = participante;
                ParticipanteSelecionadoIndex = (ParticipantesSorteio_CollectionView.ItemsSource as List<Participantes>).IndexOf(participante);
            }
        }
    }
}