using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Phonebook
{
    public partial class Admin_privilege : Form
    {
        public Admin_privilege()
        {
            InitializeComponent();
            button2.DialogResult = DialogResult.Cancel;
            button1.DialogResult = DialogResult.OK;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "25121985")
            {
                Form1.adm_priv = true;
                //button1.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Пароль неверный");
                //button1.DialogResult = DialogResult.Cancel;
                Form1.adm_priv = false;
            }
        }

        private void Admin_privilege_Load(object sender, EventArgs e)
        {

        }
    }
}
