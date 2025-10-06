using Sistema_de_Gestión_Farmacéutica.Clientes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Text;

namespace Sistema_de_Gestión_Farmacéutica.Clientes
{
    public class ClienteViewModel
    {
        public bool esEdicion { get; set; }
        public string Titulo { get; set; }

        public Cliente cliente { get; set; }

        // todas las obra sociales disponibles
        public List<ObraSocial> obrasSociales { get; set; }

        //Las obras sociales que el cliente tiene asignadas
        public List<ClienteObraSocial> obrasSocialesAsignadas { get; set; }

        //Obra social seleccionada en el ComboBox para agregar
        public ObraSocial obraSocialSeleccionada { get; set; }

        protected ClienteRepositorio repo = new ClienteRepositorio();
        //Constructor
        public ClienteViewModel(Cliente clienteExistente = null)
        {
            if (clienteExistente != null)
            {
                //Cliente existente: modo editar
                esEdicion = true;
                Titulo = "Editar Cliente";
                cliente = clienteExistente;
                //inicializamos las obras sociales
                obrasSocialesAsignadas = repo.ObtenerObrasSocialesPorCliente(clienteExistente.id_cliente);
                    
            }
            else
            {
                //Cliente nuevo: modo agregar
                esEdicion = false;
                Titulo = "Agregar Cliente";
                cliente = new Cliente();
                obrasSocialesAsignadas = new List<ClienteObraSocial>();

            }
            obrasSociales = repo.CargarObrasSociales();

        }

       
        public void AgregarObraSocial()
        {
            if(obraSocialSeleccionada == null) { return; }

            //Verificamos que no esté ya asignada
            if(obrasSocialesAsignadas.Any(os => os.id_obra_social == obraSocialSeleccionada.id_obra_social))
            {
                MessageBox.Show("La obra social ya está asignada al cliente.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //Si no está asignada, la agregamos
            repo.AgregarObraSocialACliente(cliente.id_cliente, obraSocialSeleccionada.id_obra_social);
            //Refrescamos la lista
            obrasSocialesAsignadas = repo.ObtenerObrasSocialesPorCliente(cliente.id_cliente);
        }

        public void EliminarObraSocial(ClienteObraSocial obra)
        {
            if (obra == null) { return; }
            repo.EliminarObraSocialACliente(cliente.id_cliente, obra.id_obra_social);
            obrasSocialesAsignadas = repo.ObtenerObrasSocialesPorCliente(cliente.id_cliente);
        }


        public void GuardarCliente()
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(cliente.nombre) ||
                string.IsNullOrWhiteSpace(cliente.apellido) ||
                string.IsNullOrWhiteSpace(cliente.dni) ||
                string.IsNullOrWhiteSpace(cliente.direccion) ||
                string.IsNullOrWhiteSpace(cliente.telefono) ||
                string.IsNullOrWhiteSpace(cliente.email))
            {
                MessageBox.Show("Completa los campos obligatorios.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cliente.fecha_nacimiento == null)
            {
                MessageBox.Show("Debes ingresar la fecha de nacimiento.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Guardar según sea nuevo o edición
            if (esEdicion)
                repo.EditarCliente(cliente);
            else
                repo.AltaCliente(cliente);
        }

        public void CancelarCliente(Window ventana)
        {
            ventana?.Close();
        }
    }            

}

        

        

    
