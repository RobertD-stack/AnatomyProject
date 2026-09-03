/*************************************************************************
 *
 * Copyright 2026, Mechdyne Corporation
 * ALL RIGHTS RESERVED
 *
 * UNPUBLISHED -- Rights reserved under the copyright laws of the United
 * States. Use of a copyright notice is precautionary only and does not
 * imply publication or disclosure.
 *
 * THE CONTENT OF THIS WORK CONTAINS CONFIDENTIAL AND PROPRIETARY
 * INFORMATION OF MECHDYNE CORPORATION. ANY DUPLICATION, MODIFICATION,
 * DISTRIBUTION, OR DISCLOSURE IN ANY FORM, IN WHOLE, OR IN PART, IS
 * STRICTLY PROHIBITED WITHOUT THE PRIOR EXPRESS WRITTEN PERMISSION OF
 * MECHDYNE CORPORATION.
 *
 * Version 4.5.0.4237
 *
 ************************************************************************/

using System.ComponentModel;

namespace getReal3D
{
    /// <summary>
    /// Virtual key codes
    /// </summary>
    public enum VirtualKeyCode
    {
        /// <summary>No key</summary>
        [Description("No key")]
        None = 0,

        /// <summary>Left mouse button</summary>
        [Description("Left mouse button")]
        VK_LBUTTON = 0x01,

        /// <summary>Right mouse button</summary>
        [Description("Right mouse button")]
        VK_RBUTTON = 0x02,

        /// <summary>Control-break processing</summary>
        [Description("Control-break processing")]
        VK_CANCEL = 0x03,

        /// <summary>Middle mouse button (three-button mouse)</summary>
        [Description("Middle mouse button (three-button mouse)")]
        VK_MBUTTON = 0x04,

        /// <summary>X1 mouse button</summary>
        [Description("X1 mouse button")]
        VK_XBUTTON1 = 0x05,

        /// <summary>X2 mouse button</summary>
        [Description("X2 mouse button")]
        VK_XBUTTON2 = 0x06,

        /// <summary>BACKSPACE key</summary>
        [Description("BACKSPACE key")]
        VK_BACK = 0x08,

        /// <summary>TAB key</summary>
        [Description("TAB key")]
        VK_TAB = 0x09,

        /// <summary>CLEAR key</summary>
        [Description("CLEAR key")]
        VK_CLEAR = 0x0C,

        /// <summary>ENTER key</summary>
        [Description("ENTER key")]
        VK_RETURN = 0x0D,

        /// <summary>SHIFT key</summary>
        [Description("SHIFT key")]
        VK_SHIFT = 0x10,

        /// <summary>CTRL key</summary>
        [Description("CTRL key")]
        VK_CONTROL = 0x11,

        /// <summary>ALT key</summary>
        [Description("ALT key")]
        VK_MENU = 0x12,

        /// <summary>PAUSE key</summary>
        [Description("PAUSE key")]
        VK_PAUSE = 0x13,

        /// <summary>CAPS LOCK key</summary>
        [Description("CAPS LOCK key")]
        VK_CAPITAL = 0x14,

        /// <summary>IME Kana mode</summary>
        [Description("IME Kana mode")]
        VK_KANA = 0x15,

        /// <summary>IME Hanguel mode (maintained for compatibility; use VK_HANGUL)</summary>
        [Description("IME Hanguel mode (maintained for compatibility; use VK_HANGUL)")]
        VK_HANGUEL = 0x15,

        /// <summary>IME Hangul mode</summary>
        [Description("IME Hangul mode")]
        VK_HANGUL = 0x15,

        /// <summary>IME Junja mode</summary>
        [Description("IME Junja mode")]
        VK_JUNJA = 0x17,

        /// <summary>IME final mode</summary>
        [Description("IME final mode")]
        VK_FINAL = 0x18,

        /// <summary>IME Hanja mode</summary>
        [Description("IME Hanja mode")]
        VK_HANJA = 0x19,

        /// <summary>IME Kanji mode</summary>
        [Description("IME Kanji mode")]
        VK_KANJI = 0x19,

        /// <summary>ESC key</summary>
        [Description("ESC key")]
        VK_ESCAPE = 0x1B,

        /// <summary>IME convert</summary>
        [Description("IME convert")]
        VK_CONVERT = 0x1C,

        /// <summary>IME nonconvert</summary>
        [Description("IME nonconvert")]
        VK_NONCONVERT = 0x1D,

        /// <summary>IME accept</summary>
        [Description("IME accept")]
        VK_ACCEPT = 0x1E,

        /// <summary>IME mode change request</summary>
        [Description("IME mode change request")]
        VK_MODECHANGE = 0x1F,

        /// <summary>SPACEBAR</summary>
        [Description("SPACEBAR")]
        VK_SPACE = 0x20,

        /// <summary>PAGE UP key</summary>
        [Description("PAGE UP key")]
        VK_PRIOR = 0x21,

        /// <summary>PAGE DOWN key</summary>
        [Description("PAGE DOWN key")]
        VK_NEXT = 0x22,

        /// <summary>END key</summary>
        [Description("END key")]
        VK_END = 0x23,

        /// <summary>HOME key</summary>
        [Description("HOME key")]
        VK_HOME = 0x24,

        /// <summary>LEFT ARROW key</summary>
        [Description("LEFT ARROW key")]
        VK_LEFT = 0x25,

        /// <summary>UP ARROW key</summary>
        [Description("UP ARROW key")]
        VK_UP = 0x26,

        /// <summary>RIGHT ARROW key</summary>
        [Description("RIGHT ARROW key")]
        VK_RIGHT = 0x27,

        /// <summary>DOWN ARROW key</summary>
        [Description("DOWN ARROW key")]
        VK_DOWN = 0x28,

        /// <summary>SELECT key</summary>
        [Description("SELECT key")]
        VK_SELECT = 0x29,

        /// <summary>PRINT key</summary>
        [Description("PRINT key")]
        VK_PRINT = 0x2A,

        /// <summary>EXECUTE key</summary>
        [Description("EXECUTE key")]
        VK_EXECUTE = 0x2B,

        /// <summary>PRINT SCREEN key</summary>
        [Description("PRINT SCREEN key")]
        VK_SNAPSHOT = 0x2C,

        /// <summary>INS key</summary>
        [Description("INS key")]
        VK_INSERT = 0x2D,

        /// <summary>DEL key</summary>
        [Description("DEL key")]
        VK_DELETE = 0x2E,

        /// <summary>HELP key</summary>
        [Description("HELP key")]
        VK_HELP = 0x2F,

        /// <summary>0 key</summary>
        [Description("0 key")]
        VK_0 = 0x30,

        /// <summary>1 key</summary>
        [Description("1 key")]
        VK_1 = 0x31,

        /// <summary>2 key</summary>
        [Description("2 key")]
        VK_2 = 0x32,

        /// <summary>3 key</summary>
        [Description("3 key")]
        VK_3 = 0x33,

        /// <summary>4 key</summary>
        [Description("4 key")]
        VK_4 = 0x34,

        /// <summary>5 key</summary>
        [Description("5 key")]
        VK_5 = 0x35,

        /// <summary>6 key</summary>
        [Description("6 key")]
        VK_6 = 0x36,

        /// <summary>7 key</summary>
        [Description("7 key")]
        VK_7 = 0x37,

        /// <summary>8 key</summary>
        [Description("8 key")]
        VK_8 = 0x38,

        /// <summary>9 key</summary>
        [Description("9 key")]
        VK_9 = 0x39,

        /// <summary>A key</summary>
        [Description("A key")]
        VK_A = 0x41,

        /// <summary>B key</summary>
        [Description("B key")]
        VK_B = 0x42,

        /// <summary>C key</summary>
        [Description("C key")]
        VK_C = 0x43,

        /// <summary>D key</summary>
        [Description("D key")]
        VK_D = 0x44,

        /// <summary>E key</summary>
        [Description("E key")]
        VK_E = 0x45,

        /// <summary>F key</summary>
        [Description("F key")]
        VK_F = 0x46,

        /// <summary>G key</summary>
        [Description("G key")]
        VK_G = 0x47,

        /// <summary>H key</summary>
        [Description("H key")]
        VK_H = 0x48,

        /// <summary>I key</summary>
        [Description("I key")]
        VK_I = 0x49,

        /// <summary>J key</summary>
        [Description("J key")]
        VK_J = 0x4A,

        /// <summary>K key</summary>
        [Description("K key")]
        VK_K = 0x4B,

        /// <summary>L key</summary>
        [Description("L key")]
        VK_L = 0x4C,

        /// <summary>M key</summary>
        [Description("M key")]
        VK_M = 0x4D,

        /// <summary>N key</summary>
        [Description("N key")]
        VK_N = 0x4E,

        /// <summary>O key</summary>
        [Description("O key")]
        VK_O = 0x4F,

        /// <summary>P key</summary>
        [Description("P key")]
        VK_P = 0x50,

        /// <summary>Q key</summary>
        [Description("Q key")]
        VK_Q = 0x51,

        /// <summary>R key</summary>
        [Description("R key")]
        VK_R = 0x52,

        /// <summary>S key</summary>
        [Description("S key")]
        VK_S = 0x53,

        /// <summary>T key</summary>
        [Description("T key")]
        VK_T = 0x54,

        /// <summary>U key</summary>
        [Description("U key")]
        VK_U = 0x55,

        /// <summary>V key</summary>
        [Description("V key")]
        VK_V = 0x56,

        /// <summary>W key</summary>
        [Description("W key")]
        VK_W = 0x57,

        /// <summary>X key</summary>
        [Description("X key")]
        VK_X = 0x58,

        /// <summary>Y key</summary>
        [Description("Y key")]
        VK_Y = 0x59,

        /// <summary>Z key</summary>
        [Description("Z key")]
        VK_Z = 0x5A,

        /// <summary>Left Windows key (Natural keyboard)</summary>
        [Description("Left Windows key (Natural keyboard)")]
        VK_LWIN = 0x5B,

        /// <summary>Right Windows key (Natural keyboard)</summary>
        [Description("Right Windows key (Natural keyboard)")]
        VK_RWIN = 0x5C,

        /// <summary>Applications key (Natural keyboard)</summary>
        [Description("Applications key (Natural keyboard)")]
        VK_APPS = 0x5D,

        /// <summary>Computer Sleep key</summary>
        [Description("Computer Sleep key")]
        VK_SLEEP = 0x5F,

        /// <summary>Numeric keypad 0 key</summary>
        [Description("Numeric keypad 0 key")]
        VK_NUMPAD0 = 0x60,

        /// <summary>Numeric keypad 1 key</summary>
        [Description("Numeric keypad 1 key")]
        VK_NUMPAD1 = 0x61,

        /// <summary>Numeric keypad 2 key</summary>
        [Description("Numeric keypad 2 key")]
        VK_NUMPAD2 = 0x62,

        /// <summary>Numeric keypad 3 key</summary>
        [Description("Numeric keypad 3 key")]
        VK_NUMPAD3 = 0x63,

        /// <summary>Numeric keypad 4 key</summary>
        [Description("Numeric keypad 4 key")]
        VK_NUMPAD4 = 0x64,

        /// <summary>Numeric keypad 5 key</summary>
        [Description("Numeric keypad 5 key")]
        VK_NUMPAD5 = 0x65,

        /// <summary>Numeric keypad 6 key</summary>
        [Description("Numeric keypad 6 key")]
        VK_NUMPAD6 = 0x66,

        /// <summary>Numeric keypad 7 key</summary>
        [Description("Numeric keypad 7 key")]
        VK_NUMPAD7 = 0x67,

        /// <summary>Numeric keypad 8 key</summary>
        [Description("Numeric keypad 8 key")]
        VK_NUMPAD8 = 0x68,

        /// <summary>Numeric keypad 9 key</summary>
        [Description("Numeric keypad 9 key")]
        VK_NUMPAD9 = 0x69,

        /// <summary>Multiply key</summary>
        [Description("Multiply key")]
        VK_MULTIPLY = 0x6A,

        /// <summary>Add key</summary>
        [Description("Add key")]
        VK_ADD = 0x6B,

        /// <summary>Separator key</summary>
        [Description("Separator key")]
        VK_SEPARATOR = 0x6C,

        /// <summary>Subtract key</summary>
        [Description("Subtract key")]
        VK_SUBTRACT = 0x6D,

        /// <summary>Decimal key</summary>
        [Description("Decimal key")]
        VK_DECIMAL = 0x6E,

        /// <summary>Divide key</summary>
        [Description("Divide key")]
        VK_DIVIDE = 0x6F,

        /// <summary>F1 key</summary>
        [Description("F1 key")]
        VK_F1 = 0x70,

        /// <summary>F2 key</summary>
        [Description("F2 key")]
        VK_F2 = 0x71,

        /// <summary>F3 key</summary>
        [Description("F3 key")]
        VK_F3 = 0x72,

        /// <summary>F4 key</summary>
        [Description("F4 key")]
        VK_F4 = 0x73,

        /// <summary>F5 key</summary>
        [Description("F5 key")]
        VK_F5 = 0x74,

        /// <summary>F6 key</summary>
        [Description("F6 key")]
        VK_F6 = 0x75,

        /// <summary>F7 key</summary>
        [Description("F7 key")]
        VK_F7 = 0x76,

        /// <summary>F8 key</summary>
        [Description("F8 key")]
        VK_F8 = 0x77,

        /// <summary>F9 key</summary>
        [Description("F9 key")]
        VK_F9 = 0x78,

        /// <summary>F10 key</summary>
        [Description("F10 key")]
        VK_F10 = 0x79,

        /// <summary>F11 key</summary>
        [Description("F11 key")]
        VK_F11 = 0x7A,

        /// <summary>F12 key</summary>
        [Description("F12 key")]
        VK_F12 = 0x7B,

        /// <summary>F13 key</summary>
        [Description("F13 key")]
        VK_F13 = 0x7C,

        /// <summary>F14 key</summary>
        [Description("F14 key")]
        VK_F14 = 0x7D,

        /// <summary>F15 key</summary>
        [Description("F15 key")]
        VK_F15 = 0x7E,

        /// <summary>F16 key</summary>
        [Description("F16 key")]
        VK_F16 = 0x7F,

        /// <summary>F17 key</summary>
        [Description("F17 key")]
        VK_F17 = 0x80,

        /// <summary>F18 key</summary>
        [Description("F18 key")]
        VK_F18 = 0x81,

        /// <summary>F19 key</summary>
        [Description("F19 key")]
        VK_F19 = 0x82,

        /// <summary>F20 key</summary>
        [Description("F20 key")]
        VK_F20 = 0x83,

        /// <summary>F21 key</summary>
        [Description("F21 key")]
        VK_F21 = 0x84,

        /// <summary>F22 key</summary>
        [Description("F22 key")]
        VK_F22 = 0x85,

        /// <summary>F23 key</summary>
        [Description("F23 key")]
        VK_F23 = 0x86,

        /// <summary>F24 key</summary>
        [Description("F24 key")]
        VK_F24 = 0x87,

        /// <summary>NUM LOCK key</summary>
        [Description("NUM LOCK key")]
        VK_NUMLOCK = 0x90,

        /// <summary>SCROLL LOCK key</summary>
        [Description("SCROLL LOCK key")]
        VK_SCROLL = 0x91,

        /// <summary>Left SHIFT key</summary>
        [Description("Left SHIFT key")]
        VK_LSHIFT = 0xA0,

        /// <summary>Right SHIFT key</summary>
        [Description("Right SHIFT key")]
        VK_RSHIFT = 0xA1,

        /// <summary>Left CONTROL key</summary>
        [Description("Left CONTROL key")]
        VK_LCONTROL = 0xA2,

        /// <summary>Right CONTROL key</summary>
        [Description("Right CONTROL key")]
        VK_RCONTROL = 0xA3,

        /// <summary>Left MENU key</summary>
        [Description("Left MENU key")]
        VK_LMENU = 0xA4,

        /// <summary>Right MENU key</summary>
        [Description("Right MENU key")]
        VK_RMENU = 0xA5,

        /// <summary>Browser Back key</summary>
        [Description("Browser Back key")]
        VK_BROWSER_BACK = 0xA6,

        /// <summary>Browser Forward key</summary>
        [Description("Browser Forward key")]
        VK_BROWSER_FORWARD = 0xA7,

        /// <summary>Browser Refresh key</summary>
        [Description("Browser Refresh key")]
        VK_BROWSER_REFRESH = 0xA8,

        /// <summary>Browser Stop key</summary>
        [Description("Browser Stop key")]
        VK_BROWSER_STOP = 0xA9,

        /// <summary>Browser Search key</summary>
        [Description("Browser Search key")]
        VK_BROWSER_SEARCH = 0xAA,

        /// <summary>Browser Favorites key</summary>
        [Description("Browser Favorites key")]
        VK_BROWSER_FAVORITES = 0xAB,

        /// <summary>Browser Start and Home key</summary>
        [Description("Browser Start and Home key")]
        VK_BROWSER_HOME = 0xAC,

        /// <summary>Volume Mute key</summary>
        [Description("Volume Mute key")]
        VK_VOLUME_MUTE = 0xAD,

        /// <summary>Volume Down key</summary>
        [Description("Volume Down key")]
        VK_VOLUME_DOWN = 0xAE,

        /// <summary>Volume Up key</summary>
        [Description("Volume Up key")]
        VK_VOLUME_UP = 0xAF,

        /// <summary>Next Track key</summary>
        [Description("Next Track key")]
        VK_MEDIA_NEXT_TRACK = 0xB0,

        /// <summary>Previous Track key</summary>
        [Description("Previous Track key")]
        VK_MEDIA_PREV_TRACK = 0xB1,

        /// <summary>Stop Media key</summary>
        [Description("Stop Media key")]
        VK_MEDIA_STOP = 0xB2,

        /// <summary>Play/Pause Media key</summary>
        [Description("Play/Pause Media key")]
        VK_MEDIA_PLAY_PAUSE = 0xB3,

        /// <summary>Start Mail key</summary>
        [Description("Start Mail key")]
        VK_LAUNCH_MAIL = 0xB4,

        /// <summary>Select Media key</summary>
        [Description("Select Media key")]
        VK_LAUNCH_MEDIA_SELECT = 0xB5,

        /// <summary>Start Application 1 key</summary>
        [Description("Start Application 1 key")]
        VK_LAUNCH_APP1 = 0xB6,

        /// <summary>Start Application 2 key</summary>
        [Description("Start Application 2 key")]
        VK_LAUNCH_APP2 = 0xB7,

        /// <summary>Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ';:' key</summary>
        [Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ';:' key")]
        VK_OEM_1 = 0xBA,

        /// <summary>For any country/region, the '+' key</summary>
        [Description("For any country/region, the '+' key")]
        VK_OEM_PLUS = 0xBB,

        /// <summary>For any country/region, the ',' key</summary>
        [Description("For any country/region, the ',' key")]
        VK_OEM_COMMA = 0xBC,

        /// <summary>For any country/region, the '-' key</summary>
        [Description("For any country/region, the '-' key")]
        VK_OEM_MINUS = 0xBD,

        /// <summary>For any country/region, the '.' key</summary>
        [Description("For any country/region, the '.' key")]
        VK_OEM_PERIOD = 0xBE,

        /// <summary>Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '/?' key</summary>
        [Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '/?' key")]
        VK_OEM_2 = 0xBF,

        /// <summary>Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '`~' key</summary>
        [Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '`~' key")]
        VK_OEM_3 = 0xC0,

        /// <summary>Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '[{' key</summary>
        [Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '[{' key")]
        VK_OEM_4 = 0xDB,

        /// <summary>Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '\\|' key</summary>
        [Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '\\|' key")]
        VK_OEM_5 = 0xDC,

        /// <summary>Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ']}' key</summary>
        [Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ']}' key")]
        VK_OEM_6 = 0xDD,

        /// <summary>Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the 'single-quote/double-quote' key</summary>
        [Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the 'single-quote/double-quote' key")]
        VK_OEM_7 = 0xDE,

        /// <summary>Used for miscellaneous characters; it can vary by keyboard.</summary>
        [Description("Used for miscellaneous characters; it can vary by keyboard.")]
        VK_OEM_8 = 0xDF,


        /// <summary>Either the angle bracket key or the backslash key on the RT 102-key keyboard</summary>
        [Description("Either the angle bracket key or the backslash key on the RT 102-key keyboard")]
        VK_OEM_102 = 0xE2,

        /// <summary>IME PROCESS key</summary>
        [Description("IME PROCESS key")]
        VK_PROCESSKEY = 0xE5,


        /// <summary>Used to pass Unicode characters as if they were keystrokes. The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP</summary>
        [Description("Used to pass Unicode characters as if they were keystrokes. The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP")]
        VK_PACKET = 0xE7,

        /// <summary>Attn key</summary>
        [Description("Attn key")]
        VK_ATTN = 0xF6,

        /// <summary>CrSel key</summary>
        [Description("CrSel key")]
        VK_CRSEL = 0xF7,

        /// <summary>ExSel key</summary>
        [Description("ExSel key")]
        VK_EXSEL = 0xF8,

        /// <summary>Erase EOF key</summary>
        [Description("Erase EOF key")]
        VK_EREOF = 0xF9,

        /// <summary>Play key</summary>
        [Description("Play key")]
        VK_PLAY = 0xFA,

        /// <summary>Zoom key</summary>
        [Description("Zoom key")]
        VK_ZOOM = 0xFB,

        /// <summary>PA1 key</summary>
        [Description("PA1 key")]
        VK_PA1 = 0xFD,

        /// <summary>Clear key</summary>
        [Description("Clear key")]
        VK_OEM_CLEAR = 0xFE,

        /// <summary>Number of keys</summary>
        [Description("Number of keys")]
        Count
    };

    /// <summary>
    /// Joystick button codes
    /// </summary>
    public enum ButtonCode
    {

        /// <summary>JOYSTICK_DPAD_UP</summary>
        JOYSTICK_DPAD_UP,
        /// <summary>JOYSTICK_DPAD_DOWN</summary>
        JOYSTICK_DPAD_DOWN,
        /// <summary>JOYSTICK_DPAD_LEFT</summary>
        JOYSTICK_DPAD_LEFT,
        /// <summary>JOYSTICK_DPAD_RIGHT</summary>
        JOYSTICK_DPAD_RIGHT,
        /// <summary>JOYSTICK_START</summary>
        JOYSTICK_START,
        /// <summary>JOYSTICK_BACK</summary>
        JOYSTICK_BACK,
        /// <summary>JOYSTICK_LEFT_THUMB</summary>
        JOYSTICK_LEFT_THUMB,
        /// <summary>JOYSTICK_RIGHT_THUMB</summary>
        JOYSTICK_RIGHT_THUMB,
        /// <summary>JOYSTICK_LEFT_SHOULDER</summary>
        JOYSTICK_LEFT_SHOULDER,
        /// <summary>JOYSTICK_RIGHT_SHOULDER</summary>
        JOYSTICK_RIGHT_SHOULDER,
        /// <summary>JOYSTICK_UNUSED_1</summary>
        JOYSTICK_UNUSED_1,
        /// <summary>JOYSTICK_UNUSED_2</summary>
        JOYSTICK_UNUSED_2,
        /// <summary>JOYSTICK_A</summary>
        JOYSTICK_A,
        /// <summary>JOYSTICK_B</summary>
        JOYSTICK_B,
        /// <summary>JOYSTICK_X</summary>
        JOYSTICK_X,
        /// <summary>JOYSTICK_Y</summary>
        JOYSTICK_Y,
        /// <summary>JOYSTICK_B1</summary>
        JOYSTICK_B1 = JOYSTICK_DPAD_UP,
        /// <summary>JOYSTICK_B2</summary>
        JOYSTICK_B2,
        /// <summary>JOYSTICK_B3</summary>
        JOYSTICK_B3,
        /// <summary>JOYSTICK_B4</summary>
        JOYSTICK_B4,
        /// <summary>JOYSTICK_B5</summary>
        JOYSTICK_B5,
        /// <summary>JOYSTICK_B6</summary>
        JOYSTICK_B6,
        /// <summary>JOYSTICK_B7</summary>
        JOYSTICK_B7,
        /// <summary>JOYSTICK_B8</summary>
        JOYSTICK_B8,
        /// <summary>JOYSTICK_B9</summary>
        JOYSTICK_B9,
        /// <summary>JOYSTICK_B10</summary>
        JOYSTICK_B10,
        /// <summary>JOYSTICK_B11</summary>
        JOYSTICK_B11,
        /// <summary>JOYSTICK_B12</summary>
        JOYSTICK_B12,
        /// <summary>JOYSTICK_B13</summary>
        JOYSTICK_B13,
        /// <summary>JOYSTICK_B14</summary>
        JOYSTICK_B14,
        /// <summary>JOYSTICK_B15</summary>
        JOYSTICK_B15,
        /// <summary>JOYSTICK_B16</summary>
        JOYSTICK_B16,
        /// <summary>JOYSTICK_B17</summary>
        JOYSTICK_B17,
        /// <summary>JOYSTICK_B18</summary>
        JOYSTICK_B18,
        /// <summary>JOYSTICK_B19</summary>
        JOYSTICK_B19,
        /// <summary>JOYSTICK_B20</summary>
        JOYSTICK_B20,
        /// <summary>JOYSTICK_B21</summary>
        JOYSTICK_B21,
        /// <summary>JOYSTICK_B22</summary>
        JOYSTICK_B22,
        /// <summary>JOYSTICK_B23</summary>
        JOYSTICK_B23,
        /// <summary>JOYSTICK_B24</summary>
        JOYSTICK_B24,
        /// <summary>JOYSTICK_B25</summary>
        JOYSTICK_B25,
        /// <summary>JOYSTICK_B26</summary>
        JOYSTICK_B26,
        /// <summary>JOYSTICK_B27</summary>
        JOYSTICK_B27,
        /// <summary>JOYSTICK_B28</summary>
        JOYSTICK_B28,
        /// <summary>JOYSTICK_B29</summary>
        JOYSTICK_B29,
        /// <summary>JOYSTICK_B30</summary>
        JOYSTICK_B30,
        /// <summary>JOYSTICK_B31</summary>
        JOYSTICK_B31,
        /// <summary>JOYSTICK_B32</summary>
        JOYSTICK_B32,
        /// <summary>JOYSTICK1_DPAD_UP</summary>
        JOYSTICK1_DPAD_UP,
        /// <summary>JOYSTICK1_DPAD_DOWN</summary>
        JOYSTICK1_DPAD_DOWN,
        /// <summary>JOYSTICK1_DPAD_LEFT</summary>
        JOYSTICK1_DPAD_LEFT,
        /// <summary>JOYSTICK1_DPAD_RIGHT</summary>
        JOYSTICK1_DPAD_RIGHT,
        /// <summary>JOYSTICK1_START</summary>
        JOYSTICK1_START,
        /// <summary>JOYSTICK1_BACK</summary>
        JOYSTICK1_BACK,
        /// <summary>JOYSTICK1_LEFT_THUMB</summary>
        JOYSTICK1_LEFT_THUMB,
        /// <summary>JOYSTICK1_RIGHT_THUMB</summary>
        JOYSTICK1_RIGHT_THUMB,
        /// <summary>JOYSTICK1_LEFT_SHOULDER</summary>
        JOYSTICK1_LEFT_SHOULDER,
        /// <summary>JOYSTICK1_RIGHT_SHOULDER</summary>
        JOYSTICK1_RIGHT_SHOULDER,
        /// <summary>JOYSTICK1_UNUSED_1</summary>
        JOYSTICK1_UNUSED_1,
        /// <summary>JOYSTICK1_UNUSED_2</summary>
        JOYSTICK1_UNUSED_2,
        /// <summary>JOYSTICK1_A</summary>
        JOYSTICK1_A,
        /// <summary>JOYSTICK1_B</summary>
        JOYSTICK1_B,
        /// <summary>JOYSTICK1_X</summary>
        JOYSTICK1_X,
        /// <summary>JOYSTICK1_Y</summary>
        JOYSTICK1_Y,
        /// <summary>JOYSTICK1_B1</summary>
        JOYSTICK1_B1 = JOYSTICK1_DPAD_UP,
        /// <summary>JOYSTICK1_B2</summary>
        JOYSTICK1_B2,
        /// <summary>JOYSTICK1_B3</summary>
        JOYSTICK1_B3,
        /// <summary>JOYSTICK1_B4</summary>
        JOYSTICK1_B4,
        /// <summary>JOYSTICK1_B5</summary>
        JOYSTICK1_B5,
        /// <summary>JOYSTICK1_B6</summary>
        JOYSTICK1_B6,
        /// <summary>JOYSTICK1_B7</summary>
        JOYSTICK1_B7,
        /// <summary>JOYSTICK1_B8</summary>
        JOYSTICK1_B8,
        /// <summary>JOYSTICK1_B9</summary>
        JOYSTICK1_B9,
        /// <summary>JOYSTICK1_B10</summary>
        JOYSTICK1_B10,
        /// <summary>JOYSTICK1_B11</summary>
        JOYSTICK1_B11,
        /// <summary>JOYSTICK1_B12</summary>
        JOYSTICK1_B12,
        /// <summary>JOYSTICK1_B13</summary>
        JOYSTICK1_B13,
        /// <summary>JOYSTICK1_B14</summary>
        JOYSTICK1_B14,
        /// <summary>JOYSTICK1_B15</summary>
        JOYSTICK1_B15,
        /// <summary>JOYSTICK1_B16</summary>
        JOYSTICK1_B16,
        /// <summary>JOYSTICK1_B17</summary>
        JOYSTICK1_B17,
        /// <summary>JOYSTICK1_B18</summary>
        JOYSTICK1_B18,
        /// <summary>JOYSTICK1_B19</summary>
        JOYSTICK1_B19,
        /// <summary>JOYSTICK1_B20</summary>
        JOYSTICK1_B20,
        /// <summary>JOYSTICK1_B21</summary>
        JOYSTICK1_B21,
        /// <summary>JOYSTICK1_B22</summary>
        JOYSTICK1_B22,
        /// <summary>JOYSTICK1_B23</summary>
        JOYSTICK1_B23,
        /// <summary>JOYSTICK1_B24</summary>
        JOYSTICK1_B24,
        /// <summary>JOYSTICK1_B25</summary>
        JOYSTICK1_B25,
        /// <summary>JOYSTICK1_B26</summary>
        JOYSTICK1_B26,
        /// <summary>JOYSTICK1_B27</summary>
        JOYSTICK1_B27,
        /// <summary>JOYSTICK1_B28</summary>
        JOYSTICK1_B28,
        /// <summary>JOYSTICK1_B29</summary>
        JOYSTICK1_B29,
        /// <summary>JOYSTICK1_B30</summary>
        JOYSTICK1_B30,
        /// <summary>JOYSTICK1_B31</summary>
        JOYSTICK1_B31,
        /// <summary>JOYSTICK1_B32</summary>
        JOYSTICK1_B32,
        /// <summary>JOYSTICK2_DPAD_UP</summary>
        JOYSTICK2_DPAD_UP,
        /// <summary>JOYSTICK2_DPAD_DOWN</summary>
        JOYSTICK2_DPAD_DOWN,
        /// <summary>JOYSTICK2_DPAD_LEFT</summary>
        JOYSTICK2_DPAD_LEFT,
        /// <summary>JOYSTICK2_DPAD_RIGHT</summary>
        JOYSTICK2_DPAD_RIGHT,
        /// <summary>JOYSTICK2_START</summary>
        JOYSTICK2_START,
        /// <summary>JOYSTICK2_BACK</summary>
        JOYSTICK2_BACK,
        /// <summary>JOYSTICK2_LEFT_THUMB</summary>
        JOYSTICK2_LEFT_THUMB,
        /// <summary>JOYSTICK2_RIGHT_THUMB</summary>
        JOYSTICK2_RIGHT_THUMB,
        /// <summary>JOYSTICK2_LEFT_SHOULDER</summary>
        JOYSTICK2_LEFT_SHOULDER,
        /// <summary>JOYSTICK2_RIGHT_SHOULDER</summary>
        JOYSTICK2_RIGHT_SHOULDER,
        /// <summary>JOYSTICK2_UNUSED_1</summary>
        JOYSTICK2_UNUSED_1,
        /// <summary>JOYSTICK2_UNUSED_2</summary>
        JOYSTICK2_UNUSED_2,
        /// <summary>JOYSTICK2_A</summary>
        JOYSTICK2_A,
        /// <summary>JOYSTICK2_B</summary>
        JOYSTICK2_B,
        /// <summary>JOYSTICK2_X</summary>
        JOYSTICK2_X,
        /// <summary>JOYSTICK2_Y</summary>
        JOYSTICK2_Y,
        /// <summary>JOYSTICK2_B1</summary>
        JOYSTICK2_B1 = JOYSTICK2_DPAD_UP,
        /// <summary>JOYSTICK2_B2</summary>
        JOYSTICK2_B2,
        /// <summary>JOYSTICK2_B3</summary>
        JOYSTICK2_B3,
        /// <summary>JOYSTICK2_B4</summary>
        JOYSTICK2_B4,
        /// <summary>JOYSTICK2_B5</summary>
        JOYSTICK2_B5,
        /// <summary>JOYSTICK2_B6</summary>
        JOYSTICK2_B6,
        /// <summary>JOYSTICK2_B7</summary>
        JOYSTICK2_B7,
        /// <summary>JOYSTICK2_B8</summary>
        JOYSTICK2_B8,
        /// <summary>JOYSTICK2_B9</summary>
        JOYSTICK2_B9,
        /// <summary>JOYSTICK2_B10</summary>
        JOYSTICK2_B10,
        /// <summary>JOYSTICK2_B11</summary>
        JOYSTICK2_B11,
        /// <summary>JOYSTICK2_B12</summary>
        JOYSTICK2_B12,
        /// <summary>JOYSTICK2_B13</summary>
        JOYSTICK2_B13,
        /// <summary>JOYSTICK2_B14</summary>
        JOYSTICK2_B14,
        /// <summary>JOYSTICK2_B15</summary>
        JOYSTICK2_B15,
        /// <summary>JOYSTICK2_B16</summary>
        JOYSTICK2_B16,
        /// <summary>JOYSTICK2_B17</summary>
        JOYSTICK2_B17,
        /// <summary>JOYSTICK2_B18</summary>
        JOYSTICK2_B18,
        /// <summary>JOYSTICK2_B19</summary>
        JOYSTICK2_B19,
        /// <summary>JOYSTICK2_B20</summary>
        JOYSTICK2_B20,
        /// <summary>JOYSTICK2_B21</summary>
        JOYSTICK2_B21,
        /// <summary>JOYSTICK2_B22</summary>
        JOYSTICK2_B22,
        /// <summary>JOYSTICK2_B23</summary>
        JOYSTICK2_B23,
        /// <summary>JOYSTICK2_B24</summary>
        JOYSTICK2_B24,
        /// <summary>JOYSTICK2_B25</summary>
        JOYSTICK2_B25,
        /// <summary>JOYSTICK2_B26</summary>
        JOYSTICK2_B26,
        /// <summary>JOYSTICK2_B27</summary>
        JOYSTICK2_B27,
        /// <summary>JOYSTICK2_B28</summary>
        JOYSTICK2_B28,
        /// <summary>JOYSTICK2_B29</summary>
        JOYSTICK2_B29,
        /// <summary>JOYSTICK2_B30</summary>
        JOYSTICK2_B30,
        /// <summary>JOYSTICK2_B31</summary>
        JOYSTICK2_B31,
        /// <summary>JOYSTICK2_B32</summary>
        JOYSTICK2_B32,
        /// <summary>JOYSTICK3_DPAD_UP</summary>
        JOYSTICK3_DPAD_UP,
        /// <summary>JOYSTICK3_DPAD_DOWN</summary>
        JOYSTICK3_DPAD_DOWN,
        /// <summary>JOYSTICK3_DPAD_LEFT</summary>
        JOYSTICK3_DPAD_LEFT,
        /// <summary>JOYSTICK3_DPAD_RIGHT</summary>
        JOYSTICK3_DPAD_RIGHT,
        /// <summary>JOYSTICK3_START</summary>
        JOYSTICK3_START,
        /// <summary>JOYSTICK3_BACK</summary>
        JOYSTICK3_BACK,
        /// <summary>JOYSTICK3_LEFT_THUMB</summary>
        JOYSTICK3_LEFT_THUMB,
        /// <summary>JOYSTICK3_RIGHT_THUMB</summary>
        JOYSTICK3_RIGHT_THUMB,
        /// <summary>JOYSTICK3_LEFT_SHOULDER</summary>
        JOYSTICK3_LEFT_SHOULDER,
        /// <summary>JOYSTICK3_RIGHT_SHOULDER</summary>
        JOYSTICK3_RIGHT_SHOULDER,
        /// <summary>JOYSTICK3_UNUSED_1</summary>
        JOYSTICK3_UNUSED_1,
        /// <summary>JOYSTICK3_UNUSED_2</summary>
        JOYSTICK3_UNUSED_2,
        /// <summary>JOYSTICK3_A</summary>
        JOYSTICK3_A,
        /// <summary>JOYSTICK3_B</summary>
        JOYSTICK3_B,
        /// <summary>JOYSTICK3_X</summary>
        JOYSTICK3_X,
        /// <summary>JOYSTICK3_Y</summary>
        JOYSTICK3_Y,
        /// <summary>JOYSTICK3_B1</summary>
        JOYSTICK3_B1 = JOYSTICK3_DPAD_UP,
        /// <summary>JOYSTICK3_B2</summary>
        JOYSTICK3_B2,
        /// <summary>JOYSTICK3_B3</summary>
        JOYSTICK3_B3,
        /// <summary>JOYSTICK3_B4</summary>
        JOYSTICK3_B4,
        /// <summary>JOYSTICK3_B5</summary>
        JOYSTICK3_B5,
        /// <summary>JOYSTICK3_B6</summary>
        JOYSTICK3_B6,
        /// <summary>JOYSTICK3_B7</summary>
        JOYSTICK3_B7,
        /// <summary>JOYSTICK3_B8</summary>
        JOYSTICK3_B8,
        /// <summary>JOYSTICK3_B9</summary>
        JOYSTICK3_B9,
        /// <summary>JOYSTICK3_B10</summary>
        JOYSTICK3_B10,
        /// <summary>JOYSTICK3_B11</summary>
        JOYSTICK3_B11,
        /// <summary>JOYSTICK3_B12</summary>
        JOYSTICK3_B12,
        /// <summary>JOYSTICK3_B13</summary>
        JOYSTICK3_B13,
        /// <summary>JOYSTICK3_B14</summary>
        JOYSTICK3_B14,
        /// <summary>JOYSTICK3_B15</summary>
        JOYSTICK3_B15,
        /// <summary>JOYSTICK3_B16</summary>
        JOYSTICK3_B16,
        /// <summary>JOYSTICK3_B17</summary>
        JOYSTICK3_B17,
        /// <summary>JOYSTICK3_B18</summary>
        JOYSTICK3_B18,
        /// <summary>JOYSTICK3_B19</summary>
        JOYSTICK3_B19,
        /// <summary>JOYSTICK3_B20</summary>
        JOYSTICK3_B20,
        /// <summary>JOYSTICK3_B21</summary>
        JOYSTICK3_B21,
        /// <summary>JOYSTICK3_B22</summary>
        JOYSTICK3_B22,
        /// <summary>JOYSTICK3_B23</summary>
        JOYSTICK3_B23,
        /// <summary>JOYSTICK3_B24</summary>
        JOYSTICK3_B24,
        /// <summary>JOYSTICK3_B25</summary>
        JOYSTICK3_B25,
        /// <summary>JOYSTICK3_B26</summary>
        JOYSTICK3_B26,
        /// <summary>JOYSTICK3_B27</summary>
        JOYSTICK3_B27,
        /// <summary>JOYSTICK3_B28</summary>
        JOYSTICK3_B28,
        /// <summary>JOYSTICK3_B29</summary>
        JOYSTICK3_B29,
        /// <summary>JOYSTICK3_B30</summary>
        JOYSTICK3_B30,
        /// <summary>JOYSTICK3_B31</summary>
        JOYSTICK3_B31,
        /// <summary>JOYSTICK3_B32</summary>
        JOYSTICK3_B32,
        /// <summary>JOYSTICK4_DPAD_UP</summary>
        JOYSTICK4_DPAD_UP,
        /// <summary>JOYSTICK4_DPAD_DOWN</summary>
        JOYSTICK4_DPAD_DOWN,
        /// <summary>JOYSTICK4_DPAD_LEFT</summary>
        JOYSTICK4_DPAD_LEFT,
        /// <summary>JOYSTICK4_DPAD_RIGHT</summary>
        JOYSTICK4_DPAD_RIGHT,
        /// <summary>JOYSTICK4_START</summary>
        JOYSTICK4_START,
        /// <summary>JOYSTICK4_BACK</summary>
        JOYSTICK4_BACK,
        /// <summary>JOYSTICK4_LEFT_THUMB</summary>
        JOYSTICK4_LEFT_THUMB,
        /// <summary>JOYSTICK4_RIGHT_THUMB</summary>
        JOYSTICK4_RIGHT_THUMB,
        /// <summary>JOYSTICK4_LEFT_SHOULDER</summary>
        JOYSTICK4_LEFT_SHOULDER,
        /// <summary>JOYSTICK4_RIGHT_SHOULDER</summary>
        JOYSTICK4_RIGHT_SHOULDER,
        /// <summary>JOYSTICK4_UNUSED_1</summary>
        JOYSTICK4_UNUSED_1,
        /// <summary>JOYSTICK4_UNUSED_2</summary>
        JOYSTICK4_UNUSED_2,
        /// <summary>JOYSTICK4_A</summary>
        JOYSTICK4_A,
        /// <summary>JOYSTICK4_B</summary>
        JOYSTICK4_B,
        /// <summary>JOYSTICK4_X</summary>
        JOYSTICK4_X,
        /// <summary>JOYSTICK4_Y</summary>
        JOYSTICK4_Y,
        /// <summary>JOYSTICK4_B1</summary>
        JOYSTICK4_B1 = JOYSTICK4_DPAD_UP,
        /// <summary>JOYSTICK4_B2</summary>
        JOYSTICK4_B2,
        /// <summary>JOYSTICK4_B3</summary>
        JOYSTICK4_B3,
        /// <summary>JOYSTICK4_B4</summary>
        JOYSTICK4_B4,
        /// <summary>JOYSTICK4_B5</summary>
        JOYSTICK4_B5,
        /// <summary>JOYSTICK4_B6</summary>
        JOYSTICK4_B6,
        /// <summary>JOYSTICK4_B7</summary>
        JOYSTICK4_B7,
        /// <summary>JOYSTICK4_B8</summary>
        JOYSTICK4_B8,
        /// <summary>JOYSTICK4_B9</summary>
        JOYSTICK4_B9,
        /// <summary>JOYSTICK4_B10</summary>
        JOYSTICK4_B10,
        /// <summary>JOYSTICK4_B11</summary>
        JOYSTICK4_B11,
        /// <summary>JOYSTICK4_B12</summary>
        JOYSTICK4_B12,
        /// <summary>JOYSTICK4_B13</summary>
        JOYSTICK4_B13,
        /// <summary>JOYSTICK4_B14</summary>
        JOYSTICK4_B14,
        /// <summary>JOYSTICK4_B15</summary>
        JOYSTICK4_B15,
        /// <summary>JOYSTICK4_B16</summary>
        JOYSTICK4_B16,
        /// <summary>JOYSTICK4_B17</summary>
        JOYSTICK4_B17,
        /// <summary>JOYSTICK4_B18</summary>
        JOYSTICK4_B18,
        /// <summary>JOYSTICK4_B19</summary>
        JOYSTICK4_B19,
        /// <summary>JOYSTICK4_B20</summary>
        JOYSTICK4_B20,
        /// <summary>JOYSTICK4_B21</summary>
        JOYSTICK4_B21,
        /// <summary>JOYSTICK4_B22</summary>
        JOYSTICK4_B22,
        /// <summary>JOYSTICK4_B23</summary>
        JOYSTICK4_B23,
        /// <summary>JOYSTICK4_B24</summary>
        JOYSTICK4_B24,
        /// <summary>JOYSTICK4_B25</summary>
        JOYSTICK4_B25,
        /// <summary>JOYSTICK4_B26</summary>
        JOYSTICK4_B26,
        /// <summary>JOYSTICK4_B27</summary>
        JOYSTICK4_B27,
        /// <summary>JOYSTICK4_B28</summary>
        JOYSTICK4_B28,
        /// <summary>JOYSTICK4_B29</summary>
        JOYSTICK4_B29,
        /// <summary>JOYSTICK4_B30</summary>
        JOYSTICK4_B30,
        /// <summary>JOYSTICK4_B31</summary>
        JOYSTICK4_B31,
        /// <summary>JOYSTICK4_B32</summary>
        JOYSTICK4_B32,

        /// <summary>Number of buttons</summary>
        [Description("Number of buttons")]
        Count,

        /// <summary>No button</summary>
        None = Count
    }

    /// <summary>
    /// Axis codes
    /// </summary>
    public enum AxisCode
    {
        /// <summary>JOYSTICK_THUMB_LX</summary>
        JOYSTICK_THUMB_LX,
        /// <summary>JOYSTICK_THUMB_LY</summary>
        JOYSTICK_THUMB_LY,
        /// <summary>JOYSTICK_THUMB_RX</summary>
        JOYSTICK_THUMB_RX,
        /// <summary>JOYSTICK_THUMB_RY</summary>
        JOYSTICK_THUMB_RY,
        /// <summary>JOYSTICK_TRIGGER_L</summary>
        JOYSTICK_TRIGGER_L,
        /// <summary>JOYSTICK_TRIGGER_R</summary>
        JOYSTICK_TRIGGER_R,

        /// <summary>JOYSTICK1_THUMB_LX</summary>
        JOYSTICK1_THUMB_LX,
        /// <summary>JOYSTICK1_THUMB_LY</summary>
        JOYSTICK1_THUMB_LY,
        /// <summary>JOYSTICK1_THUMB_RX</summary>
        JOYSTICK1_THUMB_RX,
        /// <summary>JOYSTICK1_THUMB_RY</summary>
        JOYSTICK1_THUMB_RY,
        /// <summary>JOYSTICK1_TRIGGER_L</summary>
        JOYSTICK1_TRIGGER_L,
        /// <summary>JOYSTICK1_TRIGGER_R</summary>
        JOYSTICK1_TRIGGER_R,

        /// <summary>JOYSTICK2_THUMB_LX</summary>
        JOYSTICK2_THUMB_LX,
        /// <summary>JOYSTICK2_THUMB_LY</summary>
        JOYSTICK2_THUMB_LY,
        /// <summary>JOYSTICK2_THUMB_RX</summary>
        JOYSTICK2_THUMB_RX,
        /// <summary>JOYSTICK2_THUMB_RY</summary>
        JOYSTICK2_THUMB_RY,
        /// <summary>JOYSTICK2_TRIGGER_L</summary>
        JOYSTICK2_TRIGGER_L,
        /// <summary>JOYSTICK2_TRIGGER_R</summary>
        JOYSTICK2_TRIGGER_R,

        /// <summary>JOYSTICK3_THUMB_LX</summary>
        JOYSTICK3_THUMB_LX,
        /// <summary>JOYSTICK3_THUMB_LY</summary>
        JOYSTICK3_THUMB_LY,
        /// <summary>JOYSTICK3_THUMB_RX</summary>
        JOYSTICK3_THUMB_RX,
        /// <summary>JOYSTICK3_THUMB_RY</summary>
        JOYSTICK3_THUMB_RY,
        /// <summary>JOYSTICK3_TRIGGER_L</summary>
        JOYSTICK3_TRIGGER_L,
        /// <summary>JOYSTICK3_TRIGGER_R</summary>
        JOYSTICK3_TRIGGER_R,

        /// <summary>JOYSTICK4_THUMB_LX</summary>
        JOYSTICK4_THUMB_LX,
        /// <summary>JOYSTICK4_THUMB_LY</summary>
        JOYSTICK4_THUMB_LY,
        /// <summary>JOYSTICK4_THUMB_RX</summary>
        JOYSTICK4_THUMB_RX,
        /// <summary>JOYSTICK4_THUMB_RY</summary>
        JOYSTICK4_THUMB_RY,
        /// <summary>JOYSTICK4_TRIGGER_L</summary>
        JOYSTICK4_TRIGGER_L,
        /// <summary>JOYSTICK4_TRIGGER_R</summary>
        JOYSTICK4_TRIGGER_R,

        /// <summary>X mouse position</summary>
        MOUSE_X,

        /// <summary>Y mouse position</summary>
        MOUSE_Y,

        /// <summary>Mouse wheel</summary>
        MOUSE_WHEEL,

        /// <summary>Axis count</summary>
        Count,

        /// <summary>No axis</summary>
        None = Count
    };

}
