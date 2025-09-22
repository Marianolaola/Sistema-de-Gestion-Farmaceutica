using Sistema_de_Gestión_Farmacéutica.Clientes;
using System.Windows;
using System.Windows.Controls;

namespace Sistema_de_Gestión_Farmacéutica
{
    public partial class MainWindow : Window
    {
        public MainWindow(string nombreUsuario, string rol)
        {
            InitializeComponent();

            txtUsuario.Text = nombreUsuario;
            txtRol.Text = rol;

            // Configurar visibilidad de botones según rol
            switch (rol)
            {
                case "Gerente":
                    btnUsuarios.Visibility = Visibility.Visible;
                    btnReportes.Visibility = Visibility.Visible;
                    btnMedicamentos.Visibility = Visibility.Visible;
                    btnVentas.Visibility = Visibility.Visible;
                    btnClientes.Visibility = Visibility.Visible;
                    break;
                case "Administrador":
                    btnUsuarios.Visibility = Visibility.Visible;
                    btnReportes.Visibility = Visibility.Collapsed;
                    btnMedicamentos.Visibility = Visibility.Visible;
                    btnVentas.Visibility = Visibility.Visible;
                    btnClientes.Visibility = Visibility.Visible;
                    break;
                case "Farmaceutico":
                    btnUsuarios.Visibility = Visibility.Collapsed;
                    btnReportes.Visibility = Visibility.Collapsed;
                    btnMedicamentos.Visibility = Visibility.Visible;
                    btnVentas.Visibility = Visibility.Visible;
                    btnClientes.Visibility = Visibility.Collapsed;
                    break;
            }

            // Abrir dashboard por defecto
            ContenidoPrincipal.Content = new TextBlock
            {
                Text = "Dashboard (Vista Prototipo)",
                FontSize = 24,
                Margin = new Thickness(20)
            };
        }

            private void AbrirMedicamentos_Click(object sender, RoutedEventArgs e)
            {
                ContenidoPrincipal.Content = new MedicamentosView()
                {
                    
                };
            }

            private void AbrirVentas_Click(object sender, RoutedEventArgs e)
            {
                ContenidoPrincipal.Content = new TextBlock
                {
                    Text = "Ventas (vista prototipo)",
                    FontSize = 20,
                    Margin = new Thickness(20)
                };
            }

        

        private void AbrirDashboard_Click(object sender, RoutedEventArgs e)
        {
            ContenidoPrincipal.Content = new TextBlock { Text = "Dashboard", FontSize = 24, Margin = new Thickness(20) };
        }

        private void AbrirUsuarios_Click(object sender, RoutedEventArgs e)
        {
            ContenidoPrincipal.Content = new UsuariosView();
            {
                
            };
        }

        private void AbrirReportes_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Reportes Abierto (Prototipo)");
        }

        private void AbrirClientes_Click(object sender, RoutedEventArgs e)
        {
            ContenidoPrincipal.Content = new ClientesView()
            {

            };
        }

        private void CerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();

            this.Close();

        }
    }
}
