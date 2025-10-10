using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpinDisplayCMGT
{
    public class Protocol
    {
        // Token: 0x04000442 RID: 1090
        public static readonly byte Head = 104;

        // Token: 0x04000443 RID: 1091
        public static readonly byte UdpBroadcast = 0;

        // Token: 0x04000444 RID: 1092
        public static readonly byte UdpConnect = 1;

        // Token: 0x04000445 RID: 1093
        public static readonly byte UdpPasswordCheck = 2;

        // Token: 0x04000446 RID: 1094
        public static readonly byte UdpPasswordChange = 5;

        // Token: 0x04000447 RID: 1095
        public static readonly byte OtherControl = 6;

        // Token: 0x04000448 RID: 1096
        public static readonly byte LanChange = 7;

        // Token: 0x04000449 RID: 1097
        public static readonly byte Connect = 16;

        // Token: 0x0400044A RID: 1098
        public static readonly byte QueryVersion = 17;

        // Token: 0x0400044B RID: 1099
        public static readonly byte QueryParameter = 18;

        // Token: 0x0400044C RID: 1100
        public static readonly byte Brightness = 19;

        // Token: 0x0400044D RID: 1101
        public static readonly byte Heart = 20;

        // Token: 0x0400044E RID: 1102
        public static readonly byte FirmwareUpdate = 21;

        // Token: 0x0400044F RID: 1103
        public static readonly byte DeviceName = 22;

        // Token: 0x04000450 RID: 1104
        public static readonly byte OnOff = 23;

        // Token: 0x04000451 RID: 1105
        public static readonly byte Angle = 24;

        // Token: 0x04000452 RID: 1106
        public static readonly byte Reset = 25;

        // Token: 0x04000453 RID: 1107
        public static readonly byte SwitchParamter = 32;

        // Token: 0x04000454 RID: 1108
        public static readonly byte ClockSet = 33;

        // Token: 0x04000455 RID: 1109
        public static readonly byte MatchCode = 34;

        // Token: 0x04000456 RID: 1110
        public static readonly byte BleVolume = 35;

        // Token: 0x04000457 RID: 1111
        public static readonly byte DisplayMode = 36;

        // Token: 0x04000458 RID: 1112
        public static readonly byte LampTest = 225;

        // Token: 0x04000459 RID: 1113
        public static readonly byte Channel = 27;

        // Token: 0x0400045A RID: 1114
        public static readonly byte FileSend = 48;

        // Token: 0x0400045B RID: 1115
        public static readonly byte PlayList = 49;

        // Token: 0x0400045C RID: 1116
        public static readonly byte DeleteFile = 50;

        // Token: 0x0400045D RID: 1117
        public static readonly byte MoveFile = 51;

        // Token: 0x0400045E RID: 1118
        public static readonly byte ChangeFile = 52;

        // Token: 0x0400045F RID: 1119
        public static readonly byte PlayControl = 53;

        // Token: 0x04000460 RID: 1120
        public static readonly byte FileHeaderSend = 54;

        // Token: 0x04000461 RID: 1121
        public static readonly byte BinSend = 56;

        // Token: 0x04000462 RID: 1122
        public static readonly byte GetBinParam = 57;

        // Token: 0x04000463 RID: 1123
        public static readonly byte NoticeAudio = 58;

        // Token: 0x04000464 RID: 1124
        public static readonly byte Transconding = 59;

        // Token: 0x04000465 RID: 1125
        public static readonly byte GetSpeed = 224;

        // Token: 0x04000466 RID: 1126
        public static readonly byte Projection = 55;

        // Token: 0x04000467 RID: 1127
        public static readonly byte ProHeader = 250;

        // Token: 0x04000468 RID: 1128
        public static readonly byte SetCoord = 80;

        // Token: 0x04000469 RID: 1129
        public static readonly byte Group = 81;

        // Token: 0x0400046A RID: 1130
        public static readonly byte JointModel = 82;

        // Token: 0x0400046B RID: 1131
        public static readonly byte SelectDevice = 83;
    }
}
