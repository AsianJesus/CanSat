#include "stdafx.h"
#include "flyinfoclass.h"


FlyInfoClass::FlyInfoClass() : 
	m_dbConnection(nullptr),m_rPosition(nullptr),m_stmt(nullptr),m_fInfo(nullptr),
	m_mInfoPrev(nullptr),m_mInfoCurrent(nullptr), m_lastCheckedTime(nullptr)
{
	m_gpsHeightScale = 0.1f;
	m_gpsXScale = 100.0f;
	m_gpsYScale = .100f;
}

FlyInfoClass::FlyInfoClass(const char * host, const char * user, const char * passw, const char * schema):
	m_dbConnection(nullptr), m_rPosition(nullptr), m_stmt(nullptr), m_fInfo(nullptr),
	m_mInfoPrev(nullptr), m_mInfoCurrent(nullptr), m_lastCheckedTime(nullptr)
{
	m_gpsHeightScale = 1;
	m_gpsXScale = 100.0f;
	m_gpsYScale = 100.0f;
	InitializeConnection(host, user, passw, schema);
	GetFlightInfo();
	GetCenterPositions();
}


FlyInfoClass::~FlyInfoClass()
{
	if (m_dbConnection) {
		delete m_dbConnection;
		m_dbConnection = 0;
	}
	if (m_fInfo) {
		delete m_fInfo;
		m_fInfo = 0;
	}
	if (m_mInfoCurrent) {
		delete m_mInfoCurrent;
		m_mInfoCurrent = 0;
	}
	if (m_mInfoPrev) {
		delete m_mInfoPrev;
		m_mInfoPrev = 0;
	}
	if (m_rPosition) {
		delete m_rPosition;
		m_rPosition = 0;
	}
	if (m_lastCheckedTime) {
		delete m_lastCheckedTime;
		m_lastCheckedTime = 0;
	}
}

void FlyInfoClass::SetDBInfo(const char * host, const char * user, const char * passw, const char * schema)
{
	if (m_dbConnection) {
		delete m_dbConnection;
	}
	InitializeConnection(host, user, passw, schema);
}

bool FlyInfoClass::GetNextPosition(DirectX::XMFLOAT3 & positionOut)
{
	if (!GetNewPosition()) {
		return false;
	}
	positionOut = m_mInfoCurrent->position;
	return true;
}

bool FlyInfoClass::GetReleativePosition(DirectX::XMFLOAT3 & positionOut)
{
	if (m_mInfoCurrent) {
		positionOut = m_mInfoCurrent->position;
		return true;
	}
	return false;
}

bool FlyInfoClass::GetFlightTime(double& timeOut)
{
	if(m_mInfoCurrent){
		timeOut = m_mInfoCurrent->flyTime;
		return true;
	}
	return false;
}

bool FlyInfoClass::GetFlightInfo(int & idOut, char *& nameOut, time_t & tStartOut, time_t & tEndOut)
{
	if (!m_fInfo) {
		return false;
	}
	idOut = m_fInfo->id;
	tStartOut = m_fInfo->startTime;
	tEndOut = m_fInfo->endTime;
	nameOut = new char[strlen(m_fInfo->name)];
	strcpy_s(nameOut, strlen(m_fInfo->name) + 1, m_fInfo->name);
	return true;
}

bool FlyInfoClass::GetFlightInfo(FlightInfo & out)
{
	return GetFlightInfo(out.id,out.name,out.startTime,out.endTime);
}

bool FlyInfoClass::GetPreviousTime(double & timeOut)
{
	if (m_mInfoPrev) {
		timeOut = m_mInfoPrev->flyTime;
		return true;
	}
	return false;
}

bool FlyInfoClass::GetPreviousPosition(DirectX::XMFLOAT3 & positionOut)
{
	if (m_mInfoPrev) {
		positionOut = m_mInfoPrev->position;
		return true;
	}
	return false;
}

bool FlyInfoClass::IsNewFlight(int& idOut,char*& name, time_t& start, time_t& end)
{
	return CheckForNewFlight(idOut,name,start,end);
}

bool FlyInfoClass::IsNewFlight(FlightInfo & out)
{
	return IsNewFlight(out.id,out.name,out.startTime,out.endTime);
}

void FlyInfoClass::ChangeFlightID(const int id)
{
	GetFlightInfo(id);
}

bool FlyInfoClass::InitializeConnection(const char * host, const char * user, const char * passw, const char *schema)
{
	try {
		m_dbConnection = new sql::MYDB(host, user, passw, schema);
		m_stmt = m_dbConnection->PrepareStatement(
			"SELECT packetID, flyTime, gpsX, gpsY, height FROM data WHERE packetID > ? AND flyID = ? LIMIT 1;");
		return true;
	}
	catch(...){
		return false;
	}
}

bool FlyInfoClass::GetFlightInfo()
{
	//try {
	auto result = m_dbConnection->ExecuteQuery(sql::SQLString("SELECT id, name, UNIX_TIMESTAMP(start) AS start, UNIX_TIMESTAMP(end) AS end FROM flights ORDER BY start DESC LIMIT 1;"));
		FlightInfo temp;
		bool empty = true;
		while (result->next()) {
			temp.id = result->getInt("id");
			sql::SQLString t = result->getString("name");
			temp.name = new char[t.length()+1];
			strcpy_s(temp.name, t.length() + 1, t.c_str());
			temp.startTime = result->getInt64("start");
			temp.endTime = result->getInt64("end");
			empty = false;
		}
		if (empty) return false;
		if (m_fInfo) {
			delete m_fInfo;
		}
		m_fInfo = new FlightInfo(temp);
		return true;
	//}
	//catch (...) {
		return false;
	//}
}

bool FlyInfoClass::GetFlightInfo(const int id)
{
	try {
		sql::ResultSet* result = m_dbConnection->ExecuteQuery(
			sql::SQLString(("SELECT id,name,UNIX_TIMESTAMP(start) as start,UNIX_TIMESTAMP(end) as end FROM flights WHERE id = " + std::to_string(id)).c_str()));
		FlightInfo temp;
		bool empty = true;
		while (result->next()) {
			temp.id = result->getInt("id");
			temp.name = (char*)result->getString("name").c_str();
			temp.startTime = result->getInt64("start");
			temp.endTime = result->getInt64("end");
			empty = false;
		}
		if (empty) return false;
		if (m_fInfo) {
			delete m_fInfo;
		}
		m_fInfo = new FlightInfo(temp);
	}
	catch (...) {
		return false;
	}
	return true;
}

bool FlyInfoClass::GetNewPosition()
{
	double gpsX, gpsY, gpsZ;
	size_t packetID;
	double flyTime;
	bool check = false;
	try {
		m_stmt->setInt(1, m_mInfoCurrent ? m_mInfoCurrent->packetID : -1);
		m_stmt->setInt(2, m_fInfo->id);
		sql::ResultSet* result = m_dbConnection->ExecuteQuery(false);
		while (result->next()) {
			packetID = result->getInt(1);
			flyTime = result->getDouble(2);
			gpsX = result->getDouble(3);
			gpsY = result->getDouble(4);
			gpsZ = result->getDouble(5);
			check = true;
		}
	}
	catch (...) {
		return false;
	}
	if (!check) {
		return false;
	}
	m_mInfoPrev = m_mInfoCurrent;
	if ((m_gpsZeroX == 0 || m_gpsZeroY == 0 || m_gpsZeroHeight == 0) || (gpsZ < m_gpsZeroHeight))
		GetCenterPositions();
	DirectX::XMFLOAT3 positions = {
		(float)(gpsX - m_gpsZeroX) * m_gpsXScale,
		(float)(gpsZ-m_gpsZeroHeight) * m_gpsHeightScale,
		(float)(gpsY - m_gpsZeroY) * m_gpsYScale
	};
	m_mInfoCurrent = (gpsX == 0 || gpsY == 0 || gpsZ == 0) ? (m_mInfoCurrent ? new ModelInfo(*m_mInfoCurrent) : new ModelInfo()) 
		: new ModelInfo(packetID, flyTime, positions);
	if (flyTime == 0)m_mInfoCurrent->flyTime = m_mInfoCurrent ? m_mInfoCurrent->flyTime : 0;
	m_mInfoCurrent->packetID = packetID;
	return true;
}

bool FlyInfoClass::CheckForNewFlight(int & id, char *& name, time_t & start, time_t & end)
{
	int fId;
	time_t tStart, tEnd;
	char* fName = nullptr;
	try {
		//sql::SQLString query = "SELECT id, name, UNIX_TIMESTAMP(start) AS start, UNIX_TIMESTAMP(end) AS end FROM flights WHERE UNIX_TIMESTAMP(start) > " + (m_lastCheckedTime ? std::to_string(*m_lastCheckedTime) : " 0") + " ;";
		auto result = m_dbConnection->ExecuteQuery(sql::SQLString(("SELECT id, name, UNIX_TIMESTAMP(start) AS start, UNIX_TIMESTAMP(end) AS end FROM flights WHERE UNIX_TIMESTAMP(start) > " + std::to_string(m_lastCheckedTime ? *m_lastCheckedTime : (m_fInfo ? m_fInfo->startTime : 0)) + ";").c_str()));
		while (result->next()) {
			fId = result->getInt("id");
			sql::SQLString t = result->getString("name");
			fName = new char[t.length() + 1];
			strcpy_s(fName, t.length() + 1, t.c_str());
			tStart = result->getInt("start");
			tEnd = result->getInt("end");
		}
	}
	catch (...) {
		return false;
	}
	if (fName) {
		if (m_lastCheckedTime) {
			delete m_lastCheckedTime;
		}
		m_lastCheckedTime = new time_t(tStart);
	}
	else {
		return false;
	}
	id = fId;
	start = tStart;
	end = tEnd;
	name = new char[strlen(fName) + 1];
	strcpy_s(name, strlen(fName)+1, fName);
	return true;
}

bool FlyInfoClass::GetCenterPositions()
{
	bool empty = true;
	try {
		auto result = m_dbConnection->ExecuteQuery(sql::SQLString(("SELECT MIN(gpsZ), AVG(gpsX), AVG(gpsY) FROM data WHERE flyID = " + std::to_string(m_fInfo->id) + ";").c_str()));
		while (result->next()) {
			m_gpsZeroHeight = result->getDouble(1);
			m_gpsZeroX = result->getDouble(2);
			m_gpsZeroY = result->getDouble(3);
		}
	}
	catch (...) {
		return false;
	}

	return true;
}
