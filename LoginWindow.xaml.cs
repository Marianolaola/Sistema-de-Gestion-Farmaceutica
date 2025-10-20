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
using BCrypt;

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

                //traemos al usuario con su email
                Usuario usuarioEncontrado = repositorio.obtenerUsuarioPorEmail(email);

                if(usuarioEncontrado != null && BCrypt.Net.BCrypt.Verify(contraseña, usuarioEncontrado.contraseña)){

                    // setteando los valores de la sesión iniciada
                    SesionActual.Usuario = usuarioEncontrado;
                    MainWindow mainWindow = new MainWindow(SesionActual.Usuario.nombre + " " + SesionActual.Usuario.apellido, SesionActual.Usuario.rol);
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    // Usuario no encontrado o contraseña incorrecta
                    MessageBox.Show("Usuario o contraseña incorrectos.", "Error de autenticación", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
            }
            catch (Exception ex)
            {
                // Esto muestra el error si algo falla (por ejemplo, la conexión)
                MessageBox.Show("Error al conectarse a la base de datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

    }


}
