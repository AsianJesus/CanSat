using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CanSat_Desktop
{
    public class ViewControlUnit
    {
        private ChartArea area;
        private double widthX, widthY;
        private double centerX, centerY;
        private SideX apprSideX;
        private SideY apprSideY;
        private double savedMinX, savedMinY, savedMaxX, savedMaxY;
        public AxisScrollBar ScrollBarX
        {
            get
            {
                return area.AxisX.ScrollBar;
            }
            set
            {
                area.AxisX.ScrollBar = value;
            }
        }
        public AxisScrollBar ScrollBarY
        {
            get
            {
                return area.AxisY.ScrollBar;
            }
            set
            {
                area.AxisY.ScrollBar = value;
            }
        }
        public ViewControlUnit(Chart c, int i = 0)
        {
            area = c.ChartAreas[i];
        }
        public ViewControlUnit(ChartArea a)
        {
            area = a;
        }
        public double SizeX
        {
            get
            {
                return widthX;
            }
            set
            {
                widthX = value;

            }
        }
        public double SizeY
        {
            get
            {
                return widthY;
            }
            set
            {
                widthY = value;

            }
        }
        public double MinimumX
        {
            get
            {
                return area.AxisX.Minimum;
            }
        }
        public double MinimumY
        {
            get
            {
                return area.AxisY.Minimum;
            }
        }
        public double MaximumX
        {
            get
            {
                return area.AxisX.Maximum;
            }
        }
        public double MaximumY
        {
            get
            {
                return area.AxisY.Maximum;
            }
        }
        public void Snapshot()
        {
            savedMinX = MinimumX;
            savedMaxX = MaximumX;
            savedMinY = MinimumY;
            savedMaxY = MaximumY;
        }
        public void Rollback()
        {
            Resize(savedMinX, savedMinY, savedMaxX, savedMaxY);
        }
        public void Resize(double startX, double startY, double endX, double endY)
        {
            area.AxisX.Minimum = startX;
            area.AxisY.Minimum = startY;
            area.AxisY.Maximum = endY;
            area.AxisX.Maximum = endX;
        }
        public void ResizeX(SideX s = SideX.Right)
        {
            apprSideX = s;
            switch (s)
            {
                case SideX.Left:
                    area.AxisX.Maximum = area.AxisX.Minimum + widthX;
                    break;
                case SideX.Center:
                    double center = (area.AxisX.Minimum + area.AxisX.Maximum) / 2;
                    area.AxisX.Minimum = center - widthX / 2;
                    area.AxisX.Maximum = center + widthX / 2;
                    break;
                case SideX.Right:
                    area.AxisX.Minimum = area.AxisX.Maximum - widthX;
                    break;
            }
            centerX = area.AxisX.Maximum / 2 + area.AxisX.Minimum / 2;
        }
        public enum SideY
        {
            Down,
            Center,
            Up,
        }
        public void ResizeY(SideY s = SideY.Up)
        {
            apprSideY = s;
            switch (s)
            {
                case SideY.Down:
                    area.AxisY.Maximum = area.AxisY.Minimum + widthY;
                    break;
                case SideY.Center:
                    double center = (area.AxisY.Minimum + area.AxisY.Maximum) / 2;
                    area.AxisY.Minimum = center - widthY / 2;
                    area.AxisY.Maximum = center + widthY / 2;
                    break;
                case SideY.Up:
                    area.AxisY.Minimum = area.AxisY.Maximum - widthY;
                    break;
            }
            centerY = area.AxisY.Minimum / 2 + area.AxisY.Maximum / 2;
        }
        public enum SideX
        {
            Left,
            Center,
            Right,
        }
        public void FullSize(double xMin,double yMin,double xMax, double yMax) {
            area.AxisX.Minimum = xMin;
            area.AxisX.Maximum = xMax;
            area.AxisY.Minimum = yMin;
            area.AxisY.Maximum = yMax;
        }
        public void MoveTo(double x, double width = 0)
        {
            if (width != 0)
                widthX = width;
            centerX = x;
            area.AxisX.Minimum = centerX - widthX / 2;
            area.AxisX.Maximum = centerX + widthX / 2;
        }
    }
}
