using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Collections.Concurrent;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Windows.Forms.DataVisualization.Charting;
using Microsoft.Win32;

namespace CanSat_Desktop
{
    public partial class CanSat : Form
    {
        Dictionary<string, string> mysqlCommands = new Dictionary<string,string>(){
            { "getID", "SELECT id,name,start from flights order by start desc limit 1;" },
            {"getInfo",
                "SELECT packetID, recTime, flyTime, temp, press,height,speed,humidity,voltage,batch, gpsX,gpsY,gpsZ,co2,nh3,no2 FROM data WHERE flyID = @id AND recTime > @lasttime GROUP BY packetID ORDER BY packetID ;"},
            {"getFlights",
                "SELECT id,name,start,end, (SELECT count(packetID) FROM data WHERE flyID = id) FROM flights ORDER BY start DESC;"},
            {"getFlightInfo",
                "SELECT * from flights WHERE id = @id;"}
        };
        string comGetData, comGetID,comGetFlights,comGetFlightInfo;
        const string dbHostDef = "localhost", dbUIDDef = "root", dbPasswDef = "1123581321ElViNBV", dbNameDef = "pourtest";
        double lastTime;
        int flyIDCurrent, flyIDLastChecked;
        string flyName,flyStart;
        double flyTime;
        GPSCoordinates lastPosition;
        DBControl dbConnection,dbUpdateChannel;
        PointControl plotTemp,plotPress,plotSpeed,plotHeight,plotVolt,plotCharge, plotHum,plotCO,plotNH,plotNO;
        string imagePath = "image\\";
        ConcurrentBag<Packet> receivedPackets;
        List<Packet> allPackets;
        Thread listening;
        wfMap mapWindow = null;
        private double? ConvertTonullable(string text)
        {
            double result;
            return Double.TryParse(text, out result) ? (double?)result : null;
        }
        private void btnFullsize_Click(object sender, EventArgs e)
        {
        }

        private void btnSaveImg_Click(object sender, EventArgs e)
        {
        }

        private void changeImageDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult ans = folderBrowserDialog1.ShowDialog();
            if (ans == DialogResult.OK)
                imagePath = folderBrowserDialog1.SelectedPath + "\\plots";
        }
        private bool IsNewFlight(out int id)
        {
            bool result = false;

            try
            {
                MySqlCommand command = new MySqlCommand(comGetID);
                var recID = int.Parse(dbConnection.ExecuteQuery(command)[0][0]);
                if(recID != flyIDLastChecked)
                {
                    id = recID;
                    result = true;
                }
                else{
                    id = flyIDLastChecked;
                }
            }
            catch
            {
                id = flyIDLastChecked;
            }
            return result;
        }
        private bool CheckCommands()
        {
            try
            { 
                dbConnection.ExetuceNonQuery(new MySqlCommand(comGetFlights));
                MySqlCommand comm = new MySqlCommand(comGetData);
                comm.Parameters.AddWithValue("@id", 0);
                comm.Parameters.AddWithValue("@lasttime", 0);
                dbConnection.ExetuceNonQuery(comm);
                comm = new MySqlCommand(comGetFlightInfo);
                comm.Parameters.AddWithValue("@id", 0);
                dbConnection.ExetuceNonQuery(comm);
                dbConnection.ExetuceNonQuery(new MySqlCommand(comGetID));
                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool CheckDBConnection(string host, string user, string password, string schema)
        {
            try
            {
                DBControl db = new DBControl(host, user, password, schema);
                return db.Open();
            }
            catch
            {
                return false;
            }
        }
        private void changeFlyIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InputID form = new InputID();
            DialogResult res = form.ShowDialog(this);
            if (res == DialogResult.OK)
            {
                int id = form.Result;
                ChangeFlightID(id);
            }
        }
        private void ChangeFlightID(int id)
        {
            flyIDCurrent = id;
            ResetEverything();
            List<String> info = GetFlightInfo(id);
            if (info != null && info.Count != 0)
            {
                tbFlyID.Text = info[0];
                tbFlyName.Text = info[1];
                tbFlyStart.Text = info[2];
            }
            ListeningInitAndStart();
            UpdateInfo();
        }
        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateInfo();
        }
        private void ResetEverything()
        {
            ListeningStop();
            ResetPlots();
            tbGPSX.Clear();
            tbGPSY.Clear();
            tbGPSZ.Clear();
            analogClock1.Date = new DateTime(0);
            lastTime = 0;
            flyTime = 0;
            ClearReceivedPackets();
            allPackets.Clear();
            if (mapWindow != null)
            {
                mapWindow.Dispose();
                mapWindow = null;
            }
        }
        private void ClearReceivedPackets()
        {
            while (!receivedPackets.IsEmpty)
            {
                receivedPackets.TryTake(out Packet p);
            }
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            DBConfigure form = new DBConfigure();
            DialogResult result = form.ShowDialog(this);
            if (result == DialogResult.OK)
            {

                SaveInReg(
                        new Dictionary<string, string>()
                        {
                            { "server", form.Server },
                            { "uid", form.User },
                            { "passw", form.Password },
                            { "database",form.Database }
                        });
                string dbHost = form.Server;
                string dbUID = form.User;
                string dbPassw = form.Password;
                string dbName = form.Database;
                ResetEverything();
                getFlyID(out flyIDCurrent, out flyName, out flyStart);
                ListeningInitAndStart();
                UpdateInfo();
                InitializeConnection(dbHost,dbUID,dbPassw,dbName);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            ChDBCommands form = new ChDBCommands();
            form.DefComData = comGetData;
            form.DefComID = comGetID;
            form.DefComFlights = comGetFlights;
            form.DefComFlightInfo = comGetFlightInfo;
            DialogResult result = form.ShowDialog(this);
            if(result == DialogResult.OK)
            {
                Dictionary<string, string> valuesForSave = new Dictionary<string, string>();
                if(form.GetID != comGetID)
                {                    
                    comGetID = form.GetID;
                    getFlyID(out flyIDCurrent, out flyName, out flyStart);
                    valuesForSave.Add("comGetID", comGetID);
                }
                if (form.GetData != comGetData)
                {
                    comGetData = form.GetData;
                    valuesForSave.Add("ComGetData", comGetData);
                }
                if(form.GetFlights != comGetFlights)
                {
                    comGetFlights = form.GetFlights;
                    valuesForSave.Add("comGetFlights", comGetFlights);
                }
                if (form.GetFlightInfo != comGetFlightInfo)
                {
                    comGetFlightInfo = form.GetFlightInfo;
                    valuesForSave.Add("comGetFlightInfo", comGetFlights);
                }
                if (valuesForSave.Count != 0)
                {
                    ResetEverything();
                    getFlyID(out flyIDCurrent, out flyName, out flyStart);
                    ListeningInitAndStart();
                    UpdateInfo();
                }
                SaveInReg(valuesForSave);
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            ResetEverything();
            SaveInReg(
                        new Dictionary<string, string>()
                        {
                            { "server", dbHostDef },
                            { "uid", dbUIDDef },
                            { "passw", dbPasswDef},
                            { "database",dbNameDef},
                            {"ComGetData",mysqlCommands["getInfo"] },
                            {"comGetID", mysqlCommands["getID"] },
                            {"comGetFlights",mysqlCommands["getFlights"] }
                        });
            string dbHost = dbHostDef;
            string dbUID = dbUIDDef;
            string dbPassw = dbPasswDef;
            string dbName = dbNameDef;
            comGetID = mysqlCommands["getID"];
            comGetData = mysqlCommands["getInfo"];
            comGetFlights = mysqlCommands["getFlights"];
            comGetFlightInfo = mysqlCommands["getFlightInfo"];
            InitializeConnection(dbHost,dbUID,dbPassw,dbName);
            ListeningInitAndStart();
        }
        private void SaveInReg(Dictionary<string,string> pairs)
        {
            foreach(var kv in pairs)
            {
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Cansat_desktop", kv.Key, kv.Value);
            }
        }

        private void saveEveryPlotToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }
            SaveImage(cTemp, imagePath);
            SaveImage(cPress, imagePath);
            SaveImage(cSpeed, imagePath);
            SaveImage(cHeight, imagePath);
            SaveImage(cCharge, imagePath);
            SaveImage(cGas, imagePath);
            SaveImage(cHum, imagePath);
            SaveImage(cVoltage, imagePath);
        }
        private void SaveImage(Chart chart, string folderName)
        {
            chart.SaveImage(folderName + "//" +  chart.Name + ".jpg", ChartImageFormat.Jpeg);
        }
        private void updateToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (flyIDCurrent == -1)
                getFlyID(out flyIDCurrent, out flyName,out flyStart);
            Update();
        }

        private void resetAndUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetEverything();
            if (flyIDCurrent == -1)
                getFlyID(out flyIDCurrent, out flyName,out flyStart);
            ListeningInitAndStart();
            UpdateInfo();
        }

        private void gpsX_TextChanged(object sender, EventArgs e)
        {

        }

        private void CanSat_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void CanSat_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.F5)
            {
                    updateToolStripMenuItem1_Click(sender, e);
            }
            if (e.KeyData == Keys.F12)
                resetAndUpdateToolStripMenuItem_Click(sender, e);
        }

        private void chart3_Click(object sender, EventArgs e)
        {

        }

        private void chart8_Click(object sender, EventArgs e)
        {

        }
        private void ResetPlots()
        {
            plotTemp.Clear();
            plotPress.Clear();
            plotSpeed.Clear();
            plotHeight.Clear();
            plotCharge.Clear();
            plotCO.Clear();
            plotNH.Clear();
            plotNO.Clear();
            plotVolt.Clear();
            plotHum.Clear();
            tbGPSX.Clear();
            tbGPSY.Clear();
            tbGPSZ.Clear();
        }
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            Flight_List flist = new Flight_List();
            flist.Show(this);
            try
            {
                var result = dbConnection.ExecuteQuery(new MySqlCommand(comGetFlights));
                if (result.Count == 0)
                {
                    return;
                }
                flist.IDs.Lines = result.Select(s => s[0]).ToArray();
                flist.Names.Lines = result.Select(s => s[1]).ToArray();
                flist.Starts.Lines = result.Select(s => s[2]).ToArray();
                flist.Ends.Lines = result.Select(s => s[3]).ToArray();
                flist.PacketCounts.Lines = result.Select(s => s[4]).ToArray();
            }
            catch(Exception exc)
            {
                MessageBox.Show("Error appeared on loading " + exc.ToString());
            }
        }

        private void cPress_DoubleClick(object sender, EventArgs e)
        {
            ShowGraph(plotPress);
        }

        private void cSpeed_DoubleClick(object sender, EventArgs e)
        {
            ShowGraph(plotSpeed);
        }

        private void cHeight_DoubleClick(object sender, EventArgs e)
        {
            ShowGraph(plotHeight);
        }

        private void cVoltage_DoubleClick(object sender, EventArgs e)
        {
            ShowGraph(plotVolt);
        }

        private void cCharge_DoubleClick(object sender, EventArgs e)
        {
            ShowGraph(plotCharge);
        }

        private void cHum_DoubleClick(object sender, EventArgs e)
        {
            ShowGraph(plotHum);
        }

        private void cGases_DoubleClick(object sender, EventArgs e)
        {
            ShowGraph(new PointControl[]{ plotCO,plotNO,plotNH });
        }

        private void cTemp_DoubleClick(object sender, EventArgs e)
        {
            ShowGraph(plotTemp);
        }

        private string GetFromReg(string key, string def = "NULL")
        {
            return Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Cansat_desktop", key, def).ToString();
        }
        public CanSat()
        {
            InitializeComponent();
        }
        private void ShowMap()
        {
            if (mapWindow == null || mapWindow.IsDisposed)
            {
                mapWindow = new wfMap();
                if (lastPosition != null)
                    mapWindow.SatellitePosition = new MapCoordinates((double)lastPosition.CoordinateX, (double)lastPosition.CoordinateY);
                else
                    mapWindow.SatellitePosition = new MapCoordinates(0, 0);
            }
            mapWindow.Show();
        }
        private Dictionary<string,string> LoadDBInfo()
        {
            string dbHost = GetFromReg("server", dbHostDef);
            string dbPassw = GetFromReg("passw", dbPasswDef);
            string dbUID = GetFromReg("uid", dbUIDDef);
            string dbName = GetFromReg("database", dbNameDef);
            if(!CheckDBConnection(dbHost, dbUID, dbPassw, dbName))
            {
                dbHost = dbHostDef;
                dbPassw = dbPasswDef;
                dbUID = dbUIDDef;
                dbName = dbNameDef;
                //SaveInReg()
            }
            comGetID = GetFromReg("comGetID", mysqlCommands["getID"]);
            comGetData = GetFromReg("ComGetData", mysqlCommands["getInfo"]);
            comGetFlights = GetFromReg("comGetFlights", mysqlCommands["getFlights"]);
            comGetFlightInfo = GetFromReg("comGetFlightInfo", mysqlCommands["getFlights"]);
            if (!CheckCommands())
            {
                comGetID = mysqlCommands["getID"];//GetFromReg("comGetID", mysqlCommands["getID"]);
                comGetData = mysqlCommands["getInfo"];//GetFromReg("ComGetData", mysqlCommands["getInfo"]);
                comGetFlights = mysqlCommands["getFlights"];//GetFromReg("comGetFlights", mysqlCommands["getFlights"]);
                comGetFlightInfo = mysqlCommands["getFlightInfo"];//GetFromReg("comGetFlightInfo", mysqlCommands["getFlights"]);
                //SaveInReg();
            }
            return new Dictionary<string, string> { { "host", dbHost }, { "password", dbPassw }, { "uid", dbUID }, { "database", dbName } }; 
        }

        private void cSpeed_Click(object sender, EventArgs e)
        {

        }

        private void InitializeConnection(string dbHost,string dbUID,string dbPassw, string dbName)
        { 
            dbConnection = new DBControl(dbHost, dbUID, dbPassw, dbName);
            dbUpdateChannel = new DBControl(dbHost, dbUID, dbPassw, dbName);
        }

        private void analogClock1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            KeyPreview = true;
            Dictionary<string, string> dbConnInfo = LoadDBInfo();
            InitializeConnection(dbConnInfo["host"], dbConnInfo["uid"], dbConnInfo["password"], dbConnInfo["database"]);
            plotTemp = new PointControl(cTemp.Series[0]);
            plotPress = new PointControl(cPress.Series[0]);
            plotSpeed = new PointControl(cSpeed.Series[0]);
            plotHeight = new PointControl(cHeight.Series[0]);
            plotVolt = new PointControl(cVoltage.Series[0]);
            plotCharge = new PointControl(cCharge.Series[0]);
            plotHum = new PointControl(cHum.Series[0]);
            plotCO = new PointControl(cGas.Series[0]);
            plotNH = new PointControl(cGas.Series[2]);
            plotNO = new PointControl(cGas.Series[1]);

            getFlyID(out flyIDCurrent, out flyName,out flyStart);

            tbFlyID.Text = flyIDCurrent.ToString();
            tbFlyName.Text = flyName;
            tbFlyStart.Text = flyStart;

            lastTime = 0;
            flyTime = 0;
            receivedPackets = new ConcurrentBag<Packet>();
            allPackets = new List<Packet>();
            ListeningInitAndStart();
            UpdateInfo();
            mapWindow = new wfMap();
            flyIDLastChecked = flyIDCurrent;
        }
        private void ListeningInitAndStart()
        {
            int id = flyIDCurrent;
            double startTime = lastTime;
            listening = new Thread(new ThreadStart(() => { CheckAndUpdate(id, startTime, out lastTime); }));
            listening.Start();
        }
        private void ListeningStop()
        {
            if (listening.IsAlive)
            {
                listening.Abort();
            }
        }
        private bool ListeningAlive
        {
            get
            {
                return listening != null && listening.IsAlive;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
        }
        private bool getFlyID(out int id, out string name,out string start)
        {
            MySqlCommand comm = new MySqlCommand(comGetID);
            try
            {
                List<List<string>> execResults = dbConnection.ExecuteQuery(comm);
                id = int.Parse(execResults[0][0]);
                name = execResults[0][1];
                start = execResults[0][2];
                return true;
            }
            catch
            {
                id = -1;
                name = "";
                start = "";
                return false;
            }
        }

        private void tCheckNewSession_Tick(object sender, EventArgs e)
        {
            CheckNewFlight();
        }
        private void CheckNewFlight()
        {
            int fID;
            if (IsNewFlight(out fID))
            {
                var result = 
                    MessageBox.Show(this, "There is new session - " + fID + "\nWould you like to change?", "New Session", MessageBoxButtons.YesNo);
                if(result == DialogResult.Yes)
                {
                    ChangeFlightID(fID);
                }
                flyIDLastChecked = fID;
            }
        }
        private void tsbMap_Click(object sender, EventArgs e)
        {
            ShowMap();
        }

        private void CanSat_FormClosing(object sender, FormClosingEventArgs e)
        {
            ListeningStop();
            mapWindow.Close();
        }
        private List<string> GetFlightInfo(int id)
        {
            MySqlCommand cmd = new MySqlCommand(comGetFlightInfo);
            cmd.Parameters.AddWithValue("@id", id);
            var result = dbConnection.ExecuteQuery(cmd);
            return result.Count != 0 ? result[0] : new List<string>() { "-1", "Undefined","None" };
        }
        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void chart1_Click(object sender, EventArgs e)
        {
        }

        private void chart1_Click_1(object sender, EventArgs e)
        {
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            cPress.SaveImage("C:\\Users\\fruit\\Desktop\\1.jpg",ChartImageFormat.Jpeg);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }
        int waits = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if(waits < 10)
            {
                waits++;
            }
            else{
                waits = 0;
                getFlyID(out flyIDCurrent, out flyName, out flyStart);
            }
            if (dbConnection.IsOpen() && flyIDCurrent != -1)
                UpdateInfo();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        private void CheckAndUpdate(int id, double startTime,out double endTime)
        {
            double time = startTime;
            int sesID = id;
            List<Packet> packets;
            try
            {
                while (true)
                {
                    Thread.Sleep(400);
                    packets = GetInfo(sesID, time);
                    packets.ForEach(p => receivedPackets.Add(p));
                    if (packets.Count == 0)
                        continue;
                    time = packets.Max(p => p.ReceivedTime);
                }
            }
            catch(ThreadAbortException)
            {
                endTime = time;
            }
            finally
            {
                endTime = time;
            }
            
        }
        private List<Packet> GetInfo(int id, double time)
        {
            List<List<String>> rawData;
            try{
                MySqlCommand comm = new MySqlCommand(comGetData);
                comm.Parameters.AddWithValue("@id", id);
                comm.Parameters.AddWithValue("@lasttime", time);
                rawData = dbUpdateChannel.ExecuteQuery(comm);
            }
            catch{
               return new List<Packet>();
            }
            List<Packet> packets = new List<Packet>();
            if (rawData == null || rawData.Count() == 0)
                return new List<Packet>();

            foreach (List<String> row in rawData.Where((List<String> l) => { return l.Count >= 15; }))
            {
                //packetID,  recTime,flyTime, temp, pressure,height,speed,hum,voltage,batch, gx,gz,gy,co2,nh3,no2
                packets.Add(
                    new Packet(
                        int.Parse(row[0]),
                        Convert.ToDouble(row[1]),
                        ConvertTonullable(row[2]),
                        ConvertTonullable(row[3]),
                        ConvertTonullable(row[4]),
                        ConvertTonullable(row[5]),
                        ConvertTonullable(row[6]),
                        ConvertTonullable(row[7]),
                        ConvertTonullable(row[8]),
                        ConvertTonullable(row[9]),
                        new GPSCoordinates(ConvertTonullable(row[10]), ConvertTonullable(row[11]), ConvertTonullable(row[12])),
                        new Gases(ConvertTonullable(row[13]), ConvertTonullable(row[14]), ConvertTonullable(row[15]))
                    )
               );                
            }
            return packets;
        }
        private void UpdateInfo()
        {
            List<Packet> packets = new List<Packet>();
            {
                Packet packetBuffer;
                while (!receivedPackets.IsEmpty && packets.Count < 10)
                {
                    receivedPackets.TryTake(out packetBuffer);
                    packets.Add(packetBuffer);
                }
            }
            if (packets.Count == 0)
                    return;
            if(allPackets.Count != 0 && allPackets.Max(p=>p.PacketID) > packets.Min(p => p.PacketID))
            {
                ResetPlots();
                allPackets.AddRange(packets);
                packets = allPackets;
            }
            packets.Sort(delegate (Packet x, Packet y)
            {
                return x.PacketID.CompareTo(y.PacketID);
            });
            try
            {
                UpdateGraphs(packets);
            }
            finally { 
            allPackets = packets;
               }
        }
        private void UpdateGraphs(List<Packet> packets)
        {
            plotTemp.AddPointsXY(
                    packets.Where(p => { return p.Temperature != null; }).ToDictionary(p => { return (double)p.PacketID; }, p => { return (double)p.Temperature; })
                    );
            plotPress.AddPointsXY(
                    packets.Where(p => { return p.Pressure != null; }).ToDictionary(p => { return (double)p.PacketID; }, p => { return (double)p.Pressure; })
                    );
            plotHeight.AddPointsXY(
                    packets.Where(p => { return p.Height != null; }).ToDictionary(p => { return (double)p.PacketID; }, p => { return (double)p.Height; })
                    );
            plotSpeed.AddPointsXY(
                    packets.Where(p => { return p.Speed != null; }).ToDictionary(p => { return (double)p.PacketID; }, p => { return (double)p.Speed; })
                    );
            plotHum.AddPointsXY(
                    packets.Where(p => { return p.Humidity != null; }).ToDictionary(p => { return (double)p.PacketID; }, p => { return (double)p.Humidity; })
                    );
            plotVolt.AddPointsXY(
                    packets.Where(p => { return p.Voltage != null; }).ToDictionary(p => { return (double)p.PacketID; }, p => { return (double)p.Voltage; })
                    );
            plotCharge.AddPointsXY(
                    packets.Where(p => { return p.BatCharge != null; }).ToDictionary(p => { return (double)p.PacketID; }, elementSelector: p => { return (double)p.BatCharge; })
                    );
            plotCO.AddPointsXY(
                    packets.Where(p => { return p.Gases.CO2 != null; }).ToDictionary(p => { return (double)p.PacketID; }, p => { return (double)p.Gases.CO2; })
                    );
            plotNH.AddPointsXY(
                    packets.Where(p => { return p.Gases.NH3 != null; }).ToDictionary(p => { return (double)p.PacketID; }, p => { return (double)p.Gases.NH3; })
                    );
            plotNO.AddPointsXY(
                    packets.Where(p => { return p.Gases.NO2 != null; }).ToDictionary(p => { return (double)p.PacketID; }, p => { return (double)p.Gases.NO2; })
                    );
            var positions = packets.Where(p => { return p.GpsX != null && p.GpsY != null && p.GpsZ != null; });
            flyTime = packets.Count != 0 ? packets.Where(p => { return p.FlyingTime != null; }).Max(p => { return (double)p.FlyingTime; }) : flyTime;
            DateTime d = new DateTime((long)flyTime * 10000000, DateTimeKind.Unspecified);
            analogClock1.Date = d;
            GPSCoordinates pos = positions.Last().GPS;
            if (positions.Count() != 0)
            {
                tbGPSX.Text = pos.CoordinateX.ToString();
                tbGPSY.Text = pos.CoordinateY.ToString();
                tbGPSZ.Text = pos.CoordinateZ.ToString();

                if (mapWindow != null && !mapWindow.IsDisposed)
                {
                    mapWindow.SatellitePosition.X = (double)pos.CoordinateX;
                    mapWindow.SatellitePosition.Y = (double)pos.CoordinateY;
                }
                lastPosition = pos;
            }
        }
        private void ShowGraph(PointControl plot)
        {
            GraphicViewer graphic = new GraphicViewer();
            graphic.Name = plot.Name;
            graphic.Plots = new PointControl[]{ plot};
            graphic.Show(this);
        }
        private void ShowGraph(PointControl[] plots)
        {
            GraphicViewer graphic = new GraphicViewer();
            graphic.Plots = plots;
            graphic.Show(this);
        }
    } 
}
