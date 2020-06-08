using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic;

namespace HaralickGLCMProject
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void bulkExtractGLCMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                #region
                bool validAll = false;
                int vars = 1;
                int[] gtls = new int[1];
                int[] ds = new int[1];
                do
                {
                    try
                    {
                        vars = Convert.ToInt32(Interaction.InputBox("Input number of parameter variations:", "Parameter variations"));
                        gtls = new int[vars];
                        ds = new int[vars];
                        validAll = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Please input valid numeric values!");
                    }
                } while (!validAll);

                validAll = true;
                for (int i = 0; i < vars; i++)
                {
                    try
                    {
                        gtls[i] = Convert.ToInt32(Interaction.InputBox("Input g:", "Variation #" + (i + 1) + " - Gray-tone Level"));
                        ds[i] = Convert.ToInt32(Interaction.InputBox("Input d:", "Variation #" + (i + 1) + "  - Distance"));
                        validAll = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Please input valid numeric values!");
                        i--;
                    }
                }
                #endregion

                string note = "GLCM Extraction Finished!\n Look at your results at:\n";
                for (int i = 0; i < vars; i++)
                {
                    string[] file_extensions = new string[8] { "*.png", "*.jpg", "*.jpeg", "*.jfif", "*.bmp", "*.tif", "*.tiff", "*.gif" };
                    IEnumerable<string> filenames = new List<string>();
                    foreach (string s in file_extensions)
                        filenames = filenames.Concat(Directory.GetFiles(fbd.SelectedPath, s, SearchOption.AllDirectories).ToList());

                    string to_write = "";
                    HaralickGLCM haralickglcm;

                    to_write += "filename,";
                    to_write += "m_f1,";
                    to_write += "m_f2,";
                    to_write += "m_f3,";
                    to_write += "m_f4,";
                    to_write += "m_f5,";
                    to_write += "m_f6,";
                    to_write += "m_f7,";
                    to_write += "m_f8,";
                    to_write += "m_f9,";
                    to_write += "m_f10,";
                    to_write += "m_f11,";
                    to_write += "m_f12,";
                    to_write += "m_f13,";
                    to_write += "m_f14,";
                    to_write += "r_f1,";
                    to_write += "r_f2,";
                    to_write += "r_f3,";
                    to_write += "r_f4,";
                    to_write += "r_f5,";
                    to_write += "r_f6,";
                    to_write += "r_f7,";
                    to_write += "r_f8,";
                    to_write += "r_f9,";
                    to_write += "r_f10,";
                    to_write += "r_f11,";
                    to_write += "r_f12,";
                    to_write += "r_f13,";
                    to_write += "r_f14\n";

                    validAll = false;
                    int g = gtls[i], d = ds[i];

                    foreach (string filename in filenames)
                    {
                        Console.WriteLine(" Processing: " + Path.GetFileName(filename));
                        //haralickglcm = new HaralickGLCM(ImageProcessing.grayscale(ImageProcessing.scale(new Bitmap(filename), 500, 500)), g, d);
                        haralickglcm = new HaralickGLCM(ImageProcessing.grayscale(new Bitmap(filename)), g, d);
                        to_write += Path.GetFileName(filename) + ",";

                        to_write += haralickglcm.MeanF1 + ",";
                        to_write += haralickglcm.MeanF2 + ",";
                        to_write += haralickglcm.MeanF3 + ",";
                        to_write += haralickglcm.MeanF4 + ",";
                        to_write += haralickglcm.MeanF5 + ",";
                        to_write += haralickglcm.MeanF6 + ",";
                        to_write += haralickglcm.MeanF7 + ",";
                        to_write += haralickglcm.MeanF8 + ",";
                        to_write += haralickglcm.MeanF9 + ",";
                        to_write += haralickglcm.MeanF10 + ",";
                        to_write += haralickglcm.MeanF11 + ",";
                        to_write += haralickglcm.MeanF12 + ",";
                        to_write += haralickglcm.MeanF13 + ",";
                        to_write += haralickglcm.MeanF14 + ",";

                        to_write += haralickglcm.RangeF1 + ",";
                        to_write += haralickglcm.RangeF2 + ",";
                        to_write += haralickglcm.RangeF3 + ",";
                        to_write += haralickglcm.RangeF4 + ",";
                        to_write += haralickglcm.RangeF5 + ",";
                        to_write += haralickglcm.RangeF6 + ",";
                        to_write += haralickglcm.RangeF7 + ",";
                        to_write += haralickglcm.RangeF8 + ",";
                        to_write += haralickglcm.RangeF9 + ",";
                        to_write += haralickglcm.RangeF10 + ",";
                        to_write += haralickglcm.RangeF11 + ",";
                        to_write += haralickglcm.RangeF12 + ",";
                        to_write += haralickglcm.RangeF13 + ",";
                        to_write += haralickglcm.RangeF14 + ",";

                        to_write += "\n";
                        haralickglcm = null;
                    }

                    StreamWriter sw = new StreamWriter(fbd.SelectedPath + "\\" + g + "g" + d + "d" + "_glcm.csv");
                    note += "(" + (i+1) + ") " + fbd.SelectedPath + "\\" + g + "g" + d + "d" + "_glcm.csv\n";
                    sw.Write(to_write);
                    sw.Close();
                    Console.WriteLine("Finished!");
                }
                MessageBox.Show(note);
            }
        }
    }
}
