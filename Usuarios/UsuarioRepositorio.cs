using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Sistema_de_Gestión_Farmacéutica.Usuarios
{
    public class UsuarioRepositorio
    {
        private string connectionString = "Server=localhost\\SQLEXPRESS; Database=SistemaFarmaceutico; Trusted_Connection=True; TrustServerCertificate=True;";


        // Traer todos los usuarios
        public DataTable ObtenerUsuarios()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Usuario", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        public Usuario obtenerUsuarioPorEmail(string email)
        {
            Usuario usuario = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT * FROM Usuario WHERE email = @email";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@email", email);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        usuario = new Usuario
                        {
                            id_usuario = (int)reader["id_usuario"],
                            nombre = reader["nombre"].ToString(),
                            apellido = reader["apellido"].ToString(),
                            email = reader["email"].ToString(),
                            rol = reader["rol"].ToString(),
                            contraseña = reader["contraseña"].ToString().Trim()//traemos el HASH guardado
                        };
                    }
                }
            }
            return usuario;
        }

        // Dar de alta un usuario
        public void AltaUsuario(Usuario usuario)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Usuario (nombre, apellido, contraseña,email, rol) VALUES (@nombre, @apellido, @contraseña,@email,@rol)", con);

                //hasheamos la contrraseña antes de guardarla
                string hashContraseña = BCrypt.Net.BCrypt.HashPassword(usuario.contraseña);


                cmd.Parameters.AddWithValue("@nombre", usuario.nombre);
                cmd.Parameters.AddWithValue("@apellido", usuario.apellido);
                cmd.Parameters.AddWithValue("@contraseña", hashContraseña);
                cmd.Parameters.AddWithValue("@email", usuario.email);
                cmd.Parameters.AddWithValue("@rol", usuario.rol);
                cmd.ExecuteNonQuery();
            }
        }

        // Dar de baja un usuario
        public void BajaUsuario(Usuario usuario)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Usuario WHERE id_usuario=@id", con);
                cmd.Parameters.AddWithValue("@id", usuario.id_usuario);
                cmd.ExecuteNonQuery();
            }
        }

        public void EditarUsuario(Usuario usuario)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Usuario SET nombre=@nombre, apellido=@apellido, contraseña=@contraseña, rol=@rol WHERE id_usuario=@id", con);

                string hashDeContraseña = BCrypt.Net.BCrypt.HashPassword(usuario.contraseña);

                cmd.Parameters.AddWithValue("@nombre", usuario.nombre);
                cmd.Parameters.AddWithValue("@apellido", usuario.apellido);
                cmd.Parameters.AddWithValue("@contraseña", hashDeContraseña);
                cmd.Parameters.AddWithValue("@email", usuario.email);
                cmd.Parameters.AddWithValue("@rol", usuario.rol);
                cmd.Parameters.AddWithValue("@id", usuario.id_usuario);
                cmd.ExecuteNonQuery();
            }
        }
    }
}

 
