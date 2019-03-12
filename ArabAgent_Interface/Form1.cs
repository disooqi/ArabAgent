using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArabAgent;
namespace ArabAgent_Interface
{
    public partial class Form1 : Form
    {
        ArabAgent_Class userAgent;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            userAgent = new ArabAgent_Class("dos");
             
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //category_detector cd_obj = new category_detector();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            userAgent.updateAgent(textBox2.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = userAgent.calculateInterestingValue(textBox2.Text).ToString();
        }
    }
}
