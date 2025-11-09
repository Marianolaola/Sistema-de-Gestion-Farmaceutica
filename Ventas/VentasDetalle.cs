using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Sistema_de_Gestión_Farmacéutica.Sesion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_de_Gestión_Farmacéutica.Ventas
{
    public class VentasDetalle
    {
        public int id_venta { get; set; }
        public DateTime fecha_venta { get; set; }
        public int id_cliente { get; set; }
        public int id_usuario { get; set; }

        public decimal total { get; set; }

        public VentasDetalle() { }

        public VentasDetalle(int id_venta, DateTime fecha_venta, int id_cliente, int id_usuario, decimal total)
        {
            this.id_venta = id_venta;
            this.fecha_venta = fecha_venta;
            this.id_cliente = id_cliente;
            this.id_usuario = id_usuario;
            this.total = total;
        }

    }
}


