using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Sistema_de_Gestión_Farmacéutica.Clientes
{
    /// <summary>
    /// Lógica de interacción para AgregarCliente.xaml
    /// </summary>
    public partial class AgregarCliente : Window
    {
        public Cliente clienteCreado { get; private set; }
        public AgregarCliente()
        {
            InitializeComponent();
        }
        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
               string.IsNullOrWhiteSpace(txtApellido.Text) ||
               string.IsNullOrWhiteSpace(txtEmail.Text) ||
               string.IsNullOrEmpty(txtDNI.Text) ||
               string.IsNullOrEmpty(txtTelefono.Text) ||
               string.IsNullOrEmpty(txtDireccion.Text))
            {
                MessageBox.Show("Todos los campos son obligatoios", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            //Validar Fecha de Nacimiento
            if (!txtFecha.SelectedDate.HasValue)
            {
                MessageBox.Show("La fecha de nacimiento es obligatoria", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //validar Emial
            if (!Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("El email debe ser un correo válido -> (usuario@gmail.com)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(!Regex.IsMatch(txtDNI.Text, @"^\d+$"))
            {
                MessageBox.Show("El DNI debe contener solo números.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Regex.IsMatch(txtTelefono.Text, @"^\d+$"))
            {
                MessageBox.Show("El Telefono debe contener solo números.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            clienteCreado = new Cliente
             {
                nombre = txtNombre.Text,
                apellido = txtApellido.Text,
                email = txtEmail.Text,
                dni = txtDNI.Text,
                telefono = txtTelefono.Text,
                direccion = txtDireccion.Text,
                fecha_nacimiento = txtFecha.SelectedDate.Value
            };

            this.DialogResult = true;
            this.Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
