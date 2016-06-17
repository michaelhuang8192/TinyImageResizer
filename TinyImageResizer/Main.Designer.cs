namespace TinyImageResizer
{
    partial class Main
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_size = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_quality = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label_numPending = new System.Windows.Forms.Label();
            this.label_errorCount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 19);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(160, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Dimension (In Pixels):";
            // 
            // textBox_size
            // 
            this.textBox_size.Location = new System.Drawing.Point(181, 16);
            this.textBox_size.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox_size.Name = "textBox_size";
            this.textBox_size.Size = new System.Drawing.Size(55, 26);
            this.textBox_size.TabIndex = 1;
            this.textBox_size.TabStop = false;
            this.textBox_size.Text = "1000";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(141, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Quality (JPG Only):";
            // 
            // textBox_quality
            // 
            this.textBox_quality.Location = new System.Drawing.Point(181, 54);
            this.textBox_quality.Name = "textBox_quality";
            this.textBox_quality.Size = new System.Drawing.Size(54, 26);
            this.textBox_quality.TabIndex = 3;
            this.textBox_quality.TabStop = false;
            this.textBox_quality.Text = "90";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.OliveDrab;
            this.label3.Location = new System.Drawing.Point(58, 107);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Pending:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(81, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 20);
            this.label4.TabIndex = 5;
            this.label4.Text = "Error:";
            // 
            // label_numPending
            // 
            this.label_numPending.AutoSize = true;
            this.label_numPending.Location = new System.Drawing.Point(140, 107);
            this.label_numPending.Name = "label_numPending";
            this.label_numPending.Size = new System.Drawing.Size(18, 20);
            this.label_numPending.TabIndex = 6;
            this.label_numPending.Text = "0";
            // 
            // label_errorCount
            // 
            this.label_errorCount.AutoSize = true;
            this.label_errorCount.Location = new System.Drawing.Point(140, 138);
            this.label_errorCount.Name = "label_errorCount";
            this.label_errorCount.Size = new System.Drawing.Size(18, 20);
            this.label_errorCount.TabIndex = 7;
            this.label_errorCount.Text = "0";
            // 
            // Main
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(260, 185);
            this.Controls.Add(this.label_errorCount);
            this.Controls.Add(this.label_numPending);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_quality);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_size);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.Text = "Tiny Image Resizer";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Main_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Main_DragEnter);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_size;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_quality;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label_numPending;
        private System.Windows.Forms.Label label_errorCount;
    }
}

