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
    public partial class MaintenanceEntryFormOSBL : Form
    {
        public MaintenanceEntryFormOSBL()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var isPending = string.Empty;//
            if (checkBox1.CheckState == CheckState.Checked)
            {

                isPending = "No";
                DialogResult result = MessageBox.Show("Do you want to Complete Entry Later?\n", "Confirm", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    return;

                }

            }
            else
            {
                isPending = "Yes";
            }
            var equipmentTag = comboBox1.Text;
            var equipmentName = comboBox2.Text;
            var attendedBy = textBox1.Text;
            var attendedDate = dateTimePicker1.Value;
            var action = richTextBox2.Text;
            var material = richTextBox1.Text;
            var maintenanceId = textBox2.Text;
            var isPrimaryId = 0;
            string queryString = "";
            OleDbCommand command = new OleDbCommand();

            if (maintenanceId != "")
            {
                isPrimaryId = 1;
                //                queryString = string.Format("UPDATE  Maintenance SET [EquipmentTag]=@equipmentTag,[EquipmentName]=@equipmentName,[AttendedBy]=@attendedBy,[AttendedDate]=@attendedDate,[Action]=@action,[Material]=@material,[PlantType]=@plantType WHERE [MaintenanceID]=@maintenanceId");
                queryString = string.Format("UPDATE  Maintenance SET [EquipmentTag]=?,[EquipmentName]=?,[AttendedBy]=?,[AttendedDate]=?,[Action]=?,[Material]=?,[PlantType]=?,[EntryCompleted]=? WHERE [MaintenanceID]=?");
                var p1 = command.CreateParameter();
                p1.Value = equipmentTag;

                var p2 = command.CreateParameter();
                p2.Value = equipmentName;
                var p3 = command.CreateParameter();
                p3.Value = attendedBy;
                var p4 = command.CreateParameter();
                p4.Value = Program.GetDateWithoutMilliseconds(attendedDate);
                var p5 = command.CreateParameter();
                p5.Value = action;
                var p6 = command.CreateParameter();
                p6.Value = material;
                var p7 = command.CreateParameter();
                p7.Value = "OSBL";
                var p8 = command.CreateParameter();
                p8.Value = isPending;
                var p9 = command.CreateParameter();
                p9.Value = maintenanceId;




                command.Parameters.Add(p1);
                command.Parameters.Add(p2);
                command.Parameters.Add(p3);
                command.Parameters.Add(p4);
                command.Parameters.Add(p5);
                command.Parameters.Add(p6);
                command.Parameters.Add(p7);
                command.Parameters.Add(p8);
                command.Parameters.Add(p9);
            }
            else
            {
                queryString = string.Format("Insert into Maintenance([EquipmentTag],[EquipmentName],[AttendedBy],[AttendedDate],[Action],[Material],[PlantType],[EntryCompleted]) Values(@equipmentTag,@equipmentName,@attendedBy,@attendedDate,@action,@material,@plantType,@pending)");
                command.Parameters.AddWithValue("@equipmentTag", equipmentTag);
                command.Parameters.AddWithValue("@equipmentName", equipmentName);
                command.Parameters.AddWithValue("@attendedBy", attendedBy);
                command.Parameters.AddWithValue("@attendedDate", Program.GetDateWithoutMilliseconds(attendedDate));
                command.Parameters.AddWithValue("@action", action);
                command.Parameters.AddWithValue("@material", material);
                command.Parameters.AddWithValue("@plantType", "OSBL");
                command.Parameters.AddWithValue("@pending", isPending);
            }


            string connectionString = Program.ConnectionStringWithFilePath;


            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {

                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    command.CommandText = queryString;


                    connection.Open();
                    command.ExecuteNonQuery();
                    if (isPrimaryId == 0)
                    {
                        MessageBox.Show("Record has beed added successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Record has beed updated successfully!");
                    }

                    Program.CallMaintenanceGridRefresh(ref this.dataGridView1, "OSBL", 0);

                    this.comboBox1.DataSource = Program.GetEquipmenttype();
                    comboBox2.SelectedValue = "";
                    textBox1.Text = "";
                    dateTimePicker1.Value = DateTime.Now;
                    richTextBox2.Text = "";
                    richTextBox1.Text = "";
                    this.button1.BackColor = DefaultBackColor;
                    this.textBox2.Text = "";
                    this.textBox2.Enabled = false;
                    this.textBox2.Hide();
                    this.label8.Hide();
                    this.checkBox1.CheckState = CheckState.Unchecked;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect to data source");
            }

            



           


        }


        private void MaintenanceEntryForm_Load(object sender, EventArgs e)
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
                    command.Parameters.AddWithValue("@plantType", "OSBL");
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

           
            GetAllEquipmentNames();
            Program.CallMaintenanceGridRefresh(ref this.dataGridView1, "OSBL",0);
            


        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            OSBLHomeForm osblHomeForm = new OSBLHomeForm();
            osblHomeForm.Show();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
           
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var type = comboBox1.Text;

            if (type != "" && type != "System.Data.DataRowView")
            {
                var queryForEquipment = "SELECT * from EquipmentMaster Where EquipmentTag=@equipmentTag ";
                string connectionString = Program.ConnectionStringWithFilePath;

                string queryString = queryForEquipment;
                try
                {
                    using (OleDbConnection connection = new OleDbConnection(connectionString))
                    {
                        OleDbCommand command = new OleDbCommand();
                        command.CommandType = CommandType.Text;
                        command.Connection = connection;
                        command.CommandText = queryString;

                        command.Parameters.AddWithValue("@equipmentTag", type);
                        //command.Parameters.AddWithValue("@plantType", "ISBL");
                        connection.Open();
                        var reader = command.ExecuteReader();
                        var dataTable = new DataTable();
                        dataTable.Load(reader);

                        this.comboBox2.DataSource = dataTable;
                        this.comboBox2.DisplayMember = "EquipmentName";
                        this.comboBox2.ValueMember = "EquipmentName";

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to connect to data source");
                }
            }
        } 

        private void CallMaintenanceGridRefresh()
        {


            var queryForEquipment = "SELECT * from Maintenance Where [PlantType]=@plantType ORDER BY [MaintenanceID] DESC ";
            string connectionString = Program.ConnectionStringWithFilePath;

            string queryString = queryForEquipment;
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand command = new OleDbCommand();
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    command.CommandText = queryString;

                    command.Parameters.AddWithValue("@plantType", "OSBL");
                    //command.Parameters.AddWithValue("@plantType", "ISBL");
                    connection.Open();
                    var reader = command.ExecuteReader();
                    var dataTable = new DataTable();
                    dataTable.Load(reader);

                    this.dataGridView1.DataSource = dataTable;


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect to data source");
            }
        }

        private void GetAllEquipmentNames()
        {

            var query = "SELECT * from EquipmentMaster";


            string connectionString = Program.ConnectionStringWithFilePath;

            string queryString = query;
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand command = new OleDbCommand();
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                  //  command.Parameters.AddWithValue("@plantType", "ISBL");
                    command.CommandText = queryString;

                    connection.Open();

                    var dataReader = command.ExecuteReader();
                    AutoCompleteStringCollection stringCollection = new AutoCompleteStringCollection();
                    while (dataReader.Read())
                    {

                        stringCollection.Add(dataReader["EquipmentTag"].ToString() + ":" + dataReader["EquipmentName"].ToString());



                    }
                    //var dataTable = new DataTable();
                    //dataTable.Load(dataReader);
                    //this.comboBox1.DataSource = dataTable;

                    this.comboBox2.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    this.comboBox2.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    this.comboBox2.AutoCompleteCustomSource = stringCollection;

                    this.comboBox2.ValueMember = "EquipmentName";
                    this.comboBox2.DisplayMember = "EquipmentName";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect to data source");
            }


        }

        private void button4_Click(object sender, EventArgs e)
        {
            Program.CallMaintenanceGridRefresh(ref this.dataGridView1, "OSBL", 1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Int32 selectedCellCount =
       dataGridView1.GetCellCount(DataGridViewElementStates.Selected);
            var selectedRows = dataGridView1.SelectedRows.Count;
            if (selectedRows > 0)
            {
                if (dataGridView1.AreAllCellsSelected(true) || selectedRows > 1)
                {
                    MessageBox.Show("Please select the single record!!", "Unable to perform the action");
                }
                else
                {
                    System.Text.StringBuilder sb =
                        new System.Text.StringBuilder();

                    var isPending = false;

                    for (int i = 0;
                        i < selectedCellCount; i++)
                    {
                        if (dataGridView1.SelectedCells[i].OwningColumn.Name == "EntryCompleted")
                        {
                            var pendingValue = (dataGridView1.SelectedCells[i].Value);

                            if (pendingValue != null)
                            {
                                var pending = Convert.ToString(pendingValue);
                                if (pending == "No")
                                {
                                    isPending = true;
                                }

                            }
                        }



                    }
                    if (isPending == true)
                    {
                        for (int i = 0; i < selectedCellCount; i++)
                        {
                            if (dataGridView1.SelectedCells[i].OwningColumn.Name == "MaintenanceID")
                            {
                                var maintenanceIDValue = (dataGridView1.SelectedCells[i].Value);
                                if (maintenanceIDValue != null)
                                {
                                    var maintenanceID = Convert.ToInt32(maintenanceIDValue);
                                    UpdateMaintenanceRecord(maintenanceID);
                                }

                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("The record you have selected is already completed\n.Please select a 'Complete Later' record !", "Unable to perform the action");
                    }


                }
            }
            else
            {
                MessageBox.Show("Please select the the record !!", "Unable to perform the action");
            }
        }

        private void UpdateMaintenanceRecord(int maintenanceId)
        {
            var query = "SELECT * from Maintenance Where [MaintenanceID]=@maintenanceId";


            string connectionString = Program.ConnectionStringWithFilePath;

            string queryString = query;
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand command = new OleDbCommand();
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    command.Parameters.AddWithValue("@maintenanceId", maintenanceId);
                    command.CommandText = queryString;

                    connection.Open();

                    var dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {

                        var equipmentTag = dataReader["EquipmentTag"].ToString();
                        var equipmentName = dataReader["EquipmentName"].ToString();
                        var attendedBy = dataReader["AttendedBy"].ToString();
                        var attendedDate = Convert.ToDateTime(dataReader["AttendedDate"].ToString());
                        var action = dataReader["Action"].ToString();
                        var material = dataReader["Material"].ToString();
                        var pending = dataReader["EntryCompleted"].ToString();
                        comboBox1.Text = equipmentTag;
                        comboBox2.Text = equipmentName;
                        textBox1.Text = attendedBy;
                        dateTimePicker1.Value = attendedDate;
                        richTextBox2.Text = action;
                        richTextBox1.Text = material;
                        textBox2.Text = maintenanceId.ToString();
                        this.button1.BackColor = Color.Red;
                        if (pending == "No")
                        {
                            this.checkBox1.CheckState = CheckState.Checked;
                        }
                        else
                        {
                            this.checkBox1.CheckState = CheckState.Unchecked;
                        }
                        //this.textBox2.Enabled=false;
                        // this.textBox2.Show();
                        //this.label8.Show();

                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect to data source");
            }
        }



    }
}
