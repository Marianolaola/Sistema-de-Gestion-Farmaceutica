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
    /// Lógica de interacción para AgregarObraSocial.xaml
    /// </summary>
    public partial class AgregarObraSocial : Window
    {
        public ObraSocial obraSocialCreada { get; private set; }
        public AgregarObraSocial()
        {
            InitializeComponent();
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre de la obra social no puede estar vacío.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

                obraSocialCreada = new ObraSocial
                {
                    nombre = txtNombre.Text
                };
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
