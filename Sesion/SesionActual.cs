using Sistema_de_Gestión_Farmacéutica.Usuarios;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_de_Gestión_Farmacéutica.Sesion
{
    public static class SesionActual
    {
        public static Usuario Usuario { get; set; }

        public static void IniciarSesion(Usuario usuarioIniciado)
        {
            Usuario = usuarioIniciado;
        }

        public static void CerrarSesion()
        {
            Usuario = null;
        }

        public static bool estaLogueado()
        {
            return Usuario != null;
        }
    }
}
