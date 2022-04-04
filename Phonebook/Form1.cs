using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Phonebook
{
    public partial class Form1 : Form
    {
        ///Переменная для отображения
        public static Privilages adm_priv = Privilages.User;
        public static string connStr = "Server=10.0.77.14; Port = 5432; Database=bars_bd;User ID=postgres;Password=1234;CommandTimeout=60000;";
        public Form1()
        {
            InitializeComponent();
            Data();
            SetDoubleBuffered(dataGridView1, true); //плавный скролл
        }
        public void SetDoubleBuffered(Control c, bool value)
        {
            PropertyInfo pi = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic);
            if (pi != null)
            {
                pi.SetValue(c, value, null);

                MethodInfo mi = typeof(Control).GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic);
                if (mi != null)
                {
                    mi.Invoke(c, new object[] { ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true });
                }

                mi = typeof(Control).GetMethod("UpdateStyles", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic);
                if (mi != null)
                {
                    mi.Invoke(c, null);
                }
            }
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
            try
            {
                NpgsqlConnection conn = new NpgsqlConnection(connStr);
                NpgsqlCommand cmd = new NpgsqlCommand(cmdText, conn);
                NpgsqlDataAdapter sqlDataAdap = new NpgsqlDataAdapter(cmd);

                DataTable dtRecord = new DataTable();
                sqlDataAdap.Fill(dtRecord);
                dataGridView1.DataSource = dtRecord;
                //наименование столбцов
                dataGridView1.Columns[0].HeaderText = "Ид юзера";
                dataGridView1.Columns[1].HeaderText = "ФИО сотрудника";
                dataGridView1.Columns[2].HeaderText = "Местоположение";
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

                dataGridView1.Columns["name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                close_open_column(adm_priv);
            }
            catch (System.ArgumentOutOfRangeException sore)
            {
                MessageBox.Show("Подключение неактивно+\r\n"+sore.Message);
            }
            catch (Exception ex)
            {
                Encoding utf = Encoding.UTF8;
                Encoding win = Encoding.GetEncoding(1251);

                byte[] winArr = Encoding.Convert(win, utf, utf.GetBytes(ex.Message));

                MessageBox.Show(ex.Message); 
            }
            
        }
        private void close_open_column(Privilages val)
        {
            if (val == Privilages.User)
            {
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[4].Visible = false;
                dataGridView1.Columns[6].Visible = false;
                dataGridView1.Columns[8].Visible = false;
                dataGridView1.Columns[11].Visible = false;
                dataGridView1.Columns[12].Visible = false;
                dataGridView1.Columns[13].Visible = false;
                dataGridView1.Columns[14].Visible = false;
                adminoptionToolStripMenuItem.Visible = false;
                updateToolStripMenuItem.Visible = false;
            }
            else
            {
                dataGridView1.Columns[0].Visible = true;
                dataGridView1.Columns[4].Visible = true;
                dataGridView1.Columns[6].Visible = true;
                dataGridView1.Columns[8].Visible = true;
                //будущее
                if (val == Privilages.Admin)
                {
                    dataGridView1.Columns[11].Visible = true;
                    dataGridView1.Columns[12].Visible = true;
                    dataGridView1.Columns[13].Visible = true;
                    dataGridView1.Columns[14].Visible = true;
                    adminoptionToolStripMenuItem.Visible = true;
                    updateToolStripMenuItem.Visible = true;
                }
            }
            
        }
        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Export export = new Export();
            export.ShowDialog();
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (adminToolStripMenuItem.Checked == false)
            {
                Admin_privilege adm = new Admin_privilege(false);
                adm.ShowDialog();
                close_open_column(adm_priv);
                if (adm_priv == Privilages.Admin || adm_priv == Privilages.Specialist)
                {
                    adminToolStripMenuItem.Checked = true;
                }
            }
            else
            {
                adm_priv = Privilages.User;
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
            //MessageBox.Show(Privilages.Dev.ToString());
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

        public enum Privilages
        {
            Admin=1,
            Specialist=2,
            User=3
        }

        private void adduserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Admin_privilege adm = new Admin_privilege(true);
            adm.ShowDialog();
        }
    }
}
