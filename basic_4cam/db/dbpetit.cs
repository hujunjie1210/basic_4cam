using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace basic_4cam
{
    public partial class dbpetit : Form
    {
        public dbpetit()
        {
            InitializeComponent();
            this.BackColor = Color.FromArgb(1, 255, 255);
            this.TransparencyKey = Color.FromArgb(1, 255, 255);
            this.pictureBox1.Parent = this;
            this.pictureBox1.BackColor = Color.Transparent;
            this.dashboardpetit1.Parent = this.pictureBox1;
            //this.Opacity = 0.5;
            dashboardpetit1.Real = 0;
        }

        private void dbpetit_Load(object sender, EventArgs e)
        {/*
            var form1 = new Form1();
            form1.Show();
            form1.get_speed += form1_getspeed;
            */
        }
        
    }
}
