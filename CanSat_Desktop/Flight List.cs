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
    public partial class Flight_List : Form
    {
        
        public Flight_List()
        {
            InitializeComponent();
        }

        private void Flight_List_Load(object sender, EventArgs e)
        {

        }
        public RichTextBox IDs
        {
            get
            {
                return rtbId;
            }
            set
            {
                rtbId = value;
            }
        }
        public RichTextBox Names
        {
            get
            {
                return rtbName;
            }
            set
            {
                rtbName = value;
            }
        }
        public RichTextBox Starts
        {
            get
            {
                return rtbSTime;
            }
            set
            {
                rtbSTime = value;
            }
        }
        public RichTextBox Ends
        {
            get
            {
                return rtbETime;
            }
            set
            {
                rtbETime = value;
            }
        }
    }
}
