using System.Data;
using Microsoft.Data.SqlClient;

namespace Sistema_de_Gestión_Farmacéutica
{
    public class Usuario
    {
        private string connectionString = @"Server=DESKTOP-SQ7CFSI\SQLEXPRESS;Database=[Sistema Farmaceútico];Trusted_Connection=True;TrustServerCertificate=True;";


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

        // Dar de alta un usuario
        public void AltaUsuario(string nombre, string apellido, string contraseña, string rol)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Usuario (nombre, apellido, contraseña, rol) VALUES (@nombre, @apellido, @contraseña, @rol)", con);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@apellido", apellido);
                cmd.Parameters.AddWithValue("@contraseña", contraseña);
                cmd.Parameters.AddWithValue("@rol", rol);
                cmd.ExecuteNonQuery();
            }
        }

        // Dar de baja un usuario
        public void BajaUsuario(int id_usuario)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Usuario WHERE id_usuario=@id", con);
                cmd.Parameters.AddWithValue("@id", id_usuario);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
