using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace TinyImageResizer
{
    class GD
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Image
        {
            public IntPtr ptr;
            public Int32 sx;
            public Int32 sy;
        }

        public static void gdImageSize(IntPtr imagePtr, out int sx, out int sy)
        {
            Image img = (Image)Marshal.PtrToStructure(imagePtr, typeof(Image));
            sx = img.sx;
            sy = img.sy;
        }

        //maybe it's only compatible with GD compiled by visual studio 2013
        [DllImport("msvcr120.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr _wfopen(string _Filename, string _Mode);

        [DllImport("msvcr120.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int fclose(IntPtr file);

        public static IntPtr gdOpenFile(string fileName, string mode)
        {
            return _wfopen(fileName, mode);
        }

        public static int gdCloseFile(IntPtr file)
        {
            return fclose(file);
        }

        [DllImport("libgd.dll")]
        public static extern void gdImageDestroy(IntPtr img);

        [DllImport("libgd.dll")]
        public static extern IntPtr gdImageCreateTrueColor(int sx, int sy);

        [DllImport("libgd.dll")]
        public static extern IntPtr gdImageCreateFromJpeg(IntPtr file);

        [DllImport("libgd.dll")]
        public static extern IntPtr gdImageCreateFromPng(IntPtr file);

        [DllImport("libgd.dll")]
        public static extern void gdImageCopyResampled(IntPtr dst, IntPtr src, int dstX, int dstY,
                                      int srcX, int srcY, int dstW, int dstH, int srcW,
                                      int srcH);

        [DllImport("libgd.dll")]
        public static extern void gdImagePng(IntPtr img, IntPtr file);

        [DllImport("libgd.dll")]
        public static extern void gdImagePngEx (IntPtr img, IntPtr file, int level);

        [DllImport("libgd.dll")]
        public static extern void gdImageJpeg(IntPtr img, IntPtr file, int quality);

    }

}
