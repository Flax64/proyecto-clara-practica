namespace backend_CLARA.Models
{
    public class VentaDTO
    {
        public int Id { get; set; }
        public string Fecha { get; set; } = string.Empty; // Usaremos esta para el 17/03/2026
        public string Hora { get; set; } = string.Empty;  // Usaremos esta para las 02:57 PM
        public string Cliente { get; set; } = string.Empty;
        public string Vendedor { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Metodo { get; set; } = string.Empty;
        public string Estatus { get; set; } = string.Empty;
    }
}
