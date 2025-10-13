using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_de_Gestión_Farmacéutica.Productos
{
    public class MedicamentoRepositorio
    {
        private string connectionString = "Server=localhost\\SQLEXPRESS; Database=SistemaFarmaceutico; Trusted_Connection=True; TrustServerCertificate=True;";

        public DataTable ObtenerMedicamentosActivos()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Medicamento WHERE activado = 1", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        public DataTable ObtenerMedicamentosInactivos()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Medicamento WHERE activado = 0", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        public void AltaMedicamento(Medicamento p_medicamento)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Medicamento (nombre_comercial, precio_unitario, presentacion, laboratorio, stock, stock_minimo) " +
                    "VALUES (@nombre_comercial, @precio_unitario, @presentacion, @laboratorio, @stock, @stock_minimo)", con);
                cmd.Parameters.AddWithValue("@nombre_comercial", p_medicamento.nombre_comercial);
                cmd.Parameters.AddWithValue("@precio_unitario", p_medicamento.precio_unitario);
                cmd.Parameters.AddWithValue("@presentacion", p_medicamento.presentacion);
                cmd.Parameters.AddWithValue("@laboratorio", p_medicamento.laboratorio);
                cmd.Parameters.AddWithValue("@stock", p_medicamento.stock);
                cmd.Parameters.AddWithValue("@stock_minimo", p_medicamento.stock_minimo);
                cmd.ExecuteNonQuery();
            }
        }

        public void BajaMedicamento(int id_medicamento)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Medicamento SET activado=0 WHERE id_medicamento=@id", con);
                cmd.Parameters.AddWithValue("@id", id_medicamento);
                cmd.ExecuteNonQuery();
            }
        }

        public void ActivarMedicamento(int id_medicamento)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Medicamento SET activado=1 WHERE id_medicamento=@id", con);
                cmd.Parameters.AddWithValue("@id", id_medicamento);
                cmd.ExecuteNonQuery();
            }
        }

        public void EditarMedicamento(Medicamento p_medicamento)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Medicamento SET nombre_comercial=@nombre_comercial, precio_unitario=@precio_unitario, presentacion=@presentacion, laboratorio=@laboratorio, stock=@stock, stock_minimo=@stock_minimo WHERE id_medicamento=@id", con);
                cmd.Parameters.AddWithValue("@nombre_comercial", p_medicamento.nombre_comercial);
                cmd.Parameters.AddWithValue("@precio_unitario", p_medicamento.precio_unitario);
                cmd.Parameters.AddWithValue("@presentacion", p_medicamento.presentacion);
                cmd.Parameters.AddWithValue("@laboratorio", p_medicamento.laboratorio);
                cmd.Parameters.AddWithValue("@stock", p_medicamento.stock);
                cmd.Parameters.AddWithValue("@stock_minimo", p_medicamento.stock_minimo);
                cmd.Parameters.AddWithValue("@id", p_medicamento.id_medicamento);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
