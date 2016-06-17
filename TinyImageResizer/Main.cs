using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Threading;
using System.IO;

namespace TinyImageResizer
{
    public partial class Main : Form
    {

        private static int mNumPending = 0;
        private static int mErrorCount = 0;
        private System.Windows.Forms.Timer mTimer;

        public Main()
        {
            InitializeComponent();

            mTimer = new System.Windows.Forms.Timer();
            mTimer.Interval = 400;
            mTimer.Tick += new EventHandler(onTimer);
            mTimer.Start();

            int workerThreads;
            int completionPortThreads;
            ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
            ThreadPool.SetMaxThreads(Environment.ProcessorCount, completionPortThreads);
        }

        public void onTimer(object sender, EventArgs e)
        {
            this.label_numPending.Text = mNumPending.ToString();
            this.label_errorCount.Text = mErrorCount.ToString();
        }

        public static void _resizeImage(object state)
        {
            try
            {
                if(!(bool)typeof(Main).GetMethod("resizeImage").Invoke(null, (object[])state))
                    Interlocked.Increment(ref mErrorCount);
            }
            catch (Exception)
            {
                Interlocked.Increment(ref mErrorCount);
            }

            Interlocked.Decrement(ref mNumPending);
        }

        public static bool calcSize(int size, int srcW, int srcH, out int dstW, out int dstH)
        {
            dstW = srcW;
            dstH = srcH;

            try
            {
                if (srcW > srcH)
                {
                    if (srcW > size)
                    {
                        dstH = (int)((float)size / srcW * srcH);
                        dstW = size;
                        return true;
                    }
                }
                else
                {
                    if (srcH > size)
                    {
                        dstW = (int)((float)size / srcH * srcW);
                        dstH = size;
                        return true;
                    }
                }
            }
            catch (DivideByZeroException)
            {
            }

            return false;
        }

        public static bool resizeImage(int type, string srcFileName, string dstFileName, int quality, int size)
        {
            bool ret = false;
            IntPtr srcFile = new IntPtr(0);
            IntPtr srcImg = new IntPtr(0);
            IntPtr dstImg = new IntPtr(0);
            IntPtr dstFile = new IntPtr(0);

            srcFile = GD.gdOpenFile(srcFileName, "rb");
            if (srcFile.ToInt32() == 0) return ret;

            if (type == 0)
                srcImg = GD.gdImageCreateFromJpeg(srcFile);
            else if (type == 1)
                srcImg = GD.gdImageCreateFromPng(srcFile);

            if (srcImg.ToInt32() == 0) goto close_src_file;

            int srcW;
            int srcH;
            GD.gdImageSize(srcImg, out srcW, out srcH);

            int dstW;
            int dstH;
            calcSize(size, srcW, srcH, out dstW, out dstH);
            
            dstImg = GD.gdImageCreateTrueColor(dstW, dstH);
            GD.gdImageCopyResampled(dstImg, srcImg, 0, 0, 0, 0, dstW, dstH, srcW, srcH);

            dstFile = GD.gdOpenFile(dstFileName, "wb");
            if (dstFile.ToInt32() == 0) goto close_dst_img;

            if (type == 0)
                GD.gdImageJpeg(dstImg, dstFile, quality);
            else if (type == 1)
                GD.gdImagePng(dstImg, dstFile);

            ret = true;
            goto close_dst_file;
            close_dst_file: if (dstFile.ToInt32() != 0) GD.fclose(dstFile);
            close_dst_img: if (dstImg.ToInt32() != 0) GD.gdImageDestroy(dstImg);
            close_src_img: if (srcImg.ToInt32() != 0) GD.gdImageDestroy(srcImg);
            close_src_file: if (srcFile.ToInt32() != 0) GD.fclose(srcFile);

            return ret;
        }

        public static void _resizeLinks(object state)
        {
            typeof(Main).GetMethod("resizeLinks").Invoke(null, (object[])state);
        }

        public static int getImageType(string fnz)
        {
            int idx = fnz.Length - 4;
            if(idx < 0) return -1;

            string extension = fnz.Substring(idx).ToLower();
            if (extension == ".jpg")
                return 0;
            else if (extension == ".png")
                return 1;

            return -1;
        }

        public static void processDirectory(string link, int quality, int size)
        {
            DirectoryInfo rootDirInfo = new DirectoryInfo(link);
            string dstRoot = Path.Combine(rootDirInfo.Parent.FullName, rootDirInfo.Name + "_" + size);

            Stack<object[]> dirs = new Stack<object[]>();
            dirs.Push(new object[] {rootDirInfo , ""});
            while (dirs.Count > 0)
            {
                object[] top = dirs.Pop();
                DirectoryInfo srcDirInfo = (DirectoryInfo)top[0];
                string relLink = (string)top[1];

                DirectoryInfo[] subdirs = srcDirInfo.GetDirectories();
                FileInfo[] subfiles = srcDirInfo.GetFiles();
                DirectoryInfo dstDirInfo = new DirectoryInfo(Path.Combine(dstRoot, relLink));

                if (dstDirInfo.Exists) continue;
                if (subdirs.Length > 0 || subfiles.Length > 0)
                {
                    try
                    {
                        dstDirInfo.Create();
                    }
                    catch (IOException)
                    {
                        continue;
                    }
                }

                foreach (DirectoryInfo dirInfo in subdirs)
                {
                    dirs.Push(new object[] { dirInfo, Path.Combine(relLink, dirInfo.Name) });
                }

                foreach(FileInfo fileInfo in subfiles)
                {
                    int type = getImageType(fileInfo.FullName);
                    if (type < 0) continue;
                    Interlocked.Increment(ref mNumPending);
                    string fnz = Path.Combine(dstDirInfo.FullName, fileInfo.Name);
                    ThreadPool.QueueUserWorkItem(_resizeImage, new object[] { type, fileInfo.FullName, fnz, quality, size });
                }

            }
        }

        public static void processFile(string link, int quality, int size)
        {
            int type = getImageType(link);
            if (type < 0) return;
            Interlocked.Increment(ref mNumPending);
            string fnz = link.Substring(0, link.Length - 4) + "_" + size + link.Substring(link.Length - 4);
            ThreadPool.QueueUserWorkItem(_resizeImage, new object[] { type, link, fnz, quality, size });
        }

        public static void resizeLinks(string[] links, int quality, int size)
        {
            foreach(string link in links)
            {
                try
                {
                    if (Directory.Exists(link))
                        processDirectory(link, quality, size);
                    else if (File.Exists(link))
                        processFile(link, quality, size);
                }
                catch (Exception)
                {
                }
            }
        }

        private void Main_DragDrop(object sender, DragEventArgs e)
        {
            string[] links = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (links == null) return;

            int quality;
            try
            {
                quality = int.Parse(this.textBox_quality.Text);
            }
            catch (Exception)
            {
                quality = 90;
            }

            int size;
            try
            {
                size = int.Parse(this.textBox_size.Text);
            }
            catch (Exception)
            {
                size = 1000;
            }

            ThreadPool.QueueUserWorkItem(_resizeLinks, new object[] { links, quality, size });
        }

        private void Main_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else
                e.Effect = DragDropEffects.None;
        }


    }
}
