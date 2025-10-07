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

        private ClienteRepositorio clienteRepo = new ClienteRepositorio();
        public ClientesView()
        {
            InitializeComponent();
            CargarClientes();
            CargarObrasSociales();
        }

        private void CargarClientes()
        {
            int? idObraSocial = null;

            if (cmbObraSocial.SelectedValue != null)
            {
                int seleccionado = Convert.ToInt32(cmbObraSocial.SelectedValue);
                if(seleccionado > 0)
                {
                    idObraSocial = seleccionado;
                }
                else if(seleccionado == -1)
                {
                    idObraSocial = -1; // Indica "Sin Obra Social"
                }
                else
                {
                    idObraSocial = null; // Indica "Todos"
                }
                    dgClientes.ItemsSource = clienteRepo.ObtenerClientesConObras(idObraSocial).DefaultView;
            }
           
        }

        private void CargarObrasSociales()
        {
            var obrasSociales = clienteRepo.CargarObrasSociales();

            obrasSociales.Insert(0, new ObraSocial { id_obra_social = 0, nombre = "Todos" });
            obrasSociales.Insert(1, new ObraSocial { id_obra_social = -1, nombre = "Sin Obra Social" });
            cmbObraSocial.ItemsSource = obrasSociales;
            cmbObraSocial.DisplayMemberPath = "nombre";
            cmbObraSocial.SelectedValuePath = "id_obra_social";
            cmbObraSocial.SelectedValue = 0;
        }

        private void cmbObraSocial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbObraSocial.SelectedValue == null) return;

            int idObraSocial = Convert.ToInt32(cmbObraSocial.SelectedValue);

            if (idObraSocial == 0)
            {
                CargarClientes();
            }
            dgClientes.ItemsSource = clienteRepo.ObtenerClientesPorObraSocial(idObraSocial).DefaultView;
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            ClienteWindow ventana = new ClienteWindow();
            bool? resultado = ventana.ShowDialog();

                CargarClientes();
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if(dgClientes.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un cliente para eliminar.", "Atención", MessageBoxButton.OK,MessageBoxImage.Warning);
                return;
            }
            DataRowView filaseleccionada = (DataRowView)dgClientes.SelectedItem;
            int idCliente = Convert.ToInt32(filaseleccionada["id_cliente"]);

            var confirmar = MessageBox.Show("¿Está seguro que desea eliminar el cliente seleccionado?", "Confirmar eliminacion", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmar == MessageBoxResult.Yes)
            {
                Cliente clienteAEliminar = new Cliente { id_cliente = idCliente };
                bool eliminado =  clienteRepo.BajaCliente(clienteAEliminar);

                if (eliminado)
                {
                    MessageBox.Show("Cliente eliminado correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    CargarClientes();
                }
                else
                {
                    MessageBox.Show("No se puede eliminar el cliente porque tiene ventas asociadas.", "Error al eliminar", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgClientes.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un cliente para editar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView filaSeleccionada = (DataRowView)dgClientes.SelectedItem;
            int idCliente = Convert.ToInt32(filaSeleccionada["id_cliente"]);

            // Obtener cliente completo desde el repositorio
            Cliente clienteAEditar = clienteRepo.ObtenerClientesConObras()
                                                .AsEnumerable()
                                                .Select(r => new Cliente
                                                {
                                                    id_cliente = Convert.ToInt32(r["id_cliente"]),
                                                    nombre = r["nombre"].ToString(),
                                                    apellido = r["apellido"].ToString(),
                                                    dni = r["dni"].ToString(),
                                                    fecha_nacimiento = r["fecha_nacimiento"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["fecha_nacimiento"]),
                                                    telefono = r["telefono"].ToString(),
                                                    direccion = r["direccion"].ToString(),
                                                    email = r["email"].ToString()
                                                })
                                                .First(c => c.id_cliente == idCliente);

            ClienteWindow ventana = new ClienteWindow(clienteAEditar); // Pasamos el cliente existente
            ventana.ShowDialog();

            // Recargar clientes después de editar
            CargarClientes();
        }

    }
}

