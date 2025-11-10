using System.Data;
using Microsoft.Data.SqlClient;

namespace Sistema_de_Gestión_Farmacéutica.Usuarios
{
    public class Usuario
    {  
        public int id_usuario { get;  set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string email { get; set; } 
        public string contraseña { get; set; }
        public string rol { get; set; }

        public bool activo { get; set; }

        public Usuario() { }

        public Usuario(int id_usuario, string nombre, string apellido, string email, string contraseña, string rol, bool activo)
        {
            this.id_usuario = id_usuario;
            this.nombre = nombre;
            this.apellido = apellido;
            this.email = email;
            this.contraseña = contraseña;
            this.rol = rol;
            this.activo = activo;
        }
    }
}
