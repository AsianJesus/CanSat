namespace CanSat_Desktop
{
    partial class GraphicViewer
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphicViewer));
            this.tbZoom = new System.Windows.Forms.TrackBar();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnMiddle = new System.Windows.Forms.Button();
            this.btnEnd = new System.Windows.Forms.Button();
            this.btnBegin = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.tbZoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // tbZoom
            // 
            this.tbZoom.Location = new System.Drawing.Point(12, 295);
            this.tbZoom.Maximum = 20;
            this.tbZoom.Name = "tbZoom";
            this.tbZoom.Size = new System.Drawing.Size(1085, 45);
            this.tbZoom.TabIndex = 0;
            this.tbZoom.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // chart1
            // 
            this.chart1.BackColor = System.Drawing.Color.Transparent;
            chartArea1.AxisX.Interval = 1D;
            chartArea1.AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
            chartArea1.BackColor = System.Drawing.Color.Transparent;
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.BackColor = System.Drawing.Color.Transparent;
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            legend1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            legend1.IsTextAutoFit = false;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(12, 12);
            this.chart1.Name = "chart1";
            this.chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            series1.BorderColor = System.Drawing.Color.DimGray;
            series1.BorderWidth = 3;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Color = System.Drawing.Color.Black;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(1085, 277);
            this.chart1.TabIndex = 1;
            this.chart1.Text = "chart1";
            this.chart1.Click += new System.EventHandler(this.chart1_Click);
            // 
            // btnMiddle
            // 
            this.btnMiddle.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnMiddle.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnMiddle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMiddle.Font = new System.Drawing.Font("Matura MT Script Capitals", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnMiddle.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.btnMiddle.Location = new System.Drawing.Point(511, 326);
            this.btnMiddle.Name = "btnMiddle";
            this.btnMiddle.Size = new System.Drawing.Size(91, 31);
            this.btnMiddle.TabIndex = 2;
            this.btnMiddle.Text = "Middle";
            this.btnMiddle.UseVisualStyleBackColor = false;
            this.btnMiddle.Click += new System.EventHandler(this.btnFullsize_Click);
            // 
            // btnEnd
            // 
            this.btnEnd.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnEnd.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnEnd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEnd.Font = new System.Drawing.Font("Matura MT Script Capitals", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnEnd.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.btnEnd.Location = new System.Drawing.Point(999, 326);
            this.btnEnd.Name = "btnEnd";
            this.btnEnd.Size = new System.Drawing.Size(91, 31);
            this.btnEnd.TabIndex = 3;
            this.btnEnd.Text = "End";
            this.btnEnd.UseVisualStyleBackColor = false;
            this.btnEnd.Click += new System.EventHandler(this.btnEnd_Click);
            // 
            // btnBegin
            // 
            this.btnBegin.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnBegin.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnBegin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBegin.Font = new System.Drawing.Font("Matura MT Script Capitals", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnBegin.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.btnBegin.Location = new System.Drawing.Point(12, 326);
            this.btnBegin.Name = "btnBegin";
            this.btnBegin.Size = new System.Drawing.Size(76, 31);
            this.btnBegin.TabIndex = 4;
            this.btnBegin.Text = "Begin";
            this.btnBegin.UseVisualStyleBackColor = false;
            this.btnBegin.Click += new System.EventHandler(this.btnBegin_Click);
            // 
            // GraphicViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.Azure;
            this.ClientSize = new System.Drawing.Size(1102, 369);
            this.Controls.Add(this.btnBegin);
            this.Controls.Add(this.btnEnd);
            this.Controls.Add(this.btnMiddle);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.tbZoom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GraphicViewer";
            this.Text = "GraphicViewer";
            this.Load += new System.EventHandler(this.GraphicViewer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tbZoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar tbZoom;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Button btnMiddle;
        private System.Windows.Forms.Button btnEnd;
        private System.Windows.Forms.Button btnBegin;
    }
}