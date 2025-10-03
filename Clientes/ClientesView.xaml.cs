using Microsoft.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sistema_de_Gestión_Farmacéutica.Clientes
{
    /// <summary>
    /// Lógica de interacción para ClientesView.xaml
    /// </summary>
    public partial class ClientesView : UserControl
    {

        private ClienteRepositorio clienteRepo = new ClienteRepositorio();
        public ClientesView()
        {
            InitializeComponent();
            CargarClientes();
            CargarObrasSociales();
        }

        private void CargarClientes()
        {
            dgClientes.ItemsSource = clienteRepo.ObtenerClientes().DefaultView;
        }

        private void CargarObrasSociales()
        {
            DataTable obrasSociales = clienteRepo.CargarObrasSociales();

            cmbObraSocial.ItemsSource = obrasSociales.DefaultView;
            cmbObraSocial.DisplayMemberPath = "nombre";
            cmbObraSocial.SelectedValuePath = "id_obra_social";
            cmbObraSocial.SelectedValue = 0;
        }

        private void cmbObraSocial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbObraSocial.SelectedValue == null) return;

            int idObraSocial = Convert.ToInt32(cmbObraSocial.SelectedValue);
            dgClientes.ItemsSource = clienteRepo.ObtenerClientesPorObraSocial(idObraSocial).DefaultView;
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            AgregarCliente ventana = new AgregarCliente();
            bool? resultado = ventana.ShowDialog();

            if (resultado == true)
            {
                Cliente 
            }
        }
    }
}

