using CarPricer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarValue
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UnitTests.printMsg += (e) => { richTextBox1.AppendText(e + "\n"); };
            UnitTests unitTests = new UnitTests();
            unitTests.CalculateCarValue();
        }
    }
}
