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

        

        public DataTable ObtenerClientesConObras(int? idObraSocial = null)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Consulta base con LEFT JOIN para traer obras sociales
                string query = @"
            SELECT c.id_cliente, c.nombre, c.apellido, c.dni, c.email, c.telefono, c.direccion, fecha_nacimiento,
                   STRING_AGG(o.nombre, ', ') AS obras_sociales
            FROM Cliente c
            LEFT JOIN Cliente_Obra_Social co ON c.id_cliente = co.id_cliente
            LEFT JOIN Obra_Social o ON co.id_obra_social = o.id_obra_social
            WHERE c.activo = 1";

                // Filtrado opcional por obra social
                if (idObraSocial.HasValue)
                {
                    if (idObraSocial.Value > 0)
                    {
                        query += " AND co.id_obra_social = @idObraSocial ";
                    }
                    else if (idObraSocial.Value == -1)
                    {
                        query += " AND co.id_obra_social IS NULL ";
                    }
                }

                query += @"
            GROUP BY c.id_cliente, c.nombre, c.apellido, c.dni, c.email, c.telefono, c.direccion, fecha_nacimiento
            ORDER BY c.nombre, c.apellido";

                SqlCommand cmd = new SqlCommand(query, con);

                if (idObraSocial.HasValue && idObraSocial.Value > 0)
                    cmd.Parameters.AddWithValue("@idObraSocial", idObraSocial.Value);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }


        public int AltaCliente(Cliente cliente)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Cliente (nombre,apellido,dni,fecha_nacimiento,telefono,direccion,email) " +
                    "OUTPUT INSERTED.id_cliente " +
                    "VALUES (@nombre,@apellido,@dni,@fecha_nacimiento,@telefono,@direccion,@email)", con);
                cmd.Parameters.AddWithValue("@nombre", cliente.nombre);
                cmd.Parameters.AddWithValue("@apellido", cliente.apellido);
                cmd.Parameters.AddWithValue("@dni", cliente.dni);
                cmd.Parameters.AddWithValue("@fecha_nacimiento", cliente.fecha_nacimiento);
                cmd.Parameters.AddWithValue("@telefono", cliente.telefono);
                cmd.Parameters.AddWithValue("@direccion", cliente.direccion);
                cmd.Parameters.AddWithValue("@email", cliente.email);
                int id = Convert.ToInt32(cmd.ExecuteScalar());
                return id;
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

        //Obra Sociales---------------------------------------------------------------

        public List<ObraSocial> CargarObrasSociales()
        {
            var lista = new List<ObraSocial>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT id_obra_social, nombre FROM Obra_Social", con);
                SqlDataReader read = cmd.ExecuteReader();

                while (read.Read())
                {
                    lista.Add(new ObraSocial
                    {
                        id_obra_social = read.GetInt32(0),
                        nombre = read.GetString(1)
                    });
                }
            }
            return lista;
        }


        public List<ClienteObraSocial> ObtenerObrasSocialesPorCliente(int idCliente)
        {
            var lista = new List<ClienteObraSocial>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    @"SELECT co.id_cliente, co.id_obra_social, co.nro_afiliado, o.nombre
                    FROM Cliente_Obra_Social co
                    INNER JOIN Obra_Social o ON co.id_obra_social = o.id_obra_social
                    WHERE co.id_cliente = @idCliente", con);

                cmd.Parameters.AddWithValue("@idCliente", idCliente);
                SqlDataReader read = cmd.ExecuteReader();

                while (read.Read())
                {
                    lista.Add(new ClienteObraSocial
                    {
                        id_cliente = read.GetInt32(0),
                        id_obra_social = read.GetInt32(1),
                        nro_afiliado = read.GetInt32(2),
                        nombre_obra_social = read.GetString(3)
                    });
                }
            }

            return lista;
        }


        public DataTable ObtenerClientesPorObraSocial(int idObraSocial)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"
                SELECT c.id_cliente, c.nombre, c.apellido, c.dni, c.email, c.telefono, c.direccion,
                STRING_AGG(o.nombre, ', ') AS obras_sociales
                FROM Cliente c
                LEFT JOIN Cliente_Obra_Social co ON c.id_cliente = co.id_cliente
                LEFT JOIN Obra_Social o ON co.id_obra_social = o.id_obra_social";

                if (idObraSocial > 0)
                {
                    // Filtra por obra social específica
                    query += " WHERE co.id_obra_social = @id_ObraSocial AND c.activo = 1";
                }
                else if (idObraSocial == -1)
                {
                    // Sin obra social
                    query += " WHERE co.id_obra_social IS NULL AND c.activo = 1";
                }
                else
                {
                    query += " WHERE c.activo = 1";
                }

                query += " GROUP BY c.id_cliente, c.nombre, c.apellido, c.dni, c.email, c.telefono, c.direccion";

                SqlCommand cmd = new SqlCommand(query, con);

                if (idObraSocial > 0)
                    cmd.Parameters.AddWithValue("@id_ObraSocial", idObraSocial);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }


        public void AgregarObraSocialACliente(int id_cliente, int id_obraSocial)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Cliente_Obra_Social(id_cliente, id_obra_social)" +
                    "VALUES (@id_cliente, @id_obra_social)", con);
                cmd.Parameters.AddWithValue("@id_cliente", id_cliente);
                cmd.Parameters.AddWithValue("@id_obra_social", id_obraSocial);
                cmd.ExecuteNonQuery();
            }
        }

        public void EliminarObraSocialACliente(int id_cliente, int id_obraSocial)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM Cliente_Obra_Social " +
                    "WHERE id_cliente=@id_cliente AND id_obra_social = @id_obra_social", con);
                cmd.Parameters.AddWithValue("@id_cliente", id_cliente);
                cmd.Parameters.AddWithValue("@id_obra_social", id_obraSocial);
                cmd.ExecuteNonQuery();
            }
        }

        public bool BuscarEmailExistente(string email)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Cliente WHERE email=@email AND activo = 1", con);
                cmd.Parameters.AddWithValue("@email", email);
                int conteo = (int)cmd.ExecuteScalar();
                return conteo > 0;
            }
        }

        public bool BuscarDNIExistente(string dni)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Cliente WHERE dni=@dni AND activo = 1", con);
                cmd.Parameters.AddWithValue("@dni", dni);
                int conteo = (int)cmd.ExecuteScalar();
                return conteo > 0;
            }
        }

        public bool BuscarTelefonoExistente(string telefono)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Cliente WHERE telefono=@telefono AND activo = 1", con);
                    cmd.Parameters.AddWithValue("@telefono", telefono);
                    int conteo = (int)cmd.ExecuteScalar();
                    return conteo > 0;
                }
            }

        }
    }
}