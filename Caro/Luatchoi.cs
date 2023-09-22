using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Caro
{
    public partial class Luatchoi : Form
    {
        public Luatchoi()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void Luatchoi_Load(object sender, EventArgs e)
        {
            tbLuatchoi.Text = "Game gồm có 2 người chơi, một người cầm quân X, một người cầm quân O. Hai người chơi sẽ lần lượt đánh vào các ô trên bàn cờ có kích thước chọn trước cho đến khi kết thúc game. Hai người chơi có thể đánh vào bất kỳ ô nào trên bàn cờ miễn là ô đó chưa được đánh. Game kết thúc khi hết thời gian của lượt đánh hoặc có người chơi đánh được 5 quân thằng hàng liên tiếp nhau (ngang, dọc hoặc chéo) hoặc khi tất cả các ô trên bàn cờ đã được đánh hết (hòa)";
        }
    }
}
