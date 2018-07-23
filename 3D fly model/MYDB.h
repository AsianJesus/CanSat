#pragma once

#include <mysql_connection.h>
#include <cppconn/driver.h>
#include <cppconn/statement.h>
#include <cppconn/prepared_statement.h>
#include <cppconn/resultset.h>
namespace sql {
	using namespace std;
	struct ConnInfo{
		SQLString server;
		SQLString user;
		SQLString password;
		SQLString database;
		ConnInfo(){}
		ConnInfo(const SQLString s, const SQLString u, const SQLString p, const SQLString db)
			: server(s),user(u),password(p),database(db){}
		bool operator!() { return server.c_str()[0] == '\0' || user.c_str()[0] == '\0' || database.c_str()[0] == '\0'; }
	};
	class DB {
	public:
		virtual void Connect(const SQLString&, const SQLString&, const SQLString&, const SQLString&) = 0;
		virtual void CloseConnection() = 0;
		virtual void SetStatement(const SQLString) = 0;
		virtual PreparedStatement* PrepareStatement(const SQLString&) = 0;
		virtual void Execute(const bool erase = true) = 0;
		virtual ResultSet* ExecuteQuery(const bool) = 0;
		virtual void ExecuteUpdate(const bool) = 0;
		virtual void Execute(const SQLString) = 0;
		virtual ResultSet* ExecuteQuery(const SQLString&) = 0;
		virtual void ExecuteUpdate(const SQLString&) = 0;
	};
	class MYDB : public DB
	{
	private:
		enum STATEMENT {
			NONE,
			UNPREPARED,
			PREPARED
		};
		Driver * driver;
		Connection* conn;
		PreparedStatement * pstmt;
		ConnInfo cinfo;
		SQLString stmtCommand;
		void OpenConnection();
		bool isOpened() const;
		STATEMENT whichStmt() const;
	public:
		MYDB();
		MYDB(const SQLString& server, const SQLString& uid, const SQLString& passw, const SQLString& db);
		MYDB(const MYDB&);
		virtual ~MYDB();
		virtual void Connect(const SQLString&, const SQLString&, const SQLString&, const SQLString&);
		virtual void SetStatement(const SQLString);
		virtual void CloseConnection();
		virtual PreparedStatement* PrepareStatement(const SQLString&);
		virtual void Execute(const bool erase = true);
		virtual ResultSet* ExecuteQuery(const bool erase = true);
		virtual void ExecuteUpdate(const bool erase = true);
		virtual void Execute(const SQLString);
		virtual ResultSet* ExecuteQuery(const SQLString&);
		virtual void ExecuteUpdate(const SQLString&);
		MYDB& operator=(const MYDB&);

	};
	
}