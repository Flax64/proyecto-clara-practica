namespace backend_CLARA.Models
{
    public class MedicamentoRequest
    {
        public int IdEstatus { get; set; }
        public string Nombre { get; set; } = string.Empty; 
        public string Descripcion { get; set; } = string.Empty; 
        public decimal Precio { get; set; }
        public decimal ConcentracionValor { get; set; } = string.Empty; 
        public string ConcentracionUnidad { get; set; } = string.Empty; 
    }
}
