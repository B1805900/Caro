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
    public partial class Banco : Form
    {
        #region Properties
        ChessBoardManager ChessBoard;
        #endregion




        public Banco(String user1,String user2)
        {
            InitializeComponent();
            ChessBoard = new ChessBoardManager(pnelBanco, user1, user2,btnLuotdi);
            ChessBoard.EndedGame += ChessBoard_EndedGame;
            ChessBoard.PlayerMarked += ChessBoard_PlayerMarked;
            ChessBoard.HoaGame += ChessBoard_HoaGame;
            prcbCooldown.Step = Setting.COOL_DOWN_STEP;
            prcbCooldown.Maximum = Setting.COOL_DOWN_TIME;
            prcbCooldown.Value = 0;

            tmCooldown.Interval = Setting.COOL_DOWN_INTERVAL;

            NewGame();

            Loaddata();

            btnLuotdi.BackgroundImage = ChessBoard.Nguoichoi[ChessBoard.CurrentPlayer].Mark;

        }

        private void Loaddata()
        {
            lbThang1.Text = ChessBoard.Nguoichoi[0].Dthang.ToString() + " Trận";

            lbThua1.Text = ChessBoard.Nguoichoi[0].Dthua.ToString() + " Trận";

            lbHoa1.Text = ChessBoard.Nguoichoi[0].Dhoa.ToString() + " Trận";

            lbHoten1.Text = ChessBoard.Nguoichoi[0].Hoten;

            lbThang2.Text = ChessBoard.Nguoichoi[1].Dthang.ToString() + " Trận";

            lbThua2.Text = ChessBoard.Nguoichoi[1].Dthua.ToString() + " Trận";

            lbHoa2.Text = ChessBoard.Nguoichoi[1].Dhoa.ToString() + " Trận";

            lbHoten2.Text = ChessBoard.Nguoichoi[1].Hoten;
        }

        private void ChessBoard_HoaGame(object sender, EventArgs e)
        {
            tmCooldown.Stop();
            pnelBanco.Enabled = false;
            MessageBox.Show("Hòa!");
            if (MessageBox.Show("Bạn có muốn ván mới?", "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                NewGame();
                
        }

        private void ChessBoard_PlayerMarked(object sender, EventArgs e)
        {
            tmCooldown.Start();
            prcbCooldown.Value = 0;
        }

        private void ChessBoard_EndedGame(object sender, EventArgs e)
        {
            EndGame();
        }

        public void EndGame()
        {
            tmCooldown.Stop();
            pnelBanco.Enabled = false;
            ChessBoard.Thongbao();
            if (MessageBox.Show("Bạn có muốn ván mới?", "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                NewGame();
        }

        public void NewGame()
        {
            prcbCooldown.Value = 0;
            tmCooldown.Stop();
            ChessBoard.oco();
            Loaddata();
        }

        void Exit()
        {
            Application.Exit();            
        }

   
        private void Banco_Load(object sender, EventArgs e)
        {

        }

        private void tmCooldown_Tick(object sender, EventArgs e)
        {
            prcbCooldown.PerformStep();
            if (prcbCooldown.Value >= prcbCooldown.Maximum)
            {
                ChessBoard.CurrentPlayer = ChessBoard.CurrentPlayer == 1 ? 0 : 1;
                EndGame();
            }
        }

        private void prcbCooldown_Click(object sender, EventArgs e)
        {

        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void Banco_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn thoát?", "Thông báo", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                e.Cancel = true;
        }

        private void btnLuotdi_Click(object sender, EventArgs e)
        {

        }
    }
}
