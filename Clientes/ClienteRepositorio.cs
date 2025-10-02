using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_de_Gestión_Farmacéutica.Clientes
{
    public class ClienteRepositorio
    {
        private string connectionString = "Server=localhost\\SQLEXPRESS; Database=SistemaFarmaceutico; Trusted_Connection=True; TrustServerCertificate=True;";

        public DataTable ObtenerClientes()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Cliente", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        public DataTable CargarObrasSociales()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT id_obra_social, nombre FROM Obra_Social", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                
                da.Fill(dt);

                // Agregar opción "Sin Filtro"
                DataRow filaTodos = dt.NewRow();
                filaTodos["id_obra_social"] = 0;
                filaTodos["nombre"] = "Sin Filtro";
                dt.Rows.InsertAt(filaTodos, 0);

            }
                return dt;

        }


        public DataTable ObtenerClientesPorObraSocial(int idObraSocial)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query;

                if (idObraSocial == 0)
                {
                    query = @"
                SELECT DISTINCT c.id_cliente, c.nombre, c.apellido, c.dni, 
                                c.fecha_nacimiento, c.telefono, c.direccion, c.email,
                                o.nombre AS nombre_obra_social
                FROM Cliente c
                LEFT JOIN Cliente_Obra_Social co ON c.id_cliente = co.id_cliente
                LEFT JOIN Obra_Social o ON co.id_obra_social = o.id_obra_social";
                }
                else
                {
                    query = @"
                SELECT c.id_cliente, c.nombre, c.apellido, c.dni, 
                       c.fecha_nacimiento, c.telefono, c.direccion, c.email,
                       o.nombre AS nombre_obra_social,
                       co.nro_afiliado
                FROM Cliente c
                INNER JOIN Cliente_Obra_Social co ON c.id_cliente = co.id_cliente
                INNER JOIN Obra_Social o ON co.id_obra_social = o.id_obra_social
                WHERE co.id_obra_social = @idObraSocial";
                }

                SqlCommand cmd = new SqlCommand(query, con);
                if (idObraSocial != 0)
                    cmd.Parameters.AddWithValue("@idObraSocial", idObraSocial);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

    }
}
