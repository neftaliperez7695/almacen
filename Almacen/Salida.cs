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
    public partial class Salida : Form
    {
        private const string connectionString = "Data Source=DESKTOP-GNTM77B\\SQLEXPRESS;Initial Catalog=AlmacenDB;Integrated Security=True";

        public Salida()
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
        private void Salida_Load(object sender, EventArgs e)
        {
            // TODO: esta línea de código carga datos en la tabla 'almacenDBDataSet3.Productos' Puede moverla o quitarla según sea necesario.
            this.productosTableAdapter.Fill(this.almacenDBDataSet3.Productos);

            CargarDatosDataGridView();
            estadoActual = EstadoFormulario.Agregar;
            ActualizarVisibilidadBotones();

        }

        private void CargarDatosDataGridView()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("ObtenerDetallesSalida", connection))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dtgSalidas.DataSource = dt;
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
            if (estadoActual == EstadoFormulario.Agregar)
            {
                // Lógica para agregar un nuevo producto
                AgregarSalida();
            }
            else if (estadoActual == EstadoFormulario.Editar)
            {
                // Lógica para editar el producto actual
                editarSalida();
            }

            LimpiarCampos();
            RestablecerEstado();
        }

        private void AgregarSalida()
        {
            int cantidad;
            string productoId = cboProducto.SelectedValue.ToString();
            DateTime fecha = dtFecha.Value;

            if (!int.TryParse(txtCantidad.Text, out cantidad))
            {
                MessageBox.Show("Ingrese valores válidos para cantidad", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Lógica para agregar el producto a la base de datos
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (VerificarStockDisponible(connection, productoId, cantidad))
                    {
                        using (SqlCommand command = new SqlCommand("InsertarSalida", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            // Añadir parámetros
                            command.Parameters.AddWithValue("@ProductoID", productoId);
                            command.Parameters.AddWithValue("@Cantidad", cantidad);
                            command.Parameters.AddWithValue("@FechaSalida", fecha);
                            command.Parameters.AddWithValue("@UsuarioID", Session.UsuarioID);

                            command.ExecuteNonQuery();
                        }

                        MessageBox.Show("Producto agregado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarCampos();
                        CargarDatosDataGridView();
                    }
                    else
                    {
                        MessageBox.Show("No hay suficiente stock disponible para la salida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar el producto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool VerificarStockDisponible(SqlConnection connection, string productoId, int cantidad)
        {
            // Consultar el stock actual del producto
            string query = "SELECT Stock FROM Productos WHERE ProductoID = @ProductoID";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ProductoID", productoId);

                int stockActual = Convert.ToInt32(command.ExecuteScalar());

                // Verificar si hay suficiente stock disponible
                return stockActual >= cantidad;
            }
        }

        private void editarSalida()
        {
            // Verificar si hay un proveedor seleccionado
            if (dtgSalidas.SelectedRows.Count > 0)
            {
                // Obtener el ProveedorID de la fila seleccionada
                int entradaID = Convert.ToInt32(dtgSalidas.SelectedRows[0].Cells["SalidaID"].Value);

                // Obtener los nuevos valores de los TextBox
                string nuevoCantidad = txtCantidad.Text;
                string nuevoProductId = cboProducto.SelectedValue.ToString();                
                DateTime nuevoFecha = dtFecha.Value;

                // Crear la conexión
                using (SqlConnection xcon = new SqlConnection(connectionString))
                {
                    // Abrir la conexión
                    xcon.Open();
                    string query = "UPDATE Salidas SET ProductoID = @ProductoID, Cantidad = @Cantidad, FechaSalida = @FechaSalida WHERE SalidaID = @SalidaID;";

                    // Crear el comando SQL
                    using (SqlCommand cmd = new SqlCommand(query, xcon))
                    {
                        // Asignar parámetros
                        cmd.Parameters.AddWithValue("@Cantidad", nuevoCantidad);
                        cmd.Parameters.AddWithValue("@ProductoID", nuevoProductId);                        
                        cmd.Parameters.AddWithValue("@FechaSalida", nuevoFecha);
                        cmd.Parameters.AddWithValue("@SalidaID", entradaID);

                        // Ejecutar la consulta
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        // Verificar si la actualización fue exitosa
                        if (filasAfectadas > 0)
                        {
                            MessageBox.Show("Salida actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            CargarDatosDataGridView(); // Vuelve a cargar los datos en el DataGridView
                            LimpiarCampos(); // Limpia los campos después de la actualización
                            RestablecerEstado(); // Restablece el estado del formulario
                        }
                        else
                        {
                            MessageBox.Show("No se pudo actualizar el entrada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione una salida para editar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            // Establecer el texto de los TextBoxes a una cadena vacía
            txtCantidad.Text = "";            
            cboProducto.Text = "";
            dtFecha.Text = "";
        }

        private void RestablecerEstado()
        {
            estadoActual = EstadoFormulario.Agregar;
            btnAgregar.Text = "AGREGAR";

            ActualizarVisibilidadBotones();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            // Limpiar el TextBox de búsqueda
            txtBuscar.Text = "";

            // Recargar todos los datos en el DataGridView
            CargarDatosDataGridView();
        }

        private void EliminarSalida(int proveedorID)
        {
            // Crear la conexión
            using (SqlConnection xcon = new SqlConnection(connectionString))
            {
                // Abrir la conexión
                xcon.Open();

                // Crear la consulta SQL de eliminación
                string query = "DELETE FROM Salidas WHERE SalidaID = @SalidaID";

                // Crear el comando SQL
                using (SqlCommand cmd = new SqlCommand(query, xcon))
                {
                    // Asignar parámetro
                    cmd.Parameters.AddWithValue("@SalidaID", proveedorID);

                    // Ejecutar la consulta
                    int filasAfectadas = cmd.ExecuteNonQuery();

                    // Verificar si la eliminación fue exitosa
                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Salida eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar el salida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            // Obtener el texto del TextBox de búsqueda
            string filtro = txtBuscar.Text.Trim();

            if (!string.IsNullOrEmpty(filtro))
            {
                // Crear la conexión
                using (SqlConnection xcon = new SqlConnection(connectionString))
                {
                    // Abrir la conexión
                    xcon.Open();

                    // Crear la consulta SQL con la cláusula WHERE
                    string query = @"SELECT
                                S.SalidaID,
                                P.Nombre AS ProductoNombre,
                                P.Precio AS PrecioProducto,
                                P.Stock AS Stock,
                                S.Cantidad,
                                S.FechaSalida
                            FROM
                                Salidas S
                                INNER JOIN Productos P ON S.ProductoID = P.ProductoID
                            WHERE
                                P.Nombre LIKE '%' + @Filtro + '%'
                            ORDER BY
                                S.SalidaID DESC";

                    // Crear el comando SQL
                    using (SqlCommand cmd = new SqlCommand(query, xcon))
                    {
                        // Asignar parámetros
                        cmd.Parameters.AddWithValue("@Filtro", filtro);

                        // Crear el adaptador de datos
                        SqlDataAdapter da = new SqlDataAdapter(cmd);

                        // Crear el DataTable para almacenar los resultados
                        DataTable dt = new DataTable();

                        // Llenar el DataTable con los resultados de la consulta
                        da.Fill(dt);

                        // Mostrar los resultados en el DataGridView
                        dtgSalidas.DataSource = dt;
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, ingrese un término de búsqueda.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            // Verificar si hay al menos una fila seleccionada
            if (dtgSalidas.SelectedRows.Count > 0)
            {
                int proveedorID = Convert.ToInt32(dtgSalidas.SelectedRows[0].Cells["SalidaID"].Value);
                // Confirmar la eliminación con un cuadro de diálogo
                DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar este salida?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    // Llamar a la función que realiza la eliminación
                    EliminarSalida(proveedorID);

                    // Actualizar la vista después de la eliminación
                    CargarDatosDataGridView();
                    LimpiarCampos();
                    RestablecerEstado();
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un salida para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dtgSalidas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verificar si el índice de la celda no es -1 (fuera de los límites)
            if (e.RowIndex != -1)
            {
                estadoActual = EstadoFormulario.Editar;
                btnAgregar.Text = "EDITAR";

                string cantidad = dtgSalidas.Rows[e.RowIndex].Cells["CantidadSalida"].Value.ToString();
                string producto = dtgSalidas.Rows[e.RowIndex].Cells["NombreProducto"].Value.ToString();                
                object fechaEntradaCellValue = dtgSalidas.Rows[e.RowIndex].Cells["FechaSalida"].Value;

                if (fechaEntradaCellValue != null)
                {
                    // Convertir el valor a DateTime
                    DateTime fechaEntrada = Convert.ToDateTime(fechaEntradaCellValue);

                    // Ahora, 'fechaEntrada' es un objeto DateTime que puedes utilizar como necesites
                    // Por ejemplo, puedes mostrarlo en un TextBox:
                    dtFecha.Text = fechaEntrada.ToString("yyyy-MM-dd HH:mm:ss");
                }


                // Establecer valores en los campos de texto
                txtCantidad.Text = cantidad;
                cboProducto.Text = producto.ToString();                


                ActualizarVisibilidadBotones();
            }
        }
    }
}
