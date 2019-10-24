namespace Borderless.Api
{
    public static class WinEvents
    {
        public static uint WINEVENT_OUTOFCONTEXT = 0x0000; // Events are ASYNC

        public static uint WINEVENT_SKIPOWNTHREAD = 0x0001; // Don't call back for events on installer's thread

        public static uint WINEVENT_SKIPOWNPROCESS = 0x0002; // Don't call back for events on installer's process

        public static uint WINEVENT_INCONTEXT = 0x0004; // Events are SYNC, this causes your dll to be injected into every process

        public static uint EVENT_MIN = 0x00000001;

        public static uint EVENT_MAX = 0x7FFFFFFF;

        public static uint EVENT_SYSTEM_SOUND = 0x0001;

        public static uint EVENT_SYSTEM_ALERT = 0x0002;

        public static uint EVENT_SYSTEM_FOREGROUND = 0x0003;

        public static uint EVENT_SYSTEM_MENUSTART = 0x0004;

        public static uint EVENT_SYSTEM_MENUEND = 0x0005;

        public static uint EVENT_SYSTEM_MENUPOPUPSTART = 0x0006;

        public static uint EVENT_SYSTEM_MENUPOPUPEND = 0x0007;

        public static uint EVENT_SYSTEM_CAPTURESTART = 0x0008;

        public static uint EVENT_SYSTEM_CAPTUREEND = 0x0009;

        public static uint EVENT_SYSTEM_MOVESIZESTART = 0x000A;

        public static uint EVENT_SYSTEM_MOVESIZEEND = 0x000B;

        public static uint EVENT_SYSTEM_CONTEXTHELPSTART = 0x000C;

        public static uint EVENT_SYSTEM_CONTEXTHELPEND = 0x000D;

        public static uint EVENT_SYSTEM_DRAGDROPSTART = 0x000E;

        public static uint EVENT_SYSTEM_DRAGDROPEND = 0x000F;

        public static uint EVENT_SYSTEM_DIALOGSTART = 0x0010;

        public static uint EVENT_SYSTEM_DIALOGEND = 0x0011;

        public static uint EVENT_SYSTEM_SCROLLINGSTART = 0x0012;

        public static uint EVENT_SYSTEM_SCROLLINGEND = 0x0013;

        public static uint EVENT_SYSTEM_SWITCHSTART = 0x0014;

        public static uint EVENT_SYSTEM_SWITCHEND = 0x0015;

        public static uint EVENT_SYSTEM_MINIMIZESTART = 0x0016;

        public static uint EVENT_SYSTEM_MINIMIZEEND = 0x0017;

        public static uint EVENT_SYSTEM_DESKTOPSWITCH = 0x0020;

        public static uint EVENT_SYSTEM_END = 0x00FF;

        public static uint EVENT_OEM_DEFINED_START = 0x0101;

        public static uint EVENT_OEM_DEFINED_END = 0x01FF;

        public static uint EVENT_UIA_EVENTID_START = 0x4E00;

        public static uint EVENT_UIA_EVENTID_END = 0x4EFF;

        public static uint EVENT_UIA_PROPID_START = 0x7500;

        public static uint EVENT_UIA_PROPID_END = 0x75FF;

        public static uint EVENT_CONSOLE_CARET = 0x4001;

        public static uint EVENT_CONSOLE_UPDATE_REGION = 0x4002;

        public static uint EVENT_CONSOLE_UPDATE_SIMPLE = 0x4003;

        public static uint EVENT_CONSOLE_UPDATE_SCROLL = 0x4004;

        public static uint EVENT_CONSOLE_LAYOUT = 0x4005;

        public static uint EVENT_CONSOLE_START_APPLICATION = 0x4006;

        public static uint EVENT_CONSOLE_END_APPLICATION = 0x4007;

        public static uint EVENT_CONSOLE_END = 0x40FF;

        public static uint EVENT_OBJECT_CREATE = 0x8000; // hwnd ID idChild is created item

        public static uint EVENT_OBJECT_DESTROY = 0x8001; // hwnd ID idChild is destroyed item

        public static uint EVENT_OBJECT_SHOW = 0x8002; // hwnd ID idChild is shown item

        public static uint EVENT_OBJECT_HIDE = 0x8003; // hwnd ID idChild is hidden item

        public static uint EVENT_OBJECT_REORDER = 0x8004; // hwnd ID idChild is parent of zordering children

        public static uint EVENT_OBJECT_FOCUS = 0x8005; // hwnd ID idChild is focused item

        public static uint
            EVENT_OBJECT_SELECTION =
                0x8006; // hwnd ID idChild is selected item (if only one), or idChild is OBJID_WINDOW if complex

        public static uint EVENT_OBJECT_SELECTIONADD = 0x8007; // hwnd ID idChild is item added

        public static uint EVENT_OBJECT_SELECTIONREMOVE = 0x8008; // hwnd ID idChild is item removed

        public static uint EVENT_OBJECT_SELECTIONWITHIN = 0x8009; // hwnd ID idChild is parent of changed selected items

        public static uint EVENT_OBJECT_STATECHANGE = 0x800A; // hwnd ID idChild is item w/ state change

        public static uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B; // hwnd ID idChild is moved/sized item

        public static uint EVENT_OBJECT_NAMECHANGE = 0x800C; // hwnd ID idChild is item w/ name change

        public static uint EVENT_OBJECT_DESCRIPTIONCHANGE = 0x800D; // hwnd ID idChild is item w/ desc change

        public static uint EVENT_OBJECT_VALUECHANGE = 0x800E; // hwnd ID idChild is item w/ value change

        public static uint EVENT_OBJECT_PARENTCHANGE = 0x800F; // hwnd ID idChild is item w/ new parent

        public static uint EVENT_OBJECT_HELPCHANGE = 0x8010; // hwnd ID idChild is item w/ help change

        public static uint EVENT_OBJECT_DEFACTIONCHANGE = 0x8011; // hwnd ID idChild is item w/ def action change

        public static uint EVENT_OBJECT_ACCELERATORCHANGE = 0x8012; // hwnd ID idChild is item w/ keybd accel change

        public static uint EVENT_OBJECT_INVOKED = 0x8013; // hwnd ID idChild is item invoked

        public static uint EVENT_OBJECT_TEXTSELECTIONCHANGED = 0x8014; // hwnd ID idChild is item w? test selection change

        public static uint EVENT_OBJECT_CONTENTSCROLLED = 0x8015;

        public static uint EVENT_SYSTEM_ARRANGMENTPREVIEW = 0x8016;

        public static uint EVENT_OBJECT_END = 0x80FF;

        public static uint EVENT_AIA_START = 0xA000;

        public static uint EVENT_AIA_END = 0xAFFF;
    }
}