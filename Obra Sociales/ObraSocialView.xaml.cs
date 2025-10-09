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
using System.Windows.Shapes;

namespace Sistema_de_Gestión_Farmacéutica.Obra_Sociales
{
    /// <summary>
    /// Lógica de interacción para ObraSocialView.xaml
    /// </summary>
    public partial class ObraSocialView : Window
    {
        private ObrasRepositorio obrasrepo = new ObrasRepositorio();
        public ObraSocialView()
        {
            InitializeComponent();
            CargarObraSociales();
        }

        // Cargar todas las Obras Sociales en el DataGrid
        private void CargarObraSociales()
        {
            dgObrasSociales.ItemsSource = obrasrepo.ObtenerObraSociales().DefaultView;
        }

        //Boton de Agregar una Obra Social
        private void btnAgregarOS_Click(object sender, RoutedEventArgs e)
        {


        }

        //Boton de Editar una Obra Social
        private void btnEditarOS_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEliminarOS_Click(object sender, RoutedEventArgs e)
        {
            if (dgObrasSociales.SelectedItem == null)
            {
                MessageBox.Show("Seleccione una obra social para eliminar.");
                return;
            }

            DataRowView fila = (DataRowView)dgObrasSociales.SelectedItem;
            int idObraSocial = Convert.ToInt32(fila["id_obra_social"]);


            var confirmar = MessageBox.Show("¿Está seguro que desea eliminar la obra social seleccionada?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirmar == MessageBoxResult.Yes)
            {
                bool eliminado = obrasrepo.EliminarObraSocial(idObraSocial);
                if (eliminado)
                {
                    MessageBox.Show("Obra social eliminada correctamente.");
                    CargarObraSociales();
                }
                else
                {
                    MessageBox.Show("No se puede eliminar la obra social porque está asociada a uno o más clientes.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
