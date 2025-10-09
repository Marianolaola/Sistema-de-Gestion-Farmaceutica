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


        public DataTable ObtenerClientesFiltrados(int idObraSocial, string rangoEdad = "Todos")
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                var queryBuilder = new StringBuilder(@"SELECT
                                    c.id_cliente
                                    c.nombre,
                                    c.apellido,
                                    c.dni,
                                    c.fecha_nacimiento,
                                    DATEDIFF(year, c.fecha_nacimiento, GETDATE())  AS edad,
                                    c.email,
                                    c.telefono,
                                    c.direccion,
                                    ISNULL(STRING_AGG(o.nombre, ', '), '') AS nombre_obra_social,
                                FROM Cliente c
                                LEFT JOIN Cliente_Obra_Social AS cos ON c.id_cliente = cos.id_cliente
                                LEFT JOIN Obra_Social AS os ON cos.id_obra_social = os.id_obra_social
                                WHERE activo = 1");

                SqlCommand cmd = new SqlCommand();

                //--Lógica para Filtros de Obra Social--
                if(idObraSocial == -1) // sin Obra Social
                {
                    queryBuilder.Append("AND NOT EXISTS(SELECT 1 FROM Cliente_Obra_Social AS cos WHERE cos.id_cliente = c.id_cliente)");
                }
                else if (idObraSocial > 0) // Obras Sociales Específicas
                {
                    queryBuilder.Append("AND EXISTS FROM Cliente_Obra_Social AS cos WHERE cos.id_cliente = c.id_cliente AND cos.id_obra_social = @idObraSocial)");
                    cmd.Parameters.AddWithValue("@idObraSocial", idObraSocial);
                }

                //SI es idObraSocial == 0, no se aplica ninguna condición
                queryBuilder.Append(@"GROUP BY c.id_cliente, c.nombre, c.apellido, c.dni, c.fecha_nacimiento, 
                                               c.telefono, c.direccion, c.email
                                      ORDER BY c.apellido, c.nombre");

                cmd.Connection = con;
                cmd.CommandText = queryBuilder.ToString();
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
                SqlCommand cmd = new SqlCommand("SELECT id_obra_social, nombre FROM Obra_Social ORDER BY nombre", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(dt);

                // Agregar opción "Todos"
                DataRow filaTodos = dt.NewRow();
                filaTodos["id_obra_social"] = 0; // ID para Todos
                filaTodos["nombre"] = "Todos";
                dt.Rows.InsertAt(filaTodos, 0);

                // Agregar opción "Sin Obra Social"
                DataRow filaSinOs = dt.NewRow();
                filaSinOs["id_obra_social"] = -1; // ID para Sin Obras Sociales
                filaSinOs["nombre"] = "Sin Obra Social";
                dt.Rows.InsertAt(filaSinOs, -1);
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
