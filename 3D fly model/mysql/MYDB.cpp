#include "stdafx.h"
#include "MYDB.h"

using namespace std;

namespace sql {
	MYDB::MYDB() : driver(get_driver_instance()), conn(NULL)
	{
		
	}
	
	MYDB::MYDB(const SQLString& server, const SQLString& uid, const SQLString& passw, const SQLString& db) : driver(get_driver_instance()){
		cinfo = ConnInfo(server, uid, passw, db);
	}

	MYDB::MYDB(const MYDB & mydb)
	{
		cinfo = ConnInfo(mydb.cinfo);
	}

	MYDB::~MYDB()
	{
		delete pstmt;
		delete conn;
	}

	void MYDB::Connect(const SQLString& s, const SQLString& u, const SQLString& p, const SQLString& db) {
		cinfo = ConnInfo(s, u, p, db);
		
	}
	MYDB& MYDB::operator=(const MYDB& other) {
		if (this == &other)
			return *this;
		CloseConnection();
		delete conn;
		delete pstmt;
		cinfo = other.cinfo;
		return *this;
	}
	void MYDB::SetStatement(const SQLString cmd) {
		stmtCommand = cmd;
	}
	PreparedStatement* MYDB::PrepareStatement(const SQLString& cmd) {
		OpenConnection();
		pstmt = conn->prepareStatement(cmd);
		return pstmt;
	}
	void MYDB::Execute(const bool erase) {
		OpenConnection();
		switch (whichStmt()) {
		case PREPARED: 
			pstmt->execute();
			if (erase) {
				delete pstmt;
				pstmt = NULL;
			}
			return; 
		case UNPREPARED:
			Statement * stmt = conn->createStatement();
			stmt->execute(stmtCommand);
			if (erase)
				stmtCommand = "";
			return;
		}
	}
	ResultSet* MYDB::ExecuteQuery(const bool erase) {
		OpenConnection();
		ResultSet* res;
		switch (whichStmt()) {
		case PREPARED:
			res = pstmt->executeQuery();
			if (erase) {
				delete pstmt;
				pstmt = NULL;
			}
			return res;
		case UNPREPARED:
			Statement * stmt = conn->createStatement();
			res = stmt->executeQuery(stmtCommand);
			if (erase)
				stmtCommand = "";
			return res;
		}
		return NULL;
	}
	void MYDB::ExecuteUpdate(const bool erase) {
		OpenConnection();
		switch (whichStmt()) {
		case PREPARED:
			pstmt->executeUpdate();
			if (erase) {
				delete pstmt;
				pstmt = NULL;
			}
			return;
		case UNPREPARED:
			Statement * stmt = conn->createStatement();
			stmt->executeUpdate(stmtCommand);
			if (erase)
				stmtCommand = "";
			return;
		}
	}
	void MYDB::Execute(const SQLString cmd) {
		OpenConnection();
		Statement* stmt = conn->createStatement();

		stmt->execute(cmd);
		delete stmt;
	}
	ResultSet* MYDB::ExecuteQuery(const SQLString& cmd) {
		OpenConnection();	
		Statement* stmt = conn->createStatement();
		ResultSet* res = stmt->executeQuery(cmd);
		delete stmt;
		return res;
	}
	void MYDB::ExecuteUpdate(const SQLString& cmd) {
		OpenConnection();
		Statement* stmt = conn->createStatement();
		stmt->executeUpdate(cmd);
		delete stmt;
	}
	void MYDB::OpenConnection() {
		if (!cinfo)
			throw "No info";
		if (isOpened())
			return;
		conn = driver->connect(cinfo.server, cinfo.user, cinfo.password);
		conn->setSchema(cinfo.database);
	}
	bool MYDB::isOpened() const {
		return (conn != NULL && (!conn->isClosed()));
	}
	MYDB::STATEMENT MYDB::whichStmt() const{
		if (pstmt != NULL)
			return PREPARED;
		if (stmtCommand != "")
			return UNPREPARED;
		return NONE;
	}
	void MYDB::CloseConnection() {
		if (conn == NULL || conn->isClosed())
			return;
		conn->close();
	}

}