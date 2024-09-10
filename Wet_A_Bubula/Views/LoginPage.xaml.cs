using Wet_A_Bubula.ViewModels;
using Microsoft.Maui.Controls;


namespace Wet_A_Bubula;

public partial class LoginPage : ContentPage
{
	
    public LoginPage()
    {
        InitializeComponent();
     
        BindingContext = new LoginPageViewModel();
    }

    private async void SignUp(object sender, EventArgs e)
    {
        var registryPage = new RegistryPage();
        await Navigation.PushAsync(registryPage);
    }
}