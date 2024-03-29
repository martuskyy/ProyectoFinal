﻿using Microsoft.AspNetCore.Mvc;
using SistemaGestionEntities;
using SistemaGestionData;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace WebAPISistemaGestion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : Controller
    {
        private static string connectionString = "Server=.; Database=master; Trusted_Connection=True;";

        [HttpGet("{nombreUsuario}/{contraseña}")]
        public IActionResult IniciarSesion(string nombreUsuario, string contraseña)
        {
            string query = "SELECT * FROM Usuario WHERE NombreUsuario = @NombreUsuario AND Contraseña = @Contraseña";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                    command.Parameters.AddWithValue("@Contraseña", contraseña);
                    try
                    {
                        connection.Open();

                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            return Ok("Inicio de sesión exitoso");
                        }
                        else
                        {
                            return BadRequest("Nombre de usuario o contraseña incorrectos");
                        }
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, $"Error al iniciar sesion: {ex.Message}");
                    }
                }
            }
        }

        [HttpGet("{nombreUsuario}")]
        public IActionResult TraerUsuario(string nombreUsuario)
        {
            string query = "SELECT * FROM Usuario WHERE NombreUsuario = @NombreUsuario";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);

                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var usuario = new
                                {
                                    Id = reader["Id"],
                                    Nombre = reader["Nombre"],
                                    Apellido = reader["Apellido"],
                                    Clave = reader["Contraseña"],
                                    Mail = reader["Mail"]                                    
                                };

                                return Ok(usuario);
                            }
                            else
                            {
                                return NotFound();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, $"Error al traer usuario: {ex.Message}");
                    }
                }
            }
        }

        [HttpPost]
        public IActionResult CrearUsuario(Usuario usuario)
        {
            if (string.IsNullOrEmpty(usuario.Nombre) || string.IsNullOrEmpty(usuario.Apellido) || string.IsNullOrEmpty(usuario.NombreUsuario) ||
                string.IsNullOrEmpty(usuario.Clave) || string.IsNullOrEmpty(usuario.Email))
            {
                return BadRequest("Todos los campos son obligatorios");
            }

            IActionResult usuarioExistente = TraerUsuario(usuario.NombreUsuario);
            if (usuarioExistente is OkResult)
            {
                return BadRequest("El nombre de usuario ya existe");
            }

            try
            {
                UsuarioData.CrearUsuario(usuario);
                return Ok("Usuario creado exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear usuario: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public IActionResult ModificarUsuarioPorId(int id, [FromBody] Usuario usuario)
        {
            try
            {
                UsuarioData.ModificarUsuarios(id, usuario);
                return Ok("Usuario modificado exitosamente");   
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al modificar usuario: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult EliminarUsuario(int id)
        {
            try
            {
                UsuarioData.EliminarUsuario(id);
                return Ok("Usuario eliminado exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar usuario: {ex.Message}");
            }
        }
    }
}
