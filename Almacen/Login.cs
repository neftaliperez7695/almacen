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
    public partial class Login : Form
    {
        private const string connectionString = "Data Source=DESKTOP-GNTM77B\\SQLEXPRESS;Initial Catalog=AlmacenDB;Integrated Security=True";

        public Login()
        {
            InitializeComponent();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Por favor, ingrese el nombre de usuario y la contraseña.", "Error de inicio de sesión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (AutenticarUsuario(usuario, password))
            {
                MessageBox.Show("Inicio de sesión exitoso", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Redirigir a la pantalla Form2
                Principal form2 = new Principal();
                form2.Show();

                // O puedes ocultar el formulario actual si es necesario
                this.Hide();
            }
            else
            {
                MessageBox.Show("Nombre de usuario o contraseña incorrectos.", "Error de inicio de sesión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool AutenticarUsuario(string usuario, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT Password FROM Usuarios WHERE NombreUsuario = @Usuario";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Usuario", usuario);

                    // Obtener la contraseña cifrada desde la base de datos
                    string contrasenaCifrada = command.ExecuteScalar() as string;

                    if (contrasenaCifrada != null)
                    {
                        // Comparar contraseñas cifradas
                        return VerificarHash(password, contrasenaCifrada);
                    }
                }                
            }
            // si no encontro usaurio retornar false
            return false;
        }

        private bool VerificarHash(string contraena, string contrasenaCifrada)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convertir la contraseña de entrada en un hash
                string hashInput = ObtenerHash(contraena);

                // Comparar los hashes
                return StringComparer.OrdinalIgnoreCase.Compare(hashInput, contrasenaCifrada) == 0;
            }
        }

        private string ObtenerHash(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convertir la cadena de entrada en un arreglo de bytes y calcular el hash
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Convertir el arreglo de bytes a una cadena hexadecimal
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    stringBuilder.Append(bytes[i].ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
    }
}
