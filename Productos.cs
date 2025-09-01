using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_de_Gestión_Farmacéutica
{
    public class Productos
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public string? Principio_Activo { get; set; }
        public int Stock_Minimo { get; set; }
        public DateTime Fecha_Vencimiento { get; set; }

    }
}
