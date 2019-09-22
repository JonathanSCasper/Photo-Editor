using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Photo_Editor
{    public partial class Form1 : Form
    {
        int i = 1;
        private CancellationTokenSource cancellationTokenSource;
        string rootFolder = "\\Users\\" + Environment.UserName + "\\Pictures";

        public Form1()
        {
            InitializeComponent();

            PopulateTreeView(rootFolder);
            treeView1.ExpandAll();

            cancellationTokenSource = new CancellationTokenSource();

            this.treeView1.NodeMouseClick +=
            new TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
        }

        //Edited code from https://docs.microsoft.com/en-us/dotnet/framework/winforms/controls/creating-an-explorer-style-interface-with-the-listview-and-treeview
        private void PopulateTreeView(string rootFolder)
        {
            TreeNode rootNode;
            treeView1.Nodes.Clear();

            DirectoryInfo info = new DirectoryInfo(rootFolder);
            if (info.Exists)
            {
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                GetDirectories(info.GetDirectories(), rootNode);
                treeView1.Nodes.Add(rootNode);
            }

        }

        private void GetDirectories(DirectoryInfo[] subDirs,
            TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;
            foreach (DirectoryInfo subDir in subDirs)
            {
                aNode = new TreeNode(subDir.Name, 0, 0);
                aNode.Tag = subDir;
                aNode.ImageKey = "folder";
                subSubDirs = subDir.GetDirectories();
                if (subSubDirs.Length != 0)
                {
                    GetDirectories(subSubDirs, aNode);
                }
                nodeToAddTo.Nodes.Add(aNode);
            }
        }

        async void treeView1_NodeMouseClick(object sender,
        TreeNodeMouseClickEventArgs e)
        {
            //The NodeMouseClick handler isn't a function, so I had to call it once with the cancellationTokenSource.Cancel()
            //to stop the current process, than reset the cancellationTokenSource, and call DoSomething again...
            cancellationTokenSource.Cancel();
            await DoSomething(sender, e);
            cancellationTokenSource = new CancellationTokenSource();
            await DoSomething(sender, e);
        }

        async Task DoSomething(object sender,
        TreeNodeMouseClickEventArgs e)
        {
            //Start background thread
            await Task.Run(() =>
            {
                    TreeNode newSelected = e.Node;
                    Invoke((Action)delegate ()
                    {
                        listView1.Items.Clear();
                    });
                    DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
                    ListViewItem.ListViewSubItem[] subItems;
                    ListViewItem item = null;
                    ImageList imageListSmall = imageList2;
                    ImageList imageListLarge = imageList3;

                foreach (DirectoryInfo dir in nodeDirInfo.GetDirectories("*.jpg"))
                {
                    item = new ListViewItem(dir.Name, 0);
                    subItems = new ListViewItem.ListViewSubItem[]
                        {new ListViewItem.ListViewSubItem(item, "Directory"),
                    new ListViewItem.ListViewSubItem(item,
                    dir.LastAccessTime.ToShortDateString())};
                    item.SubItems.AddRange(subItems);
                    listView1.Items.Add(item);
                    if (cancellationTokenSource.IsCancellationRequested)
                        break;
                }

                foreach (FileInfo file in nodeDirInfo.GetFiles("*.jpg"))
                {
                    // DON'T USE Image.FromFile IT WILL CAUSE ISSUES FOR EXTRA CREDIT
                    Invoke((Action)delegate ()
                    {
                        imageList1.Images.Add(Image.FromFile(file.FullName));
                        imageListSmall.Images.Add(Image.FromFile(file.FullName));
                        imageListLarge.Images.Add(Image.FromFile(file.FullName));
                    });
                    //Shows photo in imagebox
                    item = new ListViewItem(file.Name, i);
                    subItems = new ListViewItem.ListViewSubItem[]
                        { new ListViewItem.ListViewSubItem(item, file.CreationTime.ToString()),
                new ListViewItem.ListViewSubItem(item,
                (file.Length/1024) + " KB")};

                    item.SubItems.AddRange(subItems);
                    Invoke((Action)delegate ()
                    {
                        listView1.Items.Add(item);
                    });
                    //Increment i for image index
                    i++;
                    Invoke((Action)delegate ()
                    {

                        listView1.SmallImageList = imageListSmall;
                        listView1.LargeImageList = imageListLarge;
                    });
                    if (cancellationTokenSource.IsCancellationRequested)
                        break;
                }
            });
        }
        private void DetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.Details;
            detailsToolStripMenuItem.Checked = true;
            smallToolStripMenuItem.Checked = false;
            largeToolStripMenuItem.Checked = false;
        }

        private void SmallToolStripMenuItem_Click(object sender, EventArgs e)
        {        
            listView1.View = View.SmallIcon;
            detailsToolStripMenuItem.Checked = false;
            smallToolStripMenuItem.Checked = true;
            largeToolStripMenuItem.Checked = false;
        }

        private void LargeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.LargeIcon;
            detailsToolStripMenuItem.Checked = false;
            smallToolStripMenuItem.Checked = false;
            largeToolStripMenuItem.Checked = true;
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

            AboutBox1 box = new AboutBox1();
            box.Show();
        }
        private void ExitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void LocateOnDiskToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Process.Start(rootFolder);
        }

        private void SelectRootFolderToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowDialog();
            rootFolder = folderBrowserDialog.SelectedPath;
            PopulateTreeView(rootFolder);
            treeView1.ExpandAll();

        }
    }
}
