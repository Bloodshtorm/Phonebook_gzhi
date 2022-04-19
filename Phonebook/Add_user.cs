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
    public partial class Add_user : Form
    {
        public Add_user(Form1.Privilages adm_priv)
        {
            InitializeComponent();
            button2.DialogResult = DialogResult.Cancel;
            button4.DialogResult = DialogResult.OK;

            label15.Visible = false;
            label10.Visible = false;
            label11.Visible = false;
            label12.Visible = false;
            textBox5.Visible = false;
            textBox6.Visible = false;
            textBox7.Visible = false;
            textBox8.Visible = false;
            label8.Visible = false;
            label7.Visible = false;
            label13.Visible = false;
            label14.Visible = false;
            button2.Visible = false;
            button4.Visible = false;

            if (adm_priv == Form1.Privilages.Specialist)
            {
                button2.Visible = true;
                button4.Visible = true;
            }
            else
            {
                if (adm_priv == Form1.Privilages.Admin)
                {
                    button2.Visible = true;
                    button4.Visible = true;
                    label15.Visible = true;
                    label10.Visible = true;
                    label11.Visible = true;
                    label12.Visible = true;
                    textBox5.Visible = true;
                    textBox6.Visible = true;
                    textBox7.Visible = true;
                    textBox8.Visible = true;
                    label8.Visible = true;
                    label7.Visible = true;
                    label13.Visible = true;
                    label14.Visible = true;
                }
            }

            string connStr = Form1.connStr;
            //string cmdText = $"SELECT name FROM public.b4_position;";
            NpgsqlConnection conn = new NpgsqlConnection(connStr); conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, name FROM public.b4_position order by 1;", conn);
            try
            {
                using (NpgsqlDataReader ndr = cmd.ExecuteReader())
                {
                    if (ndr.HasRows) // если есть данные
                    {
                        while (ndr.Read()) // построчно считываем данные
                        {
                            comboBox1.Items.Add(ndr.GetValue(1));
                        }
                    }
                }

                cmd = new NpgsqlCommand("SELECT id, name FROM public.b4_otdel order by 1;", conn);
                using (NpgsqlDataReader ndr = cmd.ExecuteReader())
                {
                    if (ndr.HasRows) // если есть данные
                    {
                        while (ndr.Read()) // построчно считываем данные
                        {
                            comboBox2.Items.Add(ndr.GetValue(1));
                        }
                    }
                }

                cmd = new NpgsqlCommand("SELECT id, name FROM public.b4_control order by 1;", conn);
                using (NpgsqlDataReader ndr = cmd.ExecuteReader())
                {
                    if (ndr.HasRows) // если есть данные
                    {
                        while (ndr.Read()) // построчно считываем данные
                        {
                            comboBox3.Items.Add(ndr.GetValue(1));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            NpgsqlConnection conn = new NpgsqlConnection(Form1.connStr); conn.Open();
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand($"SELECT id FROM public.b4_position where name='{comboBox1.SelectedItem.ToString()}';", conn);
                label10.Text = cmd.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                label10.Text = "1";
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            NpgsqlConnection conn = new NpgsqlConnection(Form1.connStr); conn.Open();
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand($"SELECT id FROM public.b4_otdel where name='{comboBox2.SelectedItem.ToString()}';", conn);
                label11.Text = cmd.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                label11.Text = "1";
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            NpgsqlConnection conn = new NpgsqlConnection(Form1.connStr); conn.Open();
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand($"SELECT id FROM public.b4_control where name='{comboBox3.SelectedItem.ToString()}';", conn);
                label12.Text = cmd.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                label12.Text = "1";
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            NpgsqlConnection conn = new NpgsqlConnection(Form1.connStr); conn.Open();
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand($@"INSERT INTO public.b4_user(name, office_room, phone, id_position, id_otdel, id_control, email, email_pass, name_pc, pc_pass, ip_pc)
                VALUES('{textBox1.Text}', '{textBox2.Text}', '{textBox3.Text}', {label10.Text}, {label11.Text}, {label12.Text}, '{textBox4.Text}', 
                '{textBox5.Text}', '{textBox6.Text}', '{textBox7.Text}', '{textBox8.Text}')", conn);
                cmd.ExecuteNonQuery();
                MessageBox.Show($"Добавлен пользователь:\r\n{textBox1.Text}\r\n{textBox2.Text}\r\n{textBox3.Text}\r\n{textBox4.Text}");
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

    }
}
