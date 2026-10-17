namespace backend_CLARA.Models
{
    public class RecetaItem
    {
        public int IdMedicamento { get; set; }
        public string Dosis { get; set; } = string.Empty;
        public string Frecuencia { get; set; } = string.Empty;
        public string Duracion { get; set; } = string.Empty;
    }
}
