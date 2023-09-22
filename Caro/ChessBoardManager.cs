using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Caro
{
    public class ChessBoardManager
    {
        #region Properties
        private SqlConnection conn;
        private SqlCommand comm;
        private Panel chessBoard;

        public Panel ChessBoard
        {
            get { return chessBoard; }
            set { chessBoard = value; }
        }

        private List<Nguoichoi> nguoichoi;
        internal List<Nguoichoi> Nguoichoi
        {
            get { return nguoichoi; }
            set { nguoichoi = value; }
        }


        private int currentPlayer;
        public int CurrentPlayer
        {
            get { return currentPlayer; }
            set { currentPlayer = value; }
        }

        private List<List<Button>> matrix;
        public List<List<Button>> Matrix
        {
            get { return matrix; }
            set { matrix = value; }
        }

        private event EventHandler playerMarked;
        public event EventHandler PlayerMarked
        {
            add
            {
                playerMarked += value;
            }
            remove
            {
                playerMarked -= value;
            }
        }

        private event EventHandler endedGame;
        public event EventHandler EndedGame
        {
            add
            {
                endedGame += value;
            }
            remove
            {
                endedGame -= value;
            }
        }

        private event EventHandler hoaGame;
        public event EventHandler HoaGame
        {
            add
            {
                hoaGame += value;
            }
            remove
            {
                hoaGame -= value;
            }
        }

        private Button btnLuotdi;

        public Button BtnLuotdi
        {
            get { return btnLuotdi; }
            set { btnLuotdi = value; }
        }

        #endregion

        #region Initialize
        public ChessBoardManager(Panel chessBoard, String user1, String user2, Button btnLuotdi)
        {
            this.ChessBoard = chessBoard;
            this.BtnLuotdi = btnLuotdi;
            CurrentPlayer = 0;
            
            int[] diemso = GetDiem(user1);
            int[] diemsoo = GetDiem(user2);
            this.Nguoichoi = new List<Nguoichoi>()
                    {
                        new Nguoichoi(user1,GetHoten(user1),diemso[0],diemso[1],diemso[2],Image.FromFile("D:\\VSCode\\Caro\\Caro\\Resources\\x.png")),
                        new Nguoichoi(user2,GetHoten(user2),diemsoo[0],diemsoo[1],diemsoo[2],Image.FromFile("D:\\VSCode\\Caro\\Caro\\Resources\\o.png"))
                    };
        }
       
        #endregion

        #region Methods
        public String GetHoten(String username)
        {
            conn = new SqlConnection("Server=localhost\\SQLEXPRESS;Initial Catalog=CARO;User Id=sa;pwd=sa2008;");
            comm = new SqlCommand("Select hoten From LOGIN where username='" + username + "'", conn);
            conn.Open();
            String hoten = Convert.ToString(comm.ExecuteScalar());
            conn.Close();
            return hoten;
        }

        public int[] GetDiem (String username){
            int[] bienMang = new int[3];
            conn = new SqlConnection("Server=localhost\\SQLEXPRESS;Initial Catalog=CARO;User Id=sa;pwd=sa2008;");
            comm = new SqlCommand("Select diemthang From POINT where username='" + username + "'", conn);
            conn.Open();
            bienMang[0] = Convert.ToInt32(comm.ExecuteScalar());
            comm = new SqlCommand("Select diemthua From POINT where username='" + username + "'", conn);
            bienMang[1] = Convert.ToInt32(comm.ExecuteScalar());
            comm = new SqlCommand("Select diemhoa From POINT where username='" + username + "'", conn);
            bienMang[2] = Convert.ToInt32(comm.ExecuteScalar());
            conn.Close();
            return bienMang;
        }

        int soquanco = 0;
        public void oco()
        {
            soquanco = 0;
            ChessBoard.Enabled = true;
            ChessBoard.Controls.Clear();
            Matrix = new List<List<Button>>();
            Button oldButton = new Button();
            for (int i = 1; i <= Setting.Banco_Size; i++)
            {
                Button btn1 = new Button()
                {
                    Width = Setting.Oco_Size,
                    Height = Setting.Oco_Size,
                    Enabled = false,
                    Font = new Font("Times New Roman", 10, FontStyle.Bold),
                    ForeColor = Color.Black,
                    BackColor = Color.White,
                    Location = new Point(oldButton.Location.X + i*(Setting.Oco_Size), oldButton.Location.Y),
                    Text = i.ToString()
                };

                ChessBoard.Controls.Add(btn1);
                Button btn2 = new Button()
                {
                    Width = Setting.Oco_Size,
                    Height = Setting.Oco_Size,
                    Enabled = false,
                    Font = new Font("Times New Roman", 10, FontStyle.Bold),
                    ForeColor = Color.Black,
                    BackColor = Color.White,
                    Location = new Point(oldButton.Location.X, oldButton.Location.Y + i * (Setting.Oco_Size)),
                    Text = i.ToString()
                };
                ChessBoard.Controls.Add(btn2);
            }


            Button old = new Button() { Width = 0, Location = new Point(oldButton.Location.X + Setting.Oco_Size, oldButton.Location.Y + Setting.Oco_Size) };
            for (int i = 0; i < Setting.Banco_Size; i++)
            {
                Matrix.Add(new List<Button>());
                for (int j = 0; j < Setting.Banco_Size+1; j++)
                {
                    Button btn = new Button()
                    {
                        Width = Setting.Oco_Size,
                        Height = Setting.Oco_Size,
                        Location = new Point(old.Location.X + old.Width, old.Location.Y),
                        BackgroundImageLayout = ImageLayout.Stretch,
                        Tag = i.ToString()
 
                    };

                    btn.Click += btn_Click;

                    Matrix[i].Add(btn);

                    ChessBoard.Controls.Add(btn);

                    old = btn;
                    
                }
                old.Location = new Point(Setting.Oco_Size, old.Location.Y + Setting.Oco_Size);
                old.Width = 0;
                old.Height = 0;
            }
        }

        void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
           
            if (btn.BackgroundImage != null)
                return;
                            
            btn.BackgroundImage = Nguoichoi[CurrentPlayer].Mark;

            if (playerMarked != null)
                playerMarked(this, new EventArgs());

            soquanco++;

            if (isEndgame(btn))
            {
                Endgame();
            }

            if (soquanco >= Setting.Banco_Size * Setting.Banco_Size)
            {
                Nguoichoi[1].Dhoa++;
                Nguoichoi[0].Dhoa++;
                UpdateCSDL(Nguoichoi[0]);
                UpdateCSDL(Nguoichoi[1]);
                if (hoaGame != null)
                    hoaGame(this, new EventArgs());
            }

            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
            BtnLuotdi.BackgroundImage = Nguoichoi[CurrentPlayer].Mark;
            if (CurrentPlayer == 1 && Nguoichoi[1].Username == "Computer")
            {
                Maydanhco();
                soquanco++;
            }

        }

        public void Endgame()
        {
            if (endedGame != null)
                endedGame(this, new EventArgs());
        }

        public void Thongbao()
        {
            Nguoichoi[CurrentPlayer].Dthang++;
            Nguoichoi[CurrentPlayer == 1 ? 0 : 1].Dthua++;
            UpdateCSDL(Nguoichoi[0]);
            UpdateCSDL(Nguoichoi[1]);
            MessageBox.Show("Game over! " + Nguoichoi[CurrentPlayer].Hoten + " Win!");
        }

        private bool isEndgame(Button btn)
        {
            return Checkhang(btn) || Checkcot(btn) || Cheochinh(btn) || Cheophu(btn);
        }

        private Point GetChessPoint(Button btn)
        {
            int cot = Convert.ToInt32(btn.Tag);
            int hang = Matrix[cot].IndexOf(btn);
            Point point = new Point(cot, hang);
            return point;
        }

        public void Maydanhco()
        {
            int DiemMax = 0;
            int DiemPhongNgu = 0;
            int DiemTanCong = 0;
            int hang = 0;
            int cot = 0;
     
                for (int i = 0; i < Setting.Banco_Size; i++)
                {
                    for (int j = 0; j < Setting.Banco_Size; j++)
                    {

                        if (Matrix[j][i].BackgroundImage == null)
                        {
                            int DiemTam;

                            DiemTanCong = duyetTC_Ngang(j, i) + duyetTC_Doc(j, i) + duyetTC_CheoXuoi(j, i) + duyetTC_CheoNguoc(j, i);
                            DiemPhongNgu = duyetPN_Ngang(j, i) + duyetPN_Doc(j, i) + duyetPN_CheoXuoi(j, i) + duyetPN_CheoNguoc(j, i);


                            if (DiemPhongNgu > DiemTanCong)
                            {
                                DiemTam = DiemPhongNgu;
                            }
                            else
                            {
                                DiemTam = DiemTanCong;
                            }

                            if (DiemMax < DiemTam)
                            {
                                DiemMax = DiemTam;
                                hang = j;
                                cot = i;
                            }
                        }
                    }
                }
            if (soquanco == 0){
                Danhco(Setting.Banco_Size/2, Setting.Banco_Size/2);
            }
            else
            {
                Danhco(hang, cot);
            }
            
        }

        public void Danhco(int i, int j)
        {
            Matrix[i][j].BackgroundImage = Nguoichoi[1].Mark;
            if (isEndgame(Matrix[i][j]))
            {
                Endgame();
            }
            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
            BtnLuotdi.BackgroundImage = Nguoichoi[CurrentPlayer].Mark;
        }

        private int[] MangDiemTanCong = new int[7] { 0, 4, 25, 246, 7300, 6561, 59049 };
        private int[] MangDiemPhongNgu = new int[7] { 0, 3, 24, 243, 2197, 19773, 177957 };

        //Duyệt điểm tấn công
        public int duyetTC_Ngang(int dongHT, int cotHT)
        {
            int DiemTanCong = 0;
            int SoQuanTa = 0;
            int SoQuanDichPhai = 0;
            int SoQuanDichTrai = 0;
            int KhoangTrong = 0;

            //bên phải
            for (int dem = 1; dem <= 4 && cotHT < Setting.Banco_Size - 5; dem++)
            {

                if (Matrix[dongHT][cotHT + dem].BackgroundImage == Nguoichoi[1].Mark)
                {
                    if (dem == 1)
                        DiemTanCong += 37;

                    SoQuanTa++;
                    KhoangTrong++;
                }
                else
                    if (Matrix[dongHT][cotHT + dem].BackgroundImage == Nguoichoi[0].Mark)
                    {
                        SoQuanDichPhai++;
                        break;
                    }
                    else KhoangTrong++;
            }
            //bên trái
            for (int dem = 1; dem <= 4 && cotHT > 4; dem++)
            {
                if (Matrix[dongHT][cotHT - dem].BackgroundImage == Nguoichoi[1].Mark)
                {
                    if (dem == 1)
                        DiemTanCong += 37;

                    SoQuanTa++;
                    KhoangTrong++;

                }
                else
                    if (Matrix[dongHT][cotHT - dem].BackgroundImage == Nguoichoi[0].Mark)
                    {
                        SoQuanDichTrai++;
                        break;
                    }
                    else KhoangTrong++;
            }
            //bị chặn 2 đầu khoảng chống không đủ tạo thành 5 nước
            if (SoQuanDichPhai > 0 && SoQuanDichTrai > 0 && KhoangTrong < 4)
                return 0;

            DiemTanCong -= MangDiemPhongNgu[SoQuanDichPhai + SoQuanDichTrai];
            DiemTanCong += MangDiemTanCong[SoQuanTa];
            return DiemTanCong;
        }

        public int duyetTC_Doc(int dongHT, int cotHT)
        {
            int DiemTanCong = 0;
            int SoQuanTa = 0;
            int SoQuanDichTren = 0;
            int SoQuanDichDuoi = 0;
            int KhoangTrong = 0;

            //bên trên
            for (int dem = 1; dem <= 4 && dongHT > 4; dem++)
            {
                if (Matrix[dongHT - dem][cotHT].BackgroundImage == Nguoichoi[1].Mark)
                {
                    if (dem == 1)
                        DiemTanCong += 37;

                    SoQuanTa++;
                    KhoangTrong++;

                }
                else
                    if (Matrix[dongHT - dem][cotHT].BackgroundImage == Nguoichoi[0].Mark)
                    {
                        SoQuanDichTren++;
                        break;
                    }
                    else KhoangTrong++;
            }
            //bên dưới
            for (int dem = 1; dem <= 4 && dongHT < Setting.Banco_Size - 5; dem++)
            {
                if (Matrix[dongHT + dem][cotHT].BackgroundImage == Nguoichoi[1].Mark)
                {
                    if (dem == 1)
                        DiemTanCong += 37;

                    SoQuanTa++;
                    KhoangTrong++;

                }
                else
                    if (Matrix[dongHT + dem][cotHT].BackgroundImage == Nguoichoi[0].Mark)
                    {
                        SoQuanDichDuoi++;
                        break;
                    }
                    else KhoangTrong++;
            }
            //bị chặn 2 đầu khoảng chống không đủ tạo thành 5 nước
            if (SoQuanDichTren > 0 && SoQuanDichDuoi > 0 && KhoangTrong < 4)
                return 0;

            DiemTanCong -= MangDiemPhongNgu[SoQuanDichTren + SoQuanDichDuoi];
            DiemTanCong += MangDiemTanCong[SoQuanTa];
            return DiemTanCong;
        }

        public int duyetTC_CheoXuoi(int dongHT, int cotHT)
        {
            int DiemTanCong = 1;
            int SoQuanTa = 0;
            int SoQuanDichCheoTren = 0;
            int SoQuanDichCheoDuoi = 0;
            int KhoangTrong = 0;

            //bên chéo xuôi xuống
            for (int dem = 1; dem <= 4 && cotHT < Setting.Banco_Size - 5 && dongHT < Setting.Banco_Size - 5; dem++)
            {
                if (Matrix[dongHT + dem][cotHT + dem].BackgroundImage == Nguoichoi[1].Mark)
                {
                    if (dem == 1)
                        DiemTanCong += 37;

                    SoQuanTa++;
                    KhoangTrong++;

                }
                else
                    if (Matrix[dongHT + dem][cotHT + dem].BackgroundImage == Nguoichoi[0].Mark)
                    {
                        SoQuanDichCheoTren++;
                        break;
                    }
                    else KhoangTrong++;
            }
            //chéo xuôi lên
            for (int dem = 1; dem <= 4 && dongHT > 4 && cotHT > 4; dem++)
            {
                if (Matrix[dongHT - dem][cotHT - dem].BackgroundImage == Nguoichoi[1].Mark)
                {
                    if (dem == 1)
                        DiemTanCong += 37;

                    SoQuanTa++;
                    KhoangTrong++;

                }
                else
                    if (Matrix[dongHT - dem][cotHT - dem].BackgroundImage == Nguoichoi[0].Mark)
                    {
                        SoQuanDichCheoDuoi++;
                        break;
                    }
                    else KhoangTrong++;
            }
            //bị chặn 2 đầu khoảng chống không đủ tạo thành 5 nước
            if (SoQuanDichCheoTren > 0 && SoQuanDichCheoDuoi > 0 && KhoangTrong < 4)
                return 0;

            DiemTanCong -= MangDiemPhongNgu[SoQuanDichCheoTren + SoQuanDichCheoDuoi];
            DiemTanCong += MangDiemTanCong[SoQuanTa];
            return DiemTanCong;
        }

        public int duyetTC_CheoNguoc(int dongHT, int cotHT)
        {
            int DiemTanCong = 0;
            int SoQuanTa = 0;
            int SoQuanDichCheoTren = 0;
            int SoQuanDichCheoDuoi = 0;
            int KhoangTrong = 0;

            //chéo ngược lên
            for (int dem = 1; dem <= 4 && cotHT < Setting.Banco_Size - 5 && dongHT > 4; dem++)
            {
                if (Matrix[dongHT - dem][cotHT + dem].BackgroundImage == Nguoichoi[1].Mark)
                {
                    if (dem == 1)
                        DiemTanCong += 37;

                    SoQuanTa++;
                    KhoangTrong++;

                }
                else
                    if (Matrix[dongHT - dem][cotHT + dem].BackgroundImage == Nguoichoi[0].Mark)
                    {
                        SoQuanDichCheoTren++;
                        break;
                    }
                    else KhoangTrong++;
            }
            //chéo ngược xuống
            for (int dem = 1; dem <= 4 && cotHT > 4 && dongHT < Setting.Banco_Size - 5; dem++)
            {
                if (Matrix[dongHT + dem][cotHT - dem].BackgroundImage == Nguoichoi[1].Mark)
                {
                    if (dem == 1)
                        DiemTanCong += 37;

                    SoQuanTa++;
                    KhoangTrong++;

                }
                else
                    if (Matrix[dongHT + dem][cotHT - dem].BackgroundImage == Nguoichoi[0].Mark)
                    {
                        SoQuanDichCheoDuoi++;
                        break;
                    }
                    else KhoangTrong++;
            }
            //bị chặn 2 đầu khoảng chống không đủ tạo thành 5 nước
            if (SoQuanDichCheoTren > 0 && SoQuanDichCheoDuoi > 0 && KhoangTrong < 4)
                return 0;

            DiemTanCong -= MangDiemPhongNgu[SoQuanDichCheoTren + SoQuanDichCheoDuoi];
            DiemTanCong += MangDiemTanCong[SoQuanTa];
            return DiemTanCong;
        }

        //Duyệt điểm phòng ngự
        public int duyetPN_Ngang(int dongHT, int cotHT)
        {
            int DiemPhongNgu = 0;
            int SoQuanTaTrai = 0;
            int SoQuanTaPhai = 0;
            int SoQuanDich = 0;
            int KhoangTrongPhai = 0;
            int KhoangTrongTrai = 0;
            bool ok = false;


            for (int dem = 1; dem <= 4 && cotHT < Setting.Banco_Size - 5; dem++)
            {
                if (Matrix[dongHT][cotHT + dem].BackgroundImage == Nguoichoi[0].Mark)
                {
                    if (dem == 1)
                        DiemPhongNgu += 9;

                    SoQuanDich++;
                }
                else
                    if (Matrix[dongHT][cotHT + dem].BackgroundImage == Nguoichoi[1].Mark)
                    {
                        if (dem == 4)
                            DiemPhongNgu -= 170;

                        SoQuanTaTrai++;
                        break;
                    }
                    else
                    {
                        if (dem == 1)
                            ok = true;

                        KhoangTrongPhai++;
                    }
            }

            if (SoQuanDich == 3 && KhoangTrongPhai == 1 && ok)
                DiemPhongNgu -= 200;

            ok = false;

            for (int dem = 1; dem <= 4 && cotHT > 4; dem++)
            {
                if (Matrix[dongHT][cotHT - dem].BackgroundImage == Nguoichoi[0].Mark)
                {
                    if (dem == 1)
                        DiemPhongNgu += 9;

                    SoQuanDich++;
                }
                else
                    if (Matrix[dongHT][cotHT - dem].BackgroundImage == Nguoichoi[1].Mark)
                    {
                        if (dem == 4)
                            DiemPhongNgu -= 170;

                        SoQuanTaPhai++;
                        break;
                    }
                    else
                    {
                        if (dem == 1)
                            ok = true;

                        KhoangTrongTrai++;
                    }
            }

            if (SoQuanDich == 3 && KhoangTrongTrai == 1 && ok)
                DiemPhongNgu -= 200;

            if (SoQuanTaPhai > 0 && SoQuanTaTrai > 0 && (KhoangTrongTrai + KhoangTrongPhai + SoQuanDich) < 4)
                return 0;

            DiemPhongNgu -= MangDiemTanCong[SoQuanTaPhai + SoQuanTaPhai];
            DiemPhongNgu += MangDiemPhongNgu[SoQuanDich];

            return DiemPhongNgu;
        }

        public int duyetPN_Doc(int dongHT, int cotHT)
        {
            int DiemPhongNgu = 0;
            int SoQuanTaTrai = 0;
            int SoQuanTaPhai = 0;
            int SoQuanDich = 0;
            int KhoangTrongTren = 0;
            int KhoangTrongDuoi = 0;
            bool ok = false;

            //lên
            for (int dem = 1; dem <= 4 && dongHT > 4; dem++)
            {
                if (Matrix[dongHT - dem][cotHT].BackgroundImage == Nguoichoi[0].Mark)
                {
                    if (dem == 1)
                        DiemPhongNgu += 9;

                    SoQuanDich++;

                }
                else
                    if (Matrix[dongHT - dem][cotHT].BackgroundImage == Nguoichoi[1].Mark)
                    {
                        if (dem == 4)
                            DiemPhongNgu -= 170;

                        SoQuanTaPhai++;
                        break;
                    }
                    else
                    {
                        if (dem == 1)
                            ok = true;

                        KhoangTrongTren++;
                    }
            }

            if (SoQuanDich == 3 && KhoangTrongTren == 1 && ok)
                DiemPhongNgu -= 200;

            ok = false;
            //xuống
            for (int dem = 1; dem <= 4 && dongHT < Setting.Banco_Size - 5; dem++)
            {
                //gặp quân địch
                if (Matrix[dongHT + dem][cotHT].BackgroundImage == Nguoichoi[0].Mark)
                {
                    if (dem == 1)
                        DiemPhongNgu += 9;

                    SoQuanDich++;
                }
                else
                    if (Matrix[dongHT + dem][cotHT].BackgroundImage == Nguoichoi[1].Mark)
                    {
                        if (dem == 4)
                            DiemPhongNgu -= 170;

                        SoQuanTaTrai++;
                        break;
                    }
                    else
                    {
                        if (dem == 1)
                            ok = true;

                        KhoangTrongDuoi++;
                    }
            }

            if (SoQuanDich == 3 && KhoangTrongDuoi == 1 && ok)
                DiemPhongNgu -= 200;

            if (SoQuanTaPhai > 0 && SoQuanTaTrai > 0 && (KhoangTrongTren + KhoangTrongDuoi + SoQuanDich) < 4)
                return 0;

            DiemPhongNgu -= MangDiemTanCong[SoQuanTaTrai + SoQuanTaPhai];
            DiemPhongNgu += MangDiemPhongNgu[SoQuanDich];
            return DiemPhongNgu;
        }

        public int duyetPN_CheoXuoi(int dongHT, int cotHT)
        {
            int DiemPhongNgu = 0;
            int SoQuanTaTrai = 0;
            int SoQuanTaPhai = 0;
            int SoQuanDich = 0;
            int KhoangTrongTren = 0;
            int KhoangTrongDuoi = 0;
            bool ok = false;

            //lên
            for (int dem = 1; dem <= 4 && dongHT < Setting.Banco_Size - 5 && cotHT < Setting.Banco_Size - 5; dem++)
            {
                if (Matrix[dongHT + dem][cotHT + dem].BackgroundImage == Nguoichoi[0].Mark)
                {
                    if (dem == 1)
                        DiemPhongNgu += 9;

                    SoQuanDich++;
                }
                else
                    if (Matrix[dongHT + dem][cotHT + dem].BackgroundImage == Nguoichoi[1].Mark)
                    {
                        if (dem == 4)
                            DiemPhongNgu -= 170;

                        SoQuanTaPhai++;
                        break;
                    }
                    else
                    {
                        if (dem == 1)
                            ok = true;

                        KhoangTrongTren++;
                    }
            }

            if (SoQuanDich == 3 && KhoangTrongTren == 1 && ok)
                DiemPhongNgu -= 200;

            ok = false;
            //xuống
            for (int dem = 1; dem <= 4 && dongHT > 4 && cotHT > 4; dem++)
            {
                if (Matrix[dongHT - dem][cotHT - dem].BackgroundImage == Nguoichoi[0].Mark)
                {
                    if (dem == 1)
                        DiemPhongNgu += 9;

                    SoQuanDich++;
                }
                else
                    if (Matrix[dongHT - dem][cotHT - dem].BackgroundImage == Nguoichoi[1].Mark)
                    {
                        if (dem == 4)
                            DiemPhongNgu -= 170;

                        SoQuanTaTrai++;
                        break;
                    }
                    else
                    {
                        if (dem == 1)
                            ok = true;

                        KhoangTrongDuoi++;
                    }
            }

            if (SoQuanDich == 3 && KhoangTrongDuoi == 1 && ok)
                DiemPhongNgu -= 200;

            if (SoQuanTaPhai > 0 && SoQuanTaTrai > 0 && (KhoangTrongTren + KhoangTrongDuoi + SoQuanDich) < 4)
                return 0;

            DiemPhongNgu -= MangDiemTanCong[SoQuanTaPhai + SoQuanTaTrai];
            DiemPhongNgu += MangDiemPhongNgu[SoQuanDich];

            return DiemPhongNgu;
        }

        public int duyetPN_CheoNguoc(int dongHT, int cotHT)
        {
            int DiemPhongNgu = 0;
            int SoQuanTaTrai = 0;
            int SoQuanTaPhai = 0;
            int SoQuanDich = 0;
            int KhoangTrongTren = 0;
            int KhoangTrongDuoi = 0;
            bool ok = false;

            //lên
            for (int dem = 1; dem <= 4 && dongHT > 4 && cotHT < Setting.Banco_Size - 5; dem++)
            {

                if (Matrix[dongHT - dem][cotHT + dem].BackgroundImage == Nguoichoi[0].Mark)
                {
                    if (dem == 1)
                        DiemPhongNgu += 9;

                    SoQuanDich++;
                }
                else
                    if (Matrix[dongHT - dem][cotHT + dem].BackgroundImage == Nguoichoi[1].Mark)
                    {
                        if (dem == 4)
                            DiemPhongNgu -= 170;

                        SoQuanTaPhai++;
                        break;
                    }
                    else
                    {
                        if (dem == 1)
                            ok = true;

                        KhoangTrongTren++;
                    }
            }


            if (SoQuanDich == 3 && KhoangTrongTren == 1 && ok)
                DiemPhongNgu -= 200;

            ok = false;

            //xuống
            for (int dem = 1; dem <= 4 && dongHT < Setting.Banco_Size - 5 && cotHT > 4; dem++)
            {
                if (Matrix[dongHT + dem][cotHT - dem].BackgroundImage == Nguoichoi[0].Mark)
                {
                    if (dem == 1)
                        DiemPhongNgu += 9;

                    SoQuanDich++;
                }
                else
                    if (Matrix[dongHT + dem][cotHT - dem].BackgroundImage == Nguoichoi[1].Mark)
                    {
                        if (dem == 4)
                            DiemPhongNgu -= 170;

                        SoQuanTaTrai++;
                        break;
                    }
                    else
                    {
                        if (dem == 1)
                            ok = true;

                        KhoangTrongDuoi++;
                    }
            }

            if (SoQuanDich == 3 && KhoangTrongDuoi == 1 && ok)
                DiemPhongNgu -= 200;

            if (SoQuanTaPhai > 0 && SoQuanTaTrai > 0 && (KhoangTrongTren + KhoangTrongDuoi + SoQuanDich) < 4)
                return 0;

            DiemPhongNgu -= MangDiemTanCong[SoQuanTaTrai + SoQuanTaPhai];
            DiemPhongNgu += MangDiemPhongNgu[SoQuanDich];

            return DiemPhongNgu;
        }



        //Kiểm tra chiến thắng
        public bool Checkhang(Button btn)
        {
            Point point = GetChessPoint(btn);
            int countLeft = 0;
            for (int i = point.Y; i >= 0; i--)
            {
                if (Matrix[point.X][i].BackgroundImage == btn.BackgroundImage)
                {
                    countLeft++;
                }
                else
                    break;
            }
            int countRight = 0;
            for (int i = point.Y+1; i < Setting.Banco_Size; i++)
            {
                if (Matrix[point.X][i].BackgroundImage == btn.BackgroundImage)
                {
                    countRight++;
                }
                else
                    break;
            }
            return countLeft + countRight >= 5;
        }

        private bool Checkcot(Button btn)
        {
            Point point = GetChessPoint(btn);
            int countTop = 0;
            for (int i = point.X; i >= 0; i--)
            {
                if (Matrix[i][point.Y].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                    break;
            }
            int countBot = 0;
            for (int i = point.X + 1; i < Setting.Banco_Size; i++)
            {
                if (Matrix[i][point.Y].BackgroundImage == btn.BackgroundImage)
                {
                    countBot++;
                }
                else
                    break;
            }
            return countTop + countBot >= 5;
        }

        private bool Cheochinh(Button btn)
        {
            Point point = GetChessPoint(btn);
            int countTop = 0;
            for (int i = 0; i <= 5; i++)
            {
                if (point.X - i < 0 || point.Y - i < 0)
                    break;

                if (Matrix[point.X - i][point.Y - i].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                    break;
            }
            int countBot = 0;
            for (int i = 1; i <= 5; i++)
            {
                if (point.X + i >= Setting.Banco_Size || point.Y + i >= Setting.Banco_Size)
                    break;
                if (Matrix[point.X +i][point.Y + i].BackgroundImage == btn.BackgroundImage)
                {
                    countBot++;
                }
                else
                    break;
            }
            return countTop + countBot >= 5;
        }

        private bool Cheophu(Button btn)
        {
            Point point = GetChessPoint(btn);

            int countTopp = 0;
            for (int i = 0; i <= 5; i++)
            {
                if (point.X + i >= Setting.Banco_Size || point.Y - i < 0)
                    break;
               
                if (Matrix[point.X + i][point.Y - i].BackgroundImage == btn.BackgroundImage)
                {
                    countTopp++;
                }
                else
                    break;
            }
            
            int countBott = 0;
            for (int i = 1; i <= 5; i++)
            {
                if (point.X - i < 0 || point.Y + i >= Setting.Banco_Size)
                {
                    break;
                }
                    
                if (Matrix[point.X - i][point.Y + i].BackgroundImage == btn.BackgroundImage)
                {
                    countBott++;
                }
                else
                    break;
            }
            return countTopp + countBott >= 5;
        }
        
        private void UpdateCSDL(Nguoichoi Player)
        {
            try
            {
                conn = new SqlConnection("Server=localhost\\SQLEXPRESS;Initial Catalog=CARO;User Id=sa;pwd=sa2008;");
                comm = new SqlCommand("UPDATE POINT SET diemthang = "+Player.Dthang+",diemthua = "+Player.Dthua+",diemhoa = "+Player.Dhoa+" Where username = '"+Player.Username+"';", conn);
                conn.Open();
                comm.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }
        #endregion
    }
}
