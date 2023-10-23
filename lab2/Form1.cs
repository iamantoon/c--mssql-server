using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace lab2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void PerformInitialization()
        {
            string DB = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=Products;Integrated Security=True";
            string Query = "SELECT * FROM Countries WHERE Country IN ('Brazil', 'Argentina', 'Colombia', 'Chile', 'Ecuador', 'Mexico', 'Panama', 'Dominican Republic', 'Haiti', 'Uruguay', 'Venezuela', 'Guatemala', 'Honduras', 'Suriname', 'Belize', 'Paraguay', 'Peru', 'Bolivia', 'Costa Rica', 'Nicaragua', 'Cuba', 'Barbados')";

            using (SqlConnection Conn = new SqlConnection(DB))
            {
                try
                {
                    Conn.Open();

                    using (SqlCommand command = new SqlCommand(Query, Conn))
                    {
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            comboBox1.Items.Clear();
                            while (reader.Read())
                            {
                                string countryName = reader["Country"].ToString();
                                string countryId = reader["ID"].ToString();
                                string country = countryId + countryName;
                                comboBox1.Items.Add(country);
                            }
                        }
                        else
                        {
                            // "Якщо таких записів в другій таблиці немає - вставити ці записи в таблицю"
                            string insertQuery = "INSERT INTO Countries (Country, Location) VALUES (@Country, @Location)";
                            try
                            {
                                using (SqlCommand insertCommand = new SqlCommand(insertQuery, Conn))
                                {
                                    insertCommand.Parameters.AddWithValue("@Country", "Brazil");
                                    insertCommand.Parameters.AddWithValue("@Location", "South America");

                                    Conn.Open();
                                    int rowsAffected = insertCommand.ExecuteNonQuery();

                                    if (rowsAffected > 0)
                                    {
                                        MessageBox.Show("Record inserted successfully");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Insert operation failed");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                // Handle any exceptions that may occur during database access
                                MessageBox.Show("Error: " + ex.Message);
                            }
                            finally
                            {
                                Conn.Close();
                            }
                        }

                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    Conn.Close();
                }
            }
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            PerformInitialization();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=Products;Integrated Security=True";
            string Query = "SELECT ProductName, ExportValue, ExportYear, ExportQuantity, Country from Products as a inner join Countries as b on a.CountryID = b.ID";

            string id = textBox1.Text;
            string productName = textBox2.Text.Trim();
            string exportQuantity = textBox4.Text.ToString().Trim();
            string exportValue = textBox5.Text.ToString().Trim();
            string exportYear = textBox6.Text.ToString().Trim();

            string productNamePattern = @"^[\p{L} ]+$";
            string exportValuePattern = @"^\d+$";
            string exportYearPattern = @"^20\d{2}$";
            string exportQuantityPattern = @"^\d+$";

            string selectedCountry = "1Brazil";
            string countryID = "1";

            if (comboBox1.SelectedItem != null)
            {
                string digitsOnly = Regex.Replace(comboBox1.SelectedItem.ToString(), "[^0-9]", "");
                countryID = digitsOnly;
                string lettersOnly = Regex.Replace(comboBox1.SelectedItem.ToString(), "[^a-zA-Z]", "");
                selectedCountry = lettersOnly;
            }

            string deleteCommand = $"DELETE FROM Products WHERE ID = '{id}'";

            string insertCommand = $"INSERT INTO Products (ProductName, CountryID, ExportValue, ExportYear, ExportQuantity) VALUES (N'{productName}', '{countryID}', '{exportValue}', '{exportYear}', '{exportQuantity}')";
            string updateCommand = $"UPDATE Products\r\nSET\r\n  ProductName = N'{productName}',\r\n  CountryID='{countryID}',\r\n  ExportValue = '{exportValue}',\r\n  ExportYear = '{exportValue}', \r\n  ExportQuantity = '{exportQuantity}'\r\nWHERE ID = '{id}';";
            
            string selectCommand1 = $"SELECT ProductName, ExportValue, ExportYear, ExportQuantity, Country from Products as a inner join Countries as b on a.CountryID = b.ID WHERE a.CountryID like '{countryID}'";
            string selectCommand2 = $"SELECT ProductName, ExportValue, ExportYear, ExportQuantity, Country from Products as a inner join Countries as b on a.CountryID = b.ID WHERE a.ID like '{id}'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                if (radioButton1.Checked)
                {
                    if (checkBox1.Checked)
                    {
                        using (SqlConnection Conn = new SqlConnection(connectionString))
                        {
                            DataTable dt = new DataTable();
                            Conn.Open();
                            SqlDataAdapter da = new SqlDataAdapter(selectCommand1, connectionString);
                            da.Fill(dt);
                            dataGridView1.DataSource = dt;
                        }
                    }
                    if (checkBox2.Checked)
                    {
                        using (SqlConnection Conn = new SqlConnection(connectionString))
                        {
                            DataTable dt = new DataTable();
                            Conn.Open();
                            SqlDataAdapter da = new SqlDataAdapter(selectCommand2, connectionString);
                            da.Fill(dt);
                            dataGridView1.DataSource = dt;
                        }
                    }
                }

                if (radioButton2.Checked)
                {
                    try
                    {
                        if (Regex.IsMatch(productName, productNamePattern) && Regex.IsMatch(exportValue, exportValuePattern) && Regex.IsMatch(exportYear, exportYearPattern) && Regex.IsMatch(exportQuantity, exportQuantityPattern) && !String.IsNullOrWhiteSpace(productName))
                        {
                            using (SqlCommand command = new SqlCommand(insertCommand, connection))
                            {
                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show($"Record inserted successfully. {rowsAffected} was added to the table");
                                    textBox2.Clear();
                                    textBox4.Clear();
                                    textBox5.Clear();
                                    textBox6.Clear();
                                }
                                else
                                {
                                    MessageBox.Show("Insertion failed. Check your SQL command and database connection.");
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Fill all the fields with valid data");
                        }
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine("Format error: " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An unexpected error occurred: " + ex.Message);
                    }
                }

                if (radioButton4.Checked)
                {
                    if (Regex.IsMatch(id, exportValuePattern))
                    {
                        using (SqlCommand command = new SqlCommand(deleteCommand, connection))
                        {
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show($"Record deleted successfully. {rowsAffected} was deleted");
                                textBox1.Clear();
                                textBox2.Clear();
                                textBox4.Clear();
                            }
                            else
                            {
                                MessageBox.Show("Deletion failed. Check your SQL command and database connection.");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid ID");
                    }
                }

                if (radioButton3.Checked)
                {
                    if (Regex.IsMatch(id, exportValuePattern) && Regex.IsMatch(productName, productNamePattern) && Regex.IsMatch(exportValue, exportValuePattern) && Regex.IsMatch(exportYear, exportYearPattern) && Regex.IsMatch(exportQuantity, exportQuantityPattern))
                    {
                        using (SqlCommand command = new SqlCommand(updateCommand, connection))
                        {
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show($"Record updated successfully. {rowsAffected} rows were updated");
                                textBox1.Clear();
                                textBox2.Clear();
                                textBox4.Clear();
                                textBox5.Clear();
                                textBox6.Clear();
                            }
                            else
                            {
                                MessageBox.Show("Update failed. Check your SQL command and database connection.");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("If you want to update some record you have to fill all the fields");
                    }
                }
            }

            if (!checkBox1.Checked && !checkBox2.Checked)
            {
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

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 childForm = new Form2(this);

            this.Hide();

            childForm.ShowDialog();

            this.Show();
        }

    }
}