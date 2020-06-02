using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace HaralickGLCMProject
{
    class ImageProcessing
    {
        public static void setPixel(BitmapData bmd, int x, int y, Color c)
        {
            unsafe
            {
                byte* p = (byte*)bmd.Scan0 + (y * bmd.Stride) + (x * 3);
                p[0] = c.B;
                p[1] = c.G;
                p[2] = c.R;
            }
        }

        public static Color getPixel(BitmapData bmd, int x, int y)
        {

            Color c = Color.White;

            unsafe
            {
                byte* ptr = (byte*)bmd.Scan0 + (y * bmd.Stride) + (x * 3);

                c = Color.FromArgb(ptr[2], ptr[1], ptr[0]);
            }

            return c;
        }

        public static Bitmap grayscale(Image img)
        {

            Bitmap b = new Bitmap(img);
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            for (int i = 0; i < bmData.Width; i++)
            {
                for (int j = 0; j < bmData.Height; j++)
                {
                    Color c = getPixel(bmData, i, j);
                    int gray = (int)(.299 * c.R + .587 * c.G + .114 * c.B);
                    setPixel(bmData, i, j, Color.FromArgb(gray, gray, gray));
                }
            }

            b.UnlockBits(bmData);

            return b;
        }

        public static Bitmap thresholding(Image img, int threshold)
        {

            Bitmap b = new Bitmap(img);
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            for (int i = 0; i < bmData.Width; i++)
            {
                for (int j = 0; j < bmData.Height; j++)
                {
                    Color c = getPixel(bmData, i, j);
                    setPixel(bmData, i, j, (c.R <= threshold) ? Color.Black : Color.White);
                }
            }

            b.UnlockBits(bmData);

            return b;
        }

        public static Bitmap edgeDetection(Image img, EdgeDetectionTechniques technique)
        {
            Bitmap i = new Bitmap(img);
            Bitmap r = new Bitmap(img);
            BitmapData iBmData = i.LockBits(new Rectangle(0, 0, i.Width, i.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData rBmData = r.LockBits(new Rectangle(0, 0, r.Width, r.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);


            int[,] mask = new int[3, 3];
            switch (technique)
            {
                case EdgeDetectionTechniques.PrewittHorizontal:
                    mask[0, 0] = 1; mask[1, 0] = 1; mask[2, 0] = 1;
                    mask[0, 1] = 0; mask[1, 1] = 0; mask[2, 1] = 0;
                    mask[0, 2] = -1; mask[1, 2] = -1; mask[2, 2] = -1;
                    break;
                case EdgeDetectionTechniques.PrewittVertical:
                    mask[0, 0] = 1; mask[1, 0] = 0; mask[2, 0] = -1;
                    mask[0, 1] = 1; mask[1, 1] = 0; mask[2, 1] = -1;
                    mask[0, 2] = 1; mask[1, 2] = 0; mask[2, 2] = -1;
                    break;
                case EdgeDetectionTechniques.SobelHorizontal:
                    mask[0, 0] = 1; mask[1, 0] = 2; mask[2, 0] = 1;
                    mask[0, 1] = 0; mask[1, 1] = 0; mask[2, 1] = 0;
                    mask[0, 2] = -1; mask[1, 2] = -2; mask[2, 2] = -1;
                    break;
                case EdgeDetectionTechniques.SobelVertical:
                    mask[0, 0] = 1; mask[1, 0] = 0; mask[2, 0] = -1;
                    mask[0, 1] = 2; mask[1, 1] = 0; mask[2, 1] = -2;
                    mask[0, 2] = 1; mask[1, 2] = 0; mask[2, 2] = -1;
                    break;
                case EdgeDetectionTechniques.SobelMixed:
                    mask[0, 0] = -2; mask[1, 0] = 0; mask[2, 0] = 2;
                    mask[0, 1] = 0; mask[1, 1] = 0; mask[2, 1] = 0;
                    mask[0, 2] = 2; mask[1, 2] = 0; mask[2, 2] = -2;
                    break;
            }

            for (int x = 1; x < i.Width - 1; x++)
            {
                for (int y = 1; y < i.Height - 1; y++)
                {
                    int result = 0;
                    for (int a = 0; a < 3; a++)
                    {
                        for (int b = 0; b < 3; b++)
                        {
                            Color f = getPixel(iBmData, x + a - 1, y + b - 1);
                            result += f.R * mask[a, b];
                        }
                    }

                    setPixel(rBmData, x, y, (result >= 0) ? Color.Black : Color.White);
                }
            }

            i.UnlockBits(iBmData);
            r.UnlockBits(rBmData);

            i.Dispose();

            return r;
        }

        public static Bitmap dilate(Image img)
        {
            Bitmap b = new Bitmap(img);
            Bitmap result = new Bitmap(b);
            BitmapData bBmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData resultBmData = result.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            bool[,] bBinImg = binarizeImage(bBmData);
            bool[,] mask = new bool[3, 3];
            mask[0, 0] = false; mask[1, 0] = true; mask[2, 0] = false;
            mask[0, 1] = true; mask[1, 1] = true; mask[2, 1] = true;
            mask[0, 2] = false; mask[1, 2] = true; mask[2, 2] = false;

            unsafe
            {
                int width = result.Width - 1;
                int height = result.Height - 1;
                for (int y = 1; y < height; ++y)
                {
                    for (int x = 1; x < width; ++x)
                    {
                        bool value = false;
                        for (int mx = 0; mx < 3; mx++)
                        {
                            for (int my = 0; my < 3; my++)
                            {

                                value |= mask[mx, my] && bBinImg[x + mx - 1, y + my - 1];
                            }
                        }

                        setPixel(resultBmData, x, y, (value) ? Color.Black : Color.White);
                    }
                }
            }

            b.UnlockBits(bBmData);
            result.UnlockBits(resultBmData);

            b.Dispose();

            return result;
        }

        public static Bitmap erode(Image img)
        {
            Bitmap b = new Bitmap(img);
            Bitmap result = new Bitmap(b);
            BitmapData bBmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData resultBmData = result.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            bool[,] bBinImg = binarizeImage(bBmData);
            bool[,] mask = new bool[3, 3];
            mask[0, 0] = false; mask[1, 0] = true; mask[2, 0] = false;
            mask[0, 1] = true; mask[1, 1] = true; mask[2, 1] = true;
            mask[0, 2] = false; mask[1, 2] = true; mask[2, 2] = false;

            unsafe
            {
                int width = result.Width - 1;
                int height = result.Height - 1;
                for (int y = 1; y < height; ++y)
                {
                    for (int x = 1; x < width; ++x)
                    {
                        bool value = true;
                        for (int mx = 0; mx < 3; mx++)
                        {
                            for (int my = 0; my < 3; my++)
                            {

                                value &= !mask[mx, my] || bBinImg[x + mx - 1, y + my - 1];
                            }
                        }

                        setPixel(resultBmData, x, y, (value) ? Color.Black : Color.White);
                    }
                }
            }

            b.UnlockBits(bBmData);
            result.UnlockBits(resultBmData);

            b.Dispose();

            return result;
        }

        /// <summary>
        /// Helper Function to Erode/Dilate
        /// </summary>
        /// <param name="bmData"></param>
        /// <returns></returns>
        private static bool[,] binarizeImage(BitmapData bmData)
        {
            bool[,] binImg = new bool[bmData.Width, bmData.Height];

            for (int y = 1; y < bmData.Height; y++)
            {
                for (int x = 1; x < bmData.Width; x++)
                {
                    binImg[x, y] = (getPixel(bmData, x, y).R == 0);
                }
            }

            return binImg;
        }

        public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }

        public static Bitmap scale(Image img, int width, int height)
        {
            int targetWidth = width;
            int targetHeight = height;
            int xTarget, yTarget, xSource, ySource;

            Bitmap a = new Bitmap(img);
            Bitmap b = new Bitmap(targetWidth, targetHeight);
            BitmapData bBmData = a.LockBits(new Rectangle(0, 0, a.Width, a.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData resultBmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            for (xTarget = 0; xTarget < targetWidth; xTarget++)
            {
                for (yTarget = 0; yTarget < targetHeight; yTarget++)
                {
                    xSource = xTarget * a.Width / targetWidth;
                    ySource = yTarget * a.Height / targetHeight;
                    setPixel(resultBmData, xTarget, yTarget, getPixel(bBmData, xSource, ySource));
                }
            }

            a.UnlockBits(bBmData);
            b.UnlockBits(resultBmData);

            a.Dispose();

            return b;
        }

        public static int[] generateGrayHistogram(Image grayscaleImage)
        {
            int[] histogram = new int[256];
            Bitmap b = new Bitmap(grayscaleImage);
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            for (int i = 0; i < grayscaleImage.Width; i++)
            {
                for (int j = 0; j < grayscaleImage.Height; j++)
                {
                    histogram[getPixel(bmData, i, j).R]++;
                }
            }

            return histogram;
        }

        public static Bitmap histogram_equalization(Image i)
        {
            Bitmap b = new Bitmap(i);
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int height = b.Height;
            int width = b.Width;
            int numSamples, histSumR = 0, histSumG = 0, histSumB = 0;

            int[,] Ymap = new int[3, 256];
            int[,] hist = new int[3, 256];
            Color nakuha;

            unsafe
            {
                //histogram
                for (int x = 0; x < b.Width; x++)
                {
                    for (int y = 0; y < b.Height; y++)
                    {
                        nakuha = getPixel(bmData, x, y);
                        hist[0, nakuha.R]++;
                        hist[1, nakuha.G]++;
                        hist[2, nakuha.B]++;
                    }
                }
                // remap the Ys, use the maximum contrast (percent == 100) 
                // based on histogram equalization
                numSamples = (b.Width * b.Height);   // # of samples that contributed to the histogram
                for (int h = 0; h < 256; h++)
                {
                    histSumR += hist[0, h];
                    histSumG += hist[1, h];
                    histSumB += hist[2, h];
                    Ymap[0, h] = (int)((histSumR / (double)numSamples) * 255);
                    Ymap[1, h] = (int)((histSumG / (double)numSamples) * 255);
                    Ymap[2, h] = (int)((histSumB / (double)numSamples) * 255);
                }
                // enhance the region by remapping the intensities
                for (int y = 0; y < b.Height; y++)
                {
                    for (int x = 0; x < b.Width; x++)
                    {
                        Color temp = Color.FromArgb(Ymap[0, getPixel(bmData, x, y).R], Ymap[1, getPixel(bmData, x, y).G], Ymap[2, getPixel(bmData, x, y).B]);
                        setPixel(bmData, x, y, temp);
                    }
                }
            }

            b.UnlockBits(bmData);

            return b;
        }

        public static Bitmap GenerateGrayHistogramImage(int[] gray_hist)
        {
            int max = gray_hist.Max();
            Bitmap b = new Bitmap(256, max);

            Graphics g = Graphics.FromImage(b);

            //histogram 1d data;
            int[] histdata = gray_hist;
            g.Clear(Color.White);

            // plotting points based from histdata
            for (int x = 0; x < 256; x++)
            {
                g.DrawLine(Pens.Black, new Point(x, max - 1), new Point(x, max - gray_hist[x] - 1));
            }

            return b;
        }
    }

    public enum EdgeDetectionTechniques
    {
        PrewittHorizontal, PrewittVertical, SobelHorizontal, SobelVertical, SobelMixed
    }
}
