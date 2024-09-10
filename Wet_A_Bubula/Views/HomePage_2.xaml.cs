
using Wet_A_Bubula.Model;
using Wet_A_Bubula.ViewModels;
namespace Wet_A_Bubula;

public partial class HomePage_2 : ContentPage
{
    private HomePage2ViewModel _viewModel;
    public HomePage_2(UserModel user)
    {
        InitializeComponent();
        _viewModel = new HomePage2ViewModel(user);
        BindingContext = _viewModel;
    }
    
}