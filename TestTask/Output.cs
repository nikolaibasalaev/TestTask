using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestTask
{
    public partial class Output : Form
    {
        private Command m_databuffer;
        public Output(Command dataBuffer)
        {
            InitializeComponent();
            m_databuffer = dataBuffer;
        }

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void Output_Load(object sender, EventArgs e)
        {
            richTextBox.Lines = m_databuffer.Outputdata;
        }
    }
}
