using System.Data;
using System.Windows;
using System.Windows.Input;

namespace Sistema_de_Gestión_Farmacéutica.Clientes
{
    public partial class ClientesInactivosWindow : Window
    {
        private ClienteRepositorio clienteRepo = new ClienteRepositorio();
        public bool ClienteReactivado { get; private set; } = false; // Para avisar a la vista principal

        public ClientesInactivosWindow()
        {
            InitializeComponent();
            CargarInactivos();
        }

        private void CargarInactivos()
        {
            dgClientesInactivos.ItemsSource = clienteRepo.ObtenerClientesInactivos().DefaultView;
        }

        

        private void btnReactivar_Click(object sender, RoutedEventArgs e)
        {
            if (dgClientesInactivos.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccione un cliente para reactivar.", "Selección requerida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView fila = (DataRowView)dgClientesInactivos.SelectedItem;
            int idCliente = (int)fila["id_cliente"];

            var confirmar = MessageBox.Show($"¿Está seguro que desea reactivar al cliente '{fila["nombre"]} {fila["apellido"]}'?",
                                            "Confirmar Reactivación", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmar == MessageBoxResult.Yes)
            {
                clienteRepo.ReactivarCliente(idCliente);
                ClienteReactivado = true; // Avisamos que se hizo un cambio
                CargarInactivos(); // Recargamos la lista de inactivos
                MessageBox.Show("Cliente reactivado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void txtBusquedaDNI_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string busquedaEmail = txtBusquedaDNI.Text;

            if (string.IsNullOrWhiteSpace(busquedaEmail))
            {
                //Si la barra está vacía, muestra a todos los inactivos
                CargarInactivos();
            }
            else
            {
                dgClientesInactivos.ItemsSource = clienteRepo.BuscarInactivosPorDNI(busquedaEmail).DefaultView;
            }
        }

        private void dgClientesInactivos_KeyDown(object sender, KeyEventArgs e)
        {
            //Se comprueba si la tecla presionada fue "Enter"
            if (e.Key == Key.Enter)
            {
                //Si fue Enter, llama al mismo método que el botón "Reactivar"
                btnReactivar_Click(sender, e);
            }
            e.Handled = true;
        }
    }
}