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
    public partial class HomePageViewModel : BaseViewModel
    {
        private readonly UserRepository _userRepository;
        private readonly UserModel _user;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private List<AppointmentModel> futureAppointments;

        public IAsyncRelayCommand GoToHomePage2Command { get; }
        public IAsyncRelayCommand GoToVisitsPageCommand { get; }
        public IAsyncRelayCommand GoToProfilePageCommand { get; }
        public HomePageViewModel(UserModel user)
        {
            _user = user;
            _userRepository = new UserRepository();
            Name = user.Imie ?? string.Empty;
           FutureAppointments = new List<AppointmentModel>();
            GoToHomePage2Command = new AsyncRelayCommand(GoToHomePage2);
            GoToVisitsPageCommand = new AsyncRelayCommand(GoToVisitsPage);
            GoToProfilePageCommand = new AsyncRelayCommand(GoToProfilePage);

            Task.Run(async () => await LoadInitialData());
        }
        private async Task LoadInitialData()
        {

            try
            {
                var appointments = await _userRepository.GetFutureAppointmentsAsync(_user.IdPet);

                Debug.WriteLine($"Loaded {appointments.Count} appointments from repository");

                FutureAppointments.Clear();
                foreach (var appointment in appointments)
                {
                    Debug.WriteLine($"Adding appointment: {appointment.Data}, {appointment.Leczenie}");
                    FutureAppointments.Add(appointment);
                }

                Debug.WriteLine($"FutureAppointments Count after loading: {FutureAppointments.Count}");

               
                OnPropertyChanged(nameof(FutureAppointments));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        private async Task GoToHomePage2()
        {
            await Shell.Current.Navigation.PushAsync(new HomePage_2(_user));
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
