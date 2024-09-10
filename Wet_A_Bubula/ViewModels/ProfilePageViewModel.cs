using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net;
using System.Threading.Tasks;
using Wet_A_Bubula.Model;
using Wet_A_Bubula.Repositories;

namespace Wet_A_Bubula.ViewModels
{
    public partial class ProfilePageViewModel : BaseViewModel
    {

        private readonly UserRepository _userRepository;

        [ObservableProperty]
        private UserModel user;

        [ObservableProperty]
        private PetModel pet;

        public ProfilePageViewModel(UserModel user)
        {
            _userRepository = new UserRepository();
            User = user;
            Pet = new PetModel();
            GoToHomePageCommand = new AsyncRelayCommand(GoToHomePage);
            GoToVisitsPageCommand = new AsyncRelayCommand(GoToVisitsPage);
            Task.Run(async () => await LoadProfile());
        }

        public IAsyncRelayCommand GoToHomePageCommand { get; }
        public IAsyncRelayCommand GoToVisitsPageCommand { get; }
        private async Task GoToHomePage()
        {
          
            await Shell.Current.Navigation.PushAsync(new HomePage(User));
        }
        private async Task GoToVisitsPage()
        {
            await Shell.Current.Navigation.PushAsync(new VisitsPage(User));
        }

        private async Task LoadProfile()
        {
            Pet = await _userRepository.GetPetAsync(User.IdPet);

        }
    }
}
