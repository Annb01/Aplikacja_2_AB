using System.Net;


namespace Wet_A_Bubula.Model
{
    public interface IUserRepository
    {
        bool AuthenticateUser(NetworkCredential credential, out UserModel? user, out Guid guid);
        Task<List<string>> GetAnimalSpecies();
        Task<bool> AddVisit(DateOnly dataWizyty, TimeOnly godzinaWizyty, string powod, Guid vetId, Guid userIdPet, string leczenie);
        bool Add(UserModel? userModel);
        bool AddZ(PetModel? petModel);
        Task<List<string>> GetTreatment();
        Task<List<string>> GetVeterinarian();
        Task<List<AppointmentModel>> GetFutureAppointmentsAsync(Guid idPet);
        Task<List<AppointmentModel>> GetPastAppointmentsAsync(Guid idPet);
        Task<Guid> Getidvet(string selectedname);
        Task<PetModel> GetPetAsync(Guid idPet);
        bool UserExists(string username);
        bool PetExists(string petname);

    }
}
