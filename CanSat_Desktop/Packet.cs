using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanSat_Desktop
{
    class Gases : Object
    {
        private double? co2;
        private double? nh3;
        private double? no2;
        public Gases(double? c, double? nh, double? no)
        {
            co2 = c;
            nh3 = nh;
            no2 = no;
        }
        public double? CO2{
            get { return co2; }
        }
        public double? NH3
        {
            get { return nh3; }
        }
        public double? NO2
        {
            get { return no2; }
        }
        
    }
    class GPSCoordinates : Object
    {
        private double? x;
        private double? y;
        private double? z;
        public GPSCoordinates(double? gX, double? gY, double? gZ)
        {
            x = gX;
            y = gY;
            z = gZ;
        }
        public double? CoordinateX
        {
            get { return x; }
        }
        public double? CoordinateY
        {
            get { return y; }
        }
        public double? CoordinateZ
        {
            get { return z; }
        }
    }
    class Packet : Object
    {
        int id;
        double? temperature;
        double? pressure;
        GPSCoordinates gps;
        double? height;
        double? hum;
        Gases gases;
        double? speed;
        double? voltage;
        double? flyTime;
        String utcTime;
        public Packet(int packId,double? flyTime = null , double? temp = null, double? press = null,double? h = null, double? sp = null, double? hum = null,
            double? volt = null, GPSCoordinates gPS = null, Gases g = null,String utcTime = null) {
            id = packId;
            this.flyTime = flyTime;
            gps = gPS;
            temperature = temp;
            pressure = press;
            height = h;
            speed = sp;
            voltage = volt;
            gases = g;
            this.hum = hum;
            this.UtcTime = utcTime;
        }
        public int PacketID
        {
            get { return id; }
        }
        public double? Temperature
        {
            get { return temperature; }
        }
        public double? Pressure
        {
            get { return pressure; }
        }
        public double? GpsX
        {
            get { return gps.CoordinateX; }
        }
        public double? GpsY
        {
            get { return gps.CoordinateY; }
        }
        public double? GpsZ
        {
            get { return gps.CoordinateZ; }

        }
        public double? Height
        {
            get { return height; }
        }
        public double? Speed
        {
            get { return speed; }
        }
        public double? Voltage
        {
            get { return voltage; }
        }
        public double? FlyingTime
        {
            get { return flyTime; }
        }
        public double? Humidity
        {
            get { return hum; }
        }
        public GPSCoordinates GPS
        {
            get { return gps; }
        }
        public Gases Gases
        {
            get { return gases; }
        }

        public string UtcTime { get => utcTime; set => utcTime = value; }
        //packetID, temp, pressure, gx,gz,gy,height,hum,co2,nh3,no2,speed,voltage,batch,recTime, flyTime
    }
}
