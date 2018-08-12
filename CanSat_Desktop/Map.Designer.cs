﻿namespace CanSat_Desktop
{
    partial class wfMap
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
            this.pbMap = new System.Windows.Forms.PictureBox();
            this.tbGpsX = new System.Windows.Forms.TextBox();
            this.tbGpsY = new System.Windows.Forms.TextBox();
            this.btnAddMarker = new System.Windows.Forms.Button();
            this.rtbMarkers = new System.Windows.Forms.RichTextBox();
            this.cbColor = new System.Windows.Forms.ComboBox();
            this.lMarkers = new System.Windows.Forms.Label();
            this.lColor = new System.Windows.Forms.Label();
            this.lGpsX = new System.Windows.Forms.Label();
            this.lGpsY = new System.Windows.Forms.Label();
            this.cbMarkers = new System.Windows.Forms.ComboBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnZoomMinus = new System.Windows.Forms.Button();
            this.btnZoomPlus = new System.Windows.Forms.Button();
            this.lZoom = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbMap)).BeginInit();
            this.SuspendLayout();
            // 
            // pbMap
            // 
            this.pbMap.Location = new System.Drawing.Point(12, 12);
            this.pbMap.Name = "pbMap";
            this.pbMap.Size = new System.Drawing.Size(500, 500);
            this.pbMap.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbMap.TabIndex = 0;
            this.pbMap.TabStop = false;
            // 
            // tbGpsX
            // 
            this.tbGpsX.Location = new System.Drawing.Point(569, 433);
            this.tbGpsX.Name = "tbGpsX";
            this.tbGpsX.Size = new System.Drawing.Size(146, 20);
            this.tbGpsX.TabIndex = 1;
            // 
            // tbGpsY
            // 
            this.tbGpsY.Location = new System.Drawing.Point(569, 455);
            this.tbGpsY.Name = "tbGpsY";
            this.tbGpsY.Size = new System.Drawing.Size(146, 20);
            this.tbGpsY.TabIndex = 2;
            // 
            // btnAddMarker
            // 
            this.btnAddMarker.Location = new System.Drawing.Point(640, 481);
            this.btnAddMarker.Name = "btnAddMarker";
            this.btnAddMarker.Size = new System.Drawing.Size(75, 23);
            this.btnAddMarker.TabIndex = 4;
            this.btnAddMarker.Text = "Add marker";
            this.btnAddMarker.UseVisualStyleBackColor = true;
            this.btnAddMarker.Click += new System.EventHandler(this.btnAddMarker_Click);
            // 
            // rtbMarkers
            // 
            this.rtbMarkers.Location = new System.Drawing.Point(518, 12);
            this.rtbMarkers.Name = "rtbMarkers";
            this.rtbMarkers.ReadOnly = true;
            this.rtbMarkers.Size = new System.Drawing.Size(197, 273);
            this.rtbMarkers.TabIndex = 5;
            this.rtbMarkers.Text = "";
            // 
            // cbColor
            // 
            this.cbColor.FormattingEnabled = true;
            this.cbColor.Items.AddRange(new object[] {
            "Black",
            "Green",
            "Red",
            "Magenta",
            "Pink",
            "Grey",
            "Blue",
            "Orange",
            "Yellow"});
            this.cbColor.Location = new System.Drawing.Point(569, 406);
            this.cbColor.Name = "cbColor";
            this.cbColor.Size = new System.Drawing.Size(146, 21);
            this.cbColor.TabIndex = 6;
            this.cbColor.Text = "Black";
            // 
            // lMarkers
            // 
            this.lMarkers.AutoSize = true;
            this.lMarkers.Location = new System.Drawing.Point(616, 315);
            this.lMarkers.Name = "lMarkers";
            this.lMarkers.Size = new System.Drawing.Size(45, 13);
            this.lMarkers.TabIndex = 7;
            this.lMarkers.Text = "Markers";
            // 
            // lColor
            // 
            this.lColor.AutoSize = true;
            this.lColor.Location = new System.Drawing.Point(523, 406);
            this.lColor.Name = "lColor";
            this.lColor.Size = new System.Drawing.Size(31, 13);
            this.lColor.TabIndex = 8;
            this.lColor.Text = "Color";
            // 
            // lGpsX
            // 
            this.lGpsX.AutoSize = true;
            this.lGpsX.Location = new System.Drawing.Point(515, 436);
            this.lGpsX.Name = "lGpsX";
            this.lGpsX.Size = new System.Drawing.Size(39, 13);
            this.lGpsX.TabIndex = 9;
            this.lGpsX.Text = "GPS X";
            // 
            // lGpsY
            // 
            this.lGpsY.AutoSize = true;
            this.lGpsY.Location = new System.Drawing.Point(515, 458);
            this.lGpsY.Name = "lGpsY";
            this.lGpsY.Size = new System.Drawing.Size(39, 13);
            this.lGpsY.TabIndex = 10;
            this.lGpsY.Text = "GPS Y";
            // 
            // cbMarkers
            // 
            this.cbMarkers.FormattingEnabled = true;
            this.cbMarkers.Location = new System.Drawing.Point(518, 291);
            this.cbMarkers.Name = "cbMarkers";
            this.cbMarkers.Size = new System.Drawing.Size(121, 21);
            this.cbMarkers.TabIndex = 11;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(640, 291);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 21);
            this.btnDelete.TabIndex = 12;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.ImageKey = "(none)";
            this.btnRefresh.Location = new System.Drawing.Point(518, 480);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 13;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnZoomMinus
            // 
            this.btnZoomMinus.Location = new System.Drawing.Point(569, 370);
            this.btnZoomMinus.Name = "btnZoomMinus";
            this.btnZoomMinus.Size = new System.Drawing.Size(24, 23);
            this.btnZoomMinus.TabIndex = 14;
            this.btnZoomMinus.Text = "-";
            this.btnZoomMinus.UseVisualStyleBackColor = true;
            this.btnZoomMinus.Click += new System.EventHandler(this.btnZoomMinus_Click);
            // 
            // btnZoomPlus
            // 
            this.btnZoomPlus.Location = new System.Drawing.Point(690, 370);
            this.btnZoomPlus.Name = "btnZoomPlus";
            this.btnZoomPlus.Size = new System.Drawing.Size(25, 23);
            this.btnZoomPlus.TabIndex = 15;
            this.btnZoomPlus.Text = "+";
            this.btnZoomPlus.UseVisualStyleBackColor = true;
            this.btnZoomPlus.Click += new System.EventHandler(this.btnZoomPlus_Click);
            // 
            // lZoom
            // 
            this.lZoom.AutoSize = true;
            this.lZoom.Location = new System.Drawing.Point(627, 375);
            this.lZoom.Name = "lZoom";
            this.lZoom.Size = new System.Drawing.Size(34, 13);
            this.lZoom.TabIndex = 16;
            this.lZoom.Text = "Zoom";
            // 
            // wfMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(727, 516);
            this.Controls.Add(this.lZoom);
            this.Controls.Add(this.btnZoomPlus);
            this.Controls.Add(this.btnZoomMinus);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.cbMarkers);
            this.Controls.Add(this.lGpsY);
            this.Controls.Add(this.lGpsX);
            this.Controls.Add(this.lColor);
            this.Controls.Add(this.lMarkers);
            this.Controls.Add(this.cbColor);
            this.Controls.Add(this.rtbMarkers);
            this.Controls.Add(this.btnAddMarker);
            this.Controls.Add(this.tbGpsY);
            this.Controls.Add(this.tbGpsX);
            this.Controls.Add(this.pbMap);
            this.Name = "wfMap";
            this.Text = "Map";
            this.Load += new System.EventHandler(this.wfMap_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbMap)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbMap;
        private System.Windows.Forms.TextBox tbGpsX;
        private System.Windows.Forms.TextBox tbGpsY;
        private System.Windows.Forms.Button btnAddMarker;
        private System.Windows.Forms.RichTextBox rtbMarkers;
        private System.Windows.Forms.ComboBox cbColor;
        private System.Windows.Forms.Label lMarkers;
        private System.Windows.Forms.Label lColor;
        private System.Windows.Forms.Label lGpsX;
        private System.Windows.Forms.Label lGpsY;
        private System.Windows.Forms.ComboBox cbMarkers;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnZoomMinus;
        private System.Windows.Forms.Button btnZoomPlus;
        private System.Windows.Forms.Label lZoom;
    }
}