using Microsoft.Data.SqlClient;
using Sistema_de_Gestión_Farmacéutica.Sesion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_de_Gestión_Farmacéutica.Ventas
{
    public class VentaDetalleRepositorio
    {
        private string connectionString = "Server=localhost\\SQLEXPRESS; Database=SistemaFarmaceutico; Trusted_Connection=True; TrustServerCertificate=True;";

        public DataTable ObtenerVentasConDetalle()
        {
            int id_usuarioLogueado = SesionActual.Usuario.id_usuario;
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"
                    SELECT v.id_venta, v.fecha_venta, v.id_cliente, v.id_usuario,
                           dv.id_medicamento, dv.cantidad, dv.subtotal
                    FROM Venta v
                    INNER JOIN Detalle_Venta dv ON v.id_venta = dv.id_venta
                    WHERE v.id_usuario = @id_usuarioLogueado";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id_usuarioLogueado", id_usuarioLogueado);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }


        public bool RegistrarVenta(int idCliente, int idUsuario, List<DetalleItem> detalles, out string mensaje)
        {
            mensaje = "";
            bool exito = false;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction tx = con.BeginTransaction();

                try
                {
                    // Insertar venta
                    SqlCommand cmdVenta = new SqlCommand(
                        "INSERT INTO Venta (fecha_venta, id_cliente, id_usuario) OUTPUT INSERTED.id_venta VALUES (GETDATE(), @id_cliente, @id_usuario)", con, tx);
                    cmdVenta.Parameters.AddWithValue("@id_cliente", idCliente);
                    cmdVenta.Parameters.AddWithValue("@id_usuario", idUsuario);

                    int idVenta = (int)cmdVenta.ExecuteScalar();

                    // Insertar detalle
                    foreach (var d in detalles)
                    {
                        SqlCommand cmdDet = new SqlCommand(
                            "INSERT INTO Detalle_Venta (id_venta, id_medicamento, cantidad, subtotal) VALUES (@idv, @idm, @cant, @sub)", con, tx);
                        cmdDet.Parameters.AddWithValue("@idv", idVenta);
                        cmdDet.Parameters.AddWithValue("@idm", d.IdMedicamento);
                        cmdDet.Parameters.AddWithValue("@cant", d.Cantidad);
                        cmdDet.Parameters.AddWithValue("@sub", d.Subtotal);
                        cmdDet.ExecuteNonQuery();
                    }

                    tx.Commit();
                    mensaje = "Venta registrada correctamente.";
                    exito = true;
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    mensaje = "Error al registrar venta: " + ex.Message;
                    exito = false;
                }
            }

            return exito;
        }
    }
}
