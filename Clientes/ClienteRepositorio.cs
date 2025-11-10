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

                queryBuilder.Append(" ORDER BY c.id_cliente ASC");

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

                if (ventas > 0)
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

        //Devuelve una lista de las obras sociales del cliente
        public DataTable ObtenerObrasSocialesDelCliente(int idCliente)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT os.id_obra_social, os.nombre, cos.nro_afiliado
                                 FROM Cliente_Obra_Social AS cos
                                 INNER JOIN Obra_Social AS os ON cos.id_obra_social = os.id_obra_social
                                 WHERE cos.id_cliente = @idCliente";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@idCliente", idCliente);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        //Devuelve todas las obras sociales para llena la lista de "Disponibles"
        public DataTable ObtenerObrasSociales()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT id_obra_social, nombre
                                 FROM Obra_Social
                                 ORDER BY nombre";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.Fill(dt);
            }

            return dt;
        }

        //Método de guardado de cambios. Borra las asosicaciones viejas e inserta las nuevas
        public void GuardarAsociacionesCliente(int idCliente, DataTable obrasSocialesAsociadas)
        {
            using(SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaccion = con.BeginTransaction();
                try
                {
                    //Borramos las Obras sociales existentes del cliente
                    string query = "DELETE FROM Cliente_Obra_Social WHERE id_cliente = @idCliente";
                    SqlCommand cmdDelete = new SqlCommand(query, con, transaccion);
                    cmdDelete.Parameters.AddWithValue("@idCliente", idCliente);
                    cmdDelete.ExecuteNonQuery();

                    foreach(DataRow row in obrasSocialesAsociadas.Rows)
                    {
                        SqlCommand cmdInsert = new SqlCommand("INSERT INTO Cliente_Obra_Social (id_cliente, id_obra_social, nro_afiliado) " +
                                                              "VALUES (@idCliente, @idObraSocial, @nroAfiliado)", con, transaccion);
                        cmdInsert.Parameters.AddWithValue("@idCliente", idCliente);
                        cmdInsert.Parameters.AddWithValue("@idObraSocial", row["id_obra_social"]);
                        cmdInsert.Parameters.AddWithValue("@nroAfiliado", row["nro_afiliado"]);
                        cmdInsert.ExecuteNonQuery();
                    }

                    transaccion.Commit(); //Si todo es correcto, se guarda los cambios
                }
                catch (Exception)
                {
                    transaccion.Rollback(); //Si algo falla, revierte todo
                    throw; //Relanza el error para visualizarlo
                }

            }
        }



        //obtener TODOS los clientes inactivos (activo = 0)
        public DataTable ObtenerClientesInactivos()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"
                    SELECT id_cliente, nombre, apellido, dni, email, telefono 
                    FROM Cliente 
                    WHERE activo = 0 
                    ORDER BY apellido, nombre";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.Fill(dt);
            }
            return dt;
        }

        public DataTable BuscarActivosPorDNI(string dni)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"
                                SELECT 
                                    c.id_cliente, 
                                    c.nombre, 
                                    c.apellido, 
                                    c.dni, 
                                    c.fecha_nacimiento, 
                                    c.telefono, 
                                    c.direccion, 
                                    c.email,
                                    DATEDIFF(year, c.fecha_nacimiento, GETDATE()) AS edad,
                                    ISNULL(STRING_AGG(os.nombre, ', '), 'Sin Obra Social') AS nombre_obra_social
                                FROM 
                                    Cliente c
                                LEFT JOIN 
                                    Cliente_Obra_Social cos ON c.id_cliente = cos.id_cliente
                                LEFT JOIN 
                                    Obra_Social os ON cos.id_obra_social = os.id_obra_social
                                WHERE 
                                    c.activo = 1 
                                    -- se convierte el DNI (que es INT) a texto (NVARCHAR) para poder usar LIKE
                                    AND CAST(c.dni AS NVARCHAR(50)) LIKE '%' + @dni + '%'
                                GROUP BY                                    
                                    c.id_cliente, 
                                    c.nombre, 
                                    c.apellido, 
                                    c.dni, 
                                    c.fecha_nacimiento, 
                                    c.telefono, 
                                    c.direccion, 
                                    c.email
                                ORDER BY 
                                    c.id_cliente";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@dni", dni);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        //BUSCAR clientes inactivos por DNI
        public DataTable BuscarInactivosPorDNI(string dni)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"
                    SELECT id_cliente, nombre, apellido, dni, email, telefono
                    FROM Cliente 
                    WHERE activo = 0 AND dni LIKE + '%' + @dni + '%'"; // se usa LIKE para búsquedas parciales

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@dni", dni);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        //Activar un cliente (activo = 1)
        public void ReactivarCliente(int idCliente)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "UPDATE Cliente SET activo = 1 WHERE id_cliente = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", idCliente);
                cmd.ExecuteNonQuery();
            }
        }
    }
}