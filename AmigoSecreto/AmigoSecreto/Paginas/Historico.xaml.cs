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
	public partial class Historico : ContentPage
	{
        string userEmail = Application.Current.Properties["UserEmail"].ToString();
        public Historico ()
		{
			InitializeComponent();
			CarregarLista();
            BindingContext = this;
		}

		public async void CarregarLista()
		{
			var list = new ServicosPrincipais();
			var listagem = list.ListadorSorteios(userEmail);

			ListaSorteios_CollectionView.ItemsSource = await listagem;
		}

        private async void ListaSorteios_CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is AmigoSecreto.Entidades.Sorteio sorteioSelecionado)
            {
                Application.Current.Properties["SorteioId"] = sorteioSelecionado.IdSorteio;
                await Navigation.PushAsync(new EdicaoSorteio());
            }
        }

        private void AtualizarLista_Button_Clicked(object sender, EventArgs e)
        {
            CarregarLista();
        }
    }
}