using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sistema_de_Gestión_Farmacéutica.Obra_Sociales
{
    class ObrasRepositorio
    {
        private string connectionString = "Server=localhost\\SQLEXPRESS; Database=SistemaFarmaceutico; Trusted_Connection=True; TrustServerCertificate=True;";

        public DataTable ObtenerObraSociales()
        {
            DataTable dt = new DataTable();
            using(SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT id_obra_social, nombre From Obra_social ORDER BY nombre";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.Fill(dt);
            }
            return dt;
        }

        // Agregar una nueva obra social
        public void AgregarObraSocial(ObraSocial obra)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "INSERT INTO Obra_social (nombre) VALUES (@nombre)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@nombre", obra.nombre);
                cmd.ExecuteNonQuery();
            }
        }

        //Editar una obra social
        public void EditarObraSocial(ObraSocial obra)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "UPDATE Obra_social SET nombre=@nombre WHERE id_obra_social=@id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@nombre", obra.nombre);
                cmd.Parameters.AddWithValue("@id", obra.id_obra_social);
                cmd.ExecuteNonQuery();
            }
        }

        //Eliminar una obra social
        public bool EliminarObraSocial(int idObraSocial)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                //Verificar si la obra social está asociada a algún cliente
                string checkQuery = "SELECT COUNT(*) FROM Cliente_Obra_Social WHERE id_obra_social = @id";
                SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                checkCmd.Parameters.AddWithValue("@id", idObraSocial);

                int asociados = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (asociados > 0)
                {
                    return false;
                }

                string query = "DELETE FROM Obra_social WHERE id_obra_social = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", idObraSocial);
                cmd.ExecuteNonQuery();

                return true;
            }
        }
    }
}
