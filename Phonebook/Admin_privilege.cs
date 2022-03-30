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
                //Form1.adm_priv = true;
                //button1.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Пароль неверный");
                //button1.DialogResult = DialogResult.Cancel;
                //Form1.adm_priv = false;
            }
        }

        private void Admin_privilege_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string s = (HashPassword(textBox1.Text));
        }

        public static string HashPassword(string password)
        {
            // http://.com/questions/19957176/asp-net-identity-password-hashing

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

        private void button4_Click(object sender, EventArgs e)
        {
            bool z = VerifyHashedPassword(textBox1.Text, textBox2.Text);
        }
    }
}
