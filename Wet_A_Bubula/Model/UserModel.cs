
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wet_A_Bubula.Model
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string? Imie { get; set; } 
        public string? Nazwisko { get; set; }
        public string? Ulica { get; set; } 
        public string? Miasto { get; set; }
        public string? KodPocztowy { get; set; }
        public string? Kraj { get; set; } 
        public string? Telefon { get; set; } 
        public string? Username { get; set; }
        public string? Password { get; set; }
        public Guid IdPet { get; set; }

    }
}
