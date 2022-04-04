using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Phonebook
{
    public partial class Admin_privilege : Form
    {
        private const int SaltByteSize = 24;//для лучшего хэширования
        private const int HashByteSize = 24;
        private const int HasingIterationsCount = 10101;
        private static bool add_user { get; set; }

        public Admin_privilege(bool option1)
        {
            InitializeComponent();
            add_user = option1;
            if(option1)
            {
                comboBox1.Visible = true;
                button1.Text = "Добавить пользователя";

            }
            button2.DialogResult = DialogResult.Cancel;
            button1.DialogResult = DialogResult.OK;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string login = textBox2.Text;
            string bd_hash = "";
            //string s = (HashPassword(textBox1.Text));
            NpgsqlConnection conn = new NpgsqlConnection(Form1.connStr);
            NpgsqlCommand cmd = new NpgsqlCommand($"SELECT u.hash, u.role FROM public.user u where u.login = '{login}';", conn);
            conn.Open();
            if (!add_user)
            {
                try
                {
                    using (NpgsqlDataReader ndr = cmd.ExecuteReader())
                    {
                        if (ndr.HasRows) // если есть данные
                        {
                            while (ndr.Read()) // построчно считываем данные
                            {
                                bd_hash = ndr.GetValue(0).ToString();
                                if (VerifyHashedPassword(bd_hash, textBox1.Text))
                                {
                                    Form1.adm_priv = (Form1.Privilages)Enum.Parse(typeof(Form1.Privilages), ndr.GetValue(1).ToString(), true);
                                    //button1.DialogResult = DialogResult.OK;
                                    this.Close();
                                }
                                else
                                {
                                    Form1.adm_priv = Form1.Privilages.User;
                                    MessageBox.Show("Ошибка пароля");
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Указанный логин не найден!");
                        }
                    }
                }
                catch (Exception s)
                {
                    MessageBox.Show(s.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
            else
            {
                MessageBox.Show("");
            }
        }

        private void Admin_privilege_Load(object sender, EventArgs e)
        {

        }

        public static string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, SaltByteSize, HasingIterationsCount))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(HashByteSize);
            }
            byte[] dst = new byte[(SaltByteSize + HashByteSize) + 1];
            Buffer.BlockCopy(salt, 0, dst, 1, SaltByteSize);
            Buffer.BlockCopy(buffer2, 0, dst, SaltByteSize + 1, HashByteSize);
            return Convert.ToBase64String(dst);
        }

        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] _passwordHashBytes;

            int _arrayLen = (SaltByteSize + HashByteSize) + 1;

            if (hashedPassword == null)
            {
                return false;
            }

            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            byte[] src = Convert.FromBase64String(hashedPassword);

            if ((src.Length != _arrayLen) || (src[0] != 0))
            {
                return false;
            }

            byte[] _currentSaltBytes = new byte[SaltByteSize];
            Buffer.BlockCopy(src, 1, _currentSaltBytes, 0, SaltByteSize);

            byte[] _currentHashBytes = new byte[HashByteSize];
            Buffer.BlockCopy(src, SaltByteSize + 1, _currentHashBytes, 0, HashByteSize);

            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, _currentSaltBytes, HasingIterationsCount))
            {
                _passwordHashBytes = bytes.GetBytes(SaltByteSize);
            }

            return AreHashesEqual(_currentHashBytes, _passwordHashBytes);

        }

        private static bool AreHashesEqual(byte[] firstHash, byte[] secondHash)
        {
            int _minHashLength = firstHash.Length <= secondHash.Length ? firstHash.Length : secondHash.Length;
            var xor = firstHash.Length ^ secondHash.Length;
            for (int i = 0; i < _minHashLength; i++)
                xor |= firstHash[i] ^ secondHash[i];
            return 0 == xor;
        }

    }
}
