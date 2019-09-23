//Code written by Riley Judd

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace derp
{
    public partial class EditorWindowForm : Form
    {
        private CancellationTokenSource cancellationTokenSource;
        public delegate void UpdateProgressEvent(int progress);
        public event UpdateProgressEvent updateProgress;
        private Bitmap pictureBoxBitmap;
        private int totalProgress;
        private bool transformImage;
        public void setPictureBox(string filename)
        {
            pictureBox.ImageLocation = filename;
            pictureBox.LoadAsync();
            pictureBoxBitmap = new Bitmap(pictureBox.Image);
        }
        public EditorWindowForm()
        {
            InitializeComponent();
            totalProgress = 0;
            transformImage = true;
        }

        private async void brightnessBar_MouseCaptureChanged(object sender, EventArgs e)
        {
            ProgressDialogBox progress = new ProgressDialogBox();
            this.updateProgress += progress.progressDialogBox_UpdateProgress;
            progress.cancelTransform += editor_CancelTransform;
            progress.Show();
            progress.Refresh();
            //this.Enabled = false;
            Bitmap transformedBitmap = await changeBrightness(progress);
            if (transformImage)
            {
                pictureBoxBitmap = transformedBitmap;
            }
            this.Enabled = true;
            this.BringToFront();
            transformImage = true;
        }

        private async Task<Bitmap> changeBrightness(ProgressDialogBox progress)
        {
            cancellationTokenSource = new CancellationTokenSource();
            Bitmap newBitMap = pictureBoxBitmap;
            pictureBoxBitmap = new Bitmap(pictureBox.Image);
            //variables used to keep track of progress
            int totalPixels = pictureBoxBitmap.Height * pictureBoxBitmap.Width;
            int onePercent = totalPixels / 100;
            int count = 0;
            int amount = Convert.ToInt32(2 * (50 - brightnessBar.Value) * 0.01 * 255);

            await Task.Run(() =>
            {
                for (int y = 0; y < pictureBoxBitmap.Height; y++)
                {
                    for (int x = 0; x < pictureBoxBitmap.Width; x++)
                    {

                        if (transformImage)
                        {
                            Color color = pictureBoxBitmap.GetPixel(x, y);
                            int newRed = (int)color.R + amount;
                            if (newRed > 255)
                            {
                                newRed = 255;
                            }
                            else if (newRed < 0)
                            {
                                newRed = 0;
                            }

                            int newGreen = (int)color.G + amount;
                            if (newGreen > 255)
                            {
                                newGreen = 255;
                            }
                            else if (newGreen < 0)
                            {
                                newGreen = 0;
                            }

                            int newBlue = (int)color.B + amount;
                            if (newBlue > 255)
                            {
                                newBlue = 255;
                            }
                            else if (newBlue < 0)
                            {
                                newBlue = 0;
                            }

                            Color newColor = Color.FromArgb(newRed, newGreen, newBlue);
                            pictureBoxBitmap.SetPixel(x, y, newColor);
                            count++;
                            if (count % onePercent == 0)
                            {
                                totalProgress++;
                                updateProgress.Invoke(totalProgress);
                            }
                        }
                        if(cancellationTokenSource.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        break;
                    }
                }
            });
            pictureBox.Image = pictureBoxBitmap;
            totalProgress = 0;
            return newBitMap;
        }

        private async void invertButton_Click(object sender, EventArgs e)
        {
            ProgressDialogBox progress = new ProgressDialogBox();
            updateProgress += new UpdateProgressEvent(progress.progressDialogBox_UpdateProgress);
            progress.Show();
            progress.Refresh();
            this.Enabled = false;
            pictureBoxBitmap = await Task_InvertColors(progress);
            this.Enabled = true;
            this.BringToFront();
            transformImage = true;
        }

        private async Task<Bitmap> Task_InvertColors(ProgressDialogBox progress)
        {
            cancellationTokenSource = new CancellationTokenSource();
            //variables used to keep track of progress
            int totalPixels = pictureBoxBitmap.Height * pictureBoxBitmap.Width;
            int onePercent = totalPixels / 100;
            int count = 0;

            //Code by Frank McCown https://sites.harding.edu/fmccown/classes/comp4450-f19/Photo%20Editor.pdf
            Bitmap newBitMap = pictureBoxBitmap;
            pictureBoxBitmap = new Bitmap(pictureBox.Image);

            await Task.Run(() =>
            {
                for (int y = 0; y < pictureBoxBitmap.Height; y++)
                {
                    for (int x = 0; x < pictureBoxBitmap.Width; x++)
                    {
                        Color color = pictureBoxBitmap.GetPixel(x, y);
                        int newRed = Math.Abs(color.R - 255);
                        int newGreen = Math.Abs(color.G - 255);
                        int newBlue = Math.Abs(color.B - 255);
                        Color newColor = Color.FromArgb(newRed, newGreen, newBlue);
                        pictureBoxBitmap.SetPixel(x, y, newColor);
                        count++;
                        if (count % onePercent == 0)
                        {
                            if (totalProgress != 100)
                            {
                                totalProgress++;
                            }
                            updateProgress?.Invoke(totalProgress);
                        }
                        if(cancellationTokenSource.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        break;
                    }
                }
            });
            pictureBox.Image = pictureBoxBitmap;
            totalProgress = 0;
            return newBitMap;
        }

        private async void colorButton_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = tintColorDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                ProgressDialogBox progress = new ProgressDialogBox();
                this.updateProgress += progress.progressDialogBox_UpdateProgress;
                progress.Show();
                progress.Refresh();
                this.Enabled = false;
                pictureBoxBitmap = await Task_TintColors(progress);
                this.Enabled = true;
                transformImage = true;
            }
        }

        private async Task<Bitmap> Task_TintColors(ProgressDialogBox progress)
        {
            Bitmap newBitMap = pictureBoxBitmap;
            pictureBoxBitmap = new Bitmap(pictureBox.Image);
            Color tintColor = tintColorDialog.Color;
            //variables used to keep track of progress
            int totalPixels = pictureBoxBitmap.Height * pictureBoxBitmap.Width;
            int onePercent = totalPixels / 100;
            int count = 0;

            for (int y = 0; y < pictureBoxBitmap.Height; y++)
            {
                for (int x = 0; x < pictureBoxBitmap.Width; x++)
                {
                    if (transformImage)
                    {
                        Color color = pictureBoxBitmap.GetPixel(x, y);
                        float rgbPercentage = (float)((color.R + color.G + color.B) / 3) / 255;
                        int newRed = (int)((int)tintColor.R * rgbPercentage);
                        int newGreen = (int)((int)tintColor.G * rgbPercentage);
                        int newBlue = (int)((int)tintColor.B * rgbPercentage);

                        Color newColor = Color.FromArgb(newRed, newGreen, newBlue);
                        pictureBoxBitmap.SetPixel(x, y, newColor);
                        count++;
                        if (count % onePercent == 0)
                        {
                            totalProgress++;
                            updateProgress.Invoke(totalProgress);
                        }
                    }
                }
            }
            pictureBox.Image = pictureBoxBitmap;
            totalProgress = 0;
            return newBitMap;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void editor_CancelTransform()
        {
            transformImage = false;
            cancellationTokenSource.Cancel();
        }
    }

}
