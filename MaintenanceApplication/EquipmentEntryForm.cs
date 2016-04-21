using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaintenanceApplication
{
    public partial class EquipmentEntryForm : Form
    {
        public EquipmentEntryForm()
        {
            InitializeComponent();
        }

        private void EquipmentEntryForm_Load(object sender, EventArgs e)
        {

            var query = "SELECT * from EquipmentTagMaster Where [PlantType]=@plantType";


            string connectionString = Program.ConnectionStringWithFilePath;

            string queryString = query;
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand command = new OleDbCommand();
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    command.Parameters.AddWithValue("@plantType", "ISBL");
                    command.CommandText = queryString;

                    connection.Open();

                    var dataReader = command.ExecuteReader();
                    AutoCompleteStringCollection stringCollection = new AutoCompleteStringCollection();
                    while (dataReader.Read())
                    {

                        stringCollection.Add(dataReader["EquipmentTag"].ToString());



                    }
                    //var dataTable = new DataTable();
                    //dataTable.Load(dataReader);
                    //this.comboBox1.DataSource = dataTable;
                                                                            
                    this.comboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    this.comboBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    this.comboBox1.AutoCompleteCustomSource = stringCollection;
                    
                    this.comboBox1.ValueMember = "EquipmentTag";
                    this.comboBox1.DisplayMember = "EquipmentTag";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect to data source");
            }







            Program.CallEquipmentGridRefresh(ref this.dataGridView1,0);
            //          var query = "SELECT * from EquipmentMaster";

            //var dataTable = Program.GetDataTableDataFromDb(query);


            ////Set AutoGenerateColumns False
            //this.dataGridView1.AutoGenerateColumns = false;

            ////Set Columns Count
            //this.dataGridView1.ColumnCount = 3;

            ////Add Columns
            //this.dataGridView1.Columns[0].Name = "EquipmentID";
            //this.dataGridView1.Columns[0].HeaderText = "Equipment Id";
            //this.dataGridView1.Columns[0].DataPropertyName = "EquipmentID";

            //this.dataGridView1.Columns[1].HeaderText = "Equipment Type";
            //this.dataGridView1.Columns[1].Name = "Type";
            //this.dataGridView1.Columns[1].DataPropertyName = "EquipmentType";

            //this.dataGridView1.Columns[2].Name = "EquipmentName";
            //this.dataGridView1.Columns[2].HeaderText = "Name";
            //this.dataGridView1.Columns[2].DataPropertyName = "EquipmentName";

            //this.dataGridView1.DataSource = dataTable;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var tag = comboBox1.Text;
            var name = textBox2.Text;

            string connectionString = Program.ConnectionStringWithFilePath;

            string queryString = string.Format("Insert into EquipmentMaster([EquipmentTag],[EquipmentName],[PlantType]) Values(?,?,?)");
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand command = new OleDbCommand();
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    command.CommandText = queryString;

                    command.Parameters.AddWithValue("@EquipmentTag", tag);
                    command.Parameters.AddWithValue("@EquipmentName", name);
                    command.Parameters.AddWithValue("@PlantType", "ISBL");
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Record has beed added successfully!");
                    Program.CallEquipmentGridRefresh(ref this.dataGridView1,0);
                    var query = "SELECT * from EquipmentTagMaster";

                    var dataTable = Program.GetDataTableDataFromDb(query);
                    this.comboBox1.DataSource = dataTable;
                    textBox2.Text="";

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect to data source");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {

            this.Close();
            ISBLHomeForm isblHomeForm = new ISBLHomeForm();
            isblHomeForm.Show();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }



        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Program.CallEquipmentGridRefresh(ref this.dataGridView1, 1);
        }
    }
}
