using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Sistema_de_Gestión_Farmacéutica.Clientes
{
    public class ClienteObraSocial
    {
        public int id_cliente { get; set; }
        public int id_obra_social { get; set; }
        public int nro_afiliado { get; set; }

        public string nombre_obra_social { get; set; }


    }

       
}
