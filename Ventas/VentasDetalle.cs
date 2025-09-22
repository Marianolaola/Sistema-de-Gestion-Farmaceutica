using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_de_Gestión_Farmacéutica.Ventas
{
    class VentasDetalle
    {
        private string connectionString = "Server=localhost\\SQLEXPRESS; Database=SistemaFarmaceutico; Trusted_Connection=True; TrustServerCertificate=True;";

        public DataTable ObtenerVentasConDetalle()  
        {
            int id_usuarioLogueado = Usuarios.Usuario.id_usuario;
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
    }
}


