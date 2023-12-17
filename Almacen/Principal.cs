using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Almacen
{
    public partial class Principal : Form
    {
        public Principal()
        {
            InitializeComponent();
        }

        private void btnProductos_Click(object sender, EventArgs e)
        {
            Productos form2 = new Productos();
            form2.Show();
        }

        private void btnProveedor_Click(object sender, EventArgs e)
        {
            Proveedores form2 = new Proveedores();
            form2.Show();
        }

        private void btnSalida_Click(object sender, EventArgs e)
        {
            Salida form2 = new Salida();
            form2.Show();
        }

        private void btnIngreso_Click(object sender, EventArgs e)
        {
            Entrada form2 = new Entrada();
            form2.Show();
        }

        private void btnUbicacion_Click(object sender, EventArgs e)
        {
            Ubicacion form2 = new Ubicacion();
            form2.Show();
        }

        private void btnUsuarios_Click(object sender, EventArgs e)
        {

        }
    }
}
