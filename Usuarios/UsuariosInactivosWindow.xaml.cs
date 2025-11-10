using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sistema_de_Gestión_Farmacéutica.Usuarios
{
    public partial class UsuariosInactivosWindow : Window
    {
        private UsuarioRepositorio servUsuario = new UsuarioRepositorio();
        public bool UsuarioReactivado { get; private set; } = false;

        public UsuariosInactivosWindow()
        {
            InitializeComponent();
            CargarInactivos();
        }

        private void CargarInactivos()
        {
            dgUsuariosInactivos.ItemsSource = servUsuario.ObtenerUsuariosInactivos().DefaultView;
        }

        private void txtBusquedaEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            string textoBusqueda = txtBusquedaEmail.Text;

            if (string.IsNullOrWhiteSpace(textoBusqueda))
            {
                // Si la barra está vacía, muestra todos los inactivos
                CargarInactivos();
            }
            else
            {
                // Si hay texto, filtra la lista en vivo
                dgUsuariosInactivos.ItemsSource = servUsuario.BuscarInactivosPorEmail(textoBusqueda).DefaultView;
            }
        }

        private void btnReactivar_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsuariosInactivos.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccione un usuario para reactivar.", "Selección requerida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView fila = (DataRowView)dgUsuariosInactivos.SelectedItem;
            int idUsuario = (int)fila["id_usuario"];

            var confirmar = MessageBox.Show($"¿Está seguro que desea reactivar al usuario '{fila["nombre"]} {fila["apellido"]}'?",
                                            "Confirmar Reactivación", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmar == MessageBoxResult.Yes)
            {
                servUsuario.ReactivarUsuario(idUsuario);
                UsuarioReactivado = true;
                CargarInactivos();
                MessageBox.Show("Usuario reactivado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
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