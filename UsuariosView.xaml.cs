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

namespace Sistema_de_Gestión_Farmacéutica
{
    /// <summary>
    /// Lógica de interacción para UsuariosView.xaml
    /// </summary>
    public partial class UsuariosView : UserControl
    {
        private Usuario servUsuario = new Usuario();
        public UsuariosView()
        {
            InitializeComponent();
            cargarUsuario();
        }


        private void cargarUsuario()
        {
            dgUsuarios.ItemsSource = servUsuario.ObtenerUsuarios().DefaultView;
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            AgregarUsuarioWindow agregarUsuario = new AgregarUsuarioWindow();
            bool? resultado = agregarUsuario.ShowDialog();

            if (resultado == true)
            {
                servUsuario.AltaUsuario(agregarUsuario.Nombre,
                                        agregarUsuario.Apellido,
                                        agregarUsuario.Contraseña,
                                        agregarUsuario.Rol);
                cargarUsuario();
            }
            
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsuarios.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un usuario para eliminar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView filaSeleccionada = (DataRowView)dgUsuarios.SelectedItem;
            int idUsuario = Convert.ToInt32(filaSeleccionada["id_usuario"]);

            // Confirmación antes de eliminar
            var resultado = MessageBox.Show($"¿Está seguro que desea eliminar al usuario {filaSeleccionada["nombre"]} {filaSeleccionada["apellido"]}?",
                                            "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (resultado == MessageBoxResult.Yes)
            {
                servUsuario.BajaUsuario(idUsuario);
                cargarUsuario();
                MessageBox.Show("Usuario eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
