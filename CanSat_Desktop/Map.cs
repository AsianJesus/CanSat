using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Windows.Forms;
using System.Threading;

namespace CanSat_Desktop
{
    public partial class wfMap : Form
    {
        private const string urlBase = "https://maps.googleapis.com/maps/api/staticmap";
        private MapCoordinates satPosition = new MapCoordinates();
        private List<Marker> markers = new List<Marker>();
        private int zoom = 14;
        private static HttpClient http = new HttpClient();
        Task imageRefresher = null;
        public MapCoordinates SatellitePosition
        {
            get
            {
                return satPosition;
            }
            set
            {
                satPosition = value;
                RefreshMap();
            }
        }
        public wfMap()
        {
            InitializeComponent();
        }

        private void wfMap_Load(object sender, EventArgs e)
        {
            RefreshMap(true);
            markers = new List<Marker>();
        }
        private void RefreshMap(bool erase = false)
        {
            if (imageRefresher == null || imageRefresher.Status != TaskStatus.Running)
            {
                Task.Run(() =>
                {
                    if (erase)
                    {
                        imageRefresher = LoadMap(CreateRequest(), ShowMap, ShowError, 5);
                    }
                    else
                    {
                        imageRefresher = LoadMap(CreateRequest(), ShowMap, () => { }, 5);
                    }
                });
            }            
        }
        private void ShowError()
        {
            pbMap.ImageLocation = "image/error.png";
        }
        private string CreateRequest()
        {
            string centerPos = "center=" + SatellitePosition.X.ToString() + "," + SatellitePosition.Y.ToString();
            string markersDesc = "markers=color:orange|"+SatellitePosition.X + "," + SatellitePosition.Y + "&" + (markers != null ? String.Join("&", markers.Select(x => x.UrlDesc)) : "");
            string zoomDesc = "zoom=" + zoom;
            string size = "size=" + pbMap.Width + "x" + pbMap.Height;
            string mapType = "maptype=satellite";
            string request = urlBase + "?" + centerPos + "&" + markersDesc + "&" + zoomDesc + "&" + size + "&" + mapType;
            return request;
        }

        private void btnZoomMinus_Click(object sender, EventArgs e)
        {
            if (zoom > 7) {
                ChangeZoom(-1);
                RefreshMap(false);
            }
        }
        private void ShowMap(Image image)
        {
            image.Save("sat_location.png");
            pbMap.Image = image;
        }
        private void ChangeZoom(int scale)
        {
            zoom += scale;
        }

        private void btnZoomPlus_Click(object sender, EventArgs e)
        {
            if (zoom < 20)
            {
                ChangeZoom(1);
                RefreshMap(false);
            }

        }

        private void btnAddMarker_Click(object sender, EventArgs e)
        {
            string sColor = cbColor.Text;
            string pX = tbGpsX.Text, pY = tbGpsY.Text;
            MapCoordinates mapCoordinates;
            try
            {
                mapCoordinates = new MapCoordinates(float.Parse(pX), float.Parse(pY));
            }
            catch
            {
                return;
            }
            AddMarker(new Marker(mapCoordinates, sColor));
            RefreshMarkerList();
        }
        private void AddMarker(Marker m)
        {
            markers.Add(m);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int selected;
            try
            {
                selected = (int)cbMarkers.SelectedValue;
            }
            catch
            {
                return;
            }
            DeleteMarker(selected);
            RefreshMarkerList();
        }
        private void DeleteMarker(int id)
        {
            markers.RemoveAt(id);
        }
        private static async Task LoadMap(string mapUrl, Action<Image> callback, Action exceptionHandler, float timeout)
        {
            var image = LoadImage(mapUrl);
            var delay = Task.Delay(TimeSpan.FromSeconds(timeout));
            if (delay != (await Task.WhenAny(image, Task.Delay(TimeSpan.FromSeconds(timeout))))) {
                if (image.Status == TaskStatus.RanToCompletion)
                {
                    callback(image.Result);
                }
                else
                {
                    exceptionHandler();
                }
            }
        } 
        private static async Task<Image> LoadImage(string url)
        {
            Stream s = await http.GetStreamAsync(url);
            Image result = Image.FromStream(s);
            return result;
        }
        private void RefreshMarkerList()
        {
            if (markers.Count == 0) {
                rtbMarkers.Clear();
                return;
            }
            rtbMarkers.Text = String.Join("\n", markers.Select(x => x.ToString()));
            BindingList<Provider<String>> m = new BindingList<Provider<String>>();
            for(int i = 0; i < markers.Count(); i++)
            {
                m.Add(new Provider<String>() { Value = markers[i].ToString(), ID = i });
            }
            cbMarkers.DataSource = m;
            cbMarkers.DisplayMember = "Value";
            cbMarkers.ValueMember = "ID"; 
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshMap(true);
        }
    }
    class Provider<T>
    {
        private T v;
        private int id;
        public T Value
        {
            get
            {
                return v;
            }
            set
            {
                v = value;
            }
        }
        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }
    }
    public class MapCoordinates
    {
        private double x;
        private double y;
        public double X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }
        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }
        public override string ToString()
        {
            return x + "," + y;
        }
        public MapCoordinates() { }
        public MapCoordinates(double pX, double pY) { x = pX; y = pY; }
    }
    public class Marker
    {
        private MapCoordinates coordinates;
        private string color;
        public Marker()
        {

        }
        public Marker(MapCoordinates coor, string c)
        {
            coordinates = coor;
            color = c;
        }
        public Marker(float x, float y, string c)
        {
            coordinates = new MapCoordinates(x, y);
            color = c;
        }
        public MapCoordinates Coordinates
        {
            get
            {
                return coordinates;
            }
            set
            {
                coordinates = value;
            }
        }
        public string Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }
        public string UrlDesc
        {
            get
            {
                return "markers=color:" + color.ToLower() + "|" + coordinates.ToString();
            }
        }
        public override string ToString()
        {
            return coordinates.ToString() + " - " + color;
        }
    }
}
