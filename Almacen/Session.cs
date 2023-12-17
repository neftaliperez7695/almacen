using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Almacen
{
    internal class Session
    {
        private static int usuarioID;

        public static int UsuarioID
        {
            get { return usuarioID; }
            set { usuarioID = value; }
        }

        public static void LimpiarSesion()
        {
            // Agrega cualquier lógica adicional para limpiar la sesión si es necesario
            usuarioID = 0;
        }
    }
}
