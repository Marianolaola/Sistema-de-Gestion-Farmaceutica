using System;
using System.Data;
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
    /// Lógica de interacción para MisVentas.xaml
    /// </summary>
    public partial class MisVentas : UserControl
    {
        private VentaDetalleRepositorio ventas = new VentaDetalleRepositorio();
        Factura factura = new Factura();

        public MisVentas()
        {
            InitializeComponent();
            CargarVentas();
        }

        private void CargarVentas()
        {
            dgVentas.ItemsSource = ventas.ObtenerVentasConDetalle().DefaultView;
        }

        private void btn_ImprimirClick(object sender, RoutedEventArgs e)
        {
            if (dgVentas.SelectedItem == null)
            {
                MessageBox.Show("Seleccione una venta para imprimir.");
                return;
            }

            var filaSeleccionada = dgVentas.SelectedItem as DataRowView;

            int idVenta = Convert.ToInt32(filaSeleccionada["id_venta"]);
            string nombreCliente = filaSeleccionada["nombre_completo"].ToString();
            DateTime fechaVenta = Convert.ToDateTime(filaSeleccionada["fecha_venta"]);

            VentaDetalleRepositorio repo = new VentaDetalleRepositorio();
            DataTable dtDetalles = repo.ObtenerDetallePorIdVenta(idVenta);

            List<DetalleItem> detalles = dtDetalles.AsEnumerable().Select(row => new DetalleItem 
            {
                Nombre = row["Medicamento"].ToString(),
                Cantidad = Convert.ToInt32(row["cantidad"]),
                Precio = Convert.ToDecimal(row["Precio_Unitario"]),
                Subtotal = Convert.ToDecimal(row["subtotal"])
            }).ToList();

            factura.GenerarFacturaPDF(nombreCliente, fechaVenta, detalles);

        }
    }
}
