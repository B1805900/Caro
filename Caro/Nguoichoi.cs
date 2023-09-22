using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caro
{
    class Nguoichoi
    {
        private String hoten;

        public String Hoten
        {
            get { return hoten; }
            set { hoten = value; }
        }

        private String username;

        public String Username
        {
            get { return username; }
            set { username = value; }
        }

        private int dthang;

        public int Dthang
        {
            get { return dthang; }
            set { dthang = value; }
        }

        private int dthua;

        public int Dthua
        {
            get { return dthua; }
            set { dthua = value; }
        }

        private int dhoa;

        public int Dhoa
        {
            get { return dhoa; }
            set { dhoa = value; }
        }

        private Image mark;

        public Image Mark
        {
            get { return mark; }
            set { mark = value; }
        }

        public Nguoichoi(String username, String hoten, int dthang, int dthua, int dhoa, Image mark)
        {
            this.Username = username;
            this.Hoten = hoten;
            this.Dthang = dthang;
            this.Dthua = dthua;
            this.Dhoa = dhoa;
            this.Mark = mark;
        }
    }
}
