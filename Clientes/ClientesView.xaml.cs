using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sistema_de_Gestión_Farmacéutica.Clientes
{
    /// <summary>
    /// Lógica de interacción para ClientesView.xaml
    /// </summary>
    public partial class ClientesView : UserControl
    {
        private string connectionString = "Server=localhost\\SQLEXPRESS; Database=SistemaFarmaceutico; Trusted_Connection=True; TrustServerCertificate=True;";
        private Cliente cliente = new Cliente();
        public ClientesView()
        {
            InitializeComponent();
            cargarCliente();
            CargarObrasSociales();
        }

        private void cargarCliente()
        {
            dgClientes.ItemsSource = cliente.ObtenerClientes().DefaultView;
        }

        private void CargarObrasSociales()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT id_obra_social, nombre FROM Obra_Social", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                //fila para mostrar clientes sin filtro de obra social
                DataRow filaTodos = dt.NewRow();
                filaTodos["id_obra_social"] = 0; 
                filaTodos["nombre"] = "Sin Filtro";
                dt.Rows.InsertAt(filaTodos, 0);

                cmbObraSocial.ItemsSource = dt.DefaultView;
                cmbObraSocial.DisplayMemberPath = "nombre";   // lo que se muestra
                cmbObraSocial.SelectedValuePath = "id_obra_social"; // el valor real
            }
        }

        private void cmbObraSocial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbObraSocial.SelectedValue == null) return;

            int idObraSocial = Convert.ToInt32(cmbObraSocial.SelectedValue);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query;

                if (idObraSocial == 0)
                {
                    // Mostrar todos los clientes sin filtrar
                    query = @"
                        SELECT DISTINCT c.id_cliente, c.nombre, c.apellido, c.dni, c.fecha_nacimiento, c.telefono, c.direccion, c.email
                        FROM Cliente c
                        LEFT JOIN Cliente_Obra_Social co ON c.id_cliente = co.id_cliente";
                }
                else
                {
                    // Mostrar clientes filtrados por obra social
                    query = @"
                        SELECT c.id_cliente, c.nombre, c.apellido, c.dni, c.fecha_nacimiento, c.telefono, c.direccion, c.email,
                                co.nro_afiliado
                        FROM Cliente c
                        INNER JOIN Cliente_Obra_Social co ON c.id_cliente = co.id_cliente
                        WHERE co.id_obra_social = @idObraSocial";
                }

                SqlCommand cmd = new SqlCommand(query, con);

                if (idObraSocial != 0)
                    cmd.Parameters.AddWithValue("@idObraSocial", idObraSocial);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgClientes.ItemsSource = dt.DefaultView;
            }
        }
        }
    }

