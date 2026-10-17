namespace backend_CLARA.Models
{
    public class EstadoRecuperacion
    {
        public string Correo { get; set; } = string.Empty; 
        public bool ClickConfirmado { get; set; }
        public DateTime Expiracion { get; set; }
    }
}
