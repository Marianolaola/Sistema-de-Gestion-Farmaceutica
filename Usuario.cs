using System.Data;
using Microsoft.Data.SqlClient;

namespace Sistema_de_Gestión_Farmacéutica
{
    public class Usuario
    {
        private string connectionString = "Server=localhost\\SQLEXPRESS; Database=SistemaFarmaceutico; Trusted_Connection=True; TrustServerCertificate=True;";
        public static int id_usuario { get; set; }
        public static string nombre { get; set; }
        public static string apellido { get; set; }
        public static string contraseña { get; set; }
        public static string rol { get; set; }

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

        public void Sesion(int p_id_usuario, string p_nombre, string p_apellido, string p_contraseña, string p_rol)
        {
            id_usuario = p_id_usuario;
            nombre = p_nombre;
            apellido = p_apellido;
            contraseña = p_contraseña;
            rol = p_rol;
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
