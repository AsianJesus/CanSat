#pragma once
#include "mydb.h"
#include <DirectXMath.h>
class FlyInfoClass
{
public:
	struct FlightInfo {
		int id;
		char* name;
		time_t startTime;
		time_t endTime;
	};
	struct ModelInfo {
		size_t packetID;
		double flyTime;
		DirectX::XMFLOAT3 position;
		ModelInfo(){}
		ModelInfo(const ModelInfo& mi):packetID(mi.packetID),flyTime(mi.flyTime),position(mi.position){}
		ModelInfo(const size_t pid, const double ft, const DirectX::XMFLOAT3 p):packetID(pid),flyTime(ft),position(p){}
	};
public:
	FlyInfoClass();
	FlyInfoClass(const char*, const char*, const char*, const char*);
	virtual ~FlyInfoClass();
	void SetDBInfo(const char*, const char*, const char*, const char*);
	bool GetNextPosition(DirectX::XMFLOAT3&);
	bool GetReleativePosition(DirectX::XMFLOAT3&);
	bool GetFlightTime(double&);
	bool GetFlightInfo(int&, char*&,time_t&,time_t&);
	bool GetFlightInfo(FlightInfo&);
	bool GetPreviousTime(double&);
	bool GetPreviousPosition(DirectX::XMFLOAT3&);
	bool IsNewFlight(int&,char*&, time_t&,time_t&);
	bool IsNewFlight(FlightInfo&);
	void ChangeFlightID(const int);
private:
	bool InitializeConnection(const char*, const char*, const char*, const char*);
	bool GetFlightInfo();
	bool GetFlightInfo(const int);
	bool GetNewPosition();
	bool CheckForNewFlight(int&,char*&,time_t&,time_t&);
	bool GetCenterPositions();
private:
	sql::MYDB* m_dbConnection;
	sql::PreparedStatement* m_stmt;
	DirectX::XMFLOAT3* m_rPosition;
	float m_gpsZeroHeight,m_gpsZeroX,m_gpsZeroY;
	float m_gpsHeightScale, m_gpsXScale, m_gpsYScale;
	FlightInfo* m_fInfo;
	time_t* m_lastCheckedTime;
	ModelInfo* m_mInfoPrev,*m_mInfoCurrent;
};

