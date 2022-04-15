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
    public partial class Edit_user : Form
    {
        public Edit_user(Form1.Privilages adm_priv, int id_user)
        {
            InitializeComponent();
            button1.DialogResult = DialogResult.OK;
            button2.DialogResult = DialogResult.Cancel;
            button3.DialogResult = DialogResult.OK;
            button4.DialogResult = DialogResult.OK;

            if (adm_priv == Form1.Privilages.User)
            {
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
                button1.Visible = false;
                button2.Visible = false;
                button3.Visible = false;
                button4.Visible = false;
            }
            else
            {
                button1.Visible = true;
                button2.Visible = true;
                button3.Visible = true;
                button4.Visible = true;

                if (adm_priv == Form1.Privilages.Admin)
                {
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
            string cmdText = $"SELECT * FROM public.b4_user where id={id_user};";
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

            cmd = new NpgsqlCommand(cmdText, conn);
            try
            {
                using (NpgsqlDataReader ndr = cmd.ExecuteReader())
                {
                    if (ndr.HasRows) // если есть данные
                    {
                        while (ndr.Read()) // построчно считываем данные
                        {
                            label15.Text = ndr.GetValue(0).ToString();
                            textBox1.Text = ndr.GetValue(1).ToString();
                            textBox2.Text = ndr.GetValue(2).ToString();
                            textBox3.Text = ndr.GetValue(3).ToString();
                            label10.Text = ndr.GetValue(4).ToString();
                            label11.Text = ndr.GetValue(5).ToString();
                            label12.Text = ndr.GetValue(6).ToString();
                            textBox4.Text = ndr.GetValue(7).ToString();
                            textBox5.Text = ndr.GetValue(8).ToString();
                            textBox6.Text = ndr.GetValue(9).ToString();
                            textBox7.Text = ndr.GetValue(10).ToString();
                            textBox8.Text = ndr.GetValue(11).ToString();

                        }
                    }
                }
                combo_value();
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
        public void combo_value()
        {

            NpgsqlConnection conn = new NpgsqlConnection(Form1.connStr); conn.Open();
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand($"SELECT name FROM public.b4_position where id={label10.Text};", conn);
                //cmd = new NpgsqlCommand($"SELECT name FROM public.b4_position where id={label10.Text};", conn);
                if (comboBox1.Items.Contains(cmd.ExecuteScalar().ToString()))
                    comboBox1.SelectedItem = cmd.ExecuteScalar().ToString();
                cmd = new NpgsqlCommand($"SELECT name FROM public.b4_otdel where id={label11.Text};", conn);
                if (comboBox2.Items.Contains(cmd.ExecuteScalar().ToString()))
                    comboBox2.SelectedItem = cmd.ExecuteScalar().ToString();
                cmd = new NpgsqlCommand($"SELECT name FROM public.b4_control where id={label12.Text};", conn);
                if (comboBox3.Items.Contains(cmd.ExecuteScalar().ToString()))
                    comboBox3.SelectedItem = cmd.ExecuteScalar().ToString();
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() == "")
            {
                label10.Text = "1";
            }
            if (comboBox2.SelectedItem.ToString() == "")
            {
                label11.Text = "1";
            }
            if (comboBox3.SelectedItem.ToString() == "")
            {
                label12.Text = "1";
            }
            //MessageBox.Show("dsa");
            NpgsqlConnection conn = new NpgsqlConnection(Form1.connStr); conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand($@"UPDATE public.b4_user SET name='{textBox1.Text}', office_room='{textBox2.Text}', phone='{textBox3.Text}', 
            id_position={label10.Text}, id_otdel={label11.Text}, id_control={label12.Text}, email='{textBox4.Text}', email_pass='{textBox5.Text}', 
            name_pc='{textBox6.Text}', pc_pass='{textBox7.Text}', ip_pc='{textBox8.Text}'	WHERE id={label15.Text};", conn);
            cmd.ExecuteNonQuery();
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
            NpgsqlCommand cmd = new NpgsqlCommand($@"INSERT INTO public.b4_user(name, office_room, phone, id_position, id_otdel, id_control, email, email_pass, name_pc, pc_pass, ip_pc)
            VALUES('{textBox1.Text}', '{textBox2.Text}', '{textBox3.Text}', {label10.Text}, {label11.Text}, {label12.Text}, '{textBox4.Text}', 
            '{textBox5.Text}', '{textBox6.Text}', '{textBox7.Text}', '{textBox8.Text}')", conn);
            cmd.ExecuteNonQuery();

            
    }

        private void button3_Click(object sender, EventArgs e)
        {
            NpgsqlConnection conn = new NpgsqlConnection(Form1.connStr); conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand($@"DELETE FROM public.b4_user WHERE id={label15.Text};", conn);
            cmd.ExecuteNonQuery();
        }
    }
}
