using backend_CLARA.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.RateLimiting;
// No necesitas agregar el "using BCrypt.Net" arriba, usaremos la ruta completa para evitar conflictos

namespace backend_CLARA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly String _connectionString = ConexionDB.Cadena;

        // Client ID de Google Cloud (el mismo que usa la app de escritorio)
        private const string GOOGLE_CLIENT_ID = "229034863022-43f0bcin64910ceh6041venb9juvndvl.apps.googleusercontent.com";

        // Nuestra memoria RAM: Diccionario que asocia un Token con el Estado de Recuperación
        private static Dictionary<string, Models.EstadoRecuperacion> _memoriaTemporal = new Dictionary<string, Models.EstadoRecuperacion>();

        // ====================================================================
        // LOGIN DE USUARIOS (CON MIGRACIÓN HÍBRIDA SILENCIOSA)
        // ====================================================================
        [EnableRateLimiting("limite-auth")]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    int? idUsuario = null;
                    string? dbPassword = null;
                    string? estatusUsuario = null;

                    string Query = @"SELECT u.id_Usuario, u.password_Usuario, e.nombre 
                                     FROM usuarios u 
                                     INNER JOIN estatus e ON u.id_Estatus = e.id_Estatus 
                                     WHERE (BINARY u.email_Usuario = @correo OR BINARY u.username = @correo)";

                    using (MySqlCommand cmd = new MySqlCommand(Query, conn))
                    {
                        cmd.Parameters.AddWithValue("@correo", request.Email);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                idUsuario = reader.GetInt32(0);
                                dbPassword = reader.IsDBNull(1) ? null : reader.GetString(1);
                                estatusUsuario = reader.GetString(2);
                            }
                        }
                    }

                    if (string.Equals(estatusUsuario, "Inactivo", StringComparison.OrdinalIgnoreCase))
                    {
                        return Unauthorized(new { error = "Tu cuenta se encuentra inactiva. Contacta al administrador." });
                    }

                    if (idUsuario != null && dbPassword != null)
                    {
                        bool isValid = false;
                        bool needsUpgrade = false;

                        if (dbPassword.StartsWith("$2") && dbPassword.Length >= 59)
                        {
                            isValid = BCrypt.Net.BCrypt.Verify(request.Password, dbPassword);
                        }
                        else
                        {
                            if (dbPassword == request.Password)
                            {
                                isValid = true;
                                needsUpgrade = true;
                            }
                        }

                        if (isValid)
                        {
                            if (needsUpgrade)
                            {
                                string newHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                                string updateQuery = "UPDATE usuarios SET password_Usuario = @hash WHERE id_Usuario = @id";
                                using (MySqlCommand cmdUpd = new MySqlCommand(updateQuery, conn))
                                {
                                    cmdUpd.Parameters.AddWithValue("@hash", newHash);
                                    cmdUpd.Parameters.AddWithValue("@id", idUsuario.Value);
                                    cmdUpd.ExecuteNonQuery();
                                }
                            }

                            return Ok(new { message = "AUTORIZADO", idUsuario = idUsuario.Value });
                        }
                    }

                    return Unauthorized(new { error = "Usuario o contraseña incorrectos." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error en el servidor al intentar iniciar sesión. Detalles: " + ex.Message });
            }
        }

        // ====================================================================
        // LOGIN CON GOOGLE
        // Valida el idToken. Si el correo existe -> devuelve idUsuario.
        // Si no existe -> devuelve registroIncompleto = true con los datos de Google.
        // ====================================================================
        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.IdToken))
                return BadRequest(new { error = "Falta el token de Google." });

            try
            {
                var payload = await ValidarTokenGoogle(request.IdToken);

                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    int? idUsuario = null;
                    string? emailBD = null;
                    string? estatusUsuario = null;

                    string query = @"SELECT u.id_Usuario, u.email_Usuario, e.nombre 
                                     FROM usuarios u 
                                     INNER JOIN estatus e ON u.id_Estatus = e.id_Estatus 
                                     WHERE u.email_Usuario = @correo LIMIT 1";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@correo", payload.Email);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                idUsuario = reader.GetInt32(0);
                                emailBD = reader.GetString(1);
                                estatusUsuario = reader.GetString(2);
                            }
                        }
                    }

                    // Usuario nuevo: la app debe pedir los datos que faltan
                    if (idUsuario == null)
                    {
                        return Ok(new
                        {
                            registroIncompleto = true,
                            email = payload.Email,
                            nombre = payload.GivenName ?? "",
                            apellidos = payload.FamilyName ?? ""
                        });
                    }

                    if (string.Equals(estatusUsuario, "Inactivo", StringComparison.OrdinalIgnoreCase))
                    {
                        return Unauthorized(new { error = "Tu cuenta se encuentra inactiva. Contacta al administrador." });
                    }

                    return Ok(new
                    {
                        registroIncompleto = false,
                        idUsuario = idUsuario.Value,
                        email = emailBD
                    });
                }
            }
            catch (InvalidJwtException)
            {
                return Unauthorized(new { error = "Token de Google inválido o expirado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error en el servidor al iniciar sesión con Google. Detalles: " + ex.Message });
            }
        }

        public IActionResult EvaluarCredencialesLogin(int? idUsuario, string dbPassword, string estatusUsuario, string passwordIngresada, out bool needsUpgrade)
        {
            needsUpgrade = false;

            if (string.Equals(estatusUsuario, "Inactivo", StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized(new { error = "Tu cuenta se encuentra inactiva. Contacta al administrador." });
            }

            if (idUsuario != null && dbPassword != null)
            {
                bool isValid = false;

                if (dbPassword.StartsWith("$2") && dbPassword.Length >= 59)
                {
                    isValid = BCrypt.Net.BCrypt.Verify(passwordIngresada, dbPassword);
                }
                else if (dbPassword == passwordIngresada)
                {
                    isValid = true;
                    needsUpgrade = true;
                }

                if (isValid)
                {
                    return Ok(new { message = "AUTORIZADO", idUsuario = idUsuario.Value });
                }
            }

            return Unauthorized(new { error = "Usuario o contraseña incorrectos." });
        }

        // ====================================================================
        // REGISTRO CON GOOGLE (completa los datos que Google no entrega)
        // El correo se toma del token, nunca del cuerpo de la petición.
        // ====================================================================
        [HttpPost("google/registrar")]
        public async Task<IActionResult> GoogleRegistrar([FromBody] GoogleRegistroRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.IdToken))
                return BadRequest(new { error = "Falta el token de Google." });

            if (string.IsNullOrWhiteSpace(req.Nombre) ||
                string.IsNullOrWhiteSpace(req.ApellidoPaterno) ||
                string.IsNullOrWhiteSpace(req.ApellidoMaterno) ||
                string.IsNullOrWhiteSpace(req.Telefono) ||
                req.IdGenero <= 0)
            {
                return BadRequest(new { error = "Faltan datos obligatorios para el registro." });
            }

            if (!DateTime.TryParseExact(req.FechaNacimiento, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out DateTime fechaNac))
            {
                return BadRequest(new { error = "La fecha de nacimiento no tiene un formato válido." });
            }

            int edad = DateTime.Today.Year - fechaNac.Year;
            if (fechaNac.Date > DateTime.Today.AddYears(-edad)) edad--;
            if (fechaNac.Date > DateTime.Today || edad < 18)
            {
                return BadRequest(new { error = "Debes ser mayor de 18 años para registrarte." });
            }

            try
            {
                var payload = await ValidarTokenGoogle(req.IdToken);

                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    // 1. Duplicidad
                    using (MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM usuarios WHERE email_Usuario = @correo", conn))
                    {
                        checkCmd.Parameters.AddWithValue("@correo", payload.Email);
                        if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                        {
                            return BadRequest(new { error = "Este correo ya está registrado en el sistema." });
                        }
                    }

                    // 2. Rol de Paciente
                    int idRol;
                    using (MySqlCommand rolCmd = new MySqlCommand("SELECT id_Rol FROM roles WHERE nombre = 'Paciente'", conn))
                    {
                        idRol = Convert.ToInt32(rolCmd.ExecuteScalar());
                    }

                    // 3. Contraseña aleatoria (nadie la conoce): así el login normal
                    //    no puede entrar a esta cuenta y no hay que cambiar la tabla.
                    string hashAleatorio = BCrypt.Net.BCrypt.HashPassword(
                        Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));

                    // 4. Usuario + paciente en una sola transacción
                    using (MySqlTransaction tx = conn.BeginTransaction())
                    {
                        try
                        {
                            string insertQuery = "INSERT INTO usuarios (id_Genero, id_Estatus, id_Rol, nombre_Usuario, apellido_P, apellido_M, email_Usuario, password_Usuario, telefono, fecha_Nacimiento) " +
                                                 "VALUES (@idGenero, @idEstatus, @idRol, @nombre, @apellidoP, @apellidoM, @correo, @password, @telefono, @fechaNacimiento)";

                            long idNuevoUsuario;
                            using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn, tx))
                            {
                                insertCmd.Parameters.AddWithValue("@idGenero", req.IdGenero);
                                insertCmd.Parameters.AddWithValue("@idEstatus", 1);
                                insertCmd.Parameters.AddWithValue("@idRol", idRol);
                                insertCmd.Parameters.AddWithValue("@nombre", req.Nombre.Trim());
                                insertCmd.Parameters.AddWithValue("@apellidoP", req.ApellidoPaterno.Trim());
                                insertCmd.Parameters.AddWithValue("@apellidoM", req.ApellidoMaterno.Trim());
                                insertCmd.Parameters.AddWithValue("@correo", payload.Email);
                                insertCmd.Parameters.AddWithValue("@password", hashAleatorio);
                                insertCmd.Parameters.AddWithValue("@telefono", req.Telefono.Trim());
                                insertCmd.Parameters.AddWithValue("@fechaNacimiento", fechaNac);
                                insertCmd.ExecuteNonQuery();
                                idNuevoUsuario = insertCmd.LastInsertedId;
                            }

                            using (MySqlCommand pacienteCmd = new MySqlCommand("INSERT INTO pacientes (id_Usuario) VALUES (@idUsuario)", conn, tx))
                            {
                                pacienteCmd.Parameters.AddWithValue("@idUsuario", idNuevoUsuario);
                                pacienteCmd.ExecuteNonQuery();
                            }

                            tx.Commit();

                            return Ok(new { idUsuario = idNuevoUsuario, email = payload.Email });
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (InvalidJwtException)
            {
                return Unauthorized(new { error = "Token de Google inválido o expirado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error en el servidor al registrar con Google. Detalles: " + ex.Message });
            }
        }

        // ====================================================================
        // VISUAL BASIC PIDE EL ENLACE (PANTALLA 1)
        // ====================================================================
        [HttpPost("solicitar-enlace")]
        public IActionResult SolicitarEnlace([FromBody] SolicitarEnlaceRequest request)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string Query = "SELECT id_Usuario FROM usuarios WHERE email_Usuario = @correo";
                    using (MySqlCommand cmd = new MySqlCommand(Query, conn))
                    {
                        cmd.Parameters.AddWithValue("@correo", request.Correo);
                        var userId = cmd.ExecuteScalar();

                        if (userId == null)
                            return NotFound(new { error = "El correo proporcionado no existe en el sistema." });

                        string tokenMagico = Guid.NewGuid().ToString();

                        _memoriaTemporal[tokenMagico] = new Models.EstadoRecuperacion
                        {
                            Correo = request.Correo,
                            ClickConfirmado = false,
                            Expiracion = DateTime.Now.AddMinutes(10)
                        };

                        EnviarCorreoMagico(request.Correo, tokenMagico);

                        return Ok(new { Token = tokenMagico, message = "Enlace enviado correctamente." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al procesar la solicitud de recuperación. Detalles: " + ex.Message });
            }
        }

        // ====================================================================
        // EL USUARIO LE DA CLIC AL CORREO (DESDE SU NAVEGADOR)
        // ====================================================================
        [HttpGet("verificar-clic")]
        public ContentResult VerificarClic(string t)
        {
            if (_memoriaTemporal.ContainsKey(t))
            {
                if (DateTime.Now > _memoriaTemporal[t].Expiracion)
                {
                    return Content("<h1>El enlace ha expirado</h1><p>Por favor solicita uno nuevo en la aplicación.</p>", "text/html");
                }
                _memoriaTemporal[t].ClickConfirmado = true;

                string html = @"
<!DOCTYPE html>
<html lang='es'>
<head>
    <meta charset='UTF-8'> 
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Validación Exitosa</title>
    <style>
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #e9ecef; display: flex; justify-content: center; align-items: center; height: 100vh; margin: 0; }
        .tarjeta { background-color: white; padding: 40px; border-radius: 12px; box-shadow: 0 10px 20px rgba(0,0,0,0.1); text-align: center; max-width: 500px; border-top: 6px solid #2b5797; }
        h1 { color: #2b5797; margin-bottom: 10px; font-size: 28px; }
        p { color: #555; font-size: 16px; line-height: 1.6; }
        .icono { font-size: 60px; margin-bottom: 15px; }
    </style>
</head>
<body>
    <div class='tarjeta'>
        <div class='icono'>✅</div>
        <h1>¡Validación Exitosa!</h1>
        <p>Tu correo electrónico ha sido confirmado correctamente.</p>
        <p><strong>Ya puedes cerrar esta pestaña del navegador y regresar a la aplicación de la Clínica para crear tu nueva contraseña.</strong></p>
    </div>
</body>
</html>";
                return Content(html, "text/html", System.Text.Encoding.UTF8);
            }
            return Content("<h1>Enlace no válido o caducado.</h1>", "text/html");
        }

        // ====================================================================
        // VISUAL BASIC PREGUNTA CADA 3 SEGUNDOS "¿YA LE DIO CLIC?"
        // ====================================================================
        [HttpGet("estado-enlace/{token}")]
        public IActionResult RevisarEstadoEnlace(string token)
        {
            if (!_memoriaTemporal.ContainsKey(token))
                return BadRequest(new { error = "El enlace no existe o ya expiró." });

            bool yaConfirmo = _memoriaTemporal[token].ClickConfirmado;
            return Ok(new { Confirmado = yaConfirmo });
        }

        // ====================================================================
        // VISUAL BASIC MANDA LA NUEVA CONTRASEÑA (PANTALLA 2)
        // ====================================================================
        [HttpPost("restablecer-password")]
        public IActionResult RestablecerPassword([FromBody] Models.RestablecerDirectoRequest request)
        {
            if (!new Regex(@"^(?=.*[A-Z])(?=.*\d)[A-Za-z\d@$!%*?&]{8,}$").IsMatch(request.NuevaPassword))
                return BadRequest(new { error = "La contraseña proporcionada es demasiado débil." });

            if (!_memoriaTemporal.ContainsKey(request.Token) || !_memoriaTemporal[request.Token].ClickConfirmado)
                return Unauthorized(new { error = "Proceso no autorizado. Debes confirmar el enlace enviado a tu correo primero." });

            string correoSeguro = _memoriaTemporal[request.Token].Correo;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    string queryPass = "SELECT password_Usuario FROM usuarios WHERE email_Usuario = @correo";
                    string dbPass = "";

                    using (MySqlCommand cmd = new MySqlCommand(queryPass, conn))
                    {
                        cmd.Parameters.AddWithValue("@correo", correoSeguro);
                        var result = cmd.ExecuteScalar();
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                        if (result != null && result != DBNull.Value) dbPass = result.ToString();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                    }

                    bool isSame = false;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    if (dbPass.StartsWith("$2") && dbPass.Length >= 59)
                        isSame = BCrypt.Net.BCrypt.Verify(request.NuevaPassword, dbPass);
                    else
                        isSame = (dbPass == request.NuevaPassword);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                    if (isSame)
                    {
                        return BadRequest(new { error = "La nueva contraseña no puede ser igual a la anterior." });
                    }

                    string newHash = BCrypt.Net.BCrypt.HashPassword(request.NuevaPassword);

                    string queryUpdate = "UPDATE usuarios SET password_Usuario = @newpass WHERE email_Usuario = @correo";
                    using (MySqlCommand cmd = new MySqlCommand(queryUpdate, conn))
                    {
                        cmd.Parameters.AddWithValue("@newpass", newHash);
                        cmd.Parameters.AddWithValue("@correo", correoSeguro);
                        cmd.ExecuteNonQuery();
                    }

                    _memoriaTemporal.Remove(request.Token);
                    return Ok(new { message = "Contraseña restablecida exitosamente." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno al intentar restablecer la contraseña. Detalles: " + ex.Message });
            }
        }

        // ====================================================================
        // AUXILIARES
        // ====================================================================
        private async Task<GoogleJsonWebSignature.Payload> ValidarTokenGoogle(string idToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { GOOGLE_CLIENT_ID }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            if (!payload.EmailVerified)
                throw new InvalidJwtException("El correo de Google no está verificado.");

            return payload;
        }

        private void EnviarCorreoMagico(string destinatario, string token)
        {
            string miCorreo = "farmacia4850@gmail.com";
            string passwordApp = "fzat yxzn kjby kmpe";

            string urlVerificacion = $"http://35.209.106.250:5133/api/auth/verificar-clic?t={token}";

            string cuerpoHtml = $@"
                <div style='font-family: Arial; text-align: center; padding: 20px;'>
                    <h2 style='color: #2b5797;'>Recuperación de Contraseña</h2>
                    <p>Haz clic en el siguiente botón para autorizar el cambio de tu contraseña:</p>
                    <a href='{urlVerificacion}' style='background-color:#2b5797; color:white; padding:15px 25px; text-decoration:none; border-radius:5px; font-weight:bold; display:inline-block; margin: 20px 0;'>CONFIRMAR Y CONTINUAR</a>
                    <p style='color: #888; font-size:12px;'>Este enlace expirará en 10 minutos.</p>
                </div>";

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(miCorreo, "Sistema Clínico");
                mail.To.Add(destinatario);
                mail.Subject = "Autorizar Cambio de Contraseña";
                mail.Body = cuerpoHtml;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential(miCorreo, passwordApp);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }
    }
}

// ====================================================================
// MODELOS NUEVOS (si prefieres, muévelos a tu carpeta Models)
// ====================================================================
namespace backend_CLARA.Models
{
    public class GoogleLoginRequest
    {
        public string IdToken { get; set; } = string.Empty;
    }

    public class GoogleRegistroRequest
    {
        public string IdToken { get; set; } = string.Empty;
        public int IdGenero { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string FechaNacimiento { get; set; } = string.Empty;   // "yyyy-MM-dd"
    }
}