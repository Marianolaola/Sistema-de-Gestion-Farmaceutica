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
                SqlCommand cmd = new SqlCommand("SELECT * FROM Cliente WHERE activo = 1", con);
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

            lista.Insert(0, new ObraSocial
            {
                id_obra_social = 0,
                nombre = "Sin Obra Social"
            });

                return lista;

        }


        public List<ClienteObraSocial> ObtenerObrasSocialesPorCliente(int idCliente)
        {
            var lista = new List<ClienteObraSocial>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT id_cliente, id_obra_social, nro_afiliado " +
                    "FROM Cliente_Obra_Social WHERE id_cliente = @idCliente",con);
                cmd.Parameters.AddWithValue("@idCliente", idCliente);
                SqlDataReader read = cmd.ExecuteReader();
                while (read.Read())
                {
                    lista.Add(new ClienteObraSocial
                    {
                        id_cliente = read.GetInt32(0),
                        id_obra_social = read.GetInt32(1),
                        nro_afiliado = read.GetInt32(2)
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
                string query;

                if (idObraSocial == 0)
                {
                    query = @"
                SELECT DISTINCT c.id_cliente, c.nombre, c.apellido, c.dni, 
                                c.fecha_nacimiento, c.telefono, c.direccion, c.email,
                                ISNULL(o.nombre, 'Sin Obra Social') AS nombre_obra_social
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
                    "WHERE id_cliente=@id_cliente AND id_obra_social = @id_obra_social)", con);
                cmd.Parameters.AddWithValue("@id_cliente", id_cliente);
                cmd.Parameters.AddWithValue("@id_obra_social", id_obraSocial);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
