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
    public partial class OSBLHomeForm : Form
    {
        public OSBLHomeForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            EquipmentEntryFormOSBL entry = new EquipmentEntryFormOSBL();
            entry.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
            EquipmentSearchFormOSBL search = new EquipmentSearchFormOSBL();
            search.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            MaintenanceEntryFormOSBL entry = new MaintenanceEntryFormOSBL();
            entry.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            MaintenanceSearchFormOSBL search = new MaintenanceSearchFormOSBL();
            search.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
            Form1 mainForm = new Form1();
            mainForm.Show();
        }

        private void OSBLHomeForm_Load(object sender, EventArgs e)
        {
            this.richTextBox1.Text = Program.GetDailyQuotesOfTheDay();
            this.richTextBox1.Enabled = false;
            this.label1.Text = string.Format("Quote of the day : {0:dd-MMM-yyyy}", DateTime.Now);
        }
    }
}
