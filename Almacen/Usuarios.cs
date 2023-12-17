using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Almacen
{
    public partial class Usuarios : Form
    {
        private const string connectionString = "Data Source=DESKTOP-GNTM77B\\SQLEXPRESS;Initial Catalog=AlmacenDB;Integrated Security=True";

        public Usuarios()
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

        private void Usuarios_Load(object sender, EventArgs e)
        {
            CargarDatosDataGridView();
            estadoActual = EstadoFormulario.Agregar;
            ActualizarVisibilidadBotones();
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            if (estadoActual == EstadoFormulario.Agregar)
            {
                agregarUsuario();
            } 
            else if (estadoActual == EstadoFormulario.Editar)
            {
                editarUsuario();
            }
            LimpiarCampos();
            RestablecerEstado();

        }

        private void editarUsuario()
        {            
            if (dtgUsuarios.SelectedRows.Count > 0)
            {
                // Obtener el ProveedorID de la fila seleccionada
                int usuarioID = Convert.ToInt32(dtgUsuarios.SelectedRows[0].Cells["UsuarioID"].Value);

                string contrasena = txtPassword.Text;
                string usuario = txtUsuario.Text;

                string contrasenaCifrada = ObtenerHash(contrasena);

                // Crear la conexión
                using (SqlConnection xcon = new SqlConnection(connectionString))
                {
                    // Abrir la conexión
                    xcon.Open();

                    // Crear la consulta SQL de actualización
                    string query = "UPDATE Usuarios SET NombreUsuario = @NuevoUsuario, Password= @NuevoPassword WHERE UsuarioID = @UsuarioID";

                    // Crear el comando SQL
                    using (SqlCommand cmd = new SqlCommand(query, xcon))
                    {
                        // Asignar parámetros
                        cmd.Parameters.AddWithValue("@NuevoUsuario", usuario);
                        cmd.Parameters.AddWithValue("@NuevoPassword", contrasenaCifrada);                        
                        cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);

                        // Ejecutar la consulta
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        // Verificar si la actualización fue exitosa
                        if (filasAfectadas > 0)
                        {
                            MessageBox.Show("Usuario actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            CargarDatosDataGridView(); // Vuelve a cargar los datos en el DataGridView
                            LimpiarCampos(); // Limpia los campos después de la actualización
                            RestablecerEstado(); // Restablece el estado del formulario
                        }
                        else
                        {
                            MessageBox.Show("No se pudo actualizar el usuario.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un usuario para editar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void agregarUsuario()
        {
            string contrasena = txtPassword.Text;
            string usuario = txtUsuario.Text;

            if (!string.IsNullOrEmpty(contrasena) && !string.IsNullOrEmpty(usuario))
            {
                string contrasenaCifrada = ObtenerHash(contrasena);
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = "INSERT INTO Usuarios (NombreUsuario, Password) VALUES (@NombreUsuario, @Password)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@NombreUsuario", usuario);
                            command.Parameters.AddWithValue("@Password", contrasenaCifrada);

                            command.ExecuteNonQuery();

                            MessageBox.Show("Usuario creado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al agregar el usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Ingrese usuario y contraseña", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private string ObtenerHash(string contrasena)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convertir la cadena de entrada en un arreglo de bytes y calcular el hash
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(contrasena));
                
                // Convertir el arreglo de bytes a una cadena hexadecimal
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    stringBuilder.Append(bytes[i].ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }
        

        private void CargarDatosDataGridView()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT UsuarioID, NombreUsuario FROM Usuarios";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dtgUsuarios.DataSource = dt;
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
                MessageBox.Show("Por favor, ingrese el nombre del usuario.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                string consulta = "SELECT * FROM Usuarios WHERE NombreUsuario LIKE @filtro";
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
                        adaptador.Fill(ds, "Usuarios");

                        // Asignar el conjunto de datos como origen de datos para la DataGridView
                        dtgUsuarios.DataSource = ds.Tables["Usuarios"];
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

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            // Verificar si hay al menos una fila seleccionada
            if (dtgUsuarios.SelectedRows.Count > 0)
            {
                int productoID = Convert.ToInt32(dtgUsuarios.SelectedRows[0].Cells["UsuarioID"].Value);
                // Confirmar la eliminación con un cuadro de diálogo
                DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar este Usuario?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    // Llamar a la función que realiza la eliminación
                    EliminarUsuario(productoID);

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

        private void EliminarUsuario(int productoID)
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

        private void LimpiarCampos()
        {
            // Establecer el texto de los TextBoxes a una cadena vacía
            txtUsuario.Text = "";
            txtPassword.Text = "";            
        }

        private void RestablecerEstado()
        {
            estadoActual = EstadoFormulario.Agregar;
            btnCrear.Text = "AGREGAR";

            ActualizarVisibilidadBotones();
        }

        private void dtgUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verificar si el índice de la celda no es -1 (fuera de los límites)
            if (e.RowIndex != -1)
            {
                estadoActual = EstadoFormulario.Editar;
                btnCrear.Text = "EDITAR";

                string nombreUsuario = dtgUsuarios.Rows[e.RowIndex].Cells["NombreUsuario"].Value.ToString();

                // Establecer valores en los campos de texto
                txtUsuario.Text = nombreUsuario;

                ActualizarVisibilidadBotones();
            }
        }
    }
}
