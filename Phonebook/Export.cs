using ClosedXML.Excel;
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
    public partial class Export : Form
    {
        public Export()
        {
            InitializeComponent();
            textBox1.Text = @"D:\phonebook.xlsx";
        }

        private void Export_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var wb2 = new XLWorkbook(@textBox1.Text);
                char[] MyChar = { ',',' '};
                for (int i = 2; i <= Convert.ToInt32(wb2.Worksheet(1).LastRowUsed().RowNumber()); i++)
                {
                    dataGridView1.Rows.Add();
                    dataGridView1[0, i - 2].Value = wb2.Worksheet(1).Row(i).Cell(1).Value.ToString().Trim().Trim(MyChar);
                    dataGridView1[1, i - 2].Value = wb2.Worksheet(1).Row(i).Cell(2).Value.ToString().Trim().Trim(MyChar);
                    dataGridView1[2, i - 2].Value = wb2.Worksheet(1).Row(i).Cell(3).Value.ToString().Trim().Trim(MyChar).Replace(", ,",",");
                    dataGridView1[3, i - 2].Value = wb2.Worksheet(1).Row(i).Cell(4).Value.ToString();
                    dataGridView1[4, i - 2].Value = wb2.Worksheet(1).Row(i).Cell(5).Value.ToString();
                    dataGridView1[5, i - 2].Value = wb2.Worksheet(1).Row(i).Cell(6).Value.ToString();
                    dataGridView1[6, i - 2].Value = wb2.Worksheet(1).Row(i).Cell(7).Value.ToString().Trim().Trim(MyChar);
                    dataGridView1[7, i - 2].Value = wb2.Worksheet(1).Row(i).Cell(8).Value.ToString();
                    dataGridView1[8, i - 2].Value = wb2.Worksheet(1).Row(i).Cell(9).Value.ToString();
                    dataGridView1[9, i - 2].Value = wb2.Worksheet(1).Row(i).Cell(10).Value.ToString();
                }
                
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string connStr = Form1.connStr;
            string cmdText = "SELECT name FROM public.b4_user;";
            NpgsqlConnection conn = new NpgsqlConnection(connStr);
            NpgsqlCommand cmd = new NpgsqlCommand(cmdText, conn);
            NpgsqlDataAdapter sqlDataAdap = new NpgsqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            conn.Open();
            sqlDataAdap.Fill(dt);
            List<string> FIO = new List<string>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string number = dt.Rows[i][0].ToString();

                if (!FIO.Contains(number))
                {
                    FIO.Add(number);
                }

            }

            if (dataGridView1.Rows.Count > 1)
            {
                var wb2 = new XLWorkbook(@textBox1.Text);
                
                for (int i = 0; i < Convert.ToInt32(wb2.Worksheet(1).LastRowUsed().RowNumber()); i++)
                {
                    if (!FIO.Contains(dataGridView1[0, i].Value))
                    {
                        cmdText = $@"INSERT INTO public.b4_user(name, office_room, phone, id_position, id_otdel, id_control, email, email_pass, name_pc, pc_pass, ip_pc) VALUES('{dataGridView1[0, i].Value}', '{dataGridView1[1, i].Value}', '{dataGridView1[2, i].Value}', {dataGridView1[3, i].Value}, {dataGridView1[4, i].Value}, {dataGridView1[5, i].Value}, '{dataGridView1[6, i].Value}', '{dataGridView1[7, i].Value}', '{dataGridView1[8, i].Value}', '{dataGridView1[9, i].Value}', '{dataGridView1[10, i].Value}'); ";
                        textBox2.AppendText(dataGridView1[0, i].Value + " Добавлено! \r\n");
                        cmd = new NpgsqlCommand(cmdText, conn);
                        try
                        {
                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                    textBox2.AppendText(dataGridView1[0, i].Value + " Уже был! \r\n");
                }
            }
        }
    }
}
