using System.ComponentModel;

namespace MandelbrotGenerator
{
    [DefaultProperty("MinReal")]
    public class Settings
    {
        public static Settings defaultSettings = new();
        public static Settings DefaultSettings
        {
            get { return defaultSettings; }
        }

        #region Initial Area
        [Category("Initial Area"),
         Description("Minimum real value of the area")]
        public double MinReal { get; set; }
        [Category("Initial Area"),
         Description("Minimum imaginary value of the area")]
        public double MinImg { get; set; }
        [Category("Initial Area"),
         Description("Maximum real value of the area")]
        public double MaxReal { get; set; }
        [Category("Initial Area"),
         Description("Maximum imaginary value of the area")]
        public double MaxImg { get; set; }
        #endregion

        #region Generator Settings
        private int maxIterations;
        [Category("Generator Settings"),
         Description("Maximum number of iterations")]
        public int MaxIterations
        {
            get { return maxIterations; }
            set { if (value > 0) maxIterations = value; }
        }
        private double zBorder;
        [Category("Generator Settings"),
         Description("Border value for z")]
        public double ZBorder
        {
            get { return zBorder; }
            set { if (value > 0) zBorder = value; }
        }
        #endregion

        #region Parallelization Settings
        private int workers;
        [Category("Parallelization Settings"),
         Description("Number of worker threads")]
        public int Workers
        {
            get { return workers; }
            set { if (value > 0) workers = value; }
        }
        #endregion

        public Settings()
        {
            MinReal = -2; MinImg = -1;
            MaxReal = 1; MaxImg = 1;
            maxIterations = 10000;
            zBorder = 4.0;
            workers = 1;
        }
    }
}