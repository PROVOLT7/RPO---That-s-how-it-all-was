using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class AltF4Blocker : MonoBehaviour
{
    private const int WM_CLOSE = 0x0010;

#if UNITY_STANDALONE_WIN
    [DllImport("User32.dll")]
    private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, WndProcDelegate newProc);

    [DllImport("User32.dll")]
    private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    private const int GWL_WNDPROC = -4;

    private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    private static WndProcDelegate _wndProcDelegate;
    private static IntPtr _oldWndProc = IntPtr.Zero;

    public static event Action OnAltF4;

    void Start()
    {
        IntPtr window = GetActiveWindow();
        _wndProcDelegate = WndProc;
        _oldWndProc = SetWindowLongPtr(window, GWL_WNDPROC, _wndProcDelegate);
    }

    private static IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        if (msg == WM_CLOSE)
        {
            OnAltF4?.Invoke();
            return IntPtr.Zero; // блокируем закрытие
        }

        return GetWindowLongPtr(hWnd, GWL_WNDPROC);
    }
#endif
}
