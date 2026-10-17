namespace backend_CLARA.Models
{
    public class UsuarioRead
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string FechaNacimiento { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public string CedulaProfesional { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public string Estatus { get; internal set; } = string.Empty;
    }
}
