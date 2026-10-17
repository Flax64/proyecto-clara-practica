namespace backend_CLARA.Models
{
    public class CitaRead
    {
        public int IdCita { get; set; }
        public string Paciente { get; set; } = string.Empty;
        public string Medico { get; set; } = string.Empty;
        public string Fecha { get; set; } = string.Empty; // Formato dd/MM/yyyy
        public string Hora { get; set; } = string.Empty;  // Formato hh:mm tt (AM/PM)
        public string Estado { get; set; } = string.Empty;
    }
}
