using Wet_A_Bubula.Model;
using Wet_A_Bubula.ViewModels;
namespace Wet_A_Bubula;


public partial class ProfilePage : ContentPage
{
    private ProfilePageViewModel _viewModel;
    public ProfilePage(UserModel user)
    {
        InitializeComponent();
        _viewModel = new ProfilePageViewModel(user);
        BindingContext = _viewModel;
    }
    private async void LogOut(object sender, EventArgs e)
    {
        LoginPage loginPage = new LoginPage();
        await Navigation.PushAsync(loginPage);
    }
  
} 