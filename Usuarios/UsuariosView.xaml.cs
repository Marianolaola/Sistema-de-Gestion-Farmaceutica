using Sistema_de_Gestión_Farmacéutica.Usuarios;
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
            string rolUsuario = filaSeleccionada["rol"].ToString();

            if(idUsuario == Usuario.id_usuario)
            {
                MessageBox.Show("La sesión de este usuario está activa, no es posible eliminarlo","Error al borrar", MessageBoxButton.OK, MessageBoxImage.Warning );
            } else {

                if (Usuario.rol == "Administrador" && rolUsuario == "Gerente")
                {
                    MessageBox.Show("Este usuario no cuenta con los permisos para eliminar esta cuenta", "Error al borrar", MessageBoxButton.OK, MessageBoxImage.Warning);
                } else {
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

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsuarios.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un usuario para editar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView filaSeleccionada = (DataRowView)dgUsuarios.SelectedItem;
            int idUsuario = Convert.ToInt32(filaSeleccionada["id_usuario"]);
            string rolUsuarioSeleccionado = filaSeleccionada["rol"].ToString();

            // Bloquear si un Administrador quiere editar un Gerente
            if (Usuario.rol == "Administrador" && rolUsuarioSeleccionado == "Gerente")
            {
                MessageBox.Show("No tiene permisos para modificar usuarios con rol Gerente.", "Acceso denegado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            EditarUsuarioWindow editarUsuario = new EditarUsuarioWindow();
            editarUsuario.txtNombre.Text = filaSeleccionada["nombre"].ToString();
            editarUsuario.txtApellido.Text = filaSeleccionada["apellido"].ToString();
            editarUsuario.txtContraseña.Password = filaSeleccionada["contraseña"].ToString();

            string rolActual = filaSeleccionada["rol"].ToString();

            bool? resultado = editarUsuario.ShowDialog();

            if (resultado == true)
            {
                string rolAct = filaSeleccionada["rol"].ToString();

                servUsuario.EditarUsuario(idUsuario,
                                          editarUsuario.Nombre,
                                          editarUsuario.Apellido,
                                          editarUsuario.Contraseña,
                                          rolAct);
                cargarUsuario();
                MessageBox.Show("Usuario actualizado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

    }

    
}
