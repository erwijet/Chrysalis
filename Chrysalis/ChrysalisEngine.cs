using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

using System.Drawing;
using System.IO;
using System.IO.Compression;

namespace ChrysalisLib
{
    public class Chrysalis
    {
        const int MAX = 255;
        internal static Color FOLDER_FLAG = Color.FromArgb(255, 255, 0, 254);

        internal static Color GetColorFromByte(byte data)
            => (data < MAX) ? Color.FromArgb(data, 0, 0, 0) :
            (data - MAX < MAX) ? Color.FromArgb(MAX, data - MAX, 0, 0) :
            (data - (2 * MAX) < MAX) ? Color.FromArgb(MAX, MAX, data - (2 * MAX), 0) :
            Color.FromArgb(MAX, MAX, MAX, data - (3 * MAX));

        internal static int GetValueOfColor(Color c) => c.A + c.R + c.G + c.B;

        public static void DoFileDecode(FileInfo input)
        {
            string fileout = input.FullName.Substring(0, input.FullName.Length - ".png".Length);

            if (!input.Exists)
                return;
            Bitmap b = (Bitmap)Image.FromFile(input.FullName);

            int side = b.Size.Width;
            Color firstPix = b.GetPixel(0, 0);
            bool isFolder = firstPix == FOLDER_FLAG;

            BinaryWriter bin = new BinaryWriter(File.Create(fileout + ((isFolder) ? ".gz" : "")));

            for (int y = 0; y < side; y++)
            {
                for (int x = 0; x < side; x++)
                {
                    if (!(isFolder && x + y == 0))
                    {
                        Color c = b.GetPixel(x, y);
                        if (GetValueOfColor(c) != 4 * MAX) // Is this pixal a pixal that actually has data?
                        {
                            try { bin.Write((byte)GetValueOfColor(b.GetPixel(x, y))); bin.Flush(); }
                            catch { }
                        }
                    }
                }
            }

            bin.Close();
            bin.Dispose();

            if (isFolder)
            {
                DoFileDecompress(fileout + ".gz");
                File.Delete(fileout + ".gz");
            }
        }

        public static void DoFileDecompress(string file)
        {
            string newfile = file.Substring(0, file.Length - ".gz".Length) + ".zip";
            FileStream fs = File.OpenRead(file);
            GZipStream gzip = new GZipStream(fs, CompressionMode.Decompress);
            Stream s = File.Create(newfile);

            gzip.CopyTo(s);
            s.Close();

            gzip.Flush();
            gzip.Close();

            gzip.Dispose();
            s.Dispose();

            ZipFile.ExtractToDirectory(newfile, newfile.Substring(0, newfile.Length - ".zip".Length));
            File.Delete(newfile);
        }

        public static void DoFolderEncode(DirectoryInfo folder)
        {
            string image = folder.FullName;

            // Compress folder to a .gz file

            FileInfo fi = new FileInfo(image);
            ZipFile.CreateFromDirectory(folder.FullName, fi.FullName + ".zip");

            GZipStream gzip = new GZipStream(File.Create(fi.FullName + ".gz"), CompressionMode.Compress);
            Stream s = File.OpenRead(fi.FullName + ".zip");
            s.CopyTo(gzip); s.Close();
            gzip.Close();

            // Encode the .gz file

            DoFileEncode(new FileInfo(image + ".gz"), true);

            // Cleanup

            File.Delete(fi.FullName + ".zip");
            File.Delete(image + ".gz");
        }

        public static void DoFileEncode(FileInfo input, bool folderFlag)
        {
            string image = input.FullName + ".png";

            Bitmap bitmap = null;
            if (!input.Exists)
                return;

            // Continue if file exists

            byte[] data = File.ReadAllBytes(input.FullName);
            int side = (int)Math.Ceiling(Math.Sqrt(data.Length));
            bitmap = new Bitmap(side, side);

            for (int y = 0; y < side; y++)
            {
                for (int x = 0; x < side; x++)
                {
                    Color pix;
                    if (folderFlag && (x + y == 0))
                        pix = FOLDER_FLAG; // Folder Flag Code
                    else
                    {
                        try { pix = GetColorFromByte(data[((side * y) + x) - ((folderFlag) ? 1 : 0)]); }
                        catch { pix = Color.FromArgb(255, 255, 255, 255); }
                    }
                    bitmap.SetPixel(x, y, pix);
                }
            }

            bitmap.Save(image);
        }
    }
}
