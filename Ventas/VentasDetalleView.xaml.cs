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

namespace Sistema_de_Gestión_Farmacéutica.Ventas
{
    /// <summary>
    /// Lógica de interacción para VentasDetalleView.xaml
    /// </summary>
    public partial class VentasDetalleView : UserControl
    {
        private VentasDetalle servVentaDetalle = new VentasDetalle();
        public VentasDetalleView()
        {
            InitializeComponent();
            CargarVentasConDetalle();
        }

        private void CargarVentasConDetalle()
        {
            dgVentasDetalle.ItemsSource = servVentaDetalle.ObtenerVentasConDetalle().DefaultView;
        }
    }
}
