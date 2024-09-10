using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wet_A_Bubula.Model;
using Microsoft.Data.SqlClient;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using System.Diagnostics;
using Microsoft.Maui.Graphics;

namespace Wet_A_Bubula.Repositories
{
    public class UserRepository: RepositoryBase, IUserRepository
    {
        public bool Add(UserModel? userModel)
        {
            if (userModel == null)
            {
                Console.WriteLine("UserModel is null");
                return false;
            }
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("INSERT INTO [Wlasciciel] (Imie, Nazwisko, Ulica, Miasto, KodPocztowy, Kraj, Telefon, n_login, n_password) " +
            "OUTPUT INSERTED.IDWlasciciel " +
            "VALUES (@Imie, @Nazwisko, @Ulica, @Miasto, @KodPocztowy, @Kraj, @Telefon, @Login, @Password);", connection))
                {
                    command.Parameters.AddWithValue("@Imie", userModel.Imie);
                    command.Parameters.AddWithValue("@Nazwisko", userModel.Nazwisko);
                    command.Parameters.AddWithValue("@Ulica", userModel.Ulica);
                    command.Parameters.AddWithValue("@Miasto", userModel.Miasto);
                    command.Parameters.AddWithValue("@KodPocztowy", userModel.KodPocztowy);
                    command.Parameters.AddWithValue("@Kraj", userModel.Kraj);
                    command.Parameters.AddWithValue("@Telefon", userModel.Telefon);
                    command.Parameters.AddWithValue("@Login", userModel.Username);
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userModel.Password);
                    command.Parameters.AddWithValue("@Password", hashedPassword);

                    var idResult = command.ExecuteScalar();
                    if (idResult != null && Guid.TryParse(idResult.ToString(), out var id))
                    {
                        userModel.Id = id;
                        return true;
                    }
                    return false;
                }
            }
        }

        public async Task<bool> AddVisit(DateOnly dataWizyty, TimeOnly godzinaWizyty, string powod, Guid vetId, Guid userIdPet, string leczenie)
        {
            
            var openingTime = new TimeOnly(9, 0); 
            var closingTime = new TimeOnly(18, 0); 

            if (godzinaWizyty < openingTime || godzinaWizyty > closingTime)
            {
                await Shell.Current.DisplayAlert("Appointment Error", "The selected time is outside of clinic working hours (09:00 AM - 18:00 PM).", "OK");
                return false;
            }
            int upcomingVisitsCount;
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM [Wizyty] WHERE IDZwierze = @IDZwierze AND DataWizyty > @Today", connection))
                {
                    command.Parameters.AddWithValue("@IDZwierze", userIdPet);
                    command.Parameters.AddWithValue("@Today", DateTime.Today); 
                    upcomingVisitsCount = (int)command.ExecuteScalar();
                }
            }

            if (upcomingVisitsCount >= 4)
            {
                await Shell.Current.DisplayAlert("Appointment Error", "You cannot schedule more than 4 upcoming visits.", "OK");
                return false;
            }

           
            bool isAvailable = IsTimeSlotAvailable(dataWizyty, godzinaWizyty, vetId);
            if (!isAvailable)
            {
                await Shell.Current.DisplayAlert("Appointment Error", "Vet is not available at that time.", "OK");
                return false;
            }
            
            
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("INSERT INTO [Wizyty] (DataWizyty, GodzinaWizyty, Powod, IDWeterynarz, IDZwierze, Leczenie) VALUES(@DataWizyty, @GodzinaWizyty, @Powod, @IDWeterynarz, @IDZwierze, @Leczenie)", connection))
                {
                    command.Parameters.AddWithValue("@DataWizyty", dataWizyty);
                    command.Parameters.AddWithValue("@GodzinaWizyty", godzinaWizyty);
                    command.Parameters.AddWithValue("@Powod", powod);
                    command.Parameters.AddWithValue("@IDWeterynarz", vetId);
                    command.Parameters.AddWithValue("@IDZwierze", userIdPet);
                    command.Parameters.AddWithValue("@Leczenie", leczenie);
                    int affectedRows = command.ExecuteNonQuery(); 
                    return affectedRows > 0;
                }
            }
        }
        public bool IsTimeSlotAvailable(DateOnly? date, TimeOnly? time, Guid vetId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"
            SELECT COUNT(*)
            FROM [Wizyty]
            WHERE DataWizyty = @DataWizyty
              AND GodzinaWizyty = @GodzinaWizyty
              AND IDWeterynarz = @IDWeterynarz";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DataWizyty", date);
                    command.Parameters.AddWithValue("@GodzinaWizyty", time);
                    command.Parameters.AddWithValue("@IDWeterynarz", vetId);

                    var count = (int)command.ExecuteScalar();
                    return count == 0; 
                }
            }
        }
        public bool AddZ(PetModel? petModel)
        {
            if (petModel == null)
            {
                Console.WriteLine("UserModel is null");
                return false;
            }
            using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand("INSERT INTO [Zwierzeta] (Imie, Plec, Gatunek, IDWlasciciel) VALUES (@Imie, @Plec, @Gatunek, @IDWlasciciel)", connection))
                    {
                        command.Parameters.AddWithValue("@Imie", petModel.Animal);
                        command.Parameters.AddWithValue("@Plec", petModel.Plec);
                        command.Parameters.AddWithValue("@Gatunek", petModel.Gatunek);
                        command.Parameters.AddWithValue("@IDWlasciciel", petModel.IdWlasciciel);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            
        }

        public bool AuthenticateUser(NetworkCredential credential, out UserModel? user, out Guid guid)
        {
            user = null;
            guid = Guid.Empty; 

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT NEWID() AS Guid, W.IDWlasciciel AS IDWlasciciel, W.Imie AS WlascicielImie, W.Nazwisko, W.Ulica, W.Miasto, W.KodPocztowy, W.Kraj, W.Telefon, W.n_password, Z.IDZwierze FROM[Wlasciciel] W JOIN[Zwierzeta] Z ON W.IDWlasciciel = Z.IDWlasciciel WHERE W.n_login = @Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", credential.UserName);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read()) 
                        {
                            bool isValidPassword = BCrypt.Net.BCrypt.Verify(credential.Password, reader["n_password"].ToString());
                            if (isValidPassword)
                            {
                                guid = (Guid)reader["Guid"];
                                user = new UserModel
                                {
                                    Id = reader["IDWlasciciel"] != DBNull.Value ? (Guid)reader["IDWlasciciel"] : Guid.Empty,
                                    Imie = reader["WlascicielImie"]?.ToString() ?? string.Empty,
                                    Nazwisko = reader["Nazwisko"]?.ToString() ?? string.Empty,
                                    Ulica = reader["Ulica"]?.ToString() ?? string.Empty,
                                    Miasto = reader["Miasto"]?.ToString() ?? string.Empty,
                                    KodPocztowy = reader["KodPocztowy"]?.ToString() ?? string.Empty,
                                    Kraj = reader["Kraj"]?.ToString() ?? string.Empty,
                                    Telefon = reader["Telefon"]?.ToString() ?? string.Empty,
                                    IdPet = reader["IDZwierze"] != DBNull.Value ? (Guid)reader["IDZwierze"] : Guid.Empty
                                };

                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }



        public bool UserExists(string username)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(1) FROM [Wlasciciel] WHERE n_login = @Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    int userCount = (int)command.ExecuteScalar();
                    return userCount > 0;
                }
            }
        }

        public bool PetExists(string petname)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(1) FROM [Zwierzeta] WHERE Imie = @petname", connection))
                {
                    command.Parameters.AddWithValue("@petname", petname);
                    int userCount = (int)command.ExecuteScalar();
                    return userCount > 0;
                }
            }
        }


        public async Task<List<string>> GetAnimalSpecies()
        {
            var speciesList = new List<string>();

        try
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT Gatunek FROM [GatunkiZwierzat]", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            speciesList.Add(reader.GetString(0));
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching animal species: {ex.Message}");
        }

        return speciesList;
        }

        public async Task<List<string>> GetTreatment()
        {
            var treatmentList = new List<string>();

            try
            {
                using (var connection = GetConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("SELECT Leczenie FROM [Leczenie]", connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync()) 
                        {
                            while (await reader.ReadAsync()) 
                            {
                                treatmentList.Add(reader.GetString(0));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching treatment: {ex.Message}");
            }

            return treatmentList;
        }

        public async Task<List<string>> GetVeterinarian()
        {
            var veterinarianList = new List<string>();

            try
            {
                using (var connection = GetConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("SELECT Imie + ' ' + Nazwisko AS PelneImie  FROM [Weterynarze]", connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                veterinarianList.Add(reader.GetString(0));
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching animal species: {ex.Message}");
            }

            return veterinarianList;
        }

        public async Task<List<AppointmentModel>> GetFutureAppointmentsAsync(Guid idPet)
        {
            var appointments = new List<AppointmentModel>();

            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(
                    @"SELECT w.DataWizyty, w.GodzinaWizyty, l.Leczenie AS Leczenie
                    FROM Wizyty w JOIN [Leczenie] l ON w.Leczenie=l.Leczenie 
                    WHERE IDZwierze = @idPet AND  W.DataWizyty > CAST(GETDATE() AS DATE) OR (W.DataWizyty = CAST(GETDATE() AS DATE) AND W.GodzinaWizyty > CAST(GETDATE() AS TIME))
                     ORDER BY W.DataWizyty, W.GodzinaWizyty",
                    connection))
                {
                    command.Parameters.AddWithValue("@idPet", idPet);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            try
                            {
                                var dataWizyty = reader.GetDateTime(reader.GetOrdinal("DataWizyty"));
                                var godzinaWizyty = reader.GetTimeSpan(reader.GetOrdinal("GodzinaWizyty"));
                                var leczenieString = reader.GetString(reader.GetOrdinal("Leczenie"));

                                var appointmentDateTime = dataWizyty.Date.Add(godzinaWizyty); 

                              
                                var formattedDate = appointmentDateTime.ToString("yyyy-MM-dd HH:mm");

                                var appointment = new AppointmentModel
                                {
                                    Data = formattedDate, 
                                    Leczenie = leczenieString,
                                };

                                appointments.Add(appointment);
                            }
                            catch (InvalidCastException ex)
                            {
                                Debug.WriteLine($"InvalidCastException: {ex.Message}");
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Exception: {ex.Message}");
                            }

                        }
                    }
                }
            }

            return appointments;
        }

        public async Task<List<AppointmentModel>> GetPastAppointmentsAsync(Guid idPet)
        {
            var appointments = new List<AppointmentModel>();

            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(
                    @"SELECT w.DataWizyty, w.GodzinaWizyty, l.Leczenie AS Leczenie
                    FROM Wizyty w JOIN [Leczenie] l ON w.Leczenie=l.Leczenie 
                    WHERE IDZwierze = @idPet AND  W.DataWizyty < CAST(GETDATE() AS DATE) OR (W.DataWizyty = CAST(GETDATE() AS DATE) AND W.GodzinaWizyty < CAST(GETDATE() AS TIME))
                     ORDER BY W.DataWizyty, W.GodzinaWizyty",
                    connection))
                {
                    command.Parameters.AddWithValue("@idPet", idPet);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            try
                            {
                                var dataWizyty = reader.GetDateTime(reader.GetOrdinal("DataWizyty"));
                                var godzinaWizyty = reader.GetTimeSpan(reader.GetOrdinal("GodzinaWizyty"));
                                var leczenieString = reader.GetString(reader.GetOrdinal("Leczenie"));

                                var appointmentDateTime = dataWizyty.Date.Add(godzinaWizyty);


                                var formattedDate = appointmentDateTime.ToString("yyyy-MM-dd HH:mm");

                                var appointment = new AppointmentModel
                                {
                                    Data = formattedDate,
                                    Leczenie = leczenieString,
                                };

                                appointments.Add(appointment);
                            }
                            catch (InvalidCastException ex)
                            {
                                Debug.WriteLine($"InvalidCastException: {ex.Message}");
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Exception: {ex.Message}");
                            }

                        }
                    }
                }
            }

            return appointments;
        }

        public async Task<Guid> Getidvet(string selectedName)
        {
            if (!string.IsNullOrEmpty(selectedName))
            {
                using (var connection = GetConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("SELECT IDWeterynarz FROM [Weterynarze] WHERE Imie + ' ' + Nazwisko = @PelneImie", connection))
                    {
                        command.Parameters.AddWithValue("@PelneImie", selectedName);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return reader["IDWeterynarz"] != DBNull.Value ? (Guid)reader["IDWeterynarz"] : Guid.Empty;
                            }
                        }
                    }
                }
            }

            return Guid.Empty;

        }

        public async Task<PetModel> GetPetAsync(Guid idPet)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(@"SELECT Imie, Plec, Gatunek FROM Zwierzeta WHERE IDZwierze=@idPet", connection))
                {
                    command.Parameters.AddWithValue("@idPet", idPet);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            try
                            {
                                var pet = new PetModel
                                {
                                    Animal = reader["Imie"]?.ToString() ?? string.Empty,
                                    Plec = reader["Plec"]?.ToString() ?? string.Empty,
                                    Gatunek = reader["Gatunek"]?.ToString() ?? string.Empty
                                };

                                return pet;
                            }
                            catch (InvalidCastException ex)
                            {
                                Debug.WriteLine($"InvalidCastException: {ex.Message}");
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Exception: {ex.Message}");
                            }
                        }
                    }
                }
            }

          
            return new PetModel
            {
                Animal = string.Empty,
                Plec = string.Empty,
                Gatunek = string.Empty
            };
        }
    
    }
}
