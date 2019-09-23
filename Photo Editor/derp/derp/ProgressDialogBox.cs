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
    public partial class ProgressDialogBox : Form
    {
        public delegate void CancelTransformEvent();
        public event CancelTransformEvent cancelTransform;
        public ProgressDialogBox()
        {
            InitializeComponent();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            cancelTransform();
            this.Close();
        }
        public void progressDialogBox_UpdateProgress(int progress)
        {
            //Invoke((Action)delegate ()
            //{

                transformProgressBar.Value = progress;
                if (transformProgressBar.Value == 100)
                {
                    this.Close();
                }
            //});

        }
    }
}
