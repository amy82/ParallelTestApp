using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;


namespace ParallelTestApp
{
    public class Globalo
    {
        public enum eMessageName : int
        {
            M_INFO = 0, M_ASK, M_WARNING, M_ERROR, M_VIEW, M_OP_CALL, M_MATERIAL_ID_FAIL, M_TERMINAL_MSG, M_NULL
        };
        public enum BOARDTYPE
        {
            BOARD_TYPE_LAON = 0,
            BOARD_TYPE_POWER,
            BOARD_TYPE_DAQ,
            BOARD_TYPE_LAON1000OP,
            BOARD_TYPE_V5FP
        }

        public static Form1 MainForm;

        public static ThreadControl threadControl;
        public static MotionControl.MotionManager motionManager;


        public const int TabLineY = 56;
        public const int MAX_PATH = 256;
        public const int CHART_ROI_COUNT = 9;
        public const int MTF_ROI_COUNT = 20;
        public const int BASE_THREAD_INTERVAL = 10;
        public static Color GridHeaderBackColor = Color.MediumAquamarine;// LightYellow;//OldLace;//WhiteSmoke; 

        public static int TerminalMessageDialog = 0;
        //Color.WhiteSmoke
        public static class ButtonColor
        {
            public static readonly string BTN_ON = "#FFB230";
            public static readonly string BTN_PAUSE_ON = "#FF0000";
            public static readonly string BTN_OFF = "#C3A279";


            public static readonly string MANUAL_BTN_ON = "#4C4743";
            public static readonly string MANUAL_BTN_OFF = "#C3A279";
            public static readonly Color BTN_START_ON = Color.BlueViolet;
            public static readonly Color BTN_START_OFF = Color.LimeGreen;
        }
    }
}
