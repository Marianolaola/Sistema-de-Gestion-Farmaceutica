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
            CargarFiltros();
            AplicarFiltros(); // carga inicial de Datos
        }


        private void CargarFiltros()
        {
            DataTable obrasSociales = clienteRepo.CargarObrasSociales();
            cmbObraSocial.ItemsSource = obrasSociales.DefaultView;
            cmbObraSocial.DisplayMemberPath = "nombre";
            cmbObraSocial.SelectedValuePath = "id_obra_social";
            cmbObraSocial.SelectedValue = 0; // Se selecciona "todos por defecto"
        }

        private void AplicarFiltros()
        {
            if(cmbObraSocial.SelectedValue == null) return;

            int idObraSocialSeleccionada = Convert.ToInt32(cmbObraSocial.SelectedValue);

           DataTable dt = clienteRepo.ObtenerClientesFiltrados(idObraSocialSeleccionada);

            dgClientes.ItemsSource = dt.DefaultView;

            ActualizarConteoClientes(dt.Rows.Count);
        }

        private void ActualizarConteoClientes(int conteo)
        {
            if (conteo == 1)
            {
                txtConteoClientes.Text = "1 Cliente";
            }
            else
            {
                txtConteoClientes.Text = $"{conteo} Clientes";
            }
        }


        private void cmbObraSocial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // cada vez que el usuario cambiar el filtor,, aplciamos el método
            AplicarFiltros();
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            AgregarCliente ventana = new AgregarCliente();
            bool? resultado = ventana.ShowDialog();

            if (resultado == true)
            {
                clienteRepo.AltaCliente(ventana.clienteCreado);
                AplicarFiltros();
            }
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
                    AplicarFiltros();
                }
                else
                {
                    MessageBox.Show("No se puede eliminar el cliente porque tiene ventas asociadas.", "Error al eliminar", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if(dgClientes.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un cliente para editar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView fila = (DataRowView)dgClientes.SelectedItem;

            Cliente clienteAEditar = new Cliente
            {
                id_cliente = Convert.ToInt32(fila["id_cliente"]),
                nombre = fila["nombre"].ToString(),
                apellido = fila["apellido"].ToString(),
                email = fila["email"].ToString(),
                dni = fila["dni"].ToString(),
                telefono = fila["telefono"].ToString(),
                direccion = fila["direccion"].ToString(),
                fecha_nacimiento = Convert.ToDateTime(fila["fecha_nacimiento"])

            };

            EditarCliente ventana = new EditarCliente(clienteAEditar);
            bool? resultado = ventana.ShowDialog();

            if (resultado == true)
            {
                clienteRepo.EditarCliente(clienteAEditar);
                AplicarFiltros();
                MessageBox.Show("Cliente editado correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnGestionarOS_Click(object sender, RoutedEventArgs e)
        {
            if(dgClientes.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un cliente para gestionar la obra social.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView fila = (DataRowView)dgClientes.SelectedItem;

            int idCliente = (int)fila["id_cliente"];
            string nombreCliente = $"{fila["nombre"]} {fila["apellido"]}";

            //Abrimos ventana de Gestión
            GestionarObrasSociales gestionOS = new GestionarObrasSociales(idCliente, nombreCliente);
            bool? resultado = gestionOS.ShowDialog();

            //Si se guardan los cambios, recargamos la lista de clientes
            if(resultado == true)
            {
                AplicarFiltros();
            }

        }
    }
}

