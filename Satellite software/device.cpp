#include "stdafx.h"
#include "device.h"

#define SLEEP(x) ;

RODevice::RODevice()
{
}
RODevice::RODevice(unsigned short pinIN) : pIN(pinIN)
{
}
RODevice::~RODevice()
{
}
bool RODevice::read(float& out)
{
	float result = 0;
	/* This part is undone and need finishing */
	if (result == failCode)
		return false;
	out = result;
	return result;
}

WODevice::WODevice()
{
}
WODevice::WODevice(unsigned short pinOUT) : pOUT(pinOUT)
{
}
void WODevice::write(const float f)
{
	/* This part is undone and need finishing */
}

RWDevice::RWDevice()
{
}
RWDevice::RWDevice(unsigned short pinIN, unsigned short pinOUT) : dIn(pinIN),dOut(pinOUT)
{
}
void RWDevice::write(const float c)
{
	dOut.write(c);
}
bool RWDevice::read(float& out) 
{
	return dIn.read(out);
}

MISC::MISC()
{
}
MISC::MISC(unsigned short pin) : misc(pin)
{
}
MISC::~MISC()
{
}
Vector<float> MISC::GetGases(unsigned int timeout)
{
	/* This part is undone and need finishing */
	return Vector<float>();
}

Clock::Clock()
{
}
Clock::Clock(unsigned short pin) : clock(pin)
{
}
Clock::~Clock()
{
}
float Clock::GetTime(unsigned int timeout)
{
	unsigned int time = 0;
	float result;
	while (!clock.read(result) && time <= timeout) {
		time += 50;
	}
	return result == 0 ? -1 : result;
}

BMP::BMP()
{
}
BMP::BMP(unsigned short pin) : bmp(pin)
{
}
BMP::~BMP()
{
}
float BMP::GetPressure(unsigned int timeout)
{
	/* This part is undone and need finishing */
	return 0.0f;
}
float BMP::GetHeight(unsigned int timeout)
{
	/* This part is undone and need finishing */
	return 0.0f;
}

DHT::DHT()
{
}
DHT::DHT(unsigned short pin) : dht(pin)
{
}
DHT::~DHT()
{
}
float DHT::GetTemperature(unsigned int timeout)
{
	/* This part is undone and need finishing */
	return 0.0f;
}
float DHT::GetHumidity(unsigned int timeout)
{
	/* This part is undone and need finishing */
	return 0.0f;
}

float VoltageDivider::calculateVoltage(const float voltFirst, const float voltSecond)
{
	/* This part is undone and need finishing */
	return 0.0f;
}
VoltageDivider::VoltageDivider(){
}
VoltageDivider::VoltageDivider(unsigned short pin1, unsigned short pin2) : voltageFirst(pin1),voltageSecond(pin2)
{
}
VoltageDivider::~VoltageDivider()
{
}
float VoltageDivider::GetVoltage(unsigned int timeout)
{
	float volF, volS;
	unsigned int time = 0;
	while (!voltageFirst.read(volF) && time <= timeout) {
		time += 50;
	}
	while (!voltageSecond.read(volS) && time <= timeout) {
		time += 50;
	}
	float result = volF != 0 && volS != 0 ? calculateVoltage(volF,volS) : -1;
	return result;
}

PitoTube::PitoTube()
{
}
PitoTube::PitoTube(unsigned short pin) : pt(pin)
{
}
PitoTube::~PitoTube()
{
}
float PitoTube::GetSpeed(unsigned int timeout)
{
	unsigned int time = 0;
	float result;
	while (!pt.read(result) && time <= timeout) {
		time += 50;
	}
	return result == 0 ? -1 : result; 
}

LIS::LIS()
{
}
LIS::LIS(unsigned short pin) : lis(pin)
{
}
LIS::~LIS()
{
}
float LIS::GetMagField(unsigned int timeout)
{
	unsigned int time = 0;
	float result;
	while (!lis.read(result) && time <= timeout) {
		time += 50;
	}
	return result == 0 ? -1 : result;
}

Servo::Servo()
{
}
Servo::Servo(unsigned short pin) : servo(pin)
{
}
Servo::~Servo()
{
}
void Servo::TurnDegree(const float)
{
	/* This part is undone and need finishing */
}
void Servo::TurnRadians(const float)
{
	/* This part is undone and need finishing */
}

Beeper::Beeper()
{
}
Beeper::Beeper(unsigned short pin) : beeper(pin)
{
}
Beeper::~Beeper()
{
}
void Beeper::Start()
{
	beeper.write(start);
}
void Beeper::Stop()
{
	beeper.write(stop);
}

GPS::GPS()
{
}
GPS::GPS(unsigned short pIN, unsigned short pOUT) : gps(pIN,pOUT)
{
}
GPS::~GPS()
{
}
Vector<float> GPS::GetCoordinates(unsigned int timeout)
{
	/* This part is undone and need finishing */
	return Vector<float>();
}

Camera::Camera()
{
}
Camera::Camera(unsigned short pIN, unsigned short pOUT) : camera(pIN,pOUT)
{
}
Camera::~Camera()
{
}
void Camera::TakePhoto()
{
	/* This part is undone and need finishing */
}

XBee::XBee()
{
}
XBee::XBee(unsigned short pIN, unsigned short pOUT) : xbee(pIN,pOUT)
{
}
XBee::~XBee()
{
}
String XBee::read()
{
	/* This part is undone and need finishing */
	return nullptr;
}
void XBee::send(const String msg)
{
	/* This part is undone and need finishing */
}
void XBee::send(const Vector<byte> data)
{
	/* This part is undone and need finishing */
}
