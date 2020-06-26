using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using RCComm;
using DES_Sharp;
using logisteering;
using System.Net.NetworkInformation;
using System.IO;
using basic_remote_truck;
using System.Windows.Forms.DataVisualization.Charting;
using System.Security.Cryptography;

namespace basic_4cam
{
    public partial class dbgrand : Form
    {

        public dbgrand()
        {
            InitializeComponent();

            this.BackColor = Color.FromArgb(1, 255, 255);
            this.TransparencyKey = Color.FromArgb(1, 255, 255);
            this.pictureBox1.Parent = this;
            this.pictureBox5.Parent = this;
            this.pictureBox6.Parent = this;
            this.pictureBox8.Parent = this;
            this.pictureBox9.Parent = this;
            this.pictureBox1.BackColor = Color.Transparent;
            this.pictureBox5.BackColor = Color.Transparent;
            this.pictureBox6.BackColor = Color.Transparent;
            this.pictureBox8.BackColor = Color.Transparent;
            this.pictureBox9.BackColor = Color.Transparent;
            this.dashboard1.Parent = this;
            
        }


        private void dbgrand_Load(object sender, EventArgs e)
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.Url = new Uri(Path.Combine(Application.StartupPath, "mapbaidu.html"));
            webBrowser1.ObjectForScripting = this;

            var form1 = new Form1();
            this.Owner = form1;
            form1.Show();
            form1.ReciveData += form1_ReciveData;
            form1.choose += choose_car1;
            form1.map_loc += map_loc;

            var cir = new db();
            cir.Owner = form1;
            cir.ShowInTaskbar=false;
            cir.Show();
            var petit = new dbpetit();
            petit.Owner = form1;
            petit.ShowInTaskbar = false;
            petit.Show();

            pictureBox2.Visible = true;
            pictureBox3.Visible = false;
            pictureBox4.Visible = false;
            pictureBox7.Visible = false;
            pictureBox5.Visible = false;
            pictureBox6.Visible = false;
            pictureBox8.Visible = false;
            pictureBox9.Visible = false;

        }
        object[] ob = new object[2]; 
        void map_loc(double x, double y)
        {
            ob[0] = x;
            ob[1] = y;
            webBrowser1.Document.InvokeScript("setpos", ob);
        }
        void form1_ReciveData(short rpm,int speed,short geer)
        {
            dashboard1.Real = rpm / 80;
            //dashboard1.Real = speed;
            speedlabel.Text = Convert.ToString(speed);
            if (geer == 3)
            {
                pictureBox5.Visible = false;
                pictureBox6.Visible = false;
                pictureBox8.Visible = true;
                pictureBox9.Visible = false;
            }
            else if (geer == 2)
            {
                pictureBox5.Visible = false;
                pictureBox6.Visible = true;
                pictureBox8.Visible = false;
                pictureBox9.Visible = false;
            }
            else if (geer == 4)
            {
                pictureBox5.Visible = false;
                pictureBox6.Visible = false;
                pictureBox8.Visible = false;
                pictureBox9.Visible = true;
            }
            else
            {
                pictureBox5.Visible = false;
                pictureBox6.Visible = false;
                pictureBox8.Visible = true;
                pictureBox9.Visible = false;
            }
        }

        void choose_car1(int choice_car)
        {
            if (choice_car == 0)
            {
                pictureBox2.Visible = true;
                pictureBox3.Visible = false;
                pictureBox4.Visible = false;
                pictureBox7.Visible = false;
            }
            if (choice_car == 1)
            {
                pictureBox2.Visible = false;
                pictureBox3.Visible = true;
                pictureBox4.Visible = false;
                pictureBox7.Visible = false;
            }
            if (choice_car == 2)
            {
                pictureBox2.Visible = false;
                pictureBox3.Visible = false;
                pictureBox4.Visible = true;
                pictureBox7.Visible = false;
            }
            if (choice_car == 3)
            {
                pictureBox2.Visible = false;
                pictureBox3.Visible = false;
                pictureBox4.Visible = false;
                pictureBox7.Visible = true;
            }



        }


    }
}
