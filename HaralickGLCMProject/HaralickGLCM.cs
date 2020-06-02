using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace HaralickGLCMProject
{
    /// <summary>
    /// Source Literature: Haralick et. al - Textural Features for Image Classification, 1973
    /// Developer: Jomari Joseph A. Barrera, January 6, 2020
    /// Current Version Updates:
    /// *Features 1 to 14 are available.
    /// *Uses ALGLIB open source library for eigenvalues computation.
    /// </summary>
    class HaralickGLCM
    {
        int g; ///Number of gray-tones
        int[,] I; ///grayscale image matrix
        int[,] Ig; ///remapped grayscale image matrix to gray-tone values
        
        int[,] P_0; ///gray-tone spatial dependence matrix for 0 deg
        int[,] P_45; ///gray-tone spatial dependence matrix for 45 deg
        int[,] P_90; ///gray-tone spatial dependence matrix for 90 deg 
        int[,] P_135; ///gray-tone spatial dependence matrix for 135 deg

        double[,] p_0; ///normalized gray-tone spatial dependence matrix for 0 deg
        double[,] p_45; ///normalized gray-tone spatial dependence matrix for 45 deg
        double[,] p_90; ///normalized gray-tone spatial dependence matrix for 90 deg
        double[,] p_135; ///normalized gray-tone spatial dependence matrix for 135 deg
        
        ///Statistical values per angle a, based on the normalized gray-tone spatial-dependence matrix
        double grandmu_0, mux_0, muy_0, stdx_0, stdy_0;
        double grandmu_45, mux_45, muy_45, stdx_45, stdy_45;
        double grandmu_90, mux_90, muy_90, stdx_90, stdy_90;
        double grandmu_135, mux_135, muy_135, stdx_135, stdy_135;

        ///Textural Features per angle (index:angle - 0:0 deg, 1:45 deg, 2:90 deg, 3:135 deg)
        double[] f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, f14;

        ///Mean and Range of the textural features (0:f1, 1:f2, and so on), 28 features
        double[] mean_features, range_features;

        #region HaralickGLCM Properties
        /// <summary>
        /// Mean of F1
        /// </summary>
        public double MeanF1
        {
            get { return mean_features[0]; }
        }

        /// <summary>
        /// Mean of F2
        /// </summary>
        public double MeanF2
        {
            get { return mean_features[1]; }
        }

        /// <summary>
        /// Mean of F3
        /// </summary>
        public double MeanF3
        {
            get { return mean_features[2]; }
        }

        /// <summary>
        /// Mean of F4
        /// </summary>
        public double MeanF4
        {
            get { return mean_features[3]; }
        }

        /// <summary>
        /// Mean of F5
        /// </summary>
        public double MeanF5
        {
            get { return mean_features[4]; }
        }

        /// <summary>
        /// Mean of F6
        /// </summary>
        public double MeanF6
        {
            get { return mean_features[5]; }
        }

        /// <summary>
        /// Mean of F7
        /// </summary>
        public double MeanF7
        {
            get { return mean_features[6]; }
        }

        /// <summary>
        /// Mean of F8
        /// </summary>
        public double MeanF8
        {
            get { return mean_features[7]; }
        }

        /// <summary>
        /// Mean of F9
        /// </summary>
        public double MeanF9
        {
            get { return mean_features[8]; }
        }

        /// <summary>
        /// Mean of F10
        /// </summary>
        public double MeanF10
        {
            get { return mean_features[9]; }
        }

        /// <summary>
        /// Mean of F11
        /// </summary>
        public double MeanF11
        {
            get { return mean_features[10]; }
        }

        /// <summary>
        /// Mean of F12
        /// </summary>
        public double MeanF12
        {
            get { return mean_features[11]; }
        }

        /// <summary>
        /// Mean of F13
        /// </summary>
        public double MeanF13
        {
            get { return mean_features[12]; }
        }

        /// <summary>
        /// Mean of F14
        /// </summary>
        public double MeanF14
        {
            get { return mean_features[13]; }
        }

        /// <summary>
        /// Range of F1
        /// </summary>
        public double RangeF1
        {
            get { return range_features[0]; }
        }

        /// <summary>
        /// Range of F2
        /// </summary>
        public double RangeF2
        {
            get { return range_features[1]; }
        }

        /// <summary>
        /// Range of F3
        /// </summary>
        public double RangeF3
        {
            get { return range_features[2]; }
        }

        /// <summary>
        /// Range of F4
        /// </summary>
        public double RangeF4
        {
            get { return range_features[3]; }
        }

        /// <summary>
        /// Range of F5
        /// </summary>
        public double RangeF5
        {
            get { return range_features[4]; }
        }

        /// <summary>
        /// Range of F6
        /// </summary>
        public double RangeF6
        {
            get { return range_features[5]; }
        }

        /// <summary>
        /// Range of F7
        /// </summary>
        public double RangeF7
        {
            get { return range_features[6]; }
        }

        /// <summary>
        /// Range of F8
        /// </summary>
        public double RangeF8
        {
            get { return range_features[7]; }
        }

        /// <summary>
        /// Range of F9
        /// </summary>
        public double RangeF9
        {
            get { return range_features[8]; }
        }

        /// <summary>
        /// Range of F10
        /// </summary>
        public double RangeF10
        {
            get { return range_features[9]; }
        }

        /// <summary>
        /// Range of F11
        /// </summary>
        public double RangeF11
        {
            get { return range_features[10]; }
        }

        /// <summary>
        /// Range of F12
        /// </summary>
        public double RangeF12
        {
            get { return range_features[11]; }
        }

        /// <summary>
        /// Range of F13
        /// </summary>
        public double RangeF13
        {
            get { return range_features[12]; }
        }

        /// <summary>
        /// Range of F14
        /// </summary>
        public double RangeF14
        {
            get { return range_features[13]; }
        }
        #endregion


        /// <summary>
        /// Haralick GLCM Constructor
        /// </summary>
        /// <param name="img">Grayscale image, I</param>
        /// <param name="g">Number of gray-tones, g</param>
        /// <param name="d">Distance, d</param>
        /// Representations and terminologies
        /// *Graytone value, G ∈ {0,1,...,g-1}
        public HaralickGLCM(Image img, int g, int d)
        {
            Bitmap b = new Bitmap(img);
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            ///Transfer gray values from image to grayscale matrix
            I = new int[img.Width, img.Height];
            for (int i = 0; i < img.Width; i++)
                for (int j = 0; j < img.Height; j++)
                    I[i, j] = ImageProcessing.getPixel(bmData, i, j).R;
            b.UnlockBits(bmData);

            ///Remap to gray-tones
            this.g = g;
            Ig = new int[img.Width, img.Height];
            double g_range = (double)256 / g;
            for (int i = 0; i < I.GetLength(0); i++)
                for (int j = 0; j < I.GetLength(0); j++)
                    Ig[i, j] = (int)(I[i, j] / g_range);

            int R_0 = 0, R_45 = 0, R_90 = 0, R_135 = 0; ///sum of neighboring cells given an angle a, R_a

            ///Constructing the gray-tone spatial-dependence matrices
            P_0 = new int[g, g];
            P_45 = new int[g, g];
            P_90 = new int[g, g];
            P_135 = new int[g, g];
            for (int m = 0; m < Ig.GetLength(0); m++)
            {
                for (int n = 0; n < Ig.GetLength(1); n++)
                {
                    for (int k = Math.Max(0, m - d); k < Math.Min(Ig.GetLength(0), m + d + 1); k++)
                    {
                        for (int l = Math.Max(0, n - d); l < Math.Min(Ig.GetLength(1), n + d + 1); l++)
                        {
                            if (k - m == 0 && Math.Abs(l - n) == d)
                            {
                                P_0[Ig[k, l], Ig[m, n]]++;
                                R_0++;
                            }
                            if ((k - m == d && l - n == -d) || (k - m == -d && l - n == d))
                            {
                                P_45[Ig[k, l], Ig[m, n]]++;
                                R_45++;
                            }
                            if (Math.Abs(k - m) == d && l - n == 0)
                            {
                                P_90[Ig[k, l], Ig[m, n]]++;
                                R_90++;
                            }
                            if ((k - m == d && l - n == d) || (k - m == -d && l - n == -d))
                            {
                                P_135[Ig[k, l], Ig[m, n]]++;
                                R_135++;
                            }
                        }
                    }
                }
            }
            
            ///Constructing the normalized gray-tone spatial-dependence matrices
            p_0 = new double[g, g];
            p_45 = new double[g, g];
            p_90 = new double[g, g];
            p_135 = new double[g, g];
            for (int i = 0; i < g; i++)
            {
                for (int j = 0; j < g; j++)
                {
                    p_0[i, j] = (double)P_0[i, j] / R_0;
                    p_45[i, j] = (double)P_45[i, j] / R_45;
                    p_90[i, j] = (double)P_90[i, j] / R_90;
                    p_135[i, j] = (double)P_135[i, j] / R_135;
                }
            }

            computeStatisticalValues();
            computeAllTexturalFeatures();
        }

        /// <summary>
        /// P_x function
        /// </summary>
        /// <param name="gray_tone">i, ith entry</param>
        /// <param name="a">angle</param>
        /// <returns>Returns the ith entry in the marginal-probability matrix obtained by summing the rows of p(i,j).</returns>
        private double px(int gray_tone, Angle a)
        {
            double sumpx = 0;
            for (int i = 0; i < g; i++)
            {
                switch (a)
                {
                    case Angle.Zero: sumpx += p_0[gray_tone, i];
                        break;
                    case Angle.FortyFive: sumpx += p_45[gray_tone, i];
                        break;
                    case Angle.Ninety: sumpx += p_90[gray_tone, i];
                        break;
                    case Angle.OneHundredThirtyFive: sumpx += p_135[gray_tone, i];
                        break;
                }
            }
            return sumpx;
        }

        /// <summary>
        /// P_y function
        /// </summary>
        /// <param name="gray_tone">j, jth entry</param>
        /// <param name="a">angle</param>
        /// <returns>Returns the jth entry in the marginal-probability matrix obtained by summing the columns of p(i,j).</returns>
        private double py(int gray_tone, Angle a)
        {
            double sumpy = 0;
            for (int i = 0; i < g; i++)
            {
                switch (a)
                {
                    case Angle.Zero: sumpy += p_0[i, gray_tone];
                        break;
                    case Angle.FortyFive: sumpy += p_45[i, gray_tone];
                        break;
                    case Angle.Ninety: sumpy += p_90[i, gray_tone];
                        break;
                    case Angle.OneHundredThirtyFive: sumpy += p_135[i, gray_tone];
                        break;
                }
            }
            return sumpy;
        }

        /// <summary>
        /// P_(x-y) function
        /// </summary>
        /// <param name="k">k, gray-tone difference</param>
        /// <param name="a">angle</param>
        /// <returns>Returns the P_x-y(k) value given an angle a</returns>
        private double pxsuby(int k, Angle a)
        {
            double sumpxsuby = 0;
            for (int i = 1; i <= g; i++)
            {
                for (int j = 1; j <= g; j++)
                {
                    switch (a)
                    {
                        case Angle.Zero: sumpxsuby += (Math.Abs(i - j) == k) ? p_0[i - 1, j - 1] : 0;
                            break;
                        case Angle.FortyFive: sumpxsuby += (Math.Abs(i - j) == k) ? p_45[i - 1, j - 1] : 0;
                            break;
                        case Angle.Ninety: sumpxsuby += (Math.Abs(i - j) == k) ? p_90[i - 1, j - 1] : 0;
                            break;
                        case Angle.OneHundredThirtyFive: sumpxsuby += (Math.Abs(i - j) == k) ? p_135[i - 1, j - 1] : 0;
                            break;
                    }
                }
            }
            return sumpxsuby;
        }

        /// <summary>
        /// P_(x+y) function
        /// </summary>
        /// <param name="k">k, gray-tone sum</param>
        /// <param name="a">angle</param>
        /// <returns>Returns the P_x+y(k) value given an angle a</returns>
        private double pxaddy(int k, Angle a)
        {
            double sumpxaddy = 0;
            for (int i = 1; i <= g; i++)
            {
                for (int j = 1; j <= g; j++)
                {
                    switch (a)
                    {
                        case Angle.Zero: sumpxaddy += (i + j == k) ? p_0[i - 1, j - 1] : 0;
                            break;
                        case Angle.FortyFive: sumpxaddy += (i + j == k) ? p_45[i - 1, j - 1] : 0;
                            break;
                        case Angle.Ninety: sumpxaddy += (i + j == k) ? p_90[i - 1, j - 1] : 0;
                            break;
                        case Angle.OneHundredThirtyFive: sumpxaddy += (i + j == k) ? p_135[i - 1, j - 1] : 0;
                            break;
                    }
                }
            }
            return sumpxaddy;
        }

        /// <summary>
        /// Computes the grandmu, mux, muy, sigmax, sigmay for all 4 angles
        /// </summary>
        private void computeStatisticalValues()
        {
            #region Mean
            grandmu_0 = 0;
            grandmu_45 = 0;
            grandmu_90 = 0;
            grandmu_135 = 0;
            for (int i = 0; i < g; i++)
            {
                for (int j = 0; j < g; j++)
                {
                    grandmu_0 += p_0[i, j];
                    grandmu_45 += p_0[i, j];
                    grandmu_90 += p_0[i, j];
                    grandmu_135 += p_0[i, j];
                }
            }
            grandmu_0 /= g * g;
            grandmu_45 /= g * g;
            grandmu_90 /= g * g;
            grandmu_135 /= g * g;

            double[] meanx_0 = new double[g];
            double[] meanx_45 = new double[g];
            double[] meanx_90 = new double[g];
            double[] meanx_135 = new double[g];
            double[] meany_0 = new double[g];
            double[] meany_45 = new double[g];
            double[] meany_90 = new double[g];
            double[] meany_135 = new double[g];
            for (int i = 0; i < g; i++)
            {
                for (int j = 0; j < g; j++)
                {
                    meanx_0[i] += p_0[i, j];
                    meanx_45[i] += p_45[i, j];
                    meanx_90[i] += p_90[i, j];
                    meanx_135[i] += p_135[i, j];

                    meany_0[j] += p_0[i, j];
                    meany_45[j] += p_45[i, j];
                    meany_90[j] += p_90[i, j];
                    meany_135[j] += p_135[i, j];
                }
                meanx_0[i] /= g;
                meanx_45[i] /= g;
                meanx_90[i] /= g;
                meanx_135[i] /= g;
            }
            for (int j = 0; j < g; j++)
            {
                meany_0[j] /= g;
                meany_45[j] /= g;
                meany_90[j] /= g;
                meany_135[j] /= g;
            }
            mux_0 = meanx_0.Sum() / g;
            mux_45 = meanx_45.Sum() / g;
            mux_90 = meanx_90.Sum() / g;
            mux_135 = meanx_135.Sum() / g;
            muy_0 = meany_0.Sum() / g;
            muy_45 = meany_45.Sum() / g;
            muy_90 = meany_90.Sum() / g;
            muy_135 = meany_135.Sum() / g;
            #endregion

            #region Standard Deviation
            double[] standx_0 = new double[g];
            double[] standx_45 = new double[g];
            double[] standx_90 = new double[g];
            double[] standx_135 = new double[g];
            double[] standy_0 = new double[g];
            double[] standy_45 = new double[g];
            double[] standy_90 = new double[g];
            double[] standy_135 = new double[g];

            double[][] datax_0 = new double[g][];
            double[][] datax_45 = new double[g][];
            double[][] datax_90 = new double[g][];
            double[][] datax_135 = new double[g][];
            double[][] datay_0 = new double[g][];
            double[][] datay_45 = new double[g][];
            double[][] datay_90 = new double[g][];
            double[][] datay_135 = new double[g][];
            for (int j = 0; j < g; j++)
            {
                datay_0[j] = new double[g];
                datay_45[j] = new double[g];
                datay_90[j] = new double[g];
                datay_135[j] = new double[g];
            }
            for (int i = 0; i < g; i++)
            {
                datax_0[i] = new double[g];
                datax_45[i] = new double[g];
                datax_90[i] = new double[g];
                datax_135[i] = new double[g];
                for (int j = 0; j < g; j++)
                {
                    datax_0[i][j] = p_0[i, j];
                    datax_45[i][j] = p_45[i, j];
                    datax_90[i][j] = p_90[i, j];
                    datax_135[i][j] = p_135[i, j];
                    datay_0[j][i] = p_0[i, j];
                    datay_45[j][i] = p_45[i, j];
                    datay_90[j][i] = p_90[i, j];
                    datay_135[j][i] = p_135[i, j];
                }
            }

            for (int i = 0; i < g; i++)
            {
                standx_0[i] = Math.Sqrt(computeVariance(datax_0[i]));
                standx_45[i] = Math.Sqrt(computeVariance(datax_45[i]));
                standx_90[i] = Math.Sqrt(computeVariance(datax_90[i]));
                standx_135[i] = Math.Sqrt(computeVariance(datax_135[i]));
                standy_0[i] = Math.Sqrt(computeVariance(datay_0[i]));
                standy_45[i] = Math.Sqrt(computeVariance(datay_45[i]));
                standy_90[i] = Math.Sqrt(computeVariance(datay_90[i]));
                standy_135[i] = Math.Sqrt(computeVariance(datay_135[i]));
            }

            stdx_0 = Math.Sqrt(computeVariance(standx_0));
            stdx_45 = Math.Sqrt(computeVariance(standx_45));
            stdx_90 = Math.Sqrt(computeVariance(standx_90));
            stdx_135 = Math.Sqrt(computeVariance(standx_135));
            stdy_0 = Math.Sqrt(computeVariance(standy_0));
            stdy_45 = Math.Sqrt(computeVariance(standy_45));
            stdy_90 = Math.Sqrt(computeVariance(standy_90));
            stdy_135 = Math.Sqrt(computeVariance(standy_135));
            #endregion
        }

        /// <summary>
        /// Computes the variance of a one dimensional vector data
        /// </summary>
        /// <param name="data">Vector data</param>
        /// <returns>Variance of data</returns>
        private double computeVariance(double[] data)
        {
            double variance = 0;
            if (data.Length > 1)
            {
                double avg = data.Average();
                double sumOfSquares = 0.0;

                foreach (int num in data)
                {
                    sumOfSquares += Math.Pow((num - avg), 2.0);
                }

                variance = sumOfSquares / (double)(data.Length - 1);
            }
            return variance;
        }

        /// <summary>
        /// Computes the entropy of a one dimensional vector data
        /// </summary>
        /// <param name="data">Vector data</param>
        /// <returns>Entropy of data</returns>
        private double computeEntropy(double[] data)
        {
            double entropy = 0;
            for (int i = 0; i < data.Length; i++)
                if (data[i] != 0)
                    entropy += data[i] * Math.Log10(data[i]);
            return -entropy;
        }

        /// <summary>
        /// HXY1 Function
        /// </summary>
        /// <param name="a">angle</param>
        /// <returns>The HXY1 value of an angle a</returns>
        private double hxy1(Angle a)
        {
            double hxy1 = 0;
            for (int i = 1; i <= g; i++)
            {
                for (int j = 1; j <= g; j++)
                {
                    switch (a)
                    {
                        case Angle.Zero: if (p_0[i - 1, j - 1] != 0)
                                hxy1 += p_0[i - 1, j - 1] * Math.Log10(px(i - 1, Angle.Zero) * py(j - 1, Angle.Zero));
                            break;
                        case Angle.FortyFive: if (p_45[i - 1, j - 1] != 0)
                                hxy1 += p_45[i - 1, j - 1] * Math.Log10(px(i - 1, Angle.FortyFive) * py(j - 1, Angle.FortyFive));
                            break;
                        case Angle.Ninety: if (p_90[i - 1, j - 1] != 0)
                                hxy1 += p_90[i - 1, j - 1] * Math.Log10(px(i - 1, Angle.Ninety) * py(j - 1, Angle.Ninety));
                            break;
                        case Angle.OneHundredThirtyFive: if (p_135[i - 1, j - 1] != 0)
                                hxy1 += p_135[i - 1, j - 1] * Math.Log10(px(i - 1, Angle.OneHundredThirtyFive) * py(j - 1, Angle.OneHundredThirtyFive));
                            break;
                    }
                }
            }

            return -hxy1;
        }

        /// <summary>
        /// HXY2 Function
        /// </summary>
        /// <param name="a">angle</param>
        /// <returns>The HXY2 value of an angle a</returns>
        private double hxy2(Angle a)
        {
            double hxy2 = 0;
            for (int i = 1; i <= g; i++)
            {
                for (int j = 1; j <= g; j++)
                {
                    if (px(i - 1, a) * py(j - 1, a) != 0)
                        hxy2 += px(i - 1, a) * py(j - 1, a) * Math.Log10(px(i - 1, a) * py(j - 1, a));
                }
            }

            return -hxy2;
        }

        /// <summary>
        /// Computes the f1 values for all angles
        /// </summary>
        private void computeF1()
        {
            f1 = new double[4];
            for (int i = 1; i <= g; i++)
            {
                for (int j = 1; j <= g; j++)
                {
                    f1[0] += Math.Pow(p_0[i - 1, j - 1], 2);
                    f1[1] += Math.Pow(p_45[i - 1, j - 1], 2);
                    f1[2] += Math.Pow(p_90[i - 1, j - 1], 2);
                    f1[3] += Math.Pow(p_135[i - 1, j - 1], 2);
                }
            }
        }

        /// <summary>
        /// Computes the f2 values for all angles
        /// </summary>
        private void computeF2()
        {
            f2 = new double[4];
            for (int k = 0; k < g; k++)
            {
                f2[0] += Math.Pow(k, 2) * pxsuby(k, Angle.Zero);
                f2[1] += Math.Pow(k, 2) * pxsuby(k, Angle.FortyFive);
                f2[2] += Math.Pow(k, 2) * pxsuby(k, Angle.Ninety);
                f2[3] += Math.Pow(k, 2) * pxsuby(k, Angle.OneHundredThirtyFive);
            }
        }

        /// <summary>
        /// Computes the f3 values for all angles
        /// </summary>
        private void computeF3()
        {
            f3 = new double[4];
            for (int i = 1; i <= g; i++)
            {
                for (int j = 1; j <= g; j++)
                {
                    f3[0] += (i * j * p_0[i - 1, j - 1]) - (mux_0 * muy_0);
                    f3[1] += (i * j * p_45[i - 1, j - 1]) - (mux_45 * muy_45);
                    f3[2] += (i * j * p_90[i - 1, j - 1]) - (mux_90 * muy_90);
                    f3[3] += (i * j * p_135[i - 1, j - 1]) - (mux_135 * muy_135);
                }
            }

            f3[0] /= stdx_0 * stdy_0;
            f3[1] /= stdx_45 * stdy_45;
            f3[2] /= stdx_90 * stdy_90;
            f3[3] /= stdx_135 * stdy_135;
        }

        /// <summary>
        /// Computes the f4 values for all angles
        /// </summary>
        private void computeF4()
        {
            f4 = new double[4];
            for (int i = 1; i <= g; i++)
            {
                for (int j = 1; j <= g; j++)
                {
                    f4[0] += Math.Pow(i - grandmu_0, 2) * p_0[i - 1, j - 1];
                    f4[1] += Math.Pow(i - grandmu_45, 2) * p_45[i - 1, j - 1];
                    f4[2] += Math.Pow(i - grandmu_90, 2) * p_90[i - 1, j - 1];
                    f4[3] += Math.Pow(i - grandmu_135, 2) * p_135[i - 1, j - 1];
                }
            }
        }

        /// <summary>
        /// Computes the f5 values for all angles
        /// </summary>
        private void computeF5()
        {
            f5 = new double[4];
            for (int i = 1; i <= g; i++)
            {
                for (int j = 1; j <= g; j++)
                {
                    f5[0] += p_0[i - 1, j - 1] / (1 + Math.Pow((i - j), 2));
                    f5[1] += p_45[i - 1, j - 1] / (1 + Math.Pow((i - j), 2));
                    f5[2] += p_90[i - 1, j - 1] / (1 + Math.Pow((i - j), 2));
                    f5[3] += p_135[i - 1, j - 1] / (1 + Math.Pow((i - j), 2));
                }
            }
        }

        /// <summary>
        /// Computes the f6 values for all angles
        /// </summary>
        private void computeF6()
        {
            f6 = new double[4];
            for (int i = 2; i <= 2 * g; i++)
            {
                f6[0] += i * pxaddy(i, Angle.Zero);
                f6[1] += i * pxaddy(i, Angle.FortyFive);
                f6[2] += i * pxaddy(i, Angle.Ninety);
                f6[3] += i * pxaddy(i, Angle.OneHundredThirtyFive);
            }
        }

        /// <summary>
        /// Computes the f7 values for all angles. computeF8 needs to be called before calling this function.
        /// </summary>
        private void computeF7()
        {
            f7 = new double[4];
            for (int i = 2; i <= 2 * g; i++)
            {
                f7[0] += Math.Pow(i - f8[0], 2) * pxaddy(i, Angle.Zero);
                f7[1] += Math.Pow(i - f8[1], 2) * pxaddy(i, Angle.FortyFive);
                f7[2] += Math.Pow(i - f8[2], 2) * pxaddy(i, Angle.Ninety);
                f7[3] += Math.Pow(i - f8[3], 2) * pxaddy(i, Angle.OneHundredThirtyFive);
            }
        }

        /// <summary>
        /// Computes the f8 values for all angles.
        /// </summary>
        private void computeF8()
        {
            f8 = new double[4];
            for (int i = 2; i <= 2 * g; i++)
            {
                if (pxaddy(i, Angle.Zero) != 0)
                    f8[0] += pxaddy(i, Angle.Zero) * Math.Log10(pxaddy(i, Angle.Zero));
                if (pxaddy(i, Angle.FortyFive) != 0)
                    f8[1] += pxaddy(i, Angle.FortyFive) * Math.Log10(pxaddy(i, Angle.FortyFive));
                if (pxaddy(i, Angle.Ninety) != 0)
                    f8[2] += pxaddy(i, Angle.Ninety) * Math.Log10(pxaddy(i, Angle.Ninety));
                if (pxaddy(i, Angle.OneHundredThirtyFive) != 0)
                    f8[3] += pxaddy(i, Angle.OneHundredThirtyFive) * Math.Log10(pxaddy(i, Angle.OneHundredThirtyFive));
            }

            for (int i = 0; i < 4; i++)
                f8[i] *= -1;
        }

        /// <summary>
        /// Computes the f9 values for all angles.
        /// </summary>
        private void computeF9()
        {
            f9 = new double[4];
            for (int i = 1; i <= g; i++)
            {
                for (int j = 1; j <= g; j++)
                {
                    if (p_0[i - 1, j - 1] != 0)
                        f9[0] += p_0[i - 1, j - 1] * Math.Log10(p_0[i - 1, j - 1]);
                    if (p_45[i - 1, j - 1] != 0)
                        f9[1] += p_45[i - 1, j - 1] * Math.Log10(p_45[i - 1, j - 1]);
                    if (p_90[i - 1, j - 1] != 0)
                        f9[2] += p_90[i - 1, j - 1] * Math.Log10(p_90[i - 1, j - 1]);
                    if (p_135[i - 1, j - 1] != 0)
                        f9[3] += p_135[i - 1, j - 1] * Math.Log10(p_135[i - 1, j - 1]);
                }
            }
            for (int i = 0; i < 4; i++)
                f9[i] *= -1;
        }

        /// <summary>
        /// Computes the f10 values for all angles.
        /// </summary>
        private void computeF10()
        {
            double[][] col_f10 = new double[4][];
            for (int i = 0; i < 4; i++)
                col_f10[i] = new double[g];

            for (int k = 0; k < g; k++)
            {
                col_f10[0][k] = pxsuby(k, Angle.Zero);
                col_f10[1][k] = pxsuby(k, Angle.FortyFive);
                col_f10[2][k] = pxsuby(k, Angle.Ninety);
                col_f10[3][k] = pxsuby(k, Angle.OneHundredThirtyFive);
            }

            f10 = new double[4];
            f10[0] = computeVariance(col_f10[0]);
            f10[1] = computeVariance(col_f10[1]);
            f10[2] = computeVariance(col_f10[2]);
            f10[3] = computeVariance(col_f10[3]);
        }

        /// <summary>
        /// Computes the f11 values for all angles.
        /// </summary>
        private void computeF11()
        {
            f11 = new double[4];
            for (int i = 0; i < g; i++)
            {
                if (pxsuby(i, Angle.Zero) != 0)
                    f11[0] += pxsuby(i, Angle.Zero) * Math.Log10(pxsuby(i, Angle.Zero));
                if (pxsuby(i, Angle.FortyFive) != 0)
                    f11[1] += pxsuby(i, Angle.FortyFive) * Math.Log10(pxsuby(i, Angle.FortyFive));
                if (pxsuby(i, Angle.Ninety) != 0)
                    f11[2] += pxsuby(i, Angle.Ninety) * Math.Log10(pxsuby(i, Angle.Ninety));
                if (pxsuby(i, Angle.OneHundredThirtyFive) != 0)
                    f11[3] += pxsuby(i, Angle.OneHundredThirtyFive) * Math.Log10(pxsuby(i, Angle.OneHundredThirtyFive));
            }

            for (int i = 0; i < 4; i++)
                f11[i] *= -1;
        }

        /// <summary>
        /// Computes the f12 values for all angles. computeF9 needs to be called before calling this function.
        /// </summary>
        private void computeF12()
        {
            double[][] hx, hy;
            hx = new double[4][];
            hy = new double[4][];
            for (int i = 0; i < 4; i++)
            {
                hx[i] = new double[g];
                hy[i] = new double[g];
            }

            for (int k = 0; k < g; k++)
            {
                hx[0][k] = px(k, Angle.Zero);
                hx[1][k] = px(k, Angle.FortyFive);
                hx[2][k] = px(k, Angle.Ninety);
                hx[3][k] = px(k, Angle.OneHundredThirtyFive);
                hy[0][k] = py(k, Angle.Zero);
                hy[1][k] = py(k, Angle.FortyFive);
                hy[2][k] = py(k, Angle.Ninety);
                hy[3][k] = py(k, Angle.OneHundredThirtyFive);
            }

            double[] HX = new double[4];
            double[] HY = new double[4];
            f12 = new double[4];

            for (int i = 0; i < 4; i++)
            {
                HX[i] = computeEntropy(hx[i]);
                HY[i] = computeEntropy(hy[i]);
            }

            f12[0] = (f9[0] - hxy1(Angle.Zero)) / Math.Max(HX[0], HY[0]);
            f12[1] = (f9[1] - hxy1(Angle.FortyFive)) / Math.Max(HX[1], HY[1]);
            f12[2] = (f9[2] - hxy1(Angle.Ninety)) / Math.Max(HX[2], HY[2]);
            f12[3] = (f9[3] - hxy1(Angle.OneHundredThirtyFive)) / Math.Max(HX[3], HY[3]);
        }

        /// <summary>
        /// Computes the f13 values for all angles. computeF9 needs to be called before calling this function.
        /// </summary>
        private void computeF13()
        {
            f13 = new double[4];
            f13[0] = Math.Sqrt(1 - Math.Pow(Math.E, -2 * (hxy2(Angle.Zero) - f9[0])));
            f13[1] = Math.Sqrt(1 - Math.Pow(Math.E, -2 * (hxy2(Angle.FortyFive) - f9[1])));
            f13[2] = Math.Sqrt(1 - Math.Pow(Math.E, -2 * (hxy2(Angle.Ninety) - f9[2])));
            f13[3] = Math.Sqrt(1 - Math.Pow(Math.E, -2 * (hxy2(Angle.OneHundredThirtyFive) - f9[3])));
        }

        /// <summary>
        /// Computes the f14 values for all angles. computeF9 needs to be called before calling this function.
        /// </summary>
        private void computeF14()
        {
            ///Constructing Q matrix for all angles
            double[][,] Q = new double[4][,];
            Q[0] = new double[g, g];
            Q[1] = new double[g, g];
            Q[2] = new double[g, g];
            Q[3] = new double[g, g];

            for (int i = 0; i < g; i++)
            {
                for (int j = 0; j < g; j++)
                {
                    double[] sumk = new double[4];
                    for (int k = 0; k < g; k++)
                    {
                        if ((p_0[i, k] * p_0[j, k]) != 0)
                            sumk[0] += (p_0[i, k] * p_0[j, k]) / (px(i, Angle.Zero) * py(k, Angle.Zero));
                        if ((p_45[i, k] * p_45[j, k]) != 0)
                            sumk[1] += (p_45[i, k] * p_45[j, k]) / (px(i, Angle.FortyFive) * py(k, Angle.FortyFive));
                        if ((p_90[i, k] * p_90[j, k]) != 0)
                            sumk[2] += (p_90[i, k] * p_90[j, k]) / (px(i, Angle.Ninety) * py(k, Angle.Ninety));
                        if ((p_135[i, k] * p_135[j, k]) != 0)
                            sumk[3] += (p_135[i, k] * p_135[j, k]) / (px(i, Angle.OneHundredThirtyFive) * py(k, Angle.OneHundredThirtyFive));
                    }
                    Q[0][i, j] = sumk[0];
                    Q[1][i, j] = sumk[1];
                    Q[2][i, j] = sumk[2];
                    Q[3][i, j] = sumk[3];
                }
            }

            double[][] eigenvalues_Q = new double[4][];
            double[] dummy1; double[,] dummy2; ///dummy references for eigenvectors (not needed)

            ///Computing eigenvalues per angle
            alglib.rmatrixevd(Q[0], g, 0, out eigenvalues_Q[0], out dummy1, out dummy2, out dummy2);
            alglib.rmatrixevd(Q[1], g, 0, out eigenvalues_Q[1], out dummy1, out dummy2, out dummy2);
            alglib.rmatrixevd(Q[2], g, 0, out eigenvalues_Q[2], out dummy1, out dummy2, out dummy2);
            alglib.rmatrixevd(Q[3], g, 0, out eigenvalues_Q[3], out dummy1, out dummy2, out dummy2);

            List<double>[] ev = new List<double>[4];
            f14 = new double[4];

            ///Assigning f14 to the 2nd largest eigenvalue of Q per angle
            for (int i = 0; i < 4; i++)
            {
                ev[i] = eigenvalues_Q[i].ToList();
                ev[i].Remove(ev[i].Max());
                f14[i] = Math.Sqrt(ev[i].Max());
            }
        }

        /// <summary>
        /// Compute all 26 textural features. Computes the mean and range of features 1 to 13.
        /// </summary>
        private void computeAllTexturalFeatures()
        {
            computeF1();
            computeF2();
            computeF3();
            computeF4();
            computeF5();
            computeF6();
            computeF8();
            computeF7();
            computeF9();
            computeF10();
            computeF11();
            computeF12();
            computeF13();
            computeF14();

            mean_features = new double[14];
            range_features = new double[14];

            mean_features[0] = f1.Average();
            mean_features[1] = f2.Average();
            mean_features[2] = f3.Average();
            mean_features[3] = f4.Average();
            mean_features[4] = f5.Average();
            mean_features[5] = f6.Average();
            mean_features[6] = f7.Average();
            mean_features[7] = f8.Average();
            mean_features[8] = f9.Average();
            mean_features[9] = f10.Average();
            mean_features[10] = f11.Average();
            mean_features[11] = f12.Average();
            mean_features[12] = f13.Average();
            mean_features[13] = f14.Average();

            range_features[0] = f1.Max() - f1.Min();
            range_features[1] = f2.Max() - f2.Min();
            range_features[2] = f3.Max() - f3.Min();
            range_features[3] = f4.Max() - f4.Min();
            range_features[4] = f5.Max() - f5.Min();
            range_features[5] = f6.Max() - f6.Min();
            range_features[6] = f7.Max() - f7.Min();
            range_features[7] = f8.Max() - f8.Min();
            range_features[8] = f9.Max() - f9.Min();
            range_features[9] = f10.Max() - f10.Min();
            range_features[10] = f11.Max() - f11.Min();
            range_features[11] = f12.Max() - f12.Min();
            range_features[12] = f13.Max() - f13.Min();
            range_features[13] = f14.Max() - f14.Min();
        }

        public enum Angle { Zero, FortyFive, Ninety, OneHundredThirtyFive }
    }
}
