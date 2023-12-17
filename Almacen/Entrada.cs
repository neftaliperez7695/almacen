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
    public partial class Entrada : Form
    {
        private const string connectionString = "Data Source=DESKTOP-GNTM77B\\SQLEXPRESS;Initial Catalog=AlmacenDB;Integrated Security=True";

        public Entrada()
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

        private void Entrada_Load(object sender, EventArgs e)
        {
            // TODO: esta línea de código carga datos en la tabla 'almacenDBDataSet1.Proveedores' Puede moverla o quitarla según sea necesario.
            this.proveedoresTableAdapter.Fill(this.almacenDBDataSet1.Proveedores);
            // TODO: esta línea de código carga datos en la tabla 'almacenDBDataSet.Productos' Puede moverla o quitarla según sea necesario.
            this.productosTableAdapter.Fill(this.almacenDBDataSet.Productos);

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

                    using (SqlCommand command = new SqlCommand("ObtenerEntradas", connection))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dtEntradas.DataSource = dt;
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
                agregarEntrada();
            }
            else if (estadoActual == EstadoFormulario.Editar)
            {
                // Lógica para editar el producto actual
                editarEntrada();
            }

            LimpiarCampos();
            RestablecerEstado();            
        }

        private void RestablecerEstado()
        {
            estadoActual = EstadoFormulario.Agregar;
            btnAgregar.Text = "AGREGAR";

            ActualizarVisibilidadBotones();
        }

        private void agregarEntrada()
        {                                    
            int cantidad;
            string productoId = cboProducto.SelectedValue.ToString();
            string proveedorId = cboProveedor.SelectedValue.ToString();
            DateTime fecha = dtpFecha.Value;

            if (!int.TryParse(txtCantidad.Text, out cantidad))
            {
                MessageBox.Show("Ingrese valores válidos para entrada", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Lógica para agregar el producto a la base de datos
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("InsertarEntrada", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Añadir parámetros
                        command.Parameters.AddWithValue("@ProductoID", productoId);
                        command.Parameters.AddWithValue("@ProveedorID", proveedorId);
                        command.Parameters.AddWithValue("@Cantidad", cantidad);
                        command.Parameters.AddWithValue("@FechaEntrada", fecha);

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

        private void editarEntrada()
        {
            // Verificar si hay un proveedor seleccionado
            if (dtEntradas.SelectedRows.Count > 0)
            {
                // Obtener el ProveedorID de la fila seleccionada
                int entradaID = Convert.ToInt32(dtEntradas.SelectedRows[0].Cells["EntradaID"].Value);

                // Obtener los nuevos valores de los TextBox
                string nuevoCantidad = txtCantidad.Text;
                string nuevoProductId = cboProducto.SelectedValue.ToString();
                string nuevoProveedorId = cboProveedor.SelectedValue.ToString();
                DateTime nuevoFecha = dtpFecha.Value;

                // Crear la conexión
                using (SqlConnection xcon = new SqlConnection(connectionString))
                {
                    // Abrir la conexión
                    xcon.Open();
                    string query = "UPDATE Entradas SET ProductoID = @ProductoID, ProveedorID = @ProveedorID, Cantidad = @Cantidad, FechaEntrada = @FechaEntrada WHERE EntradaID = @EntradaID;";

                    // Crear el comando SQL
                    using (SqlCommand cmd = new SqlCommand(query, xcon))
                    {
                        // Asignar parámetros
                        cmd.Parameters.AddWithValue("@Cantidad", nuevoCantidad);
                        cmd.Parameters.AddWithValue("@ProductoID", nuevoProductId);
                        cmd.Parameters.AddWithValue("@ProveedorID", nuevoProveedorId);
                        cmd.Parameters.AddWithValue("@FechaEntrada", nuevoFecha);
                        cmd.Parameters.AddWithValue("@EntradaID", entradaID);

                        // Ejecutar la consulta
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        // Verificar si la actualización fue exitosa
                        if (filasAfectadas > 0)
                        {
                            MessageBox.Show("Entrada actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Por favor, seleccione una entrada para editar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                E.EntradaID,
                                P.Nombre AS ProductoNombre,
                                P.Precio AS PrecioProducto,
                                P.Stock AS Stock,
                                PR.Nombre AS ProveedorNombre,
                                PR.Dni AS DNI,
                                PR.Pais,
                                E.Cantidad,
                                E.FechaEntrada
                            FROM
                                Entradas E
                                INNER JOIN Productos P ON E.ProductoID = P.ProductoID
                                INNER JOIN Proveedores PR ON E.ProveedorID = PR.ProveedorID
                            WHERE
                                P.Nombre LIKE '%' + @Filtro + '%' OR
                                PR.Nombre LIKE '%' + @Filtro + '%'
                            ORDER BY
                                E.EntradaID DESC";

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
                        dtEntradas.DataSource = dt;
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, ingrese un término de búsqueda.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            // Limpiar el TextBox de búsqueda
            txtBuscar.Text = "";

            // Recargar todos los datos en el DataGridView
            CargarDatosDataGridView();
        }

        private void dtEntradas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verificar si el índice de la celda no es -1 (fuera de los límites)
            if (e.RowIndex != -1)
            {
                estadoActual = EstadoFormulario.Editar;
                btnAgregar.Text = "EDITAR";

                string cantidad = dtEntradas.Rows[e.RowIndex].Cells["CantidadEntrada"].Value.ToString();
                string producto = dtEntradas.Rows[e.RowIndex].Cells["NombreProducto"].Value.ToString();
                string proveedor = dtEntradas.Rows[e.RowIndex].Cells["NombreProveedor"].Value.ToString();
                object fechaEntradaCellValue = dtEntradas.Rows[e.RowIndex].Cells["FechaEntrada"].Value;

                if (fechaEntradaCellValue != null)
                {
                    // Convertir el valor a DateTime
                    DateTime fechaEntrada = Convert.ToDateTime(fechaEntradaCellValue);

                    // Ahora, 'fechaEntrada' es un objeto DateTime que puedes utilizar como necesites
                    // Por ejemplo, puedes mostrarlo en un TextBox:
                    dtpFecha.Text = fechaEntrada.ToString("yyyy-MM-dd HH:mm:ss");
                }


                // Establecer valores en los campos de texto
                txtCantidad.Text = cantidad;
                cboProducto.Text = producto.ToString();
                cboProveedor.Text = proveedor.ToString();


                ActualizarVisibilidadBotones();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            // Verificar si hay al menos una fila seleccionada
            if (dtEntradas.SelectedRows.Count > 0)
            {
                int proveedorID = Convert.ToInt32(dtEntradas.SelectedRows[0].Cells["EntradaID"].Value);
                // Confirmar la eliminación con un cuadro de diálogo
                DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar este entrada?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    // Llamar a la función que realiza la eliminación
                    EliminarCantidad(proveedorID);

                    // Actualizar la vista después de la eliminación
                    CargarDatosDataGridView();
                    LimpiarCampos();
                    RestablecerEstado();
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un entrada para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            // Establecer el texto de los TextBoxes a una cadena vacía
            txtCantidad.Text = "";
            cboProveedor.Text = "";
            cboProducto.Text = "";
            dtpFecha.Text = "";
        }

        private void EliminarCantidad(int proveedorID)
        {
            // Crear la conexión
            using (SqlConnection xcon = new SqlConnection(connectionString))
            {
                // Abrir la conexión
                xcon.Open();

                // Crear la consulta SQL de eliminación
                string query = "DELETE FROM Entradas WHERE EntradaID = @EntradaID";

                // Crear el comando SQL
                using (SqlCommand cmd = new SqlCommand(query, xcon))
                {
                    // Asignar parámetro
                    cmd.Parameters.AddWithValue("@EntradaID", proveedorID);

                    // Ejecutar la consulta
                    int filasAfectadas = cmd.ExecuteNonQuery();

                    // Verificar si la eliminación fue exitosa
                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Entrada eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar el entrada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
