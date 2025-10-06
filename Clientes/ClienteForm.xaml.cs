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
    /// Lógica de interacción para ClienteForm.xaml
    /// </summary>
    /// 

    public partial class ClienteForm : UserControl
    {
        private ClienteViewModel viewModel;

        public ClienteForm()   // constructor sin parámetros
        {
            InitializeComponent();
        }

        public ClienteForm(Cliente cliente = null)
        {
            InitializeComponent();
            viewModel = new ClienteViewModel(cliente);
            DataContext = viewModel;
            
        }

        private void btnAgregarObraSocial_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as ClienteViewModel;
            vm?.AgregarObraSocial();
            
        }

        private void btnEliminarObraSocial_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as ClienteViewModel;
            if(listaObrasSociales.SelectedItem is ClienteObraSocial obraSeleccionada)
            {
                vm?.EliminarObraSocial(obraSeleccionada);
            }
        }
    }
}
