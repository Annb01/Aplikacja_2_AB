using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Wet_A_Bubula.Model;
using Wet_A_Bubula.Repositories;

namespace Wet_A_Bubula.ViewModels
{
    public partial class VisitsPageViewModel: BaseViewModel
    {
        private readonly UserRepository _userRepository;
        private readonly UserModel _user;

        public VisitsPageViewModel(UserModel user)
        {
            _user = user;
            _userRepository = new UserRepository();
            TreatmentList = new ObservableCollection<string>();
            SelectedTreatment = string.Empty;
            VeterinarianList = new ObservableCollection<string>();
            SelectedVeterinarian = string.Empty;
            Task.Run(async () =>
            {
                await LoadTreatmentAsync();
                await LoadVeterinarianAsync();
            });
            GoToHomePageCommand = new AsyncRelayCommand(GoToHomePage);
            GoToProfilePageCommand = new AsyncRelayCommand(GoToProfilePage);
            VisitsPageCommand = new AsyncRelayCommand(VisitsPage);

        }

        [ObservableProperty]
        private ObservableCollection<string> treatmentList;

        [ObservableProperty]
        private string? selectedTreatment;

        [ObservableProperty]
        private ObservableCollection<string> veterinarianList;

        [ObservableProperty]
        private string? selectedVeterinarian;

        [ObservableProperty]
        private DateOnly dataNowejWizyty;

        [ObservableProperty]
        private TimeOnly czasNowejWizyty;

        [ObservableProperty]
        private string? powod;

        [ObservableProperty]
        private Guid idwe;


        public IAsyncRelayCommand GoToHomePageCommand { get; }
        public IAsyncRelayCommand GoToProfilePageCommand { get; }
        public IAsyncRelayCommand VisitsPageCommand { get; }

        private async Task LoadTreatmentAsync()
        {
            var treatmentFromDb = await _userRepository.GetTreatment();
            TreatmentList = new ObservableCollection<string>(treatmentFromDb);
        }

        private async Task LoadVeterinarianAsync()
        {
            var veterinarianFromDb = await _userRepository.GetVeterinarian();
            VeterinarianList = new ObservableCollection<string>(veterinarianFromDb);
        }

        private async Task GoToHomePage()
        {
            await Shell.Current.Navigation.PushAsync(new HomePage(_user));
        }
        
        private async Task GoToProfilePage()
        {
            await Shell.Current.Navigation.PushAsync(new ProfilePage(_user));
        }

        public async Task VisitsPage()
        {
            var userRepository = new UserRepository();

            if (string.IsNullOrEmpty(SelectedVeterinarian) ||
               string.IsNullOrEmpty(SelectedTreatment) ||
               string.IsNullOrEmpty(Powod))
            {
                await Shell.Current.DisplayAlert("SignUp Error", "Please fill in all required fields.", "OK");
                return;
            }
            var idFromDb = await _userRepository.Getidvet(SelectedVeterinarian);

            var userIdPet = _user.IdPet;
            var vetId = idFromDb;

            Debug.WriteLine($"xaml.cs -> {DataNowejWizyty}{CzasNowejWizyty}");

            bool addVisit = await _userRepository.AddVisit(DataNowejWizyty, CzasNowejWizyty, Powod, vetId, userIdPet, SelectedTreatment);

            if (addVisit == true)
            {
                await Shell.Current.DisplayAlert("Appointment", "The appointment has been added.", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Appointment Error", "The appointment has not been added.", "OK");
            }
        }

    }
}
