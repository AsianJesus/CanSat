using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace CanSat_Desktop
{
    
    public class PointControl
    {
        private class Comparer : IEqualityComparer<DataPoint>
        {
            public bool Equals(DataPoint x, DataPoint y)
            {
                return x.XValue == y.XValue;
            }

            public int GetHashCode(DataPoint obj)
            {
                unchecked  // overflow is fine
                {
                    int hash = 17;
                    hash = hash * 23 + obj.XValue.GetHashCode();
                    hash = hash * 23 + obj.YValues.GetHashCode();
                    return hash;
                }
            }
        }
        private Series curve;
        public PointControl(Chart chart, int indx) { curve = chart.Series[0]; }
        public PointControl(Series s) { curve = s;}
        public void Clear()
        {
            curve.Points.Clear();
        }
        public void AddPointsY(double[] dots)
        {
            foreach (double x in dots)
                AddPointY(x);
        }
        public void AddPointsXY(Dictionary<double, double> dots)
        {
            if (dots.Count == 0) return;
            double[] xPoints = curve.Points.Select((DataPoint d) => d.XValue).ToArray();
            ////var points = dots.Union(curve.Points.Distinct(new Comparer()).ToDictionary(x => x.XValue, y => y.YValues[0]));
            //curve.Points.Clear();
            foreach( var kp in dots)
            {
                curve.Points.AddXY(kp.Key, kp.Value);
               
            }
        }
        public void AddPointY(double x)
        {
            double xPos = MaximumX;
            AddPointXY(++xPos, x);
        }
        public void AddPointXY(double x, double y)
        {
            curve.Points.AddXY(x, y);
        }
        public DataPoint GetPoint(double x)
        {

            return curve.Points.Where((DataPoint y) => { return y.XValue == x; }).First();
        }
        public double MaximumAtRange(double leftX, double rightX)
        {
            IEnumerable<DataPoint> elements = curve.Points.Where((DataPoint x) => x.XValue >= leftX && x.XValue <= rightX);            
            return elements.Count() == 0 ? MaximumY : elements.Max((DataPoint p) => { return p.YValues.Max(); });
        }
        public double MinimumAtRange(double leftX, double rightX)
        {
            IEnumerable<DataPoint> elements = curve.Points.Where((DataPoint x) => x.XValue >= leftX && x.XValue <= rightX);
            return elements.Count() == 0 ? MaximumY : elements.Min((DataPoint p) => { return p.YValues.Min(); });
        }
        public double MaximumX
        {
            get
            {
                List<DataPoint> points = curve.Points.ToList();
                if (points.Count == 0)
                    return -1;
                double max = points.Max<DataPoint>((DataPoint x) => { return x.XValue; });
                return max;
            }
        }
        public double MinimumX
        {
            get
            {
                List<DataPoint> points = curve.Points.ToList();
                if (points.Count == 0)
                    return -1;
                double min = points.Min<DataPoint>((DataPoint x) => { return x.XValue; });
                return min;
            }
        }
        public double MaximumY
        {
            get
            {
                List<DataPoint> points = curve.Points.ToList();
                if (points.Count == 0)
                    return 0;
                double max = points.Max((DataPoint dp) => { return dp.YValues.Max(); });
                return max;
            }
        }
        public double MinimumY
        {
            get
            {
                List<DataPoint> points = curve.Points.ToList();
                if (points.Count == 0)
                    return 0;
                double min = points.Min((DataPoint dp) => { return dp.YValues.Min(); });
                return min;
            }
        }
        public void CopyTo(PointControl target)
        {
            foreach(DataPoint dp in this.curve.Points.ToArray())
            {
                target.curve.Points.AddXY(dp.XValue, dp.YValues[0]);
            }
            target.ChartType = ChartType;
            target.Name = Name;
        }
        public void CopyFrom(PointControl source)
        {
            source.CopyTo(this);
        }
        public SeriesChartType ChartType
        {
            get
            {
                return curve.ChartType;
            }
            set
            {
                curve.ChartType = value;
            }
        }
        public string Name
        {
            get
            {
                return curve.Name;
            }
            set
            {
                curve.Name = value;
            }
        }
        public void AddSeriesToChart(Chart chart)
        {
            chart.Series.Add(curve);
        }
    }
}
