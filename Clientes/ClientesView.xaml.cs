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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sistema_de_Gestión_Farmacéutica.Clientes
{
    /// <summary>
    /// Lógica de interacción para ClientesView.xaml
    /// </summary>
    public partial class ClientesView : UserControl
    {
        private Cliente cliente = new Cliente();
        public ClientesView()
        {
            InitializeComponent();
            cargarCliente();
        }

        private void cargarCliente()
        {
            dgClientes.ItemsSource = cliente.ObtenerClientes().DefaultView;
        }
    }
}
