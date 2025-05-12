using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelTestApp.MotionControl
{
    
    public static class MotorSet
    {

        public const int MAX_MOTOR_COUNT = 3;//6;       //<-이 수로 아진 모터 세팅

        //
        public static int MOTOR_MOVE_TIMEOUT = 20000;   //20s
        public static int LIFT_MOVE_TIMEOUT = 10000;   //10s
        public static int MOTOR_MANUAL_MOVE_TIMEOUT = 10000;   //10s
        public static int IO_TIMEOUT = 3000;   //3s
        public static double ENCORDER_GAP = 0.03;
        public static bool MOTOR_ACC_TYPE_SEC = true;


        public static readonly eTransferMotorList[] ValidTransferMotors =
        {
            eTransferMotorList.TRANSFER_X,
            eTransferMotorList.TRANSFER_Y,
            eTransferMotorList.TRANSFER_Z
        };
        public enum eTransferMotorList : int        
        {
            TRANSFER_X = 0, TRANSFER_Y = 1, TRANSFER_Z = 2, TOTAL_TRANSFER_MOTOR_COUNT
        };
        public static readonly eAoisocketMotorList[] ValidAoiSocketMotors =
        {
            eAoisocketMotorList.AOI_LEFT_X,
            eAoisocketMotorList.AOI_LEFT_Z,
            eAoisocketMotorList.AOI_RIGHT_X,
            eAoisocketMotorList.AOI_RIGHT_Z
        };
        public enum eAoisocketMotorList : int
        {
            AOI_LEFT_X = 7, AOI_LEFT_Z = 8, AOI_RIGHT_X = 9, AOI_RIGHT_Z = 10, TOTAL_AOI_SOCKET_MOTOR_COUNT
        };

        public static readonly eEEpromSocketMotorList[] ValidEEpromSocketMotors =
        {
            eEEpromSocketMotorList.EEPROM_FRONT_X,
            eEEpromSocketMotorList.EEPROM_BACK_X
        };
        public enum eEEpromSocketMotorList : int
        {
            EEPROM_FRONT_X = 7, EEPROM_BACK_X = 8, TOTAL_EEPROM_SOCKET_MOTOR_COUNT
        };
        public static readonly eLiftMotorList[] ValidLiftMotors =
        {
            eLiftMotorList.FRONT_X,
            eLiftMotorList.BACK_X,
            eLiftMotorList.LEFT_Z,
            eLiftMotorList.RIGHT_Z
        };
        public enum eLiftMotorList : int
        {
            FRONT_X = 3, BACK_X = 4, LEFT_Z = 5, RIGHT_Z = 6, TOTAL_LIFT_MOTOR_COUNT
        };


        public static readonly eMagazineMotorList[] ValidMagazineMotors =
        {
            eMagazineMotorList.L_MAGAZINE_Y,
            eMagazineMotorList.L_MAGAZINE_Z,
            eMagazineMotorList.R_MAGAZINE_Y,
            eMagazineMotorList.R_MAGAZINE_Z
        };

        public enum eMagazineMotorList : int
        {
            L_MAGAZINE_Y = 3, L_MAGAZINE_Z = 4, R_MAGAZINE_Y = 5, R_MAGAZINE_Z = 6, TOTAL_MAGAZINE_MOTOR_COUNT
        };

        
        //
        //EEPROM == LIFT Z 2개, GANTRY X 2개, SOCKET X축 2개 = TOTAL : 6개
        //aoi    == LIFT Z 2개, GANTRY X 2개 ,CAM X축 2개, CAM Z축 2개 = TOTAL : 8개
        //fw     == 매거진 Z 2개, Y축 2개 = TOTAL : 4개
        public enum TrayPosition        //LIFT , MAGAZINE 왼쪽, 오른족 구분
        {
            Left,
            Right,
            RightDown
        }

        public enum eJogDic : int
        {
            PLUS_MOVE = 1, MINUS_MOVE = -1
        };

        public static string[] TEACH_SET_MENU = { "원점상태", "ServoOn", "Alarm", "Limit(+)", "HOME", "Limit(-)", "속도(mm/s)", "가속도(sec)", "감속도(sec)" };


        public static double MaxLimitAccDec = 3.0;
        public static double MinLimitAccDec = 0.1;

        public static double MaxLimitSpeed = 1000.0;
        public static double MinLimitspeed = 10.0;
    }
}
