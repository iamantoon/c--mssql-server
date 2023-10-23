using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System.Text.RegularExpressions;

namespace lab2
{
    public partial class Form2 : Form
    {
        private Form1 form1;
        public Form2(Form1 form1)
        {
            InitializeComponent();
            this.form1 = form1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string id = textBox1.Text;
            string country = textBox2.Text.Trim();
            string location = textBox3.Text.Trim();
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=Products;Integrated Security=True";
            string deleteCommand = $"DELETE FROM Countries WHERE ID = '{id}'";
            string insertCommand = $"INSERT INTO Countries (Country, Location) VALUES ('{country}', '{location}')";
            string updateCommand = $"UPDATE Countries\r\nSET \r\n    Country = '{country}',\r\n  Location = '{location}' \r\n WHERE ID = '{id}';";
            string Query = "SELECT * FROM Countries";

            string idPattern = @"^\d+$";
            string countryPattern = @"^[A-Za-z\s]+$";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (radioButton2.Checked)
                    {
                        if (Regex.IsMatch(country, countryPattern) && Regex.IsMatch(location, countryPattern))
                        {
                            using (SqlCommand command = new SqlCommand(insertCommand, connection))
                            {
                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show($"'{rowsAffected}' record(s) inserted successfully.");
                                    textBox2.Clear();
                                    textBox3.Clear();
                                }
                                else
                                {
                                    MessageBox.Show("Insertion failed. Check your SQL command and database connection.");
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Fill all the field with valid data");
                        }
                    }

                    if (radioButton3.Checked)
                    {
                        if (Regex.IsMatch(country, countryPattern) && Regex.IsMatch(location, countryPattern) && Regex.IsMatch(id, idPattern))
                        {
                            using (SqlCommand command = new SqlCommand(updateCommand, connection))
                            {
                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show($"'{rowsAffected}' record(s) updated successfully.");
                                    textBox1.Clear();
                                    textBox2.Clear();
                                    textBox3.Clear();
                                }
                                else
                                {
                                    MessageBox.Show("Update failed. Check your SQL command and database connection.");
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Fill all the field with invalid data");
                        }
                    }

                    if (radioButton4.Checked)
                    {
                        if (Regex.IsMatch(id, idPattern))
                        {
                            using (SqlCommand command = new SqlCommand(deleteCommand, connection))
                            {
                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show($"'{rowsAffected}' record(s) deleted successfully.");
                                    textBox1.Clear();
                                    textBox2.Clear();
                                    textBox3.Clear();
                                }
                                else
                                {
                                    MessageBox.Show("Deletion failed. Check your SQL command and database connection.");
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Write valid data");
                        }
                    }

                    form1.PerformInitialization();

                    using (SqlConnection Conn = new SqlConnection(connectionString))
                    {
                        DataTable dt = new DataTable();
                        Conn.Open();
                        SqlDataAdapter da = new SqlDataAdapter(Query, connectionString);
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}