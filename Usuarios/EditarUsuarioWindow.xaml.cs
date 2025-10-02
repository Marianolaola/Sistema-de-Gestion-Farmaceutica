using System;
using System.Collections.Generic;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Sistema_de_Gestión_Farmacéutica.Usuarios
{
    
    /// Lógica de interacción para EditarUsuarioWindow.xaml

    public partial class EditarUsuarioWindow : Window
    {
        public Usuario usuarioEditado { get; private set; }
        public EditarUsuarioWindow()
        {
            InitializeComponent();
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtContraseña.Password) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Todos los campos son obligatorios.");
                return;
            }

            usuarioEditado = new Usuario
            {
                nombre = txtNombre.Text,
                apellido = txtApellido.Text,
                contraseña = txtContraseña.Password,
                email = txtEmail.Text
            };
            

            this.DialogResult = true;
            this.Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }


        private void SoloLetras_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!e.Text.All(c => char.IsLetter(c) || c == ' '))
            {
                MessageBox.Show("Solo se permiten letras", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                e.Handled = true;
            }
        }

        private void txtContraseña_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!e.Text.All(c => char.IsLetterOrDigit(c)))
            {
                MessageBox.Show("No se permiten espacios", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                e.Handled = true;
            }
        }
    }
}
