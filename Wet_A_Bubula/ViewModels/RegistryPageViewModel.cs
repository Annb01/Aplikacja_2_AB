using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Wet_A_Bubula.Model;
using Wet_A_Bubula.Repositories;

namespace Wet_A_Bubula.ViewModels
{
    public partial class RegistryPageViewModel : BaseViewModel
    {

        public IAsyncRelayCommand Registry1Command { get; }

        public RegistryPageViewModel()
        {
            Registry1Command = new AsyncRelayCommand(Registry1);
        }

        [ObservableProperty]
        private string imie = string.Empty;

        [ObservableProperty]
        private string nazwisko = string.Empty;

        [ObservableProperty]
        private string ulica = string.Empty;

        [ObservableProperty]
        private string miasto = string.Empty;

        [ObservableProperty]
        private string kodPocztowy = string.Empty;

        [ObservableProperty]
        private string kraj = string.Empty;

        [ObservableProperty]
        private string telefon = string.Empty;

        [ObservableProperty]
        private string login = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;


        public async Task Registry1()
        {
            var userRepository = new UserRepository();

            if (string.IsNullOrEmpty(Imie) ||
                string.IsNullOrEmpty(Nazwisko) ||
                string.IsNullOrEmpty(Ulica) ||
                string.IsNullOrEmpty(Miasto) ||
                string.IsNullOrEmpty(KodPocztowy) ||
                string.IsNullOrEmpty(Kraj) ||
                string.IsNullOrEmpty(Telefon) ||
                string.IsNullOrEmpty(Login) ||
                string.IsNullOrEmpty(Password))
            {
                await Shell.Current.DisplayAlert("SignUp Error", "Please fill in all required fields.", "OK");
                return;
            }

            Regex kodPocztowyRegex = new Regex(@"^\d{2}-\d{3}$");
            if (!kodPocztowyRegex.IsMatch(KodPocztowy))
            {
                await Shell.Current.DisplayAlert("SignUp Error", "Postal code should be in format XX-XXX (43-300).", "OK");
                return;
            }

            Regex telefonRegex = new Regex(@"^\+48\d{9}$");
            if (!telefonRegex.IsMatch(Telefon))
            {
                await Shell.Current.DisplayAlert("SignUp Error", "Phone number should be in format +48XXXXXXXXX (+48608700523).", "OK");
                return;
            }

            if (userRepository.UserExists(Login))
            {
                await Shell.Current.DisplayAlert("SignUp Error", "Username already taken. Please choose another.", "OK");
                return;
            }

            var newUser = new UserModel
            { 
                Imie = Imie,
                Nazwisko = Nazwisko,
                Ulica = Ulica,
                Miasto = Miasto,
                KodPocztowy = KodPocztowy,
                Kraj = Kraj,
                Telefon = Telefon,
                Username = Login,
                Password = Password,
            };

            bool newUserDone = userRepository.Add(newUser);
            if (newUserDone && newUser.Id != Guid.Empty)
            {
                await Shell.Current.Navigation.PushAsync(new RegistryPage2(newUser.Id, userRepository));
            }
            else
            {
                await Shell.Current.DisplayAlert("SignUp Error", "User not added", "OK");
            }
        }
    }
}
