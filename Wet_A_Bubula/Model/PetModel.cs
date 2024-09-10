using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wet_A_Bubula.Model
{
    public class PetModel
    {
        public string? Animal { get; set; }
        public string? Plec { get; set; }
        public string? Gatunek { get; set; }
        public Guid IdWlasciciel { get; set; }
    }
}
