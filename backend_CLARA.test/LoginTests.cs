using backend_CLARA.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace backend_CLARA.Tests
{
    public class LoginTests
    {
        private readonly AuthController _controller;

        public LoginTests()
        {
            _controller = new AuthController();
        }

        // 1. Prueba de bloqueo para cuentas inactivas (en sus 3 variantes de texto)
        [Theory]
        [InlineData("Inactivo")]
        [InlineData("INACTIVO")]
        [InlineData("inactivo")]
        public void Login_UsuarioConEstatusInactivo_RetornaUnauthorized(string estatus)
        {
            // Act
            var resultado = _controller.EvaluarCredencialesLogin(
                idUsuario: 1,
                dbPassword: "Password123",
                estatusUsuario: estatus,
                passwordIngresada: "Password123",
                out bool needsUpgrade
            );

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(resultado);
            Assert.False(needsUpgrade);
        }

        // 2. Prueba de Login exitoso con contraseña en texto plano (activa migración a BCrypt)
        [Fact]
        public void Login_PasswordTextoPlanoCorrecta_RetornaOkYActivaMigracion()
        {
            // Act
            var resultado = _controller.EvaluarCredencialesLogin(
                idUsuario: 4,
                dbPassword: "MiPasswordPlana",
                estatusUsuario: "Activo",
                passwordIngresada: "MiPasswordPlana",
                out bool needsUpgrade
            );

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
            Assert.True(needsUpgrade); // Confirma que se encriptará en la BD
        }

        // 3. Prueba de Login exitoso con Hash de BCrypt (no requiere migración)
        [Fact]
        public void Login_PasswordConHashBCryptCorrecta_RetornaOkSinMigrar()
        {
            // Arrange: Generamos un hash real de BCrypt como los almacenados en MySQL
            string hashGuardadoEnBD = BCrypt.Net.BCrypt.HashPassword("ClaveSegura2026");

            // Act
            var resultado = _controller.EvaluarCredencialesLogin(
                idUsuario: 2,
                dbPassword: hashGuardadoEnBD,
                estatusUsuario: "Activo",
                passwordIngresada: "ClaveSegura2026",
                out bool needsUpgrade
            );

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
            Assert.False(needsUpgrade); // Ya estaba encriptada, no hace UPDATE extra
        }

        // 4. Prueba de rechazo cuando la contraseña ingresada es incorrecta
        [Fact]
        public void Login_PasswordIncorrecta_RetornaUnauthorized()
        {
            // Arrange
            string hashGuardadoEnBD = BCrypt.Net.BCrypt.HashPassword("ClaveReal123");

            // Act
            var resultado = _controller.EvaluarCredencialesLogin(
                idUsuario: 3,
                dbPassword: hashGuardadoEnBD,
                estatusUsuario: "Activo",
                passwordIngresada: "ClaveEquivocada999",
                out bool needsUpgrade
            );

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(resultado);
            Assert.False(needsUpgrade);
        }

        // 5. Prueba de rechazo cuando el correo o username no existe en la base de datos
        [Fact]
        public void Login_UsuarioNoExisteEnBD_RetornaUnauthorized()
        {
            // Act (idUsuario y dbPassword llegan nulos cuando el SELECT no encuentra coincidencias)
            var resultado = _controller.EvaluarCredencialesLogin(
                idUsuario: null,
                dbPassword: null,
                estatusUsuario: null,
                passwordIngresada: "CualquierPass",
                out bool needsUpgrade
            );

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(resultado);
            Assert.False(needsUpgrade);
        }
    }
}