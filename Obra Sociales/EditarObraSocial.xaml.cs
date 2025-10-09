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
using System.Windows.Shapes;

namespace Sistema_de_Gestión_Farmacéutica.Obra_Sociales
{
    /// <summary>
    /// Lógica de interacción para EditarObraSocial.xaml
    /// </summary>
    public partial class EditarObraSocial : Window
    {
        public ObraSocial obraSocialEditada { get; private set; }
        public EditarObraSocial(ObraSocial obraEditada)
        {
            InitializeComponent();
            Inicializar(obraEditada);
        }

        public void Inicializar(ObraSocial obrasocial)
        {
            txtNombre.Text = obrasocial.nombre;
            obraSocialEditada = obrasocial;
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre de la obra social no puede estar vacío.");
                return;
            }

            obraSocialEditada.nombre = txtNombre.Text.Trim();

            this.DialogResult = true;
            this.Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
