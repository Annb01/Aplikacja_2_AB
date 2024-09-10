using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wet_A_Bubula.Repositories;
using Wet_A_Bubula.Model;
using System.Text.RegularExpressions;

namespace Wet_A_Bubula.ViewModels
{
    public partial class RegistryPage2ViewModel : BaseViewModel
    {
        private readonly Guid Id;
        private readonly UserRepository _userRepository;

        public RegistryPage2ViewModel(Guid id, UserRepository userRepository)
        {
            this.Id = id;
            _userRepository = userRepository;
            AnimalSpeciesList = new ObservableCollection<string>();
            SelectedSpecies = string.Empty;
            Task.Run(async () =>
            {
                await LoadAnimalSpecies();

            });

            Registry2Command = new AsyncRelayCommand(Registry2);
        }

        public IAsyncRelayCommand Registry2Command { get; }

        [ObservableProperty]
        private ObservableCollection<string> animalSpeciesList;

        [ObservableProperty]
        private string selectedSpecies;


        [ObservableProperty]
        private string animal = string.Empty;

        [ObservableProperty]
        private string plec = string.Empty;

        [ObservableProperty]
        private string gatunek = string.Empty;

        private async Task LoadAnimalSpecies()
        {
            var speciesFromDb = await _userRepository.GetAnimalSpecies();
            AnimalSpeciesList = new ObservableCollection<string>(speciesFromDb);
        }

        public async Task Registry2()
        {
            if (Id == Guid.Empty)
            {
                await Shell.Current.DisplayAlert("Error", "Invalid User ID", "OK");
                return;
            }



            if (string.IsNullOrEmpty(Animal) ||
                string.IsNullOrEmpty(Plec) || 
                string.IsNullOrEmpty(SelectedSpecies))
            {
                await Shell.Current.DisplayAlert("SignUp Error", "Please fill in all required fields.", "OK");
                return;
            }

           var userRepository = new UserRepository();

            Regex plecRegex = new Regex(@"^(Male|Female)$");
            if (!plecRegex.IsMatch(Plec))
            {
                await Shell.Current.DisplayAlert("SignUp Error", "Gender must be either 'Male' or 'Female'.", "OK");
                return;
            }

            var newPet = new PetModel
            {
                Animal = Animal, 
                Plec = Plec,
                Gatunek = SelectedSpecies,
                IdWlasciciel = Id
            };

            bool newPetDone = userRepository.AddZ(newPet);
            if (newPetDone)
            {
                await Shell.Current.DisplayAlert("SignUp", "User and Pet added. Please log in", "OK");
                await Shell.Current.Navigation.PopToRootAsync();
            }
            else
            {
                await Shell.Current.DisplayAlert("SignUp Error", "Pet not added", "OK");
            }
        }
    }
}
