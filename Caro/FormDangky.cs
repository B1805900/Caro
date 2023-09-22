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
    public partial class FormDangky : Form
    {
        public FormDangky()
        {
            InitializeComponent();
        }
        private SqlConnection conn;
        private SqlCommand comm;
        private String user, pass, hoten, sqlstr;


        private void btnDangky_Click(object sender, EventArgs e)
        {
            try
            {
                user = txtUsername.Text.ToString();
                pass = txtPassword.Text.ToString();
                hoten = txtHoten.Text.ToString();
                conn = new SqlConnection("Server=localhost\\SQLEXPRESS;Initial Catalog=CARO;User Id=sa;pwd=sa2008;");
                comm = new SqlCommand("Select Count(*) From LOGIN WHere BINARY_CHECKSUM(username)=BINARY_CHECKSUM('" + user + "')", conn);
                //Tải dữ liệu lên dataGridView
                //dataGridView1.DataSource = dt;
                conn.Open();
                int value = Convert.ToInt32(comm.ExecuteScalar());
                // MessageBox.Show("" + hoten);
                if (value > 0)
                {
                    MessageBox.Show("Tên đăng nhập đã tồn tại!");
                }
                else
                {
                    sqlstr = "Insert Into LOGIN Values(N'" + user + "', N'" + pass + "', N'" + hoten + "')";
                    comm = new SqlCommand(sqlstr, conn);
                    comm.ExecuteNonQuery();
                    sqlstr = "Insert Into POINT Values(N'" + user + "',0,0,0)";
                    comm = new SqlCommand(sqlstr, conn);
                    comm.ExecuteNonQuery();
                    conn.Close();
                    MessageBox.Show("Đăng ký thành công");
                    this.Hide();
                 //   FormLogin abc = new FormLogin();
                 //   abc.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtHoten.Text = "";
            txtPassword.Text = "";
            txtUsername.Text = "";
        }
    }
}
