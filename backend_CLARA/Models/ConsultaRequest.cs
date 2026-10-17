using backend_CLARA.Controllers;

namespace backend_CLARA.Models
{
    public class ConsultaRequest
    {
        public int IdCita { get; set; }
        public string Sintomas { get; set; } = string.Empty; 
        public string Diagnostico { get; set; } = string.Empty; 
        public string Observaciones { get; set; } = string.Empty; 
        public double Peso { get; set; }
        public double Altura { get; set; }
        public List<RecetaItem> Receta { get; set; } = new();
    }
}
