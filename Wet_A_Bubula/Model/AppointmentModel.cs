using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wet_A_Bubula.Model
{
    public class AppointmentModel
    {
        public Guid IdAppointment { get; set; }
        public string? Data { get; set; }

        public DateOnly? DataNowejWizyty { get; set; }

        public TimeOnly? CzasNowejWizyty { get; set; }
        public string? Powod { get; set; }

        public string? Leczenie { get; set; }
    }
}
