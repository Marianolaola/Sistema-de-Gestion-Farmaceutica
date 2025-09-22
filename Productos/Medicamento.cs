using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_de_Gestión_Farmacéutica
{
    public class Medicamento
    {
        private string connectionString = "Server=localhost\\SQLEXPRESS; Database=SistemaFarmaceutico; Trusted_Connection=True; TrustServerCertificate=True;";
        public DataTable ObtenerMedicamentos()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Medicamento", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        public void AltaMedicamento(string nombre, float precio_unitario, string presentacion, 
                                    string laboratorio, int stock, int stock_minimo)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Medicamento (nombre, precio_unitario, presentacion, laboratorio, stock, stock_minimo) " +
                    "VALUES (@nombre, @precio_unitario, @presentacion, @laboratorio, @stock, @stock_minimo)", con);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@precio_unitario", precio_unitario);
                cmd.Parameters.AddWithValue("@presentacion", presentacion);
                cmd.Parameters.AddWithValue("@laboratorio", laboratorio);
                cmd.Parameters.AddWithValue("@stock", stock);
                cmd.Parameters.AddWithValue("@stock_minimo", stock_minimo);
                cmd.ExecuteNonQuery();
            }
        }

        public void BajaMedicamento(int id_medicamento)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Medicamento WHERE id_medicamento=@id", con);
                cmd.Parameters.AddWithValue("@id", id_medicamento);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
