using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net.Sockets;
namespace codHammingaCs
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            TcpClient client = new TcpClient("127.0.0.1", 5595);
            byte[] data = System.Text.Encoding.UTF32.GetBytes(textBox1.Text);// = System.Text.Encoding.ASCII.GetBytes(textBox1.Text);
            string s = "";
            for (int i = 0; i < data.Length; i++)
                s += string.Format("{0,7:D7}", Convert.ToInt32(Convert.ToString(data[i], 2)));
            listBox1.Items.Add("Отправлено сообщение в двоичном формате: " + s);
            data = new byte[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '1')
                    data[i] = 1;
                else
                    data[i] = 0;
            }
            Hamming_Coder hc = new Hamming_Coder(data);
            data = hc.закодировать();
            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
            client.Close();
        }
        private string вСтроку(byte[] v)
        {
            string s = "";
            for (int i = 0; i < v.Length; i++)
                s += v[i];
            return s;
        }
    }
}
