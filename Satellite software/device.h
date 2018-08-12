#pragma once
#include "vector.h"

#define TIMEOUT_DEFAULT 100
#define String char*
#define byte unsigned char

class Device
{
public:
	virtual void write(const float) = 0;
	virtual bool read(float&) = 0;
};

class RODevice : public Device{
private:
	unsigned short pIN;
	const float failCode = -1;
public:
	RODevice();
	RODevice(unsigned short pinIN);
	virtual ~RODevice();
	virtual void write(const float){}
	virtual bool read(float&);
};
class WODevice : public Device{
private:
	unsigned short pOUT;
public:
	WODevice();
	WODevice(unsigned short pinOUT);
	virtual void write(const float);
	virtual bool read(float&) { return 0.0f; }
};
class RWDevice : public Device{
private:
	RODevice dIn;
	WODevice dOut;
public:
	RWDevice();
	RWDevice(unsigned short pinIN, unsigned short pinOUT);
	virtual void write(const float);
	virtual bool read(float&);
};

//Read-only using devices
class MISC {
private:
	RODevice misc;
public:
	MISC();
	MISC(unsigned short pin);
	virtual ~MISC();
	Vector<float> GetGases(unsigned int timeout = TIMEOUT_DEFAULT);
};

class Clock {
private:
	RODevice clock;
public:
	Clock();
	Clock(unsigned short pin);
	virtual ~Clock();
	float GetTime(unsigned int timeout = TIMEOUT_DEFAULT);
};
class BMP {
private:
	RODevice bmp;
public:
	BMP();
	BMP(unsigned short pin);
	virtual ~BMP();
	float GetPressure(unsigned int timeout = TIMEOUT_DEFAULT);
	float GetHeight(unsigned int timeout = TIMEOUT_DEFAULT);
};
class DHT {
private:
	RODevice dht;
public:
	DHT();
	DHT(unsigned short pin);
	virtual ~DHT();
	float GetTemperature(unsigned int timeout = TIMEOUT_DEFAULT);
	float GetHumidity(unsigned int timeout = TIMEOUT_DEFAULT);
};
class VoltageDivider {
private:
	RODevice voltageFirst,voltageSecond;
	static float calculateVoltage(const float voltFirst, const float voltSecond);
public:
	VoltageDivider();
	VoltageDivider(unsigned short pin1,unsigned short pin2);
	virtual ~VoltageDivider();
	float GetVoltage(unsigned int timeout = TIMEOUT_DEFAULT);
};
class PitoTube {
private:
	RODevice pt;
public:
	PitoTube();
	PitoTube(unsigned short pin);
	virtual ~PitoTube();
	float GetSpeed(unsigned int timeout = TIMEOUT_DEFAULT);
};
class LIS {
private:
	RODevice lis;
public:
	LIS();
	LIS(unsigned short pin);
	virtual ~LIS();
	float GetMagField(unsigned int timeout = TIMEOUT_DEFAULT);
};

//Write-only devices
class Servo {
private:
	WODevice servo;
public:
	Servo();
	Servo(unsigned short pin);
	virtual ~Servo();
	void TurnDegree(const float);
	void TurnRadians(const float);
};
class Beeper {
private:
	WODevice beeper;
	const float stop = 0, start = 1;
public:
	Beeper();
	Beeper(unsigned short pin);
	virtual ~Beeper();
	void Start();
	void Stop();
};

//Read-write using devices
class GPS {
private:
	RWDevice gps;
public:
	GPS();
	GPS(unsigned short pIN,unsigned short pOUT);
	virtual ~GPS();
	Vector<float> GetCoordinates(unsigned int timeout = TIMEOUT_DEFAULT);
};
class Camera {
private:
	RWDevice camera;
public:
	Camera();
	Camera(unsigned short pIN, unsigned short pOUT);
	virtual ~Camera();
	void TakePhoto();
};
class XBee {
private:
	RWDevice xbee;
public:
	XBee();
	XBee(unsigned short pIN, unsigned short pOUT);
	virtual ~XBee();
	String read();
	void send(const String msg);
	void send(const Vector<byte> data);
};