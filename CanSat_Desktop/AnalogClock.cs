using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Threading;

namespace AnalogClocks
{
    public partial class AnalogClock : UserControl
    {
        private PointF center;
        private float diam;
        private Color minColor = Color.Red, secColor = Color.Blue, hourColor = Color.Green;
        private Color colorDigFrame = Color.Navy, colorDigDial = Color.Purple, colorDigNum = Color.Silver;
        private Color colorDial = Color.Gray, colorFrame = Color.Black;
        private const double degreeToRad = Math.PI / 180;
        private float lenHour, lenMin, lenSec;
        private bool hourAvail = true, minAvail = true, secAvail = true;
        private System.Windows.Forms.Timer timer;
        private DateTime date;
        private Pen penHour,penMin, penSec;
        private bool manual = false;
        public AnalogClock()
        {
            InitializeComponent();
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler((object s, EventArgs e) => { if (!manual) Refresh();});
            timer.Start();
            penHour = new Pen(hourColor);
            penMin = new Pen(minColor);
            penSec = new Pen(secColor);
            penHour.StartCap = LineCap.Round;
            penHour.EndCap = LineCap.Triangle;
            penMin.StartCap = LineCap.Round;
            penMin.EndCap = LineCap.Triangle;
            penSec.StartCap = LineCap.RoundAnchor;
            penSec.EndCap = LineCap.ArrowAnchor;
        }

        private void AnalogClock_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            DrawFrame(10, e);
            DrawDial(e);
            DateTime time = manual ? date : DateTime.Now;
            DrawDigit(time, e);
            float degrSec = (time.Second % 60) * 6;
            float degrMin = (time.Minute + time.Second/60.0f)* 6;
            float degrHour = (time.Hour % 12 + time.Minute / 60.0f) * 30;
            if(hourAvail)
                DrawHour(degrHour, e);
            if(minAvail)
                DrawMinute(degrMin, e);
            if(secAvail)
                DrawSecond(degrSec, e);
        }
        private bool showTime = true;
        private void AnalogClock_MouseHover(object sender, EventArgs e)
        {
            if (!showTime)
                return;
            showTime = false;
            MessageBox.Show(date.ToLongTimeString());
            new Thread(() => { Thread.Sleep(3000); showTime = true; }).Start();
        }

        private void AnalogClock_Load(object sender, EventArgs e)
        {
            AnalogClock_Resize(sender, e);
        }

        private void AnalogClock_Resize(object sender, EventArgs e)
        {
            center = new PointF(Width / 2, Height / 2);
            diam = Width < Height ? Width*0.9F : Height*0.9F;
            lenSec = diam / 2 * 0.9F;
            lenMin = diam / 2 * 0.7F;
            lenHour = diam / 2 * 0.6F;
            penHour.Width = Height / 10F;
            penMin.Width  = Height / 14F;
            penSec.Width  = Height / 20F;
        }
        private void DrawFrame(float width, PaintEventArgs e)
        {
            PointF leftCorner = new PointF(center.X - diam/2,center.Y - diam/2);
            e.Graphics.DrawEllipse(new Pen(colorFrame, width), leftCorner.X,leftCorner.Y,diam,diam);
        }
        private void DrawDial(PaintEventArgs e)
        {
            e.Graphics.FillEllipse(new SolidBrush(colorDial), center.X - diam / 2, center.Y - diam / 2, diam, diam);
        }
        private void DrawMinute(float degree, PaintEventArgs e)
        {
            e.Graphics.DrawLine(penMin, center.X, center.Y, center.X + (float)(lenMin * Math.Sin(degree * degreeToRad)), center.Y - (float)(lenMin * Math.Cos(degree * Math.PI / 180)));
        }
        private void DrawHour (float degree, PaintEventArgs e)
        {
            e.Graphics.DrawLine(penHour, center.X, center.Y, center.X + (float)(lenHour * Math.Sin(degree * degreeToRad)), center.Y - (float)(lenHour * Math.Cos(degree * degreeToRad)));
        }
        private void DrawSecond(float degree, PaintEventArgs e)
        {
            e.Graphics.DrawLine(penSec, center.X, center.Y, center.X + (float)(lenSec * Math.Sin(degree * degreeToRad)), center.Y - (float)(lenSec * Math.Cos(degree * degreeToRad)));
        }
        private void DrawDigit(DateTime time,PaintEventArgs e)
        {
            string displTime = "";
            displTime += time.Hour >= 10 ? time.Hour.ToString() : "0" + time.Hour.ToString();
            displTime += ":";
            displTime += time.Minute >= 10 ? time.Minute.ToString() : "0" + time.Minute.ToString();
            displTime += ":";
            displTime += time.Second >= 10 ? time.Second.ToString() : "0" + time.Second.ToString();
            int strLen = displTime.Length;
            const float k = 1.3F;
            float fontSize = diam * (0.6F / 8) * k;
            PointF lCorner = new PointF(center.X - (diam*0.3F), center.Y + (diam*0.2F));
            float width = diam*0.61F;
            float height = diam/2 * 0.3F;
            e.Graphics.FillRectangle(new SolidBrush(colorDigDial), lCorner.X, lCorner.Y, width, height);
            e.Graphics.DrawRectangle(new Pen(colorDigFrame,3), lCorner.X, lCorner.Y, width, height);
            Font font = new Font(FontFamily.GenericSansSerif, fontSize, FontStyle.Italic);
            e.Graphics.DrawString(displTime, font, new SolidBrush(colorDigNum), lCorner);
        }
        public Color HourColor
        {
            get { return hourColor; }
            set { hourColor = value; penHour.Color = hourColor; Refresh(); }
        }
        public Color MinuteHour
        {
            get { return minColor; }
            set { minColor = value; penMin.Color = minColor;Refresh(); }
        }
        public Color SecColor
        {
            get { return secColor; }
            set { secColor = value; penSec.Color = secColor; Refresh(); }
        }
        public Color DialColor
        {
            get { return colorDial; }
            set { colorDial = value;Refresh(); }
        }
        public Color FrameColor
        {
            get { return colorFrame; }
            set { colorFrame = value; Refresh(); }
        }
        public Color DigitalFrame
        {
            get { return colorDigFrame; }
            set { colorDigFrame = value;Refresh(); }
        }
        public Color DigitalDial
        {
            get { return colorDigDial; }
            set { colorDigDial = value; Refresh(); }
        }
        public Color DigitalTime
        {
            get { return colorDigNum; }
            set { colorDigNum = value; Refresh(); }
        }
        public bool EnableHour
        {
            get { return hourAvail; }
            set { hourAvail = value; Refresh(); }
        }
        public bool EnableMinute
        {
            get { return minAvail; }
            set { minAvail = value; Refresh(); }
        }
        public bool EnableSec
        {
            get { return secAvail; }
            set { secAvail = value; Refresh(); }
        }
        public bool Manual
        {
            get { return manual; }
            set { manual = value; this.Refresh(); }
        }
        public DateTime Date
        {
            get { return date; }
            set { date = value;if (manual) Refresh(); }
        }
    }
}
