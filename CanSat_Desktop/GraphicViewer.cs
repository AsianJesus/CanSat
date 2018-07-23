using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CanSat_Desktop
{
    public partial class GraphicViewer : Form
    {
        private List<PointControl> plots;
        private ViewControlUnit viewSelected;
        public Chart ChartPlot
        {
            set
            {
                chart1 = value;                
            }
        }
        public PointControl[] Plots
        {
            get
            {
                return plots.ToArray();
            }
            set
            {
                plots.Clear();
                chart1.Series.Clear();
                PointControl tempP;
                foreach(PointControl p in value)
                {
                    Series tempS = new Series();
                    tempP = new PointControl(tempS);
                    tempP.CopyFrom(p);
                    tempP.AddSeriesToChart(chart1);
                    plots.Add(tempP);
                }
            }
        }
        private double MinimumX
        {
            get { return plots.Min((PointControl p) => { return p.MinimumX; }); }
        }
        private double MinimumY
        {
            get { return plots.Min((PointControl p) => { return p.MinimumY; }); }
        }
        private double MaximumX
        {
           get{ return plots.Max((PointControl p) => { return p.MaximumX; });
        }
        }
        private double MaximumY
        {
            get { return plots.Max((PointControl p) => { return p.MaximumY; }); }
        }
        private double MinimumAtRange(double leftLimit, double rightLimit)
        {
            return plots.Min((PointControl p) => {return p.MinimumAtRange(leftLimit, rightLimit); });
        }
        private double MaximumAtRange(double leftLimit, double rightLimit)
        {
            return plots.Max((PointControl p) => { return p.MaximumAtRange(leftLimit, rightLimit); });
        }
        public GraphicViewer()
        {
            InitializeComponent();
            plots = new List<PointControl>();
            viewSelected = new ViewControlUnit(chart1);
        }
        private void InterpretAllPlots()
        {
            plots.Clear();
            foreach(Series s in chart1.Series)
            {
                plots.Add(new PointControl(s));
            }
            viewSelected = new ViewControlUnit(chart1.ChartAreas[0]);
        }
        private void GraphicViewer_Load(object sender, EventArgs e)
        {
            trackBar1_Scroll(sender, e);
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            double lX, rX, uY, dY;
            lX = MinimumX;
            rX = MaximumX;
            if (tbZoom.Value == tbZoom.Minimum || tbZoom.Value == 0)
            {
                uY = MaximumY * 1.05;
                dY = MinimumY * 0.95;
            }
            else
            {
                double dist = rX - lX;
                dist = dist * ((double)tbZoom.Value / (double)tbZoom.Maximum);
                double offset = dist * 0.05;
                lX = lX + Math.Floor(dist - offset);
                rX = lX + chart1.ChartAreas[0].AxisX.Interval * 10;
                uY = MaximumAtRange(lX, rX) * 1.1;
                dY = MinimumAtRange(lX, rX) * 0.9;
            }
            if (lX == rX || dY == uY)
                return;
            viewSelected.Resize(lX, dY, rX, uY);
        }

        private void btnSaveImage_Click(object sender, EventArgs e)
        {

        }

        private void btnFullsize_Click(object sender, EventArgs e)
        {
            tbZoom.Value = tbZoom.Minimum/2 + tbZoom.Maximum/2;
            trackBar1_Scroll(sender, e);
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            tbZoom.Value = tbZoom.Maximum;
            trackBar1_Scroll(sender, e);
        }

        private void btnBegin_Click(object sender, EventArgs e)
        {
            tbZoom.Value = tbZoom.Minimum;
            trackBar1_Scroll(sender, e);
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
