namespace ONVIFPTZControl
{
    partial class Form1
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
            this.AlertInMB = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.SendTo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // AlertInMB
            // 
            this.AlertInMB.AutoSize = true;
            this.AlertInMB.Location = new System.Drawing.Point(12, 9);
            this.AlertInMB.Name = "AlertInMB";
            this.AlertInMB.Size = new System.Drawing.Size(60, 13);
            this.AlertInMB.TabIndex = 0;
            this.AlertInMB.Text = "Alert In KB:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(80, 6);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(50, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "200";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(80, 36);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(141, 20);
            this.textBox2.TabIndex = 3;
            this.textBox2.Text = "a0506800201@gmail.com";
            // 
            // SendTo
            // 
            this.SendTo.AutoSize = true;
            this.SendTo.Location = new System.Drawing.Point(12, 39);
            this.SendTo.Name = "SendTo";
            this.SendTo.Size = new System.Drawing.Size(51, 13);
            this.SendTo.TabIndex = 2;
            this.SendTo.Text = "Send To:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(242, 82);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.SendTo);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.AlertInMB);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label AlertInMB;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label SendTo;
    }
}

