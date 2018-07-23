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
    public partial class InputID : Form
    {
        private int result = 0;
        public InputID()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool parsing = int.TryParse(tbID.Text, out result);
            if (!parsing && tbID.Text != "")
            {
                MessageBox.Show("Try again");
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
        public int Result
        {
            get { return result; }
        }
    }
}
