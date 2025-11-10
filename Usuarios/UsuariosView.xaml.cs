using Sistema_de_Gestión_Farmacéutica.Sesion;
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
        private UsuarioRepositorio servUsuario = new UsuarioRepositorio();
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
            AgregarUsuarioWindow agregarUsuario = new AgregarUsuarioWindow(SesionActual.Usuario.rol);
            bool? resultado = agregarUsuario.ShowDialog();

            if (resultado == true)
            {
                servUsuario.AltaUsuario(agregarUsuario.usuarioCreado);
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

            Usuario usuarioActual = SesionActual.Usuario;

            // REGLA N°1: No se puede el usuario auto-eliminar
            if (usuarioActual.id_usuario == idUsuario)
            {
                MessageBox.Show("La sesión de este usuario está activa, no es posible eliminarlo", "Error al borrar", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // REGLA N°2: Un Gerente solo puede eliminar farmacéuticos

            if(usuarioActual.rol == "Gerente")
            {
                if(rolUsuario == "Gerente" || rolUsuario == "Administrador")
                {
                    MessageBox.Show("No tiene permisos para eliminar usuarios de este nivel.", "Acceso denegado", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            // REGLA N°3: El administrador puede borrar cualquier rol (ya implícito)

            //Si cumple con las validaciones, se procede a eliminar

            // Se crea una variable de tipo Usuario para pasar el id al método de BajaUsuario
                    var usuarioAEliminar = new Usuario
                    {
                        id_usuario = idUsuario
                    };

                    // Se confirma antes de eliminar
                    var resultado = MessageBox.Show($"¿Está seguro que desea eliminar al usuario {filaSeleccionada["nombre"]} {filaSeleccionada["apellido"]}?",
                                                    "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (resultado == MessageBoxResult.Yes)
                    {
                        servUsuario.BajaUsuario(usuarioAEliminar);
                        cargarUsuario();
                        MessageBox.Show("Usuario eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
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


            Usuario usuarioActual = SesionActual.Usuario;

            //Regla N°1: Un Gerente solo puede editar farmaceuticos
            if(usuarioActual.rol == "Gerente")
            {
                if(rolUsuarioSeleccionado == "Gerente" || rolUsuarioSeleccionado == "Administrador")
                {
                    MessageBox.Show("No tienes permisos para modificar usuarios de este nivel", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            //Regla N°2: Un usuario no puede modificarse a si mismo desde su pantalla
            //Evita cambio de rol accidentalmente

            if(usuarioActual.id_usuario == idUsuario)
            {
                MessageBox.Show("No puede editar su propia cuenta desde esta vista.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            //Regla N°3: El administrador puede modificar todos los roles (ya implícita)
            
            //Si cumple con las validaciones, se abre la ventana
            EditarUsuarioWindow editarUsuario = new EditarUsuarioWindow(usuarioActual.rol);
            editarUsuario.txtNombre.Text = filaSeleccionada["nombre"].ToString();
            editarUsuario.txtApellido.Text = filaSeleccionada["apellido"].ToString();
            editarUsuario.txtEmail.Text = filaSeleccionada["email"].ToString();
            editarUsuario.cmbRol.Text = filaSeleccionada["rol"].ToString();

            bool? resultado = editarUsuario.ShowDialog();

            if (resultado == true)
            {

                Usuario usuarioEditado = new Usuario
                {
                    id_usuario = idUsuario,
                    nombre = editarUsuario.usuarioEditado.nombre,
                    contraseña = editarUsuario.usuarioEditado.contraseña,
                    apellido = editarUsuario.usuarioEditado.apellido,
                    email = editarUsuario.usuarioEditado.email,
                    rol = editarUsuario.usuarioEditado.rol
                };

                servUsuario.EditarUsuario(usuarioEditado);
                cargarUsuario();
                MessageBox.Show("Usuario actualizado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnVerInactivos_Click(object sender, RoutedEventArgs e)
        {
            Usuarios.UsuariosInactivosWindow ventanaHistorial = new Usuarios.UsuariosInactivosWindow();
            ventanaHistorial.ShowDialog(); // abre la ventana

            // si un usuario fue reactivado, actualizamos la vista principal
            if (ventanaHistorial.UsuarioReactivado)
            {
                cargarUsuario();
            }
        }

        private void txtBusquedaEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            string textoBusqueda = txtBusquedaEmail.Text;

            if (string.IsNullOrWhiteSpace(textoBusqueda))
            {
                // Si la barra está vacía, muestra todos los usuarios
                cargarUsuario();
            }
            else
            {
                // Si hay texto, filtra la lista en vivo
                dgUsuarios.ItemsSource = servUsuario.BuscarUsuariosActivosPorEmail(textoBusqueda).DefaultView;
            }
        }

    }
    
}
