namespace basic_4cam
{
    partial class dbpetit
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.dashboardpetit1 = new petit.Dashboardpetit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(3440, 1440);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // dashboardpetit1
            // 
            this.dashboardpetit1.BottomTitleColor = System.Drawing.Color.Blue;
            this.dashboardpetit1.BottomTitleFont = new System.Drawing.Font("微软雅黑", 16F);
            this.dashboardpetit1.Expected = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.dashboardpetit1.IndicatorAngle = 10;
            this.dashboardpetit1.IndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(254)))), ((int)(((byte)(254)))));
            this.dashboardpetit1.InnerBackground = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.dashboardpetit1.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(106)))), ((int)(((byte)(168)))));
            this.dashboardpetit1.InnerRoundColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(254)))), ((int)(((byte)(254)))));
            this.dashboardpetit1.Location = new System.Drawing.Point(2170, 1250);
            this.dashboardpetit1.Name = "dashboardpetit1";
            this.dashboardpetit1.OuterColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(151)))), ((int)(((byte)(254)))));
            this.dashboardpetit1.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(254)))), ((int)(((byte)(254)))));
            this.dashboardpetit1.Real = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.dashboardpetit1.ScaleExpectedColor = System.Drawing.Color.Transparent;
            this.dashboardpetit1.ScaleRealColor = System.Drawing.Color.Yellow;
            this.dashboardpetit1.Size = new System.Drawing.Size(160, 160);
            this.dashboardpetit1.TabIndex = 0;
            this.dashboardpetit1.Text = "dashboardpetit1";
            // 
            // dbpetit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(3440, 1440);
            this.Controls.Add(this.dashboardpetit1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "dbpetit";
            this.Text = "dbpetit";
            this.Load += new System.EventHandler(this.dbpetit_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private petit.Dashboardpetit dashboardpetit1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}