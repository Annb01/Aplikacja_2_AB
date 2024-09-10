using System.Diagnostics;
using Wet_A_Bubula.Model;
using Wet_A_Bubula.ViewModels;

namespace Wet_A_Bubula;

public partial class HomePage : ContentPage
{


    private HomePageViewModel _viewModel;

    public HomePage(UserModel user)
    {
        InitializeComponent();
        _viewModel = new HomePageViewModel(user);
        BindingContext = _viewModel;

       
    }
    
}