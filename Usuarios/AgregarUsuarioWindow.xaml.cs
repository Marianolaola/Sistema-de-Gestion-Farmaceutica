using System.Windows;
using System.Windows.Controls;

namespace Sistema_de_Gestión_Farmacéutica
{
    public partial class AgregarUsuarioWindow : Window
    {
        public string Nombre { get; private set; }
        public string Apellido { get; private set; }
        public string Contraseña { get; private set; }
        public string Rol { get; private set; }

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

            Nombre = txtNombre.Text;
            Apellido = txtApellido.Text;
            Contraseña = txtContraseña.Password;
            Rol = (cmbRol.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;

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
