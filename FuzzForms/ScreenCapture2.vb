Imports System
Imports System.Runtime.InteropServices
Imports System.Drawing
Imports System.Drawing.Imaging
    _
'/ <summary>
'/ Provides functions to capture the entire screen, or a particular window, and save it to a file.
'/ </summary>
Public Class ScreenCapture2

    '/ <summary>
    '/ Creates an Image object containing a screen shot of the entire desktop
    '/ </summary>
    '/ <returns></returns>
    Public Function CaptureScreen() As Image
        Return CaptureWindow(User32.GetDesktopWindow())
    End Function 'CaptureScreen


    '/ <summary>
    '/ Creates an Image object containing a screen shot of a specific window
    '/ </summary>
    '/ <param name="handle">The handle to the window. (In windows forms, this is obtained by the Handle property)</param>
    '/ <returns></returns>
    Public Function CaptureWindow(ByVal handle As IntPtr) As Image
        ' get te hDC of the target window
        Dim hdcSrc As IntPtr = User32.GetWindowDC(handle)
        ' get the size
        Dim windowRect As New User32.RECT()
        User32.GetWindowRect(handle, windowRect)
        Dim width As Integer = windowRect.right - windowRect.left
        Dim height As Integer = windowRect.bottom - windowRect.top
        ' create a device context we can copy to
        Dim hdcDest As IntPtr = GDI32.CreateCompatibleDC(hdcSrc)
        ' create a bitmap we can copy it to,
        ' using GetDeviceCaps to get the width/height
        Dim hBitmap As IntPtr = GDI32.CreateCompatibleBitmap(hdcSrc, width, height)
        ' select the bitmap object
        Dim hOld As IntPtr = GDI32.SelectObject(hdcDest, hBitmap)
        ' bitblt over
        GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY)
        ' restore selection
        GDI32.SelectObject(hdcDest, hOld)
        ' clean up 
        GDI32.DeleteDC(hdcDest)
        User32.ReleaseDC(handle, hdcSrc)

        ' get a .NET image object for it
        Dim img As Image = Image.FromHbitmap(hBitmap)
        ' free up the Bitmap object
        GDI32.DeleteObject(hBitmap)

        Return img
    End Function 'CaptureWindow


    '/ <summary>
    '/ Captures a screen shot of a specific window, and saves it to a file
    '/ </summary>
    '/ <param name="handle"></param>
    '/ <param name="filename"></param>
    '/ <param name="format"></param>
    Public Sub CaptureWindowToFile(ByVal handle As IntPtr, ByVal filename As String, ByVal format As ImageFormat)
        Dim img As Image = CaptureWindow(handle)
        img.Save(filename, format)
    End Sub 'CaptureWindowToFile


    '/ <summary>
    '/ Captures a screen shot of the entire desktop, and saves it to a file
    '/ </summary>
    '/ <param name="filename"></param>
    '/ <param name="format"></param>
    Public Sub CaptureScreenToFile(ByVal filename As String, ByVal format As ImageFormat)
        Dim img As Image = CaptureScreen()
        img.Save(filename, format)
    End Sub 'CaptureScreenToFile
       _

    '/ <summary>
    '/ Helper class containing Gdi32 API functions
    '/ </summary>
    Private Class GDI32

        Public Shared SRCCOPY As Integer = &HCC0020
        ' BitBlt dwRop parameter

        Declare Function BitBlt Lib "GDI32" (ByVal hObject As IntPtr, ByVal nXDest As Integer, ByVal nYDest As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal hObjectSource As IntPtr, ByVal nXSrc As Integer, ByVal nYSrc As Integer, ByVal dwRop As Integer) As Boolean

        Declare Function CreateCompatibleBitmap Lib "GDI32" (ByVal hDC As IntPtr, ByVal nWidth As Integer, ByVal nHeight As Integer) As IntPtr

        Declare Function CreateCompatibleDC Lib "GDI32" (ByVal hDC As IntPtr) As IntPtr

        Declare Function DeleteDC Lib "GDI32" (ByVal hDC As IntPtr) As Boolean

        Declare Function DeleteObject Lib "GDI32" (ByVal hObject As IntPtr) As Boolean

        Declare Function SelectObject Lib "GDI32" (ByVal hDC As IntPtr, ByVal hObject As IntPtr) As IntPtr
    End Class 'GDI32
       _

    '/ <summary>
    '/ Helper class containing User32 API functions
    '/ </summary>
    Private Class User32
        <StructLayout(LayoutKind.Sequential)> _
        Public Structure RECT
            Public left As Integer
            Public top As Integer
            Public right As Integer
            Public bottom As Integer
        End Structure 'RECT



        Declare Function GetDesktopWindow Lib "user32" () As IntPtr


        Declare Function GetWindowDC Lib "user32" (ByVal hWnd As IntPtr) As IntPtr


        Declare Function ReleaseDC Lib "user32" (ByVal hWnd As IntPtr, ByVal hDC As IntPtr) As IntPtr


        Declare Function GetWindowRect Lib "user32" (ByVal hWnd As IntPtr, ByRef rect As RECT) As IntPtr
    End Class 'User32
End Class 'ScreenCapture 

