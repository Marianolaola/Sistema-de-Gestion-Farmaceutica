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
    /// Lógica de interacción para EditarCliente.xaml
    /// </summary>
    public partial class EditarCliente : Window
    {
        public Cliente clienteEditado { get; private set; }
        public EditarCliente(Cliente cliente)
        {
            InitializeComponent();
            Inicializar(cliente);

        }

        private void Inicializar(Cliente cliente)
        {
            txtNombre.Text = cliente.nombre;
            txtApellido.Text = cliente.apellido;
            txtDNI.Text = cliente.dni;
            txtEmail.Text = cliente.email;
            txtTelefono.Text = cliente.telefono;
            txtDireccion.Text = cliente.direccion;
            txtFecha.SelectedDate = cliente.fecha_nacimiento;
            clienteEditado = cliente;
        }


        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            //validar Emial
            if (!Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("El email debe ser un correo válido -> (usuario@gmail.com)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Regex.IsMatch(txtTelefono.Text, @"^\d+$"))
            {
                MessageBox.Show("El Telefono debe contener solo números.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            clienteEditado.nombre = txtNombre.Text;
            clienteEditado.apellido = txtApellido.Text;
            clienteEditado.email = txtEmail.Text;
            clienteEditado.telefono = txtTelefono.Text;
            clienteEditado.direccion = txtDireccion.Text;

            this.DialogResult = true;
            this.Close();

        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
