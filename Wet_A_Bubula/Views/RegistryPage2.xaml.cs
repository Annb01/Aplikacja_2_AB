using Wet_A_Bubula.ViewModels;
using Wet_A_Bubula.Repositories;
using System.Reflection;
namespace Wet_A_Bubula;

public partial class RegistryPage2 : ContentPage
{

    public RegistryPage2(Guid id, UserRepository userRepository)
    {
            InitializeComponent();
            this.BindingContext = new RegistryPage2ViewModel(id, userRepository);
    }
    
    private async void Back(object sender, EventArgs e)
    {
        var loginPage = new LoginPage();
        await Navigation.PushAsync(loginPage);
    }
}