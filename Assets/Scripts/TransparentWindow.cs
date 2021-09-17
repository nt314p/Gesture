using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class TransparentWindow : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern bool CloseWindow(IntPtr handle);

    [DllImport("Dwmapi.dll")]
    private static extern int DwmExtendFrameIntoClientArea(IntPtr handle, ref MARGINS margins);
    
    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowLongPtr(IntPtr handle, int nIndex);
    
    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowLongPtr(IntPtr handle, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr handle, IntPtr handleInsertAfter, int x, int y, int cx, int cy,
        uint flags);

    [DllImport("user32.dll")]
    private static extern bool SetLayeredWindowAttributes(IntPtr handle, int crKey, byte bAlpha, int dwFlags);

    [StructLayout(LayoutKind.Sequential)]
    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    private const int GWL_STYLE = -16;
    private const int GWL_EXSTYLE = -20;
    
    private const int WS_BORDER = 0x00800000;
    private const int WS_CAPTION = 0x00C00000;
    private const int WS_VISIBLE = 0x10000000;
    private const int WS_HSCROLL = 0x00100000;
    private const int WS_VSCROLL = 0x00200000;
    private const int WS_SYSMENU = 0x00080000;
    private const int WS_MAXIMIZE = 0x01000000;
    private const int WS_MINIMIZE = 0x20000000;
    
    private const int WS_EX_LAYERED = 0x00080000;
    private const int WS_EX_TRANSPARENT = 0x00000020;

    private readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

    private const int SWP_FRAMECHANGED = 0x020;
    private const int SWP_SHOWWINDOW = 0x040;

    private const int LWA_COLORKEY = 0x00000001;
    private const int LWA_ALPHA = 0x00000002;
    
    private IntPtr handle;

    [SerializeField] private Camera mainCamera;
    
    private void Start()
    {
#if !UNITY_EDITOR
        InitializeTransparentWindow();
#endif
        // IntPtr stylePtr = GetWindowLongPtr(handle, GWL_STYLE);
        // long styleLong = stylePtr.ToInt64();
        //
        // styleLong &= ~(WS_CAPTION | WS_HSCROLL | WS_VSCROLL | WS_SYSMENU);
        // styleLong &= WS_MAXIMIZE;
        //
        // SetWindowLongPtr(handle, GWL_STYLE, new IntPtr(styleLong));
        // SetWindowLongPtr(handle, GWL_EXSTYLE, new IntPtr(WS_EX_LAYERED | WS_EX_TRANSPARENT));
        //
        // SetLayeredWindowAttributes(handle, 0, 0, LWA_ALPHA);
        //
        // int fWidth = Screen.width;
        // int fHeight = Screen.height;
        // SetWindowPos(handle, IntPtr.Zero, 0, 0, fWidth, fHeight, SWP_FRAMECHANGED | SWP_SHOWWINDOW);
    }

    private void InitializeTransparentWindow()
    {
        handle = GetActiveWindow();
        var margins = new MARGINS { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(handle, ref margins);

        SetWindowLongPtr(handle, GWL_EXSTYLE, new IntPtr(WS_EX_LAYERED | WS_EX_TRANSPARENT));
        SetWindowPos(handle, HWND_TOPMOST, 0, 0, 0, 0, 0);
    }
    
    private void Update()
    {
        var worldMouseCoords = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var collider2d = Physics2D.OverlapPoint(worldMouseCoords);
        var hover = collider2d != null;
#if !UNITY_EDITOR
        SetClickThrough(!hover);
#endif
    }

    private void SetClickThrough(bool isClickThrough)
    {
        var style = WS_EX_LAYERED;
        if (isClickThrough) style |= WS_EX_TRANSPARENT;
        SetWindowLongPtr(handle, GWL_EXSTYLE, new IntPtr(style));
    }
    
    
}
