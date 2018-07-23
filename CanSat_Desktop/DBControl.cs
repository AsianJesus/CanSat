using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace CanSat_Desktop
{
    class DBControl
    {
        private MySqlConnection connection;
        private MySqlCommand command;
        string serv, user, passw, db;
        public DBControl(string server, string uid, string password, string database)
        {
            serv = server;
            user = uid;
            passw = password;
            db = database;
            MySqlConnectionStringBuilder stringBuilder = new MySqlConnectionStringBuilder();
            stringBuilder.Server = serv;
            stringBuilder.UserID = user;
            stringBuilder.Password = passw;
            stringBuilder.Database = db;
            connection = new MySqlConnection(stringBuilder.GetConnectionString(passw != null && passw != ""));
        }
        public DBControl(DBControl db)
        {
            this.serv = db.serv;
            this.user = db.user;
            this.passw = db.passw;
            this.db = db.db;
            MySqlConnectionStringBuilder stringBuilder = new MySqlConnectionStringBuilder();
            stringBuilder.Server = serv;
            stringBuilder.Password = passw;
            stringBuilder.UserID = user;
            stringBuilder.Database = this.db;
            connection = new MySqlConnection(stringBuilder.GetConnectionString(passw != null && passw != ""));
        }
        ~DBControl()
        { 
            connection.Close();
        }
        public bool Open()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch(Exception e)
            {
                string msg = e.ToString();
                return false;
            }
        }
        public bool Close()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void TryAdd(string key)
        {
            if (command.Parameters.IndexOf(key) == -1)
                command.Parameters.Add(new MySqlParameter(key,new object()));
        }
        public void SetParameters(List<string> keys)
        {
            foreach(string key in keys)
            {
                TryAdd(key);
            }
        }
        public void SetVariables<TVal>(Dictionary<string,TVal> parameters)
        {
            foreach (KeyValuePair<string, TVal> v in parameters)
            {
                TryAdd(v.Key);
                command.Parameters[v.Key].Value = v.Value;
            }
        }
        public bool ExetuceNonQuery(MySqlCommand com)
        {
            if (com == null)
                return false; ;
            if (!IsOpen())
                Open();
            com.Connection = connection;
            com.Prepare();
            try
            {
                com.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool ExecuteNonQuery() {
            if (command == null)
                return false; ;
            if (!IsOpen())
                Open();
            this.command.Prepare();
            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch
            {

                return false;
            }
        }
        public List<List<string>> ExecuteQuery(MySqlCommand com)
        {
            if (com == null)
                return null;
            if (!IsOpen())
            {
                if (!Open())
                {
                    return null;
                }
            }
            com.Connection = connection;
            MySqlDataReader dataReader = com.ExecuteReader();
            if (dataReader == null)
                return null;
            List<List<String>> result = new List<List<String>>();
            int indx = 0;
            while (dataReader.Read())
            {
                result.Add(new List<String>());
                for (int i = 0; i < dataReader.FieldCount; i++)
                    result[indx].Add(dataReader.GetValue(i).ToString());
                indx++;
            }
            dataReader.Close();
            return result;
        }
        public List<List<String>> ExecuteQuery()
        {
            return ExecuteQuery(command);
        }
        public bool IsOpen()
        {
            return connection != null && connection.State == System.Data.ConnectionState.Open;
        }
        public MySqlCommand Command
        {
            set
            {
                this.command = value;                
                this.command.Connection = connection;
            }
        }
    }
}
