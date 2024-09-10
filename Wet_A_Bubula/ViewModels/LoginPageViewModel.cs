using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net;
using System.Threading.Tasks;
using Wet_A_Bubula.Model;
using Wet_A_Bubula.Repositories;

namespace Wet_A_Bubula.ViewModels
{
    public partial class LoginPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string name = string.Empty;

        public IAsyncRelayCommand LoginCommand { get; set; }

        public LoginPageViewModel() 
        {
            LoginCommand = new AsyncRelayCommand(Login);
        }


        public async Task Login()
        {
            var userRepository = new UserRepository();
            var credentials = new NetworkCredential(Username, Password);

            if (userRepository.AuthenticateUser(credentials, out UserModel? user, out Guid guid))
            {

                if (user != null)
                {
                    
                    await Shell.Current.Navigation.PushAsync(new HomePage(user));
                }
                else
                {
                   
                    await Shell.Current.DisplayAlert("Login Error", "An unexpected error occurred. Please try again.", "OK");
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Login Error", "Incorrect Login or Password", "OK");
            }
        }
    }
}
