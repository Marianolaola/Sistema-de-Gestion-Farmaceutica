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


        private void btnAgregarObraSocial_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as ClienteViewModel;
            if(vm.cliente.id_cliente == 0)
            {
                vm.GuardarCliente();
                // si sigue siendo 0, significa que no se guardó
                if (vm.cliente.id_cliente == 0) return;
            }
            vm.AgregarObraSocial();
            
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
