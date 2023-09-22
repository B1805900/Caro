using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Caro
{
    public partial class Main : Form
    {
        private SqlConnection conn;
        private SqlCommand comm;
        int thang, thua, hoa;
        private String hoten;
        public String username1;
        public Main(String username)
        {
            username1 = username;
            InitializeComponent();
            conn = new SqlConnection("Server=localhost\\SQLEXPRESS;Initial Catalog=CARO;User Id=sa;pwd=sa2008;");
            comm = new SqlCommand("Select diemthang From POINT where username='" + username + "'", conn);
            conn.Open();
            thang = Convert.ToInt32(comm.ExecuteScalar());
            lbThang.Text = thang.ToString()+ " Trận";
            comm = new SqlCommand("Select diemthua From POINT where username='" + username + "'", conn);
            thua = Convert.ToInt32(comm.ExecuteScalar());
            lbThua.Text = thua.ToString() + " Trận";
            comm = new SqlCommand("Select diemhoa From POINT where username='" + username + "'", conn);
            hoa = Convert.ToInt32(comm.ExecuteScalar());
            lbHoa.Text = hoa.ToString() + " Trận";
            comm = new SqlCommand("Select hoten From LOGIN where username='" + username + "'", conn);
            hoten = Convert.ToString(comm.ExecuteScalar());
            lbHoten.Text = hoten;
            conn.Close();
        } 
        private void button1_Click(object sender, EventArgs e)
        {
            Setting.Banco_Size = Convert.ToInt32(cboSize.SelectedItem);
            Setting.COOL_DOWN_TIME = Convert.ToInt32(cboTime.SelectedItem)*1000;
            Login2 a = new Login2(username1);
            a.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Setting.Banco_Size = Convert.ToInt32(cboSize.SelectedItem);
            Setting.COOL_DOWN_TIME = Convert.ToInt32(cboTime.SelectedItem)*1000;
            Banco abc = new Banco(username1, "Computer");
            abc.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Luatchoi a = new Luatchoi();
            a.Show();
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
