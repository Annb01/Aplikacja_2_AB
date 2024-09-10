using Wet_A_Bubula.Model;
using Wet_A_Bubula.ViewModels;

namespace Wet_A_Bubula;

public partial class RegistryPage : ContentPage
{
	public RegistryPage()
	{
		InitializeComponent();
        BindingContext = new RegistryPageViewModel();
    }

    private async void Back(object sender, EventArgs e)
    {
        var loginPage = new LoginPage();
        await Navigation.PushAsync(loginPage);
    }
}