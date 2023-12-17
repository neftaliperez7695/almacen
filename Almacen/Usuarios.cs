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

        private void btnCrear_Click(object sender, EventArgs e)
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
    }
}
