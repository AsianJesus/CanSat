namespace CanSat_Desktop
{
    partial class Flight_List
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Flight_List));
            this.rtbId = new System.Windows.Forms.RichTextBox();
            this.rtbName = new System.Windows.Forms.RichTextBox();
            this.rtbSTime = new System.Windows.Forms.RichTextBox();
            this.rtbETime = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // rtbId
            // 
            this.rtbId.Font = new System.Drawing.Font("Trajan Pro 3", 14F, System.Drawing.FontStyle.Italic);
            this.rtbId.Location = new System.Drawing.Point(12, 26);
            this.rtbId.Name = "rtbId";
            this.rtbId.ReadOnly = true;
            this.rtbId.Size = new System.Drawing.Size(59, 418);
            this.rtbId.TabIndex = 0;
            this.rtbId.Text = "";
            // 
            // rtbName
            // 
            this.rtbName.Font = new System.Drawing.Font("Trajan Pro 3", 14F, System.Drawing.FontStyle.Italic);
            this.rtbName.Location = new System.Drawing.Point(77, 26);
            this.rtbName.Name = "rtbName";
            this.rtbName.ReadOnly = true;
            this.rtbName.Size = new System.Drawing.Size(139, 418);
            this.rtbName.TabIndex = 1;
            this.rtbName.Text = "";
            // 
            // rtbSTime
            // 
            this.rtbSTime.Font = new System.Drawing.Font("Trajan Pro 3", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbSTime.Location = new System.Drawing.Point(222, 26);
            this.rtbSTime.Name = "rtbSTime";
            this.rtbSTime.ReadOnly = true;
            this.rtbSTime.Size = new System.Drawing.Size(221, 417);
            this.rtbSTime.TabIndex = 2;
            this.rtbSTime.Text = "";
            // 
            // rtbETime
            // 
            this.rtbETime.Font = new System.Drawing.Font("Trajan Pro 3", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbETime.Location = new System.Drawing.Point(449, 26);
            this.rtbETime.Name = "rtbETime";
            this.rtbETime.ReadOnly = true;
            this.rtbETime.Size = new System.Drawing.Size(225, 417);
            this.rtbETime.TabIndex = 3;
            this.rtbETime.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(137, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(276, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Start time";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(555, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "End time";
            // 
            // Flight_List
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(686, 456);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rtbETime);
            this.Controls.Add(this.rtbSTime);
            this.Controls.Add(this.rtbName);
            this.Controls.Add(this.rtbId);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Flight_List";
            this.Text = "Flight_List";
            this.Load += new System.EventHandler(this.Flight_List_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbId;
        private System.Windows.Forms.RichTextBox rtbName;
        private System.Windows.Forms.RichTextBox rtbSTime;
        private System.Windows.Forms.RichTextBox rtbETime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}