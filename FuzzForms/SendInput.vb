Imports System.Runtime.InteropServices

Class SendInput

    Const ShiftKey As Short = 16
    Const ControlKey As Short = 17
    Const AltKey As Short = 18

    Public Function GetKeyboardCode(ByVal value As Char) As Int32
        Dim scan = BitConverter.GetBytes(NativeMethods.VkKeyScan(value))
        Return CInt(scan(0))
    End Function

    Public Function SendStringKeys(ByVal data As String) As Boolean
        If data Is Nothing Then Throw New ArgumentNullException("data")
        Dim tempList = New List(Of tagINPUT)
        For Each item As Char In data
            Dim scan = BitConverter.GetBytes(NativeMethods.VkKeyScan(item))
            Select Case scan(1)
                Case Is = 1 'Contain Shift
                    tempList.Add(GetKeyDownStructure(ShiftKey))
                    tempList.Add(GetKeyDownStructure(CShort(scan(0))))
                    tempList.Add(GetKeyUpStructure(CShort(scan(0))))
                    tempList.Add(GetKeyUpStructure(ShiftKey))
                Case Is = 2 ' Contain Ctrl
                    tempList.Add(GetKeyDownStructure(ControlKey))
                    tempList.Add(GetKeyDownStructure(CShort(scan(0))))
                    tempList.Add(GetKeyUpStructure(CShort(scan(0))))
                    tempList.Add(GetKeyUpStructure(ControlKey))
                Case Is = 4 ' Contain Alt
                    tempList.Add(GetKeyDownStructure(AltKey))
                    tempList.Add(GetKeyDownStructure(CShort(scan(0))))
                    tempList.Add(GetKeyUpStructure(CShort(scan(0))))
                    tempList.Add(GetKeyUpStructure(AltKey))
                Case Else
                    tempList.Add(GetKeyDownStructure(CShort(scan(0))))
                    tempList.Add(GetKeyUpStructure(CShort(scan(0))))
            End Select
        Next
        Dim objSend = tempList.ToArray
        Dim nInput = CUInt(objSend.Count)
        Return nInput = NativeMethods.SendInput(CUInt(objSend.Count), objSend, Marshal.SizeOf(GetType(tagINPUT)))
    End Function

    Public Function SendKeyPress(ByVal key As Int32) As Boolean
        If ValidateKeyCode(key) = False Then Throw New ArgumentOutOfRangeException("key")
        Dim objSend() As tagINPUT = {GetKeyDownStructure(CShort(key)), GetKeyUpStructure(CShort(key))}
        Dim nInput = CUInt(objSend.Count)
        Return nInput = NativeMethods.SendInput(CUInt(objSend.Count), objSend, Marshal.SizeOf(GetType(tagINPUT)))
    End Function

    Public Function SendKeyUp(ByVal key As Int32) As Boolean
        If ValidateKeyCode(key) = False Then Throw New ArgumentOutOfRangeException("key")
        Dim objSend = New tagINPUT(0) {GetKeyUpStructure(CShort(key))}
        Dim nInput = CUInt(objSend.Count)
        Return nInput = NativeMethods.SendInput(CUInt(objSend.Count), objSend, Marshal.SizeOf(GetType(tagINPUT)))
    End Function

    Public Function SendKeyDown(ByVal key As Int32) As Boolean
        If ValidateKeyCode(key) = False Then Throw New ArgumentOutOfRangeException("key")
        Dim objSend = New tagINPUT(0) {GetKeyDownStructure(CShort(key))}
        Dim nInput = CUInt(objSend.Count)
        Return nInput = NativeMethods.SendInput(CUInt(objSend.Count), objSend, Marshal.SizeOf(GetType(tagINPUT)))
    End Function


    Private Function GetKeyDownStructure(ByVal key As Short) As tagINPUT
        Dim retVal = New tagINPUT With {.dwType = INPUTTYPE.KEYBOARD}
        With retVal.union.ki
            .dwExtraInfo = Nothing
            .dwFlags = 0
            .time = 0
            .wScan = 0
            .wVk = key
        End With
        Return retVal
    End Function

    Private Function GetKeyUpStructure(ByVal key As Short) As tagINPUT
        Dim retVal = New tagINPUT With {.dwType = INPUTTYPE.KEYBOARD}
        With retVal.union.ki
            .dwExtraInfo = Nothing
            .dwFlags = KEYEVENTF.KEYUP
            .time = 0
            .wScan = 0
            .wVk = key
        End With
        Return retVal
    End Function

    Public Function ValidateKeyCode(ByVal key As Int32) As Boolean
        If key < 1 OrElse key > 254 Then Return False
        Return True
    End Function


#Region "Structures"

    <StructLayout(LayoutKind.Sequential)>
    Friend Structure tagHARDWAREINPUT
        Public uMsg As Int32
        Public wParamL As Int16
        Public wParamH As Int16
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Friend Structure tagKEYBDINPUT
        Public wVk As Int16
        Public wScan As Int16
        Public dwFlags As KEYEVENTF
        Public time As UInt32
        Public dwExtraInfo As UIntPtr
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Friend Structure tagMOUSEINPUT
        Public dx As Int32
        Public dy As Int32
        Public mouseData As Int32
        Public dwFlags As MOUSEEVENTF
        Public time As UInt32
        Public dwExtraInfo As UIntPtr
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Friend Structure tagINPUT
        Public dwType As INPUTTYPE
        Public union As UnionTag
        <StructLayout(LayoutKind.Explicit)>
        Public Structure UnionTag
            <FieldOffset(0)> Public mi As tagMOUSEINPUT
            <FieldOffset(0)> Public ki As tagKEYBDINPUT
            <FieldOffset(0)> Public hi As tagHARDWAREINPUT
        End Structure
    End Structure

#End Region

#Region "Enumerations"

    Friend Enum INPUTTYPE As UInt32
        MOUSE = 0
        KEYBOARD = 1
        HARDWARE = 2
    End Enum

    <Flags()>
    Friend Enum MOUSEEVENTF As UInt32
        MOVE = &H1  ' mouse move
        LEFTDOWN = &H2  ' left button down
        LEFTUP = &H4  ' left button up
        RIGHTDOWN = &H8  ' right button down
        RIGHTUP = &H10  ' right button up
        MIDDLEDOWN = &H20  ' middle button down
        MIDDLEUP = &H40  ' middle button up
        XDOWN = &H80  ' x button down
        XUP = &H100  ' x button down
        WHEEL = &H800  ' wheel button rolled
        HWHEEL = &H1000  ' hwheel button rolled
        MOVE_NOCOALESCE = &H2000  ' do not coalesce mouse moves
        VIRTUALDESK = &H4000  ' map to entire virtual desktop
        ABSOLUTE = &H8000  ' absolute move
    End Enum

    <Flags()>
    Friend Enum KEYEVENTF As UInt32
        EXTENDEDKEY = &H1
        KEYUP = &H2
        UNICODE = &H4
        SCANCODE = &H8
    End Enum

#End Region

    Private NotInheritable Class NativeMethods
        Private Sub New()
        End Sub

        Private Const WinUser As String = "user32.dll"

        <DllImport(WinUser, EntryPoint:="SendInput", SetLastError:=True)>
        Public Shared Function SendInput(<[In]()> ByVal nInput As UInt32,
                                         <[In](), MarshalAs(UnmanagedType.LPArray, ArraySubtype:=UnmanagedType.Struct, SizeParamindex:=0)> ByVal pInputs() As tagINPUT,
                                         <[In]()> ByVal cbInput As Int32) As UInt32
        End Function

        <DllImport(WinUser, EntryPoint:="VkKeyScan", SetLastError:=True)>
        Public Shared Function VkKeyScan(<[In]()> ByVal ch As Char) As UInt16
        End Function

    End Class

End Class