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
using ClosedXML;
using ClosedXML.Excel;
using System.IO;

namespace Phonebook
{
    public partial class Form1 : Form
    {
        ///Переменная для отображения
        public static Privilages adm_priv = Privilages.User;
        //public static string connStr = "Server = 10.0.77.9; Port = 5433; Database=bd_bars;User ID = postgres; Password=ltkjvfytckjdjv777;CommandTimeout=60000;";
        public static string connStr = "Server = 46.0.207.122; Port = 5433; Database=bd_bars;User ID = bars; Password=ltkjvfytckjdjv777;CommandTimeout=60000;";
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
        public void Data()
        {
            string cmdText = @"SELECT u.id, u.name, u.office_room, u.phone, u.id_position,p.name, 
                            u.id_otdel, o.name, u.id_control, c.name, u.email, u.email_pass, u.name_pc, u.pc_pass, u.ip_pc
                            FROM public.b4_user u
                            join b4_position p on p.id=u.id_position
                            join b4_otdel o on o.id=u.id_otdel
                            join b4_control c on c.id= u.id_control
                            ORDER BY u.name";
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
                dataGridView1.Columns[12].HeaderText = @"Имя ПК\Пользователь ПК";
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
                adduserToolStripMenuItem.Visible = false;
                Add_user_tool.Visible = false;
                updateToolStripMenuItem.Visible = false;
            }
            else
            {
                
                adminoptionToolStripMenuItem.Visible = true;
                Add_user_tool.Visible = true;
                //будущее
                if (val == Privilages.Admin)
                {
                    dataGridView1.Columns[0].Visible = true;
                    dataGridView1.Columns[4].Visible = true;
                    dataGridView1.Columns[6].Visible = true;
                    dataGridView1.Columns[8].Visible = true;
                    dataGridView1.Columns[11].Visible = true;
                    dataGridView1.Columns[12].Visible = true;
                    dataGridView1.Columns[13].Visible = true;
                    dataGridView1.Columns[14].Visible = true;
                    adminoptionToolStripMenuItem.Visible = true;
                    adduserToolStripMenuItem.Visible = true;
                    Add_user_tool.Visible = true;
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

        private void выгрузкаВEXCELToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string filePath = "";
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory = "c:\\";
                saveFileDialog.FileName = "Телефонная книга" + DateTime.Now.ToString("dd_MM_yyyy");
                saveFileDialog.Filter = "Файл Excel (*.xlsx)|*.xlsx";
                saveFileDialog.FilterIndex = 2;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = saveFileDialog.FileName;

                    //Read the contents of the file into a stream
                    //var fileStream = saveFileDialog.OpenFile();

                }
            }

            //MessageBox.Show(filePath, "", MessageBoxButtons.OK);

            if (filePath != "")
            {
                //DataTable dtSource = new DataTable();
                //DataView dv = (DataView)(dataGridView1.DataSource);
                string cmdText = $"SELECT u.name \"ФИО сотрудника\", u.office_room \"Местоположение\", u.phone \"Телефон\"," +
                    "p.name \"Должность\",o.name \"Отдел\", c.name \"Управление\", u.email \"Эл.почта\", p.\"Gradation\"," +
                    "o.\"Gradation\", c.\"Gradation\" FROM public.b4_user u join b4_position p on p.id=u.id_position join " +
                    "b4_otdel o on o.id=u.id_otdel join b4_control c on c.id= u.id_control ORDER BY  c.\"Gradation\", o.\"Gradation\", p.\"Gradation\"";
                NpgsqlConnection conn = new NpgsqlConnection(connStr);
                NpgsqlCommand nc = new NpgsqlCommand(cmdText, conn);

                XLWorkbook xLWorkbook = new XLWorkbook();
                List<string> headerTable = new List<string>(); //получение заголовков
                headerTable.Add("ФИО сотрудника");
                headerTable.Add("Местоположение");
                headerTable.Add("Телефон");
                headerTable.Add("Должность");
                headerTable.Add("Эл.почта");

                var excelworksheet = xLWorkbook.Worksheets.Add("Выгрузка");

                conn.Open();
                NpgsqlDataReader ndr = nc.ExecuteReader();
                string otdel = "";
                string upr = "";
                //string strok_csv = "";
                try
                {
                    for (int i = 1; i < headerTable.Count + 1; i++)
                    {
                        excelworksheet.Cell(1, i).Style.Fill.BackgroundColor = XLColor.Yellow;
                        excelworksheet.Columns(1, i).AdjustToContents();
                        excelworksheet.Cell(1, i).Value = headerTable[i - 1].ToString();
                    }

                    if (ndr.HasRows)
                    {
                        int z = 2;
                        while (ndr.Read())
                        {
                            if (upr != ndr.GetValue(5).ToString())
                            {
                                upr = ndr.GetValue(5).ToString();
                                excelworksheet.Cell(z, 1).Value = ndr.GetValue(5);
                                excelworksheet.Range("A" + z.ToString() + ":E" + z.ToString()).Row(1).Merge();
                                excelworksheet.Cell(z, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                excelworksheet.Cell(z, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                excelworksheet.Cell(z, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(220, 230, 241);
                                z++;
                            }
                            if (otdel != ndr.GetValue(4).ToString() && ndr.GetValue(4).ToString() != "-")
                            {
                                otdel = ndr.GetValue(4).ToString();
                                excelworksheet.Cell(z, 1).Value = ndr.GetValue(4);
                                excelworksheet.Range("A" + z.ToString() + ":E" + z.ToString()).Row(1).Merge();
                                excelworksheet.Cell(z, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                excelworksheet.Cell(z, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(253, 233, 217);
                                z++;
                            }

                            try
                            {
                                excelworksheet.Cell(z, 1).Value = ndr.GetValue(0);
                                excelworksheet.Cell(z, 2).Value = ndr.GetValue(1);
                                excelworksheet.Cell(z, 3).Value = ndr.GetValue(2);
                                excelworksheet.Cell(z, 4).Value = ndr.GetValue(3);
                                excelworksheet.Cell(z, 5).Value = ndr.GetValue(6);
                            }
                            catch (System.InvalidCastException)
                            {
                            }
                            z++;
                        }
                        //excelworksheet.Range("A1", "E" + z.ToString()).Style.Border.TopBorder = XLBorderStyleValues.Dotted;
                        excelworksheet.Range("A1", "E" + (z - 1).ToString()).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        excelworksheet.Range("A1", "E" + (z - 1).ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        excelworksheet.Range("A1", "E" + (z - 1).ToString()).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        excelworksheet.Range("A1", "E" + (z - 1).ToString()).Style.Border.DiagonalBorder = XLBorderStyleValues.Thin;
                        excelworksheet.Range("A1", "E1").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

                        //excelworksheet.Range("A1", "E" + z.ToString()).Style.Border.TopBorder = XLBorderStyleValues.Dotted;
                    }
                    else
                    {
                        MessageBox.Show("Не обнаружены строки для записи в csv");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    ndr.Close();
                }
                excelworksheet.Columns().AdjustToContents();
                xLWorkbook.SaveAs(filePath);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Add_user_tool_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(Privilages.Dev.ToString());
            int z = (int)(dataGridView1[0, dataGridView1.CurrentCell.RowIndex].Value);
            Add_user eu = new Add_user(adm_priv);
            eu.ShowDialog();
            if (eu.DialogResult == DialogResult.OK)
            Data();
        }
    }
}
