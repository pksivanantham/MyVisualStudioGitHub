using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaintenanceApplication
{
    public partial class ISBLHomeForm : Form
    {
        public ISBLHomeForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            EquipmentEntryForm entry = new EquipmentEntryForm();
            entry.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            MaintenanceEntryForm entry = new MaintenanceEntryForm();
            entry.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            MaintenanceSearchForm search = new MaintenanceSearchForm();
            search.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
            EquipmentSearchForm search = new EquipmentSearchForm();
            search.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
            Form1 mainForm = new Form1();
            mainForm.Show();
        }

        private void ISBLHomeForm_Load(object sender, EventArgs e)
        {
            this.richTextBox1.Text = Program.GetDailyQuotesOfTheDay();
            this.label1.Text = string.Format("Quote of the day : {0:dd-MMM-yyyy}", DateTime.Now);
            this.richTextBox1.Enabled = false;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
