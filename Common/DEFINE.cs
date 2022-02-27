using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentFactory
{
    public static class DEFINE
    {
        public static int IMAGE_SIZE_FACTOR = 10;
        public static Color COLOR_LIME = Color.FromArgb(255, 136, 187, 0);

        public const int ALARM_CONNECTION                 = 0x1000;
        public const int ALARM_CONNECTION_UDP             = 0x0001;
        public const int ALARM_CONNECTION_CAMERA          = 0x0002;
        public const int ALARM_CONNECTION_LIGHTCONTROLLER = 0x0003;
        public const int ALARM_CONNECTION_VISION_LICENSE  = 0x0004;

        public const int CAM_ALIGN_LEFT                   = 0;
        public const int CAM_ALIGN_RIGHT                  = 1;
        public const int CAM_INSPECTION_LEFT              = 2;
        public const int CAM_INSPECTION_RIGHT             = 3;
        public const int CAM_MAX_COUNT                    = 2;

        public const string IMAGE                         = "Image";
        public const string SAVE_IMAGE                    = "SAVE_IMAGE";
        public const string CAPTURE                       = "CAPTURE";
        public const string CONFIG                        = "CONFIG";
        public const string RECIPE                        = "RECIPE";

        public const double PIXELPERMM                    = 0.023D;
        public enum PatternName
        {
            PACK_L,
            PACK_R,
            INSPECTION_L,
            INSPECTION_R
        }

        public enum TrainMode
        {
            Pattern,
            Finder
        }

        public enum Direction { LeftToRight, RightToLeft, ToptoBottom, BottomToTop }
    }
}
