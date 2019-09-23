using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace derp
{
    public partial class Form1 : Form
    {
        private string filename = "c:\\users\\themi\\pictures\\diamond.jpg";
        public Form1()
        {
            InitializeComponent();
        }

        public string Filename { get => filename; set => filename = value; }

        private void button1_Click(object sender, EventArgs e)
        {
            EditorWindowForm editor = new EditorWindowForm();
            editor.setPictureBox(Filename);
            editor.ShowDialog();
        }
    }
}
