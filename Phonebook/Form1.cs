using Npgsql;
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
    public partial class Form1 : Form
    {
        ///Переменная для отображения
        public static bool adm_priv = false;
        public static string connStr = "Server=localhost; Port = 5432; Database=bars_bd;User ID=postgres;Password=1234;CommandTimeout=60000;";
        public Form1()
        {
            InitializeComponent();
            Data();
        }
        private void Data()
        {
            
            string cmdText = @"SELECT u.id, u.name, u.office_room, u.phone, u.id_position,p.name, 
                            u.id_otdel, o.name, u.id_control, c.name, u.email, u.email_pass, u.name_pc, u.pc_pass, u.ip_pc
                            FROM public.b4_user u
                            join b4_position p on p.id=u.id_position
                            join b4_otdel o on o.id=u.id_otdel
                            join b4_control c on c.id= u.id_control
                            ORDER BY 1";

            NpgsqlConnection conn = new NpgsqlConnection(connStr);
            NpgsqlCommand cmd = new NpgsqlCommand(cmdText, conn);
            NpgsqlDataAdapter sqlDataAdap = new NpgsqlDataAdapter(cmd);

            DataTable dtRecord = new DataTable();
            sqlDataAdap.Fill(dtRecord);
            dataGridView1.DataSource = dtRecord;
            //наименование столбцов
            dataGridView1.Columns[0].HeaderText = "Ид юзера";
            dataGridView1.Columns[1].HeaderText = "ФИО";
            dataGridView1.Columns[2].HeaderText = "Офис";
            dataGridView1.Columns[3].HeaderText = "Телефон";
            dataGridView1.Columns[4].HeaderText = "Ид должности";
            dataGridView1.Columns[5].HeaderText = "Должность";
            dataGridView1.Columns[6].HeaderText = "Ид отдела";
            dataGridView1.Columns[7].HeaderText = "Отдел";
            dataGridView1.Columns[8].HeaderText = "Ид управления";
            dataGridView1.Columns[9].HeaderText = "Управление";
            dataGridView1.Columns[10].HeaderText = "Эл.почта";
            dataGridView1.Columns[11].HeaderText = "Эл.почта пароль";
            dataGridView1.Columns[12].HeaderText = "Имя ПК";
            dataGridView1.Columns[13].HeaderText = "Пароль ПК";
            dataGridView1.Columns[14].HeaderText = "IP адрес";
            close_open_column(adm_priv);
        }
        private void close_open_column(bool val)
        {
            dataGridView1.Columns[0].Visible = val;
            dataGridView1.Columns[4].Visible = val;
            dataGridView1.Columns[6].Visible = val;
            dataGridView1.Columns[8].Visible = val;
            dataGridView1.Columns[11].Visible = val;
            dataGridView1.Columns[12].Visible = val;
            dataGridView1.Columns[13].Visible = val;
            dataGridView1.Columns[14].Visible = val;

            
        }
        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Export export = new Export();
            export.ShowDialog();
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (adminToolStripMenuItem.Checked==false)
            {
                Admin_privilege adm = new Admin_privilege();
                adm.ShowDialog();
                if (adm.DialogResult == DialogResult.OK)
                {
                    //adm_priv = true;
                    close_open_column(adm_priv);
                    adminToolStripMenuItem.Checked = adm_priv;
                }
            }
            else if (adminToolStripMenuItem.Checked == true)
            {
                adm_priv = false;
                close_open_column(adm_priv);
                adminToolStripMenuItem.Checked = false;
            }
        }

        private void updateToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Data();
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            //MessageBox.Show(dataGridView1[0, dataGridView1.CurrentCell.RowIndex].Value.ToString());
            int z = (int)(dataGridView1[0, dataGridView1.CurrentCell.RowIndex].Value);
            Edit_user eu = new Edit_user(adm_priv, z);
            eu.ShowDialog();
            if (eu.DialogResult == DialogResult.OK)
                Data();

        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                textBox5.Text = "";
                textBox6.Text = "";
                textBox7.Text = "";

                (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            filter();
        }
        private void filter()
        {
            try
            {
                (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = String.Format($"name Like '%{textBox1.Text}%' and office_room Like '%{textBox2.Text}%'" +
                    $" and phone Like '%{textBox3.Text}%' and name1 Like '%{textBox4.Text}%' and name2 Like '%{textBox5.Text}%'" +
                    $" and name3 Like '%{textBox6.Text}%' and email Like '%{textBox7.Text}%'");
            }
            catch (Exception ex)
            {
                (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = null;
                MessageBox.Show(ex.Message);
            }
        }
    }
}
