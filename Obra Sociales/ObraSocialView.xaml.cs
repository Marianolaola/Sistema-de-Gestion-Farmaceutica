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
    public partial class ObraSocialView : UserControl
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
            AgregarObraSocial ventanaAgregar = new AgregarObraSocial();
            //muestra la ventana al usuario y espera hasta que el usuario la cierre
            bool? resultado = ventanaAgregar.ShowDialog();

            //Si apreta el boton "Agregar" (resultado == true)
            if(resultado == true)
            {
                // llama al repositorio para guardar la nueva obra social
                obrasrepo.AgregarObraSocial(ventanaAgregar.obraSocialCreada);
                //Refrescamos el DataGrid
                CargarObraSociales();
                MessageBox.Show("Obra Social agregada correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        //Boton de Editar una Obra Social
        private void btnEditarOS_Click(object sender, RoutedEventArgs e)
        {
            if(dgObrasSociales.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccione una obra social para editar.", "Selección requerida", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            //Obtenemos la fila del DataGrid
            DataRowView fila = (DataRowView)dgObrasSociales.SelectedItem;

            //Creamos un Objeto ObraSocial con los datos de la fila seleccionada
            ObraSocial obraSocialAEditar = new ObraSocial
            {
                id_obra_social =(int)fila["id_obra_social"],
                nombre = fila["nombre"].ToString()
            };

            //Abrimos la ventana de edición pasando el objeto
            EditarObraSocial ventana = new EditarObraSocial(obraSocialAEditar);
            bool? resultado = ventana.ShowDialog();

            if(resultado == true)
            {
                obrasrepo.EditarObraSocial(ventana.obraSocialEditada);
                //Refrescamos el DataGrid
                CargarObraSociales();
                MessageBox.Show("Obra social actualizada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void btnEliminarOS_Click(object sender, RoutedEventArgs e)
        {
            if (dgObrasSociales.SelectedItem == null)
            {
                MessageBox.Show("Seleccione una obra social para eliminar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
                    CargarObraSociales();
                    MessageBox.Show("Obra social eliminada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                }
                else
                {
                    MessageBox.Show("No se puede eliminar la obra social porque está asociada a uno o más clientes.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
