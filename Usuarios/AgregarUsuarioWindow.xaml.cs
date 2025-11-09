using Sistema_de_Gestión_Farmacéutica.Usuarios;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Sistema_de_Gestión_Farmacéutica
{
    public partial class AgregarUsuarioWindow : Window
    {
        public Usuario usuarioCreado { get; private set; }
        public AgregarUsuarioWindow(string rolUsuarioActual)
        {
            InitializeComponent();
            RolesPermitidos(rolUsuarioActual);
        }

        private void RolesPermitidos(string rol)
        {
            cmbRol.Items.Clear();

            if (rol == "Administrador")
            {
                cmbRol.Items.Add(new ComboBoxItem { Content = "Administrador" });
                cmbRol.Items.Add(new ComboBoxItem { Content = "Gerente" });
                cmbRol.Items.Add(new ComboBoxItem { Content = "Farmaceutico" });
            }
            else if (rol == "Gerente")
            {
                cmbRol.Items.Add(new ComboBoxItem { Content = "Farmaceutico" });
            }
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtContraseña.Password) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                cmbRol.SelectedItem == null)
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(!Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("El email debe ser un correo válido (ej: usuario@dominio.com)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            usuarioCreado = new Usuario
            {
                nombre = txtNombre.Text,
                apellido = txtApellido.Text,
                contraseña = txtContraseña.Password,
                email = txtEmail.Text,
                rol = (cmbRol.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty
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
