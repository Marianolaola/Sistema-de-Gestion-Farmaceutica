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
using System.Windows.Shapes;

namespace Sistema_de_Gestión_Farmacéutica
{
    /// <summary>
    /// Lógica de interacción para LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Entrar_Click(object sender, RoutedEventArgs e)
        {
            string nombreUsuario = txtUsuario.Text.Trim();
            string contraseña = txtContraseña.Password.Trim();

            if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(contraseña))
            {
                MessageBox.Show("Por favor, ingrese usuario y contraseña.", "Error de autenticación", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                Usuario servUsuario = new Usuario();

                // Intentar traer los usuarios desde la base de datos
                DataTable dt = servUsuario.ObtenerUsuarios();

                DataRow usuarioEncontrado = null;
                foreach (DataRow row in dt.Rows)
                {
                    if (row["nombre"].ToString() == nombreUsuario && row["contraseña"].ToString() == contraseña)
                    {
                        usuarioEncontrado = row;
                        break;
                    }
                }

                if (usuarioEncontrado == null)
                {
                    MessageBox.Show("Usuario o contraseña incorrectos.", "Error de autenticación", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string rol = usuarioEncontrado["rol"]?.ToString() ?? "";

                MainWindow mainWindow = new MainWindow(nombreUsuario, rol);
                mainWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                // Esto muestra el error si algo falla (por ejemplo, la conexión)
                MessageBox.Show("Error al conectarse a la base de datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
