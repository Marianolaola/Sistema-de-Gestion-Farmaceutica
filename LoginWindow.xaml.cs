using Sistema_de_Gestión_Farmacéutica.Usuarios;
using Sistema_de_Gestión_Farmacéutica.Sesion;
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
            string email = txtEmail.Text.Trim();
            string contraseña = txtContraseña.Password.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(contraseña))
            {
                MessageBox.Show("Por favor, ingrese usuario y contraseña.", "Error de autenticación", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                UsuarioRepositorio repositorio = new UsuarioRepositorio();

                // Intentar traer los usuarios desde la base de datos
                DataTable dt = repositorio.ObtenerUsuarios();

                // Busca al usuario por email y contraseña
                DataRow usuarioEncontrado = dt.AsEnumerable().FirstOrDefault(row =>
                    row.Field<string>("email").Equals(email, StringComparison.OrdinalIgnoreCase) 
                    && row.Field<string>("contraseña") == contraseña);

                if (usuarioEncontrado == null)
                {
                    MessageBox.Show("Usuario o contraseña incorrectos.", "Error de autenticación", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // setteando los valores de la sesión iniciada
                SesionActual.Usuario = new Usuario
                        {
                            id_usuario = (int)usuarioEncontrado["id_usuario"],
                            nombre = usuarioEncontrado["nombre"].ToString(),
                            apellido = usuarioEncontrado["apellido"].ToString(),
                            contraseña = usuarioEncontrado["contraseña"].ToString(),
                            rol = usuarioEncontrado["rol"].ToString(),
                            email = usuarioEncontrado["email"].ToString()
                        };

                MainWindow mainWindow = new MainWindow(SesionActual.Usuario.nombre + " " + SesionActual.Usuario.apellido, SesionActual.Usuario.rol);
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
