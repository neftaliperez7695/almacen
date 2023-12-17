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
    public partial class Proveedores : Form
    {

        private const string connectionString = "Data Source=DESKTOP-GNTM77B\\SQLEXPRESS;Initial Catalog=AlmacenDB;Integrated Security=True";

        public Proveedores()
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
            txtEliminar.Visible = estadoActual == EstadoFormulario.Editar;
        }

        private void Proveedores_Load(object sender, EventArgs e)
        {
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

                    string query = "SELECT * FROM Proveedores";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dtGridViewProveedores.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            if (estadoActual == EstadoFormulario.Agregar)
            {
                // Lógica para agregar un nuevo proveedor
                agregarProvedores();
            }
            else if (estadoActual == EstadoFormulario.Editar)
            {
                // Lógica para editar el proveedor actual
                editarProvedor();
            }

            LimpiarCampos();
            RestablecerEstado();

        }

        private void agregarProvedores() 
        {
            // Obtener los detalles del proveedor desde los TextBoxes
            string nombre = txtNombre.Text.Trim();
            string telefono = txtTelefono.Text.Trim();
            string dni = txtDni.Text.Trim();
            string pais = cbPais.Text.Trim();

            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Por favor, ingrese el nombre del proveedor.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Lógica para agregar el producto a la base de datos
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO Proveedores (Nombre, Telefono, Dni, Pais) VALUES (@Nombre, @Telefono, @Dni, @Pais)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nombre", nombre);
                        command.Parameters.AddWithValue("@Telefono", telefono);
                        command.Parameters.AddWithValue("@Dni", dni);
                        command.Parameters.AddWithValue("@Pais", pais);

                        command.ExecuteNonQuery();

                        MessageBox.Show("Proveedor agregado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //LimpiarCampos();
                        CargarDatosDataGridView();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar el proveedor: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void editarProvedor()
        {
            // Verificar si hay un proveedor seleccionado
            if (dtGridViewProveedores.SelectedRows.Count > 0)
            {
                // Obtener el ProveedorID de la fila seleccionada
                int proveedorID = Convert.ToInt32(dtGridViewProveedores.SelectedRows[0].Cells["ProveedorID"].Value);

                // Obtener los nuevos valores de los TextBox
                string nuevoNombre = txtNombre.Text;
                string nuevoTelefono = txtTelefono.Text;
                string nuevoDni = txtDni.Text;
                string nuevoPais = cbPais.Text;

                // Crear la conexión
                using (SqlConnection xcon = new SqlConnection(connectionString))
                {
                    // Abrir la conexión
                    xcon.Open();

                    // Crear la consulta SQL de actualización
                    string query = "UPDATE Proveedores SET Nombre = @NuevoNombre, Telefono = @NuevoTelefono, Dni = @NuevoDni, Pais = @NuevoPais WHERE ProveedorID = @ProveedorID";

                    // Crear el comando SQL
                    using (SqlCommand cmd = new SqlCommand(query, xcon))
                    {
                        // Asignar parámetros
                        cmd.Parameters.AddWithValue("@NuevoNombre", nuevoNombre);
                        cmd.Parameters.AddWithValue("@NuevoTelefono", nuevoTelefono);
                        cmd.Parameters.AddWithValue("@NuevoDni", nuevoDni);
                        cmd.Parameters.AddWithValue("@NuevoPais", nuevoPais);
                        cmd.Parameters.AddWithValue("@ProveedorID", proveedorID);

                        // Ejecutar la consulta
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        // Verificar si la actualización fue exitosa
                        if (filasAfectadas > 0)
                        {
                            MessageBox.Show("Proveedor actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            CargarDatosDataGridView(); // Vuelve a cargar los datos en el DataGridView
                            LimpiarCampos(); // Limpia los campos después de la actualización
                            RestablecerEstado(); // Restablece el estado del formulario
                        }
                        else
                        {
                            MessageBox.Show("No se pudo actualizar el proveedor.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un proveedor para editar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RestablecerEstado()
        {
            estadoActual = EstadoFormulario.Agregar;
            btnCrear.Text = "AGREGAR";

            ActualizarVisibilidadBotones();
        }

        private void LimpiarCampos()
        {
            // Establecer el texto de los TextBoxes a una cadena vacía
            txtNombre.Text = "";
            txtTelefono.Text = "";
            txtDni.Text = "";
            cbPais.Text = "";
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            // Obtener el texto del TextBox de filtro
            string filtro = txtBuscar.Text;

            if (string.IsNullOrEmpty(filtro))
            {
                MessageBox.Show("Por favor, ingrese el nombre del proveedor.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                string consulta = "SELECT * FROM Proveedores WHERE Nombre LIKE @filtro";
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
                        adaptador.Fill(ds, "Proveedores");

                        // Asignar el conjunto de datos como origen de datos para la DataGridView
                        dtGridViewProveedores.DataSource = ds.Tables["Proveedores"];
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

        private void dtGridViewProveedores_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dtGridViewProveedores_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verificar si el índice de la celda no es -1 (fuera de los límites)
            if (e.RowIndex != -1)
            {
                estadoActual = EstadoFormulario.Editar;
                btnCrear.Text = "EDITAR";

                string nombre = dtGridViewProveedores.Rows[e.RowIndex].Cells["Nombre"].Value.ToString();
                string telefono = dtGridViewProveedores.Rows[e.RowIndex].Cells["Telefono"].Value.ToString();
                string dni = dtGridViewProveedores.Rows[e.RowIndex].Cells["Dni"].Value.ToString();
                string pais = dtGridViewProveedores.Rows[e.RowIndex].Cells["Pais"].Value.ToString();

                // Establecer valores en los campos de texto
                txtNombre.Text = nombre;
                txtTelefono.Text = telefono;
                txtDni.Text = dni;
                cbPais.Text = pais;

                ActualizarVisibilidadBotones();
            }
        }

        private void txtEliminar_Click(object sender, EventArgs e)
        {
            // Verificar si hay al menos una fila seleccionada
            if (dtGridViewProveedores.SelectedRows.Count > 0)
            {                
                int proveedorID = Convert.ToInt32(dtGridViewProveedores.SelectedRows[0].Cells["ProveedorID"].Value);
                // Confirmar la eliminación con un cuadro de diálogo
                DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar este proveedor?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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
                MessageBox.Show("Por favor, seleccione un proveedor para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                string query = "DELETE FROM Proveedores WHERE ProveedorID = @ProveedorID";

                // Crear el comando SQL
                using (SqlCommand cmd = new SqlCommand(query, xcon))
                {
                    // Asignar parámetro
                    cmd.Parameters.AddWithValue("@ProveedorID", proveedorID);

                    // Ejecutar la consulta
                    int filasAfectadas = cmd.ExecuteNonQuery();

                    // Verificar si la eliminación fue exitosa
                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Proveedor eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar el proveedor.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
