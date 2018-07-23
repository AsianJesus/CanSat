using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CanSat_Desktop
{
    public partial class DBConfigure : Form
    {
        private string server, uid, passw, db;
        public DBConfigure()
        {
            InitializeComponent();
        }

        private void DBConfigure_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            server = tbServ.Text;
            uid = tbUID.Text;
            passw = tbPassw.Text;
            db = tbDB.Text;
            DialogResult = (server == "" || uid == "" || db == "") ? DialogResult.Cancel : DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
        public string Server
        {
            get { return server; }
        }
        public string User
        {
            get { return uid; }
        }
        public string Password
        {
            get { return passw; }
        }
        public string Database
        {
            get { return db; }
        }
    }
}
