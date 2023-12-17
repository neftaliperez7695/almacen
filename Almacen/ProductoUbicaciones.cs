using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Almacen
{
    public partial class ProductoUbicaciones : Form
    {
        private const string connectionString = "Data Source=DESKTOP-GNTM77B\\SQLEXPRESS;Initial Catalog=AlmacenDB;Integrated Security=True";

        public ProductoUbicaciones()
        {
            InitializeComponent();
        }

        private void ProductoUbicaciones_Load(object sender, EventArgs e)
        {
            // TODO: esta línea de código carga datos en la tabla 'almacenDBDataSet5.Ubicaciones' Puede moverla o quitarla según sea necesario.
            this.ubicacionesTableAdapter.Fill(this.almacenDBDataSet5.Ubicaciones);
            // TODO: esta línea de código carga datos en la tabla 'almacenDBDataSet4.Productos' Puede moverla o quitarla según sea necesario.
            this.productosTableAdapter.Fill(this.almacenDBDataSet4.Productos);
            CargarDatosDataGridView();

        }

        private void CargarDatosDataGridView()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("ObtenerDetallesProductoUbicacion", connection))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dtgProdUbicaion.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            int cantidad;
            string productoId = cboProducto.SelectedValue.ToString();
            string ubicacionId = cboUbicacion.SelectedValue.ToString();

            if (!int.TryParse(txtCantidad.Text, out cantidad))
            {
                MessageBox.Show("Ingrese valores válidos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Lógica para agregar el producto a la base de datos
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("InsertarProductoUbicacion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Añadir parámetros
                        command.Parameters.AddWithValue("@ProductoID", productoId);
                        command.Parameters.AddWithValue("@UbicacionID", ubicacionId);
                        command.Parameters.AddWithValue("@Cantidad", cantidad);

                        command.ExecuteNonQuery();

                        MessageBox.Show("Producto agregado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarCampos();
                        CargarDatosDataGridView();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar el producto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            // Establecer el texto de los TextBoxes a una cadena vacía
            txtCantidad.Text = "";
            cboUbicacion.Text = "";
            cboProducto.Text = "";
        }
    }
}
