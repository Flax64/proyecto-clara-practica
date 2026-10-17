namespace backend_CLARA.Models
{
    public class HorarioRequest
    {
        public int IdMedico { get; set; }
        public int IdDia { get; set; }
        public string HoraEntrada { get; set; } = string.Empty; 
        public string HoraSalida { get; set; } = string.Empty; 
    }
}
