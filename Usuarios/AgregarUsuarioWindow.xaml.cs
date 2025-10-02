using Sistema_de_Gestión_Farmacéutica.Usuarios;
using System.Windows;
using System.Windows.Controls;

namespace Sistema_de_Gestión_Farmacéutica
{
    public partial class AgregarUsuarioWindow : Window
    {
        public Usuario usuarioCreado { get; private set; }
        public AgregarUsuarioWindow()
        {
            InitializeComponent();
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtContraseña.Password) ||
                cmbRol.SelectedItem == null)
            {
                MessageBox.Show("Todos los campos son obligatorios.");
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
