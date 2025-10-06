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

namespace Sistema_de_Gestión_Farmacéutica.Clientes
{
    /// <summary>
    /// Lógica de interacción para ClienteWindow.xaml
    /// </summary>
    public partial class ClienteWindow : Window
    {
        public ClienteWindow(Cliente cliente = null)
        {
            InitializeComponent();
            this.DataContext = new ClienteViewModel(cliente);
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as ClienteViewModel;
            vm?.GuardarCliente();
            this.Close();

            

        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as ClienteViewModel;
            vm?.CancelarCliente(this);
        }

    }
}
