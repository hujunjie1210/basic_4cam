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
    public partial class db : Form
    {
        public db()
        {
            InitializeComponent();
        }

        private void db_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(18, 28, 43);
            this.TransparencyKey = Color.FromArgb(18, 28, 43);
            this.pictureBox1.Parent = this;
            this.pictureBox1.BackColor = Color.Transparent;
            this.dashboard.Parent = this.pictureBox1;
            this.Opacity = 0.5;
        }
    }
}
