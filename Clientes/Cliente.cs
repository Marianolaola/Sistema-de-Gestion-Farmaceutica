using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_de_Gestión_Farmacéutica.Clientes
{
    public class Cliente
    {   
        public int id_cliente { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string dni { get; set; }
        public DateTime fecha_nacimiento { get; set; }
        public string telefono { get; set; }
        public string direccion { get; set; }
        public string email { get; set; }

        public Cliente() { }
        public Cliente(int id_cliente, string nombre, string apellido, string dni, DateTime fecha_nacimiento, string telefono, string direccion, string email)
        {
            this.id_cliente = id_cliente;
            this.nombre = nombre;
            this.apellido = apellido;
            this.dni = dni;
            this.fecha_nacimiento = fecha_nacimiento;
            this.telefono = telefono;
            this.direccion = direccion;
            this.email = email;
        }

        public int Edad()
        {
            var today = DateTime.Today;
            var edad = DateTime.Today.Year - fecha_nacimiento.Year;
            if (fecha_nacimiento.Date > today.AddYears(-edad)) edad--;
            return edad;
        }
        
    }
}
