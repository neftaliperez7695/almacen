using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

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
            AbrirFormHija(new Productos());
        }

        private void btnProveedor_Click(object sender, EventArgs e)
        {
            AbrirFormHija(new Proveedores());            
        }

        private void btnSalida_Click(object sender, EventArgs e)
        {
            AbrirFormHija(new Salida());
        }

        private void btnIngreso_Click(object sender, EventArgs e)
        {
            AbrirFormHija(new Entrada());
        }

        private void btnUbicacion_Click(object sender, EventArgs e)
        {
            AbrirFormHija(new Ubicacion());
        }

        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            AbrirFormHija(new Usuarios());
        }

        private void barraTitulo_Paint(object sender, PaintEventArgs e)
        {

        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void barraTitulo_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void AbrirFormHija(object formHija)
        {
            if (this.panelControl.Controls.Count > 0)
            {
                this.panelControl.Controls.RemoveAt(0);                
            }
            Form hj = formHija as Form;
            hj.TopLevel = false;
            hj.Dock = DockStyle.Fill;
            this.panelControl.Controls.Add(hj);
            this.panelControl.Tag = hj;
            hj.Show();
        }

        private void btnUbicaciones_Click(object sender, EventArgs e)
        {
            AbrirFormHija(new ProductoUbicaciones());
        }

        private void panelControl_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Principal_Load(object sender, EventArgs e)
        {
            
        }

        private void Principal_Load_1(object sender, EventArgs e)
        {
            btnProveedor_Click(null, e);
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
