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
    public partial class FormLogin : Form
    {
        private SqlConnection conn;
        private SqlCommand comm;
        private String user, pass;

        public FormLogin()
        {
            InitializeComponent();
        }

        private void btnDangky_Click(object sender, EventArgs e)
        {
            FormDangky abc = new FormDangky();
            abc.Show();
        }

        private void btnDangnhap_Click(object sender, EventArgs e)
        {
            try
            {
                user = txtUsername.Text.ToString();
                pass = txtPassword.Text.ToString();
                conn = new SqlConnection("Server=localhost\\SQLEXPRESS;Initial Catalog=CARO;User Id=sa;pwd=sa2008;");
                comm = new SqlCommand("Select Count(*) From LOGIN WHere BINARY_CHECKSUM(username)=BINARY_CHECKSUM('" + user + "') and BINARY_CHECKSUM(password)=BINARY_CHECKSUM('" + pass + "')", conn);
                //Tải dữ liệu lên dataGridView
                //dataGridView1.DataSource = dt;
                conn.Open();
                int value = Convert.ToInt32(comm.ExecuteScalar());
               // MessageBox.Show("" + hoten);
                if (value > 0)
                {   
                //    this.Hide();
                    conn.Close();
                    this.Hide();
                    Main abc = new Main(user);
                    abc.Show();
                }
                else
                {
                    MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu");
                }   
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {

        }
    }
}
