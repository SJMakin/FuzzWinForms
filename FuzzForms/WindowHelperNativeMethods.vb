Option Strict Off

Imports System.Runtime.InteropServices
Imports System.Text

Public Class WindowHelperNativeMethods 'Expose and exploit the user32 library
    Private Declare Function EnumWindows Lib "user32" (ByVal lpEnumFunc As CallBack, ByVal lParam As UIntPtr) As UInteger
    Private Declare Function GetForegroundWindow Lib "user32" () As IntPtr
    Private Declare Function GetAncestor Lib "user32" (ByVal hWnd As IntPtr, ByVal flags As Integer) As IntPtr
    Private Declare Function GetParent Lib "user32" (ByVal hwnd As IntPtr) As IntPtr
    Private Declare Function GetWindow Lib "user32" (ByVal hwnd As IntPtr, ByVal wCmd As UInt32) As IntPtr
    Private Declare Function GetWindowInteger Lib "user32" Alias "GetWindowLongA" (ByVal hwnd As IntPtr, ByVal nIndex As Integer) As Integer
    Private Declare Function GetWindowText Lib "user32" Alias "GetWindowTextA" (ByVal hwnd As IntPtr, ByVal lpString As String, ByVal cch As Integer) As Integer
    Private Declare Function IsWindowVisible Lib "user32" (ByVal hwnd As IntPtr) As Boolean
    Private Declare Function GetWindowRect Lib "user32.dll" (ByVal hwnd As IntPtr, ByRef lpRect As RECT) As Int32
    Private Declare Function ShowWindow Lib "user32" (ByVal hwnd As IntPtr, ByVal showCommand As Integer) As Boolean
    Private Declare Function SetForegroundWindow Lib "user32" (ByVal hWnd As IntPtr) As Boolean
    Private Declare Auto Function SetCursorPos Lib "User32.dll" (ByVal X As Integer, ByVal Y As Integer) As Long
    Private Declare Auto Function GetCursorPos Lib "User32.dll" (ByRef lpPoint As Point) As Long
    Private Declare Function mouse_event Lib "user32" (ByVal dwFlags As Int32, ByVal dX As Int32, ByVal dY As Int32, ByVal cButtons As Int32, ByVal dwExtraInfo As UIntPtr) As Boolean
    Private Declare Function SendInput Lib "user32" (ByVal nInputs As Integer, ByVal pInputs() As INPUT, ByVal cbSize As Integer) As Integer
    Private Declare Function GetWindowThreadProcessId Lib "user32" (ByVal hWnd As IntPtr, ByRef lpwdProcessId As Integer) As Integer
    Private Declare Function SetWindowPos Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal hWndInsertAfter As IntPtr, ByVal X As Integer, ByVal Y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal uFlags As SetWindowPosFlags) As Boolean
    Private Declare Function GetLastInputInfo Lib "user32.dll" (ByRef plii As LASTINPUTINFO) As Boolean

    Public Delegate Function CallBack(ByVal hwnd As IntPtr, ByVal lParam As Integer) As Boolean

#Region "Enums"

    <Flags()>
    Public Enum SetWindowPosFlags As UInteger
        SWP_ASYNCWINDOWPOS = &H4000
        SWP_DEFERERASE = &H2000
        SWP_DRAWFRAME = SWP_FRAMECHANGED
        SWP_FRAMECHANGED = &H20
        SWP_HIDEWINDOW = &H80
        SWP_NOACTIVATE = &H10
        SWP_NOCOPYBITS = &H100
        SWP_NOMOVE = &H2
        SWP_NOOWNERZORDER = &H200
        SWP_NOREDRAW = &H8
        SWP_NOREPOSITION = SWP_NOOWNERZORDER
        SWP_NOSENDCHANGING = &H400
        SWP_NOSIZE = &H1
        SWP_NOZORDER = &H4
        SWP_SHOWWINDOW = &H40
    End Enum

    'Note: Cast these values to IntPtr
    Public Enum SpecialWindowHandles
        HWND_TOP = 0
        HWND_BOTTOM = 1
        HWND_TOPMOST = -1
        HWND_NOTOPMOST = -2
    End Enum

    Public Enum VirtualKeys

        ' // UNASSIGNED // = &HFFFF0000    ' // [Modifiers] = -65536
        ' // UNASSIGNED // = &H0    ' // [None] = 000

        VK_LBUTTON = &H1        ' // [LButton] = 001
        VK_RBUTTON = &H2        ' // [RButton] = 002
        VK_CANCEL = &H3         ' // [Cancel] = 003
        VK_MBUTTON = &H4        ' // [MButton] = 004    ' NOT contiguous with L & RBUTTON
        VK_XBUTTON1 = &H5       ' // [XButton1] = 005   ' NOT contiguous with L & RBUTTON
        VK_XBUTTON2 = &H6       ' // [XButton2] = 006   ' NOT contiguous with L & RBUTTON
        ' // UNASSIGNED // = &H7    ' // ''UNASSIGNED = 007

        VK_BACK = &H8           ' // [Back] = 008
        VK_TAB = &H9        ' // [Tab] = 009
        ' // RESERVED // = &HA        ' // [LineFeed] = 010
        ' // RESERVED // = &HB        ' // ''UNASSIGNED = 011
        VK_CLEAR = &HC          ' // [Clear] = 012
        VK_RETURN = &HD         ' // [Return] = 013
        ' // UNDEFINED //       ' // [Enter] = 013
        VK_SHIFT = &H10         ' // [ShiftKey] = 016
        VK_CONTROL = &H11       ' // [ControlKey] = 017
        VK_MENU = &H12          ' // [Menu] = 018
        VK_PAUSE = &H13         ' // [Pause] = 019
        VK_CAPITAL = &H14       ' // [Capital] = 020
        ' // UNDEFINED //       ' // [CapsLock] = 020

        VK_HANGUL = &H15        ' // [HangulMode] = 021
        VK_HANGEUL = &H15       ' // [HanguelMode] = 021 ' old name (compatibility)
        VK_KANA = &H15          ' // [KanaMode] = 021
        VK_JUNJA = &H17         ' // [JunjaMode] = 023
        VK_FINAL = &H18         ' // [FinalMode] = 024
        VK_KANJI = &H19         ' // [KanjiMode] = 025
        VK_HANJA = &H19         ' // [HanjaMode] = 025

        VK_ESCAPE = &H1B        ' // [Escape] = 027

        VK_CONVERT = &H1C       ' // [IMEConvert] = 028
        VK_NONCONVERT = &H1D    ' // [IMENonconvert] = 029
        VK_ACCEPT = &H1E        ' // [IMEAccept] = 030
        VK_MODECHANGE = &H1F    ' // [IMEModeChange] = 031

        VK_SPACE = &H20         ' // [Space] = 032
        VK_PRIOR = &H21         ' // [Prior] = 033
        ' // UNDEFINED //       ' // [PageUp] = 033
        VK_NEXT = &H22          ' // [Next] = 034
        ' // UNDEFINED //       ' // [PageDown] = 034
        VK_END = &H23           ' // [End] = 035
        VK_HOME = &H24          ' // [Home] = 036

        VK_LEFT = &H25          ' // [Left] = 037
        VK_UP = &H26        ' // [Up] = 038
        VK_RIGHT = &H27         ' // [Right] = 039
        VK_DOWN = &H28          ' // [Down] = 040

        VK_SELECT = &H29        ' // [Select] = 041
        VK_PRINT = &H2A         ' // [Print] = 042
        VK_EXECUTE = &H2B       ' // [Execute] = 043
        VK_SNAPSHOT = &H2C      ' // [Snapshot] = 044
        ' // UNDEFINED //       ' // [PrintScreen] = 044
        VK_INSERT = &H2D        ' // [Insert] = 045
        VK_DELETE = &H2E        ' // [Delete] = 046
        VK_HELP = &H2F          ' // [Help] = 047

        VK_0 = &H30         ' // [D0] = 048
        VK_1 = &H31         ' // [D1] = 049
        VK_2 = &H32         ' // [D2] = 050
        VK_3 = &H33         ' // [D3] = 051
        VK_4 = &H34         ' // [D4] = 052
        VK_5 = &H35         ' // [D5] = 053
        VK_6 = &H36         ' // [D6] = 054
        VK_7 = &H37         ' // [D7] = 055
        VK_8 = &H38         ' // [D8] = 056
        VK_9 = &H39         ' // [D9] = 057

        ' // UNASSIGNED // = &H40 to &H4F (058 to 064)

        VK_A = &H41         ' // [A] = 065
        VK_B = &H42         ' // [B] = 066
        VK_C = &H43         ' // [C] = 067
        VK_D = &H44         ' // [D] = 068
        VK_E = &H45         ' // [E] = 069
        VK_F = &H46         ' // [F] = 070
        VK_G = &H47         ' // [G] = 071
        VK_H = &H48         ' // [H] = 072
        VK_I = &H49         ' // [I] = 073
        VK_J = &H4A         ' // [J] = 074
        VK_K = &H4B         ' // [K] = 075
        VK_L = &H4C         ' // [L] = 076
        VK_M = &H4D         ' // [M] = 077
        VK_N = &H4E         ' // [N] = 078
        VK_O = &H4F         ' // [O] = 079
        VK_P = &H50         ' // [P] = 080
        VK_Q = &H51         ' // [Q] = 081
        VK_R = &H52         ' // [R] = 082
        VK_S = &H53         ' // [S] = 083
        VK_T = &H54         ' // [T] = 084
        VK_U = &H55         ' // [U] = 085
        VK_V = &H56         ' // [V] = 086
        VK_W = &H57         ' // [W] = 087
        VK_X = &H58         ' // [X] = 088
        VK_Y = &H59         ' // [Y] = 089
        VK_Z = &H5A         ' // [Z] = 090

        VK_LWIN = &H5B          ' // [LWin] = 091
        VK_RWIN = &H5C          ' // [RWin] = 092
        VK_APPS = &H5D          ' // [Apps] = 093
        ' // RESERVED // = &H5E        ' // ''UNASSIGNED = 094
        VK_SLEEP = &H5F         ' // [Sleep] = 095

        VK_NUMPAD0 = &H60       ' // [NumPad0] = 096
        VK_NUMPAD1 = &H61       ' // [NumPad1] = 097
        VK_NUMPAD2 = &H62       ' // [NumPad2] = 098
        VK_NUMPAD3 = &H63       ' // [NumPad3] = 099
        VK_NUMPAD4 = &H64       ' // [NumPad4] = 100
        VK_NUMPAD5 = &H65       ' // [NumPad5] = 101
        VK_NUMPAD6 = &H66       ' // [NumPad6] = 102
        VK_NUMPAD7 = &H67       ' // [NumPad7] = 103
        VK_NUMPAD8 = &H68       ' // [NumPad8] = 104
        VK_NUMPAD9 = &H69       ' // [NumPad9] = 105

        VK_MULTIPLY = &H6A      ' // [Multiply] = 106
        VK_ADD = &H6B           ' // [Add] = 107
        VK_SEPARATOR = &H6C     ' // [Separator] = 108
        VK_SUBTRACT = &H6D      ' // [Subtract] = 109
        VK_DECIMAL = &H6E       ' // [Decimal] = 110
        VK_DIVIDE = &H6F        ' // [Divide] = 111

        VK_F1 = &H70        ' // [F1] = 112
        VK_F2 = &H71        ' // [F2] = 113
        VK_F3 = &H72        ' // [F3] = 114
        VK_F4 = &H73        ' // [F4] = 115
        VK_F5 = &H74        ' // [F5] = 116
        VK_F6 = &H75        ' // [F6] = 117
        VK_F7 = &H76        ' // [F7] = 118
        VK_F8 = &H77        ' // [F8] = 119
        VK_F9 = &H78        ' // [F9] = 120
        VK_F10 = &H79           ' // [F10] = 121
        VK_F11 = &H7A           ' // [F11] = 122
        VK_F12 = &H7B           ' // [F12] = 123

        VK_F13 = &H7C           ' // [F13] = 124
        VK_F14 = &H7D           ' // [F14] = 125
        VK_F15 = &H7E           ' // [F15] = 126
        VK_F16 = &H7F           ' // [F16] = 127
        VK_F17 = &H80           ' // [F17] = 128
        VK_F18 = &H81           ' // [F18] = 129
        VK_F19 = &H82           ' // [F19] = 130
        VK_F20 = &H83           ' // [F20] = 131
        VK_F21 = &H84           ' // [F21] = 132
        VK_F22 = &H85           ' // [F22] = 133
        VK_F23 = &H86           ' // [F23] = 134
        VK_F24 = &H87           ' // [F24] = 135

        ' // UNASSIGNED // = &H88 to &H8F (136 to 143)

        VK_NUMLOCK = &H90       ' // [NumLock] = 144
        VK_SCROLL = &H91        ' // [Scroll] = 145

        VK_OEM_NEC_EQUAL = &H92     ' // [NEC_Equal] = 146    ' NEC PC-9800 kbd definitions "=" key on numpad
        VK_OEM_FJ_JISHO = &H92      ' // [Fujitsu_Masshou] = 146    ' Fujitsu/OASYS kbd definitions "Dictionary" key
        VK_OEM_FJ_MASSHOU = &H93    ' // [Fujitsu_Masshou] = 147    ' Fujitsu/OASYS kbd definitions "Unregister word" key
        VK_OEM_FJ_TOUROKU = &H94    ' // [Fujitsu_Touroku] = 148    ' Fujitsu/OASYS kbd definitions "Register word" key
        VK_OEM_FJ_LOYA = &H95       ' // [Fujitsu_Loya] = 149    ' Fujitsu/OASYS kbd definitions "Left OYAYUBI" key
        VK_OEM_FJ_ROYA = &H96       ' // [Fujitsu_Roya] = 150    ' Fujitsu/OASYS kbd definitions "Right OYAYUBI" key

        ' // UNASSIGNED // = &H97 to &H9F (151 to 159)

        ' NOTE :: &HA0 to &HA5 (160 to 165) = left and right Alt, Ctrl and Shift virtual keys.
        ' NOTE :: Used only as parameters to GetAsyncKeyState() and GetKeyState().
        ' NOTE :: No other API or message will distinguish left and right keys in this way.
        VK_LSHIFT = &HA0        ' // [LShiftKey] = 160
        VK_RSHIFT = &HA1        ' // [RShiftKey] = 161
        VK_LCONTROL = &HA2      ' // [LControlKey] = 162
        VK_RCONTROL = &HA3      ' // [RControlKey] = 163
        VK_LMENU = &HA4         ' // [LMenu] = 164
        VK_RMENU = &HA5         ' // [RMenu] = 165

        VK_BROWSER_BACK = &HA6      ' // [BrowserBack] = 166
        VK_BROWSER_FORWARD = &HA7   ' // [BrowserForward] = 167
        VK_BROWSER_REFRESH = &HA8   ' // [BrowserRefresh] = 168
        VK_BROWSER_STOP = &HA9      ' // [BrowserStop] = 169
        VK_BROWSER_SEARCH = &HAA    ' // [BrowserSearch] = 170
        VK_BROWSER_FAVORITES = &HAB ' // [BrowserFavorites] = 171
        VK_BROWSER_HOME = &HAC      ' // [BrowserHome] = 172

        VK_VOLUME_MUTE = &HAD       ' // [VolumeMute] = 173
        VK_VOLUME_DOWN = &HAE       ' // [VolumeDown] = 174
        VK_VOLUME_UP = &HAF     ' // [VolumeUp] = 175

        VK_MEDIA_NEXT_TRACK = &HB0  ' // [MediaNextTrack] = 176
        VK_MEDIA_PREV_TRACK = &HB1  ' // [MediaPreviousTrack] = 177
        VK_MEDIA_STOP = &HB2    ' // [MediaStop] = 178
        VK_MEDIA_PLAY_PAUSE = &HB3  ' // [MediaPlayPause] = 179

        VK_LAUNCH_MAIL = &HB4       ' // [LaunchMail] = 180
        VK_LAUNCH_MEDIA_SELECT = &HB5 ' // [SelectMedia] = 181
        VK_LAUNCH_APP1 = &HB6       ' // [LaunchApplication1] = 182
        VK_LAUNCH_APP2 = &HB7       ' // [LaunchApplication2] = 183
        ' // UNASSIGNED // = &HB8   ' // ''UNASSIGNED = 184
        ' // UNASSIGNED // = &HB9   ' // ''UNASSIGNED = 185

        VK_OEM_1 = &HBA         ' // [Oem1] = 186           ' ";:" for USA
        ' // UNDEFINED //       ' // [OemSemicolon] = 186       ' ";:" for USA
        VK_OEM_PLUS = &HBB      ' // [Oemplus] = 187        ' "+" any country
        VK_OEM_COMMA = &HBC     ' // [Oemcomma] = 188       ' "," any country
        VK_OEM_MINUS = &HBD     ' // [OemMinus] = 189       ' "-" any country
        VK_OEM_PERIOD = &HBE    ' // [OemPeriod] = 190      ' "." any country
        VK_OEM_2 = &HBF         ' // [Oem2] = 191           ' "/?" for USA
        ' // UNDEFINED //       ' // [OemQuestion] = 191    ' "/?" for USA
        ' // UNDEFINED //       ' // [Oemtilde] = 192       ' "'~" for USA
        VK_OEM_3 = &HC0         ' // [Oem3] = 192           ' "'~" for USA

        ' // RESERVED // = &HC1 to &HD7 (193 to 215)
        ' // UNASSIGNED // = &HD8 to &HDA (216 to 218)

        VK_OEM_4 = &HDB         ' // [Oem4] = 219           ' "[{" for USA
        ' // UNDEFINED //       ' // [OemOpenBrackets] = 219    ' "[{" for USA
        ' // UNDEFINED //       ' // [OemPipe] = 220        ' "\|" for USA
        VK_OEM_5 = &HDC         ' // [Oem5] = 220           ' "\|" for USA
        VK_OEM_6 = &HDD         ' // [Oem6] = 221           ' "]}" for USA
        ' // UNDEFINED //       ' // [OemCloseBrackets] = 221   ' "]}" for USA
        ' // UNDEFINED //       ' // [OemQuotes] = 222      ' "'"" for USA
        VK_OEM_7 = &HDE         ' // [Oem7] = 222           ' "'"" for USA
        VK_OEM_8 = &HDF         ' // [Oem8] = 223

        ' // RESERVED // = &HE0        ' // ''UNASSIGNED = 224
        VK_OEM_AX = &HE1        ' // [OEMAX] = 225          ' "AX" key on Japanese AX kbd
        ' // UNDEFINED //       ' // [OemBackslash] = 226       ' "<>" or "\|" on RT 102-key kbd
        VK_OEM_102 = &HE2       ' // [Oem102] = 226         ' "<>" or "\|" on RT 102-key kbd
        VK_ICO_HELP = &HE3      ' // [ICOHelp] = 227        ' Help key on ICO
        VK_ICO_00 = &HE4        ' // [ICO00] = 228          ' 00 key on ICO

        VK_PROCESSKEY = &HE5    ' // [ProcessKey] = 229
        VK_ICO_CLEAR = &HE6     ' // [ICOClear] = 230
        VK_PACKET = &HE7        ' // [Packet] = 231
        ' // UNASSIGNED // = &HE8   ' // ''UNASSIGNED = 232

        ' NOTE :: Nokia/Ericsson definitions
        VK_OEM_RESET = &HE9     ' // [OEMReset] = 233
        VK_OEM_JUMP = &HEA      ' // [OEMJump] = 234
        VK_OEM_PA1 = &HEB       ' // [OEMPA1] = 235
        VK_OEM_PA2 = &HEC       ' // [OEMPA2] = 236
        VK_OEM_PA3 = &HED       ' // [OEMPA3] = 237
        VK_OEM_WSCTRL = &HEE    ' // [OEMWSCtrl] = 238
        VK_OEM_CUSEL = &HEF     ' // [OEMCUSel] = 239
        VK_OEM_ATTN = &HF0      ' // [OEMATTN] = 240
        VK_OEM_FINISH = &HF1    ' // [OEMFinish] = 241
        VK_OEM_COPY = &HF2      ' // [OEMCopy] = 242
        VK_OEM_AUTO = &HF3      ' // [OEMAuto] = 243
        VK_OEM_ENLW = &HF4      ' // [OEMENLW] = 244
        VK_OEM_BACKTAB = &HF5       ' // [OEMBackTab] = 245

        VK_ATTN = &HF6          ' // [Attn] = 246
        VK_CRSEL = &HF7         ' // [Crsel] = 247
        VK_EXSEL = &HF8         ' // [Exsel] = 248
        VK_EREOF = &HF9         ' // [EraseEof] = 249
        VK_PLAY = &HFA          ' // [Play] = 250
        VK_ZOOM = &HFB          ' // [Zoom] = 251
        VK_NONAME = &HFC        ' // [NoName] = 252
        VK_PA1 = &HFD           ' // [Pa1] = 253
        VK_OEM_CLEAR = &HFE     ' // [OemClear] = 254

        ' // UNASSIGNED // = &HFFFF    ' // [KeyCode] = 65535
        ' // UNASSIGNED // = &H10000    ' // [Shift] = 65536
        ' // UNASSIGNED // = &H20000    ' // [Control] = 131072
        ' // UNASSIGNED // = &H40000    ' // [Alt] = 262144

    End Enum

#End Region

#Region "Constansts"

    Const KEYEVENTF_KEYUP As Integer = &H2
    Const INPUT_MOUSE As Integer = 0
    Const INPUT_KEYBOARD As Integer = 1
    Const INPUT_HARDWARE As Integer = 2

    Const MOUSEEVENTF_LEFTDOWN = &H2 ' left button down
    Const MOUSEEVENTF_LEFTUP = &H4 ' left button up
    Const MOUSEEVENTF_MIDDLEDOWN = &H20 ' middle button down
    Const MOUSEEVENTF_MIDDLEUP = &H40 ' middle button up
    Const MOUSEEVENTF_RIGHTDOWN = &H8 ' right button down
    Const MOUSEEVENTF_RIGHTUP = &H10 ' right button up

    Const GW_CHILD = 5
    Const GW_HWNDNEXT = 2
    Const WM_GETTEXT = &HD
    Const WM_GETTEXTLENGTH = &HE

    Const SW_SHOW = 5
    Const SW_RESTORE = 9
    Const GW_OWNER = 4
    Const GWL_HWNDPARENT = (-8)
    Const GWL_EXSTYLE = (-20)
    Const WS_EX_TOOLWINDOW = &H80
    Const WS_EX_APPWINDOW = &H40000

    Const GA_PARENT = 1 'Retrieves the parent window. This does not include the owner, as it does with the GetParent function.
    Const GA_ROOT = 2 'Retrieves the root window by walking the chain of parent windows.
    Const GA_ROOTOWNER = 3 'Retrieves the owned root window by walking the chain of parent and owner windows returned by GetParent.

    Const SW_SHOWNORMAL = 1
    Const SW_SHOWMAXIMIZED = 3

    Const WM_SETTEXT = 12

#End Region

#Region "Structures"
    <StructLayout(LayoutKind.Sequential)>
    Public Structure RECT
        Public Left As Int32
        Public Top As Int32
        Public Right As Int32
        Public Bottom As Int32
    End Structure

    Structure LASTINPUTINFO
        <MarshalAs(UnmanagedType.U4)>
        Public cbSize As Integer
        <MarshalAs(UnmanagedType.U4)>
        Public dwTime As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Structure MOUSEINPUT
        Public dx As Integer
        Public dy As Integer
        Public mouseData As UInteger
        Public dwFlags As UInteger
        Public time As UInteger
        Private dwExtraInfo As IntPtr
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Structure KEYBDINPUT
        Public wVk As UShort
        Public wScan As UShort
        Public dwFlags As UInteger
        Public time As UInteger
        Private dwExtraInfo As IntPtr
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Structure HARDWAREINPUT
        Public uMsg As Integer
        Public wParamL As Short
        Public wParamH As Short
    End Structure

    <StructLayout(LayoutKind.Explicit)>
    Structure MouseKeybdHardwareInputUnion
        <FieldOffset(0)>
        Public mi As MOUSEINPUT

        <FieldOffset(0)>
        Public ki As KEYBDINPUT

        <FieldOffset(0)>
        Public hi As HARDWAREINPUT
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Structure INPUT
        Public type As UInteger
        Public mkhi As MouseKeybdHardwareInputUnion
    End Structure

#End Region

#Region "ExternalMethods"
    Private Shared searchPid As Integer
    Private Shared searchPidHandle As IntPtr = IntPtr.Zero

    Public Sub SetWindowPosition(ByVal hWnd As IntPtr, ByVal rect As RECT)
        Dim width, height As Integer
        width = rect.Right - rect.Left
        height = rect.Bottom - rect.Top
        SetWindowPos(hWnd, New IntPtr(SpecialWindowHandles.HWND_TOP), rect.Left, rect.Top, width, height, SetWindowPosFlags.SWP_SHOWWINDOW)
    End Sub

    Public Sub SendKey(ByVal text As String)
        Dim Input As String = String.Empty

        Dim charsToEscape() As Char = "{}+^%~()[]".ToCharArray
        Dim builder As New StringBuilder()
        builder.Append(Input)

        'Escape SendInput
        'As desribed https://msdn.microsoft.com/en-us/library/system.windows.forms.sendkeys%28v=vs.110%29.aspx
        For Each c As Char In text
            If charsToEscape.Contains(c) Then
                builder.Append("{" & c & "}")
            Else
                builder.Append(c)
            End If
        Next
        Input = builder.ToString()

        SendKeys.SendWait(Input)
    End Sub

    Dim idletime As Integer
    Dim lastInputInf As New LASTINPUTINFO()

    Public Function GetLastInputTime() As Integer

        idletime = 0
        lastInputInf.cbSize = Marshal.SizeOf(lastInputInf)
        lastInputInf.dwTime = 0

        If GetLastInputInfo(lastInputInf) Then
            idletime = Environment.TickCount - lastInputInf.dwTime
        End If

        Return If(idletime > 0, idletime, 0)

    End Function

    Public Sub SendKey(ByVal bKey As UShort, ByVal modifier As UShort)
        Dim GInput(3) As INPUT

        ' press the key
        GInput(0).type = INPUT_KEYBOARD
        GInput(0).mkhi.ki.wVk = modifier
        GInput(0).mkhi.ki.dwFlags = 0
        ' press the key

        GInput(1).type = INPUT_KEYBOARD
        GInput(1).mkhi.ki.wVk = bKey
        GInput(1).mkhi.ki.dwFlags = 0

        ' release the key
        GInput(2).type = INPUT_KEYBOARD
        GInput(2).mkhi.ki.wVk = bKey
        GInput(2).mkhi.ki.dwFlags = KEYEVENTF_KEYUP

        ' release the key
        GInput(3).type = INPUT_KEYBOARD
        GInput(3).mkhi.ki.wVk = modifier
        GInput(3).mkhi.ki.dwFlags = KEYEVENTF_KEYUP

        SendInput(4, GInput, Marshal.SizeOf(GetType(INPUT)))
    End Sub

    Public Sub SendKey(ByVal bKey As UShort)
        Dim GInput(1) As INPUT

        ' press the key
        GInput(0).type = INPUT_KEYBOARD
        GInput(0).mkhi.ki.wVk = bKey
        GInput(0).mkhi.ki.dwFlags = 0

        ' release the key
        GInput(1).type = INPUT_KEYBOARD
        GInput(1).mkhi.ki.wVk = bKey
        GInput(1).mkhi.ki.dwFlags = KEYEVENTF_KEYUP

        SendInput(2, GInput, Marshal.SizeOf(GetType(INPUT)))

    End Sub

    Public Function hWnd2pID(ByVal hWnd As IntPtr) As Integer
        Dim result As Integer
        GetWindowThreadProcessId(hWnd, result)
        Return result
    End Function

    Public Function windowIsRelated(ByVal Child As IntPtr, ByVal Parent As IntPtr) As Boolean
        If GetAncestor(Child, GA_PARENT) = Parent Then Return True
        If GetAncestor(Child, GA_ROOT) = Parent Then Return True
        If GetAncestor(Child, GA_ROOTOWNER) = Parent Then Return True

        'Are you from the same process?.
        Dim ChildPID, ParentPID As Integer
        GetWindowThreadProcessId(Child, ChildPID)
        GetWindowThreadProcessId(Parent, ParentPID)
        If ChildPID = ParentPID Then Return True

        Return False
    End Function

    Public Function updateWindowSize(ByVal hWnd As IntPtr) As RECT
        ' Get the window's dimensions
        Dim rct As RECT
        GetWindowRect(hWnd, rct)
        Return rct
    End Function

    Public Function CurrentWindow() As IntPtr
        Return GetForegroundWindow
    End Function

    'Mouse Stuff
    Public Sub SetCusor(ByVal X As Int32, ByVal y As Int32)
        SetCursorPos(X, y) ' Where X and Y are in pixel
    End Sub

    Public Function GetCursor() As Point
        Dim tempPos As Point
        Dim R As Long = GetCursorPos(tempPos) ' You'll get your location in TempPos
        Return tempPos
    End Function

    ' simulate clicks
    Public Sub ClickLeft()
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero)
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero)
    End Sub

    Public Sub ClickRight()
        mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, UIntPtr.Zero)
        mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, UIntPtr.Zero)
    End Sub

    Public Sub ClickMiddle()
        mouse_event(MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, UIntPtr.Zero)
        mouse_event(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, UIntPtr.Zero)
    End Sub

    Public Sub ClickAndDrag(ByVal X As Int32, ByVal y As Int32)
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero)
        SetCursorPos(X, y) ' Where X and Y are in pixel
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero)
    End Sub

    'Change the active window
    Public Function SetWindowActive(ByVal hWnd As IntPtr, ByVal Maximised As Boolean) As Boolean
        Dim Failed As Boolean

        Dim windowMode As Integer = If(Maximised, SW_SHOWMAXIMIZED, SW_SHOWNORMAL)

        If ShowWindow(hWnd, windowMode) = False Then
            Failed = True
        End If

        Failed = If(SetForegroundWindow(hWnd) = False, True, False)

        Return Failed
    End Function

    Public Function fGetWindowText(ByVal hWnd As IntPtr) As String
        Dim lReturn As Integer
        Dim sWindowText As String
        'Get the window's title
        sWindowText = Space$(256)
        lReturn = GetWindowText(hWnd, sWindowText, Len(sWindowText))
        sWindowText = Trim(sWindowText)
        Return sWindowText
    End Function

    Public Shared Function GetHandleByProcessID(PID As Integer) As IntPtr
        searchPid = PID
        searchPidHandle = IntPtr.Zero

        EnumWindows(AddressOf fEnumWindowsCallBack, UIntPtr.Zero)

        Return searchPidHandle
    End Function

    Public Shared Function fEnumWindowsCallBack(ByVal hWnd As IntPtr, ByVal lParam As Integer) As Boolean
        Dim lReturn As Integer
        Dim lExStyle As Integer
        Dim bNoOwner As Boolean
        Dim sWindowText As String
        ' This callback function is called by Windows (from
        ' the EnumWindows API call) for EVERY window that exists.
        ' Windows to display are those that:
        ' - are not this app's
        ' - are visible
        ' - do not have a parent
        ' - have no owner and are not Tool windows OR
        ' have an owner and are App windows

        If IsWindowVisible(hWnd) Then
            If GetParent(hWnd) = IntPtr.Zero Then
                bNoOwner = (GetWindow(hWnd, GW_OWNER) = IntPtr.Zero)
                lExStyle = GetWindowInteger(hWnd, GWL_EXSTYLE)

                If (((lExStyle And WS_EX_TOOLWINDOW) = 0) And bNoOwner) Or ((lExStyle And WS_EX_APPWINDOW) And Not bNoOwner) Then

                    'Get the window's title
                    sWindowText = Space$(256)
                    lReturn = GetWindowText(hWnd, sWindowText, Len(sWindowText))
                    sWindowText = Trim(sWindowText)
                    If lReturn Then
                        Dim pID As Integer
                        GetWindowThreadProcessId(hWnd, pID)
                        If pID = searchPid Then
                            searchPidHandle = hWnd
                        End If
                    End If
                End If
            End If
        End If
        fEnumWindowsCallBack = True
    End Function

#End Region

End Class