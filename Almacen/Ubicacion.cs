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
    public partial class Ubicacion : Form
    {
        private const string connectionString = "Data Source=DESKTOP-GNTM77B\\SQLEXPRESS;Initial Catalog=AlmacenDB;Integrated Security=True";

        public Ubicacion()
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

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (estadoActual == EstadoFormulario.Agregar)
            {
                // Lógica para agregar un nuevo producto
                agregarUbicacion();
            }
            else if (estadoActual == EstadoFormulario.Editar)
            {
                // Lógica para editar el producto actual
                editarUbicacion();
            }

            LimpiarCampos();
            RestablecerEstado();
        }

        private void agregarUbicacion()
        {
            // Obtener los detalles del producto desde los TextBoxes
            string nombre = txtNombre.Text.Trim();

            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Por favor, ingrese el nombre de la ubicacion.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Lógica para agregar el producto a la base de datos
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO Ubicaciones (Nombre) VALUES (@Nombre)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nombre", nombre);

                        command.ExecuteNonQuery();

                        MessageBox.Show("Ubicacion agregado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarCampos();
                        CargarDatosDataGridView();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar el ubicacion: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void editarUbicacion()
        {
            // Verificar si hay un proveedor seleccionado
            if (dtgUbicacion.SelectedRows.Count > 0)
            {
                // Obtener el ProveedorID de la fila seleccionada
                int proveedorID = Convert.ToInt32(dtgUbicacion.SelectedRows[0].Cells["UbicacionID"].Value);

                // Obtener los nuevos valores de los TextBox
                string nuevoNombre = txtNombre.Text;

                // Crear la conexión
                using (SqlConnection xcon = new SqlConnection(connectionString))
                {
                    // Abrir la conexión
                    xcon.Open();

                    // Crear la consulta SQL de actualización
                    string query = "UPDATE Ubicaciones SET Nombre = @NuevoNombre WHERE UbicacionID = @UbicacionID";

                    // Crear el comando SQL
                    using (SqlCommand cmd = new SqlCommand(query, xcon))
                    {
                        // Asignar parámetros
                        cmd.Parameters.AddWithValue("@NuevoNombre", nuevoNombre);
                        cmd.Parameters.AddWithValue("@UbicacionID", proveedorID);

                        // Ejecutar la consulta
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        // Verificar si la actualización fue exitosa
                        if (filasAfectadas > 0)
                        {
                            MessageBox.Show("ubicacion actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            CargarDatosDataGridView(); // Vuelve a cargar los datos en el DataGridView
                            LimpiarCampos(); // Limpia los campos después de la actualización
                            RestablecerEstado(); // Restablece el estado del formulario
                        }
                        else
                        {
                            MessageBox.Show("No se pudo actualizar la ubicacion.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un proveedor para editar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            // Establecer el texto de los TextBoxes a una cadena vacía
            txtNombre.Text = "";
        }

        private void Ubicacion_Load(object sender, EventArgs e)
        {
            CargarDatosDataGridView();
            estadoActual = EstadoFormulario.Agregar;
            ActualizarVisibilidadBotones();
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

                    string query = "SELECT * FROM Ubicaciones";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dtgUbicacion.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            // Obtener el texto del TextBox de filtro
            string filtro = txtBuscar.Text;

            if (string.IsNullOrEmpty(filtro))
            {
                MessageBox.Show("Por favor, ingrese el nombre de la ubicacion.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                string consulta = "SELECT * FROM Ubicaciones WHERE Nombre LIKE @filtro";
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
                        adaptador.Fill(ds, "UbicacionID");

                        // Asignar el conjunto de datos como origen de datos para la DataGridView
                        dtgUbicacion.DataSource = ds.Tables["UbicacionID"];
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

        private void dtgUbicacion_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verificar si el índice de la celda no es -1 (fuera de los límites)
            if (e.RowIndex != -1)
            {
                estadoActual = EstadoFormulario.Editar;
                btnAgregar.Text = "EDITAR";

                string nombre = dtgUbicacion.Rows[e.RowIndex].Cells["Nombre"].Value.ToString();
               
                // Establecer valores en los campos de texto
                txtNombre.Text = nombre;

                ActualizarVisibilidadBotones();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            // Verificar si hay al menos una fila seleccionada
            if (dtgUbicacion.SelectedRows.Count > 0)
            {
                int proveedorID = Convert.ToInt32(dtgUbicacion.SelectedRows[0].Cells["UbicacionID"].Value);
                // Confirmar la eliminación con un cuadro de diálogo
                DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar esta ubicacion?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    // Llamar a la función que realiza la eliminación
                    EliminarProveedor(proveedorID);

                    // Actualizar la vista después de la eliminación
                    CargarDatosDataGridView();
                    LimpiarCampos();
                    RestablecerEstado();
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione la ubicacion para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EliminarProveedor(int proveedorID)
        {
            // Crear la conexión
            using (SqlConnection xcon = new SqlConnection(connectionString))
            {
                // Abrir la conexión
                xcon.Open();

                // Crear la consulta SQL de eliminación
                string query = "DELETE FROM Ubicaciones WHERE UbicacionID = @UbicacionID";

                // Crear el comando SQL
                using (SqlCommand cmd = new SqlCommand(query, xcon))
                {
                    // Asignar parámetro
                    cmd.Parameters.AddWithValue("@UbicacionID", proveedorID);

                    // Ejecutar la consulta
                    int filasAfectadas = cmd.ExecuteNonQuery();

                    // Verificar si la eliminación fue exitosa
                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Ubicacion eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar la ubicacion.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
