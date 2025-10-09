using Microsoft.Data.SqlClient;
using Sistema_de_Gestión_Farmacéutica.Obra_Sociales;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace Sistema_de_Gestión_Farmacéutica.Clientes
{
    public class ClienteRepositorio
    {
        private string connectionString = "Server=localhost\\SQLEXPRESS; Database=SistemaFarmaceutico; Trusted_Connection=True; TrustServerCertificate=True;";


        public DataTable ObtenerClientesFiltrados(int idObraSocial, string rangoEdad = "todos")
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                var queryBuilder = new StringBuilder();
                SqlCommand cmd = new SqlCommand();

                if (idObraSocial > 0) // --- CASO 1: Se filtra por una Obra Social específica ---
                {
                    queryBuilder.Append(@"
                    SELECT 
                        c.*,
                        DATEDIFF(year, c.fecha_nacimiento, GETDATE()) as edad,
                        os.nombre AS nombre_obra_social
                    FROM Cliente c
                    INNER JOIN Cliente_Obra_Social cos ON c.id_cliente = cos.id_cliente
                    INNER JOIN Obra_Social os ON cos.id_obra_social = os.id_obra_social
                    WHERE c.activo = 1 AND os.id_obra_social = @idObraSocial ");
                    cmd.Parameters.AddWithValue("@idObraSocial", idObraSocial);
                }
                else // --- CASO 2: Se selecciona "Todos" o "Sin Obra Social" ---
                {
                    queryBuilder.Append(@"
                    SELECT 
                        c.*,
                        DATEDIFF(year, c.fecha_nacimiento, GETDATE()) as edad,
                        (
                            SELECT ISNULL(STRING_AGG(os.nombre, ', '), 'Sin Obra Social')
                            FROM Cliente_Obra_Social cos
                            JOIN Obra_Social os ON cos.id_obra_social = os.id_obra_social
                            WHERE cos.id_cliente = c.id_cliente
                        ) AS nombre_obra_social
                    FROM Cliente c
                    WHERE c.activo = 1 ");

                    if (idObraSocial == -1) // Condición extra para "Sin Obra Social"
                    {
                        queryBuilder.Append(" AND NOT EXISTS (SELECT 1 FROM Cliente_Obra_Social cos WHERE cos.id_cliente = c.id_cliente) ");
                    }
                }

                queryBuilder.Append(" ORDER BY c.apellido, c.nombre");

                cmd.Connection = con;
                cmd.CommandText = queryBuilder.ToString();

                con.Open();
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
                // Esta consulta construye la lista completa en el servidor SQL
                string query = @"
                                SELECT id_obra_social, nombre FROM (
                                SELECT 0 AS id_obra_social, 'Todos' AS nombre, 1 AS SortOrder
                                UNION ALL
                                SELECT -1 AS id_obra_social, 'Sin Obra Social' AS nombre, 2 AS SortOrder
                                UNION ALL
                                SELECT id_obra_social, nombre, 3 AS SortOrder
                                FROM Obra_Social
                                ) AS OpcionesFiltro
                                ORDER BY SortOrder, nombre";

                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }



        public void AltaCliente(Cliente cliente) 
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Cliente(nombre,apellido,dni,fecha_nacimiento,telefono,direccion,email) " +
                    "VALUES (@nombre,@apellido,@dni,@fecha_nacimiento,@telefono,@direccion,@email)", con);
                cmd.Parameters.AddWithValue("@nombre", cliente.nombre);
                cmd.Parameters.AddWithValue("@apellido", cliente.apellido);
                cmd.Parameters.AddWithValue("@dni", cliente.dni);
                cmd.Parameters.AddWithValue("@fecha_nacimiento", cliente.fecha_nacimiento);
                cmd.Parameters.AddWithValue("@telefono", cliente.telefono);
                cmd.Parameters.AddWithValue("@direccion", cliente.direccion);
                cmd.Parameters.AddWithValue("@email", cliente.email);
                cmd.ExecuteNonQuery();
            }
            
        }

        public void EditarCliente(Cliente cliente)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Cliente SET nombre=@nombre, apellido=@apellido, fecha_nacimiento=@fecha_nacimiento, email=@email, telefono=@telefono, direccion=@direccion WHERE id_cliente=@id", con);
                cmd.Parameters.AddWithValue("@id", cliente.id_cliente);
                cmd.Parameters.AddWithValue("@nombre", cliente.nombre);
                cmd.Parameters.AddWithValue("@apellido", cliente.apellido);
                cmd.Parameters.AddWithValue("@fecha_nacimiento", cliente.fecha_nacimiento);
                cmd.Parameters.AddWithValue("@email", cliente.email);
                cmd.Parameters.AddWithValue("@telefono", cliente.telefono);
                cmd.Parameters.AddWithValue("@direccion", cliente.direccion);
                cmd.ExecuteNonQuery();
            }
        }

        public bool BajaCliente(Cliente cliente)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Venta WHERE id_cliente=@id", con);
                checkCmd.Parameters.AddWithValue("@id", cliente.id_cliente);
                int ventas = (int)checkCmd.ExecuteScalar();

                if(ventas > 0)
                {
                    //tiene ventas, no se puede eliminar
                    return false;
                }

                //eliminacion lógica a cliente
                SqlCommand Bajacmd = new SqlCommand("UPDATE Cliente SET activo = 0 WHERE id_cliente = @id", con);
                Bajacmd.Parameters.AddWithValue("@id", cliente.id_cliente);

                Bajacmd.ExecuteNonQuery();
                return true;
            }
        }
    }
}
