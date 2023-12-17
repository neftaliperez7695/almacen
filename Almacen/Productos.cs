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
    public partial class Productos : Form
    {
        private const string connectionString = "Data Source=DESKTOP-GNTM77B\\SQLEXPRESS;Initial Catalog=AlmacenDB;Integrated Security=True";

        public Productos()
        {
            InitializeComponent();
        }

        private enum EstadoFormulario
        {
            Agregar,
            Editar
        }

        private EstadoFormulario estadoActual = EstadoFormulario.Agregar;

        private void ActualizarVisibilidadBotones()
        {
            // Ocultar o mostrar el botón Eliminar según el estado actual
            btnEliminar.Visible = estadoActual == EstadoFormulario.Editar;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CargarDatosDataGridView();
            estadoActual = EstadoFormulario.Agregar;
            ActualizarVisibilidadBotones();
        }

        private void dtGridProductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (estadoActual == EstadoFormulario.Agregar)
            {
                // Lógica para agregar un nuevo producto
                agregarProducto();
            }
            else if (estadoActual == EstadoFormulario.Editar)
            {
                // Lógica para editar el producto actual
                editarProducto();
            }

            LimpiarCampos();
            RestablecerEstado();
        }

        private void agregarProducto()
        {

            // Obtener los detalles del producto desde los TextBoxes
            string nombreProducto = txtNombre.Text.Trim();
            decimal precioProducto;
            int stockActualProducto;

            if (!decimal.TryParse(txtPrecio.Text, out precioProducto) ||
                !int.TryParse(txtStockActual.Text, out stockActualProducto))
            {
                MessageBox.Show("Ingrese valores válidos para precio y stock.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Lógica para agregar el producto a la base de datos
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO Productos (Nombre, Precio, Stock) VALUES (@Nombre, @Precio, @Stock)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nombre", nombreProducto);
                        command.Parameters.AddWithValue("@Precio", precioProducto);
                        command.Parameters.AddWithValue("@Stock", stockActualProducto);

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

        private void editarProducto()
        {
            // Verificar si hay un proveedor seleccionado
            if (dtGridProductos.SelectedRows.Count > 0)
            {
                // Obtener el ProveedorID de la fila seleccionada
                int productoID = Convert.ToInt32(dtGridProductos.SelectedRows[0].Cells["ProductoID"].Value);

                // Obtener los nuevos valores de los TextBox
                string nuevoNombre = txtNombre.Text;
                string nuevoPrecio = txtPrecio.Text;
                string nuevoStock = txtStockActual.Text;                

                // Crear la conexión
                using (SqlConnection xcon = new SqlConnection(connectionString))
                {
                    // Abrir la conexión
                    xcon.Open();

                    // Crear la consulta SQL de actualización
                    string query = "UPDATE Productos SET Nombre = @NuevoNombre, Precio = @NuevoPrecio, Stock = @NuevoStock WHERE ProductoID = @ProductoID";

                    // Crear el comando SQL
                    using (SqlCommand cmd = new SqlCommand(query, xcon))
                    {
                        // Asignar parámetros
                        cmd.Parameters.AddWithValue("@NuevoNombre", nuevoNombre);
                        cmd.Parameters.AddWithValue("@NuevoPrecio", nuevoPrecio);
                        cmd.Parameters.AddWithValue("@NuevoStock", nuevoStock);
                        cmd.Parameters.AddWithValue("@ProductoID", productoID);

                        // Ejecutar la consulta
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        // Verificar si la actualización fue exitosa
                        if (filasAfectadas > 0)
                        {
                            MessageBox.Show("Producto actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            CargarDatosDataGridView(); // Vuelve a cargar los datos en el DataGridView
                            LimpiarCampos(); // Limpia los campos después de la actualización
                            RestablecerEstado(); // Restablece el estado del formulario
                        }
                        else
                        {
                            MessageBox.Show("No se pudo actualizar el producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un producto para editar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RestablecerEstado()
        {
            estadoActual = EstadoFormulario.Agregar;
            btnAgregar.Text = "AGREGAR";

            ActualizarVisibilidadBotones();
        }

        private void CargarDatosDataGridView()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Productos";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dtGridProductos.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            // Establecer el texto de los TextBoxes a una cadena vacía
            txtNombre.Text = "";
            txtPrecio.Text = "";
            txtStockActual.Text = "";            
        }

        private void dtGridProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verificar si el índice de la celda no es -1 (fuera de los límites)
            if (e.RowIndex != -1)
            {
                estadoActual = EstadoFormulario.Editar;
                btnAgregar.Text = "EDITAR";

                string nombreProducto = dtGridProductos.Rows[e.RowIndex].Cells["Nombre"].Value.ToString();
                decimal precioProducto = Convert.ToDecimal(dtGridProductos.Rows[e.RowIndex].Cells["Precio"].Value);
                int stockActualProducto = Convert.ToInt32(dtGridProductos.Rows[e.RowIndex].Cells["Stock"].Value);

                // Establecer valores en los campos de texto
                txtNombre.Text = nombreProducto;
                txtPrecio.Text = precioProducto.ToString();
                txtStockActual.Text = stockActualProducto.ToString();

                ActualizarVisibilidadBotones();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            // Verificar si hay al menos una fila seleccionada
            if (dtGridProductos.SelectedRows.Count > 0)
            {
                int productoID = Convert.ToInt32(dtGridProductos.SelectedRows[0].Cells["ProductoID"].Value);
                // Confirmar la eliminación con un cuadro de diálogo
                DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar este Producto?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    // Llamar a la función que realiza la eliminación
                    EliminarProducto(productoID);

                    // Actualizar la vista después de la eliminación
                    CargarDatosDataGridView();
                    LimpiarCampos();
                    RestablecerEstado();
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un Producto para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EliminarProducto(int productoID)
        {
            // Crear la conexión
            using (SqlConnection xcon = new SqlConnection(connectionString))
            {
                // Abrir la conexión
                xcon.Open();

                // Crear la consulta SQL de eliminación
                string query = "DELETE FROM Productos WHERE ProductoID = @ProductoID";

                // Crear el comando SQL
                using (SqlCommand cmd = new SqlCommand(query, xcon))
                {
                    // Asignar parámetro
                    cmd.Parameters.AddWithValue("@ProductoID", productoID);

                    // Ejecutar la consulta
                    int filasAfectadas = cmd.ExecuteNonQuery();

                    // Verificar si la eliminación fue exitosa
                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Producto eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar el Producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            // Obtener el texto del TextBox de filtro
            string filtro = txtBuscar.Text;

            if (string.IsNullOrEmpty(filtro))
            {
                MessageBox.Show("Por favor, ingrese el nombre del producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Aplicar el filtro a la DataGridView
            FiltrarDataGridView(filtro);
        }

        private void FiltrarDataGridView(string filtro)
        {
            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                // Abrir la conexión
                conexion.Open();

                // Crear una consulta SQL parametrizada
                string consulta = "SELECT * FROM Productos WHERE Nombre LIKE @filtro";
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    // Agregar el parámetro del filtro
                    comando.Parameters.AddWithValue("@filtro", "%" + filtro + "%");

                    // Crear un adaptador de datos para ejecutar la consulta
                    using (SqlDataAdapter adaptador = new SqlDataAdapter(comando))
                    {
                        // Crear un nuevo conjunto de datos para almacenar los resultados
                        DataSet ds = new DataSet();

                        // Llenar el conjunto de datos con los resultados de la consulta
                        adaptador.Fill(ds, "Productos");

                        // Asignar el conjunto de datos como origen de datos para la DataGridView
                        dtGridProductos.DataSource = ds.Tables["Productos"];
                    }
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            // Limpiar el TextBox de búsqueda
            txtBuscar.Text = "";

            // Recargar todos los datos en el DataGridView
            CargarDatosDataGridView();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
