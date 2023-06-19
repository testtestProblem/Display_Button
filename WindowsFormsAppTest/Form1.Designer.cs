
namespace WindowsFormsAppTest
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dir = new System.Windows.Forms.Label();
            this.button = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.brightnessShow = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.keyCodeT = new System.Windows.Forms.Label();
            this.volumeT = new System.Windows.Forms.Label();
            this.connectStateT = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("PMingLiU", 15F);
            this.label1.Location = new System.Drawing.Point(132, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "direction";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("PMingLiU", 15F);
            this.label2.Location = new System.Drawing.Point(254, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "button state";
            // 
            // dir
            // 
            this.dir.AutoSize = true;
            this.dir.Font = new System.Drawing.Font("PMingLiU", 15F);
            this.dir.Location = new System.Drawing.Point(134, 42);
            this.dir.Name = "dir";
            this.dir.Size = new System.Drawing.Size(68, 20);
            this.dir.TabIndex = 5;
            this.dir.Text = "unknow";
            // 
            // button
            // 
            this.button.AutoSize = true;
            this.button.Font = new System.Drawing.Font("PMingLiU", 15F);
            this.button.Location = new System.Drawing.Point(256, 41);
            this.button.Name = "button";
            this.button.Size = new System.Drawing.Size(68, 20);
            this.button.TabIndex = 6;
            this.button.Text = "unknow";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("PMingLiU", 15F);
            this.label10.Location = new System.Drawing.Point(12, 9);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(83, 20);
            this.label10.TabIndex = 19;
            this.label10.Text = "brightness";
            // 
            // brightnessShow
            // 
            this.brightnessShow.AutoSize = true;
            this.brightnessShow.Font = new System.Drawing.Font("PMingLiU", 15F);
            this.brightnessShow.Location = new System.Drawing.Point(12, 42);
            this.brightnessShow.Name = "brightnessShow";
            this.brightnessShow.Size = new System.Drawing.Size(68, 20);
            this.brightnessShow.TabIndex = 20;
            this.brightnessShow.Text = "unknow";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("PMingLiU", 15F);
            this.label8.Location = new System.Drawing.Point(385, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(74, 20);
            this.label8.TabIndex = 26;
            this.label8.Text = "key code";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("PMingLiU", 15F);
            this.label9.Location = new System.Drawing.Point(501, 9);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(64, 20);
            this.label9.TabIndex = 27;
            this.label9.Text = "volume";
            // 
            // keyCodeT
            // 
            this.keyCodeT.AutoSize = true;
            this.keyCodeT.Font = new System.Drawing.Font("PMingLiU", 15F);
            this.keyCodeT.Location = new System.Drawing.Point(385, 42);
            this.keyCodeT.Name = "keyCodeT";
            this.keyCodeT.Size = new System.Drawing.Size(68, 20);
            this.keyCodeT.TabIndex = 28;
            this.keyCodeT.Text = "unknow";
            // 
            // volumeT
            // 
            this.volumeT.AutoSize = true;
            this.volumeT.Font = new System.Drawing.Font("PMingLiU", 15F);
            this.volumeT.Location = new System.Drawing.Point(501, 42);
            this.volumeT.Name = "volumeT";
            this.volumeT.Size = new System.Drawing.Size(68, 20);
            this.volumeT.TabIndex = 29;
            this.volumeT.Text = "unknow";
            // 
            // connectStateT
            // 
            this.connectStateT.AutoSize = true;
            this.connectStateT.Font = new System.Drawing.Font("PMingLiU", 15F);
            this.connectStateT.Location = new System.Drawing.Point(606, 41);
            this.connectStateT.Name = "connectStateT";
            this.connectStateT.Size = new System.Drawing.Size(68, 20);
            this.connectStateT.TabIndex = 32;
            this.connectStateT.Text = "unknow";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("PMingLiU", 15F);
            this.label6.Location = new System.Drawing.Point(606, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(103, 20);
            this.label6.TabIndex = 33;
            this.label6.Text = "connect state";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(712, 106);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.connectStateT);
            this.Controls.Add(this.volumeT);
            this.Controls.Add(this.keyCodeT);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.brightnessShow);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.button);
            this.Controls.Add(this.dir);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text = "PPC  Dashboard";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label dir;
        private System.Windows.Forms.Label button;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label brightnessShow;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label keyCodeT;
        private System.Windows.Forms.Label volumeT;
        private System.Windows.Forms.Label connectStateT;
        private System.Windows.Forms.Label label6;
    }
}

