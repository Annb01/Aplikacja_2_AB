using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using Wet_A_Bubula.Model;
using Wet_A_Bubula.Repositories;
using System.Diagnostics;

namespace Wet_A_Bubula.ViewModels
{
    public partial class HomePage2ViewModel : BaseViewModel
    {
        private readonly UserRepository _userRepository;
        private readonly UserModel _user;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private List<AppointmentModel> pastAppointments;

        public IAsyncRelayCommand GoToHomePageCommand { get; }
        public IAsyncRelayCommand GoToVisitsPageCommand { get; }
        public IAsyncRelayCommand GoToProfilePageCommand { get; }
        public HomePage2ViewModel(UserModel user)
        {
            _user = user;
            _userRepository = new UserRepository();
            Name = user.Imie ?? string.Empty;
            PastAppointments = new List<AppointmentModel>();
            GoToVisitsPageCommand = new AsyncRelayCommand(GoToVisitsPage);


            Task.Run(async () => await LoadInitialData2());


            GoToHomePageCommand = new AsyncRelayCommand(GoToHomePage);
            GoToProfilePageCommand = new AsyncRelayCommand(GoToProfilePage);
        }
        private async Task LoadInitialData2()
        {
            try
            {
                var appointments = await _userRepository.GetPastAppointmentsAsync(_user.IdPet);

                Debug.WriteLine($"Loaded {appointments.Count} appointments from repository");

                PastAppointments.Clear();
                foreach (var appointment in appointments)
                {
                    Debug.WriteLine($"Adding appointment: {appointment.Data}, {appointment.Leczenie}");
                    PastAppointments.Add(appointment);
                }

                Debug.WriteLine($"FutureAppointments Count after loading: {PastAppointments.Count}");

               
                OnPropertyChanged(nameof(PastAppointments));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading data: {ex.Message}");
            }
        }
        private async Task GoToHomePage()
        {
            await Shell.Current.Navigation.PushAsync(new HomePage(_user));
        }

        private async Task GoToVisitsPage()
        {
            await Shell.Current.Navigation.PushAsync(new VisitsPage(_user));
        }

        private async Task GoToProfilePage()
        {
            await Shell.Current.Navigation.PushAsync(new ProfilePage(_user));
        }
    }
}

