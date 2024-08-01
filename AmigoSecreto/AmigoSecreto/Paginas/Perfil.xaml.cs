using AmigoSecreto.Classes.Services;
using AmigoSecreto.Classes.Services.Services;
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
	public partial class Perfil : ContentPage
	{
        string userEmail = Application.Current.Properties["UserEmail"].ToString();
		

		public Perfil ()
		{
			InitializeComponent ();
			EmailPerfil_Label.Text = userEmail;
			InformacoesPerfil(userEmail);
		}

		public async void InformacoesPerfil(string email)
		{
			var user = await new ServicosUsuario().PegarInfomacoesPerfil(email);

			NamePerfil_Label.Text = user.nome;
		}
	}
}