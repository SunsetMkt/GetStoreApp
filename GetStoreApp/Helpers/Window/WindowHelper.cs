﻿using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Windowing;
using System;

namespace GetStoreApp.Helpers.Window
{
    /// <summary>
    /// 应用窗口辅助类
    /// </summary>
    public static class WindowHelper
    {
        private static OverlappedPresenter WindowPresenter { get; set; }

        // 获取窗口是否已经最小化
        public static bool IsWindowMinimized
        {
            get { return WindowPresenter is not null && WindowPresenter.State == OverlappedPresenterState.Minimized; }
        }

        // 获取窗口是否已经最大化
        public static bool IsWindowMaximized
        {
            get { return WindowPresenter is not null && WindowPresenter.State == OverlappedPresenterState.Maximized; }
        }

        // 获取窗口是否已经被隐藏
        public static bool IsWindowVisible
        {
            get { return Program.ApplicationRoot.AppWindow.IsVisible; }
        }

        /// <summary>
        /// 初始化重叠的配置显示应用窗口
        /// </summary>
        public static void InitializePresenter(AppWindow appWindow)
        {
            if (appWindow is not null)
            {
                WindowPresenter = appWindow.Presenter as OverlappedPresenter ?? OverlappedPresenter.Create();
            }
        }

        /// <summary>
        /// 隐藏窗口
        /// </summary>
        public static void HideAppWindow()
        {
            if (Program.ApplicationRoot.AppWindow.IsVisible)
            {
                Program.ApplicationRoot.AppWindow.Hide();
            }
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        public static void ShowAppWindow()
        {
            // 判断窗口状态是否处于最大化状态，如果是，直接最大化窗口
            if (IsWindowMaximized)
            {
                MaximizeAppWindow();
            }

            // 其他状态下窗口还原显示状态
            else
            {
                // 还原窗口（如果最小化）时
                RestoreAppWindow();
            }

            BringToFront();
        }

        /// <summary>
        /// 还原窗口
        /// </summary>
        public static void RestoreAppWindow()
        {
            if (WindowPresenter is not null)
            {
                if (IsWindowMinimized)
                {
                    WindowPresenter.Restore(true);
                }
                else
                {
                    WindowPresenter.Restore();
                }
            }
        }

        /// <summary>
        /// 最小化窗口
        /// </summary>
        public static void MinimizeAppWindow()
        {
            if (WindowPresenter is not null && WindowPresenter.IsMinimizable)
            {
                WindowPresenter.Minimize();
            }
        }

        /// <summary>
        /// 最大化窗口
        /// </summary>
        public static void MaximizeAppWindow()
        {
            if (WindowPresenter is not null && WindowPresenter.IsMaximizable)
            {
                WindowPresenter.Maximize();
            }
        }

        /// <summary>
        /// 将应用窗口设置到前台
        /// </summary>
        public static void BringToFront()
        {
            IntPtr hForegdWnd = User32Library.GetForegroundWindow();
            int dwCurID = Kernel32Library.GetCurrentThreadId();
            User32Library.GetWindowThreadProcessId(hForegdWnd, out int dwForeID);
            User32Library.AttachThreadInput(dwCurID, dwForeID, true);
            User32Library.SetForegroundWindow(Program.ApplicationRoot.MainWindow.GetMainWindowHandle());
            User32Library.AttachThreadInput(dwCurID, dwForeID, false);
        }

        /// <summary>
        /// 设置应用窗口置顶
        /// </summary>
        public static void SetAppTopMost(bool topMostValue)
        {
            if (WindowPresenter is not null)
            {
                WindowPresenter.IsAlwaysOnTop = topMostValue;
            }
        }
    }
}
