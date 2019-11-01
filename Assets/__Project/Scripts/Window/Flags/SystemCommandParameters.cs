namespace Window.Flags
{
    public enum SystemCommandParameters : uint
    {
        /// <summary>
        /// In WM_SYSCOMMAND messages, the four low-order bits of the wParam parameter are used internally by the system.
        /// To obtain the correct result when testing the value of wParam,
        /// an application must combine the value 0xFFF0 with the wParam value by using the bitwise AND operator.
        /// </summary>
        WParamCombineValue = 0xFFF0,

        /// <summary>
        /// Indicates whether the screen saver is secure.
        /// </summary>
        ISSecure = 0x00000001,

        /// <summary>
        /// Sizes the window. 
        /// </summary>
        Size = 0xF000,

        /// <summary>
        /// Moves the window. 
        /// </summary>
        Move = 0xF010,


        /// <summary>
        /// Minimizes the window. 
        /// </summary>
        Minimize = 0xF020,

        /// <summary>
        /// Maximizes the window. 
        /// </summary>
        Maximize = 0xF030,

        /// <summary>
        /// Moves to the next window. 
        /// </summary>
        NextWindow = 0xF040,

        /// <summary>
        /// Moves to the previous window. 
        /// </summary>
        PreviousWindow = 0xF050,

        /// <summary>
        /// Closes the window.
        /// </summary>
        Close = 0xF060,

        /// <summary>
        /// Scrolls vertically. 
        /// </summary>
        VerticalScroll = 0xF070,

        /// <summary>
        /// Scrolls horizontally. 
        /// </summary>
        HorizontalScroll = 0xF080,

        /// <summary>
        /// Retrieves the window menu as a result of a mouse click.
        /// </summary>
        MouseMenu = 0xF090,

        /// <summary>
        /// Retrieves the window menu as a result of a keystroke. For more information, see the Remarks section. 
        /// </summary>
        KeyMenu = 0xF100,

        /// <summary>
        /// Restores the window to its normal position and size.
        /// </summary>
        Restore = 0xF120,

        /// <summary>
        /// Activates the Start menu. 
        /// </summary>
        TaskList = 0xF130,

        /// <summary>
        /// Executes the screen saver application specified in the [boot] section of the System.ini file. 
        /// </summary>
        ScreenSave =
            0xF140,

        /// <summary>
        /// Activates the window associated with the application-specified hot key. The lParam parameter identifies the window to activate.
        /// </summary>
        HotKey =
            0xF150,

        /// <summary>
        /// Selects the default item; the user double-clicked the window menu. 
        /// </summary>
        Default = 0xF160,

        /// <summary>
        /// Sets the state of the display. This command supports devices that have power-saving features, such as a battery-powered personal computer.
        /// The lParam parameter can have the following values:
        /// -1 (the display is powering on)
        /// 1 (the display is going to low power)
        /// 2 (the display is being shut off)
        /// </summary>
        ///
        MonitorPower = 0xF170,

        /// <summary>
        /// Changes the cursor to a question mark with a pointer. If the user then clicks a control in the dialog box, the control receives a WM_HELP message.
        /// </summary>
        ContextHelp = 0xF180,
    }
}