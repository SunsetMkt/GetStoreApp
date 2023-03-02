﻿using GetStoreAppHelper.WindowsAPI.PInvoke.Kernel32;
using GetStoreAppHelper.WindowsAPI.PInvoke.Shell32;
using System;
using System.Runtime.InteropServices;
using Windows.Graphics;

namespace GetStoreAppHelper.WindowsAPI.PInvoke.User32
{
    public static class User32Library
    {
        private const string User32 = "User32.dll";

        /// <summary>
        /// 将消息信息传递给指定的窗口过程。
        /// </summary>
        /// <param name="lpPrevWndFunc">
        /// 上一个窗口过程。 如果通过调用设置为GWL_WNDPROC或DWL_DLGPROC的 nIndex 参数的 GetWindowLong 函数来获取此值，
        /// 则它实际上是窗口或对话框过程的地址，或者仅对 <see cref="CallWindowProc"> 有意义的特殊内部值。</param>
        /// <param name="hWnd">用于接收消息的窗口过程的句柄。</param>
        /// <param name="Msg">消息。</param>
        /// <param name="wParam">其他的消息特定信息。 此参数的内容取决于 <param name="Msg"> 参数的值。</param>
        /// <param name="lParam">其他的消息特定信息。 此参数的内容取决于 <param name="Msg"> 参数的值。</param>
        /// <returns>返回值指定消息处理的结果，具体取决于发送的消息。</returns>
        [DllImport(User32, CharSet = CharSet.Ansi, EntryPoint = "CallWindowProc", SetLastError = false)]
        public static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 创建具有扩展窗口样式的重叠、弹出窗口或子窗口;否则，此函数与 InitializeWindow 函数相同。
        /// 有关创建窗口以及 <see cref="CreateWindowEx"> 的其他参数的完整说明的详细信息，请参阅 InitializeWindow。
        /// </summary>
        /// <param name="dwExStyle">正在创建的窗口的扩展窗口样式。 </param>
        /// <param name="lpClassName">
        /// 由上一次对 <see cref="RegisterClass"> 或 RegisterClassEx 函数的调用创建的空终止字符串或类原子。 原子必须位于 <param  name="lpClassName"> 的低序单词中;高序单词必须为零。 如果 <param  name="lpClassName"> 是字符串，则指定窗口类名称。 类名可以是注册到 <see cref="RegisterClass"> 或 RegisterClassEx 的任何名称，前提是注册该类的模块也是创建窗口的模块。 类名也可以是任何预定义的系统类名称。
        /// </param>
        /// <param name="lpWindowName">
        /// 窗口名称。 如果窗口样式指定标题栏，则 <param name="lpWindowName"> 指向的窗口标题将显示在标题栏中。 使用 InitializeWindow 创建控件（如按钮、复选框和静态控件）时，请使用 <param name="lpWindowName"> 指定控件的文本。 使用 SS_ICON 样式创建静态控件时，请使用 <param name="lpWindowName"> 指定图标名称或标识符。 若要指定标识符，请使用语法“#num”。
        /// </param>
        /// <param name="dwStyle">正在创建的窗口的样式。</param>
        /// <param name="x">
        /// 窗口的初始水平位置。 对于重叠或弹出窗口， x 参数是窗口左上角的初始 x 坐标，以屏幕坐标表示。
        /// 对于子窗口， x 是窗口左上角相对于父窗口工作区左上角的 x 坐标。 如果 x 设置为 CW_USEDEFAULT，系统将选择窗口左上角的默认位置，并忽略 y 参数。
        /// CW_USEDEFAULT 仅适用于重叠窗口;如果为弹出窗口或子窗口指定， 则 x 和 y 参数设置为零。
        /// </param>
        /// <param name="y">
        /// 窗口的初始垂直位置。 对于重叠或弹出窗口， y 参数是窗口左上角的初始 y 坐标，以屏幕坐标表示。
        /// 对于子窗口， y 是子窗口左上角相对于父窗口工作区左上角的初始 y 坐标。 对于列表框 y ，是列表框工作区左上角相对于父窗口工作区左上角的初始 y 坐标。
        /// </param>
        /// <param name="nWidth">
        /// 窗口的宽度（以设备单位为单位）。 对于重叠的窗口， <param name="nWidth"> 是窗口的宽度、屏幕坐标或 CW_USEDEFAULT。
        /// 如果 <param name="nWidth"> 是CW_USEDEFAULT，则系统会为窗口选择默认宽度和高度;默认宽度从初始 x 坐标扩展到屏幕的右边缘;默认高度从初始 y 坐标扩展到图标区域的顶部。 CW_USEDEFAULT 仅适用于重叠窗口;如果为弹出窗口或子窗口指定 了CW_USEDEFAULT ， 则 <param name="nWidth"> 和 <param name="nHeight"> 参数设置为零。
        /// </param>
        /// <param name="nHeight">
        /// 窗口的高度（以设备单位为单位）。 对于重叠窗口， <param name="nHeight"> 是窗口的高度（以屏幕坐标为单位）。
        /// 如果 <param name="nWidth"> 参数设置为 CW_USEDEFAULT，则系统将忽略 <param name="nHeight">。
        /// </param>
        /// <param name="hWndParent">
        /// 正在创建的窗口的父窗口或所有者窗口的句柄。 若要创建子窗口或拥有的窗口，请提供有效的窗口句柄。 对于弹出窗口，此参数是可选的。
        /// </param>
        /// <param name="hMenu">
        /// 菜单的句柄，或指定子窗口标识符，具体取决于窗口样式。 对于重叠或弹出窗口， <param name="hMenu"> 标识要与窗口一起使用的菜单；如果使用类菜单，则为 NULL 。
        /// 对于子窗口， <param name="hMenu"> 指定子窗口标识符，即对话框控件用来通知其父级事件的整数值。
        /// 应用程序确定子窗口标识符;对于具有相同父窗口的所有子窗口，它必须是唯一的。
        /// </param>
        /// <param name="hInstance">要与窗口关联的模块实例的句柄。</param>
        /// <param name="lpParam">
        /// 指向通过 CREATESTRUCT 结构传递给窗口的值的指针， (lpCreateParams 成员) <see cref="WindowMessage.WM_CREATE"> 消息的 <param name="lpParam"> 参数所指向的值。 此消息在返回之前由此函数发送到创建的窗口。
        /// </param>
        /// <returns>如果函数成功，则返回值是新窗口的句柄。如果函数失败，则返回值为 NULL。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "CreateWindowExW", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(
           WindowStylesEx dwExStyle,
           string lpClassName,
           string lpWindowName,
           WindowStyles dwStyle,
           int x,
           int y,
           int nWidth,
           int nHeight,
           IntPtr hWndParent,
           IntPtr hMenu,
           IntPtr hInstance,
           IntPtr lpParam);

        /// <summary>
        /// 调用默认窗口过程，为应用程序未处理的任何窗口消息提供默认处理。 此函数可确保处理每个消息。 使用窗口过程收到的相同参数调用 <see cref="DefWindowProc">。
        /// </summary>
        /// <param name="hWnd">接收消息的窗口过程的句柄。</param>
        /// <param name="Msg">其他消息信息。</param>
        /// <param name="wParam">其他消息信息。 此参数的内容取决于 <param name="Msg"> 参数的值。</param>
        /// <param name="lParam">其他消息信息。 此参数的内容取决于 <param name="Msg"> 参数的值。</param>
        /// <returns>返回值是消息处理的结果，取决于消息。</returns>
        [DllImport(User32, CharSet = CharSet.Ansi, EntryPoint = "DefWindowProc", SetLastError = false)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 销毁指定的窗口。 该函数将 <see cref="WindowMessage.WM_DESTROY"> 和 <see cref="WindowMessage.WM_NCDESTROY"> 消息发送到窗口以停用它，并从中删除键盘焦点。 该函数还会销毁窗口的菜单、刷新线程消息队列、销毁计时器、删除剪贴板所有权，如果窗口位于查看器链顶部) ，则中断剪贴板查看器链。
        /// 如果指定的窗口是父窗口或所有者窗口， 则 <see cref="DestroyWindow"> 会在销毁父窗口或所有者窗口时自动销毁关联的子窗口或拥有窗口。
        /// 该函数首先销毁子窗口或拥有的窗口，然后销毁父窗口或所有者窗口。
        /// <see cref="DestroyWindow"> 还会销毁 CreateDialog 函数创建的无模式对话框。
        /// </summary>
        /// <param name="hwnd">要销毁的窗口的句柄。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "DestroyWindow", SetLastError = true)]
        public static extern bool DestroyWindow(IntPtr hwnd);

        /// <summary>
        /// 将消息调度到窗口过程。 它通常用于调度 <see cref="GetMessage"> 函数检索的消息。
        /// </summary>
        /// <param name="lpmsg">指向包含消息的结构的指针。</param>
        /// <returns>返回值指定窗口过程返回的值。 虽然其含义取决于正在调度的消息，但通常忽略返回值。</returns>
        [DllImport(User32, CharSet = CharSet.Ansi, EntryPoint = "DispatchMessage", SetLastError = true)]
        public static extern IntPtr DispatchMessage(ref MSG lpmsg);

        /// <summary>
        /// 检索一个窗口的句柄，该窗口的类名和窗口名称与指定的字符串匹配。 该函数搜索子窗口，从指定子窗口后面的子窗口开始。 此函数不执行区分大小写的搜索。
        /// </summary>
        /// <param name="parentHandle">
        /// 要搜索其子窗口的父窗口的句柄。
        /// 如果 hwndParent 为 NULL，则该函数使用桌面窗口作为父窗口。 函数在桌面的子窗口之间搜索。
        /// 如果 hwndParentHWND_MESSAGE，则函数将搜索所有 仅消息窗口。
        /// </param>
        /// <param name="childAfter">
        /// 子窗口的句柄。 搜索从 Z 顺序中的下一个子窗口开始。 子窗口必须是 hwndParent 的直接子窗口，而不仅仅是子窗口。
        /// 如果 hwndChildAfter 为 NULL，则搜索从 hwndParent 的第一个子窗口开始。
        /// 请注意，如果 hwndParent 和 hwndChildAfter 均为 NULL，则该函数将搜索所有顶级窗口和仅消息窗口。
        /// </param>
        /// <param name="className">
        /// 类名或上一次对 RegisterClass 或 RegisterClassEx 函数的调用创建的类名或类原子。 原子必须置于 lpszClass 的低序单词中;高阶单词必须为零。
        /// 如果 lpszClass 是字符串，则指定窗口类名。 类名可以是注册到 RegisterClass 或 RegisterClassEx 的任何名称，也可以是预定义的控件类名称，也可以是 MAKEINTATOM(0x8000)。 在此后一种情况下，0x8000是菜单类的原子。 有关详细信息，请参阅本主题的“备注”部分。
        /// </param>
        /// <param name="windowTitle">窗口名称 (窗口的标题) 。 如果此参数为 NULL，则所有窗口名称都匹配。</param>
        /// <returns>如果函数成功，则返回值是具有指定类和窗口名称的窗口的句柄。如果函数失败，则返回值为 NULL。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "FindWindowExW", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        /// <summary>检索指定窗口的上级句柄。</summary>
        /// <param name="hWnd">
        /// 要检索其上级窗口的句柄。 如果此参数是桌面窗口，则该函数返回 <see cref="IntPtr.Zero" />.
        /// </param>
        /// <param name="gaFlags">要检索的上级。 </param>
        /// <returns>返回值是上级窗口的句柄。</returns>
        [DllImport(User32, CharSet = CharSet.Ansi, EntryPoint = "GetAncestor", SetLastError = true)]
        public static extern IntPtr GetAncestor(IntPtr hWnd, GetAncestorFlags gaFlags);

        /// <summary>
        /// 检索鼠标光标的位置（以屏幕坐标为单位）。
        /// </summary>
        /// <param name="lpPoint">指向接收光标屏幕坐标的 <see cref="PointInt32"> 结构的指针。</param>
        /// <returns>如果成功，则返回非零值，否则返回零。 </returns>
        [DllImport(User32, CharSet = CharSet.Ansi, EntryPoint = "GetCursorPos", SetLastError = false)]
        public static extern bool GetCursorPos(out PointInt32 lpPoint);

        /// <summary>
        /// 返回指定窗口的每英寸点 (dpi) 值。
        /// </summary>
        /// <param name="hwnd">要获取相关信息的窗口。</param>
        /// <returns>窗口的 DPI，无效 的 <param name="hwnd"> 值将导致返回值 0。</returns>
        [DllImport(User32, CharSet = CharSet.Ansi, EntryPoint = "GetDpiForWindow", SetLastError = false)]
        public static extern int GetDpiForWindow(IntPtr hwnd);

        /// <summary>
        /// 从调用线程的消息队列中检索消息。 该函数将调度传入的已发送邮件，直到发布的消息可供检索。与 <see cref="GetMessage"> 不同， PeekMessage 函数不会等待在返回之前发布消息。
        /// </summary>
        /// <param name="lpMsg">指向从线程消息队列接收消息信息的 <see cref="MSG"> 结构的指针。</param>
        /// <param name="hWnd">
        /// 要检索其消息的窗口的句柄。 窗口必须属于当前线程。
        /// 如果 hWnd 为 NULL， 则 <see cref="GetMessage"> 会检索属于当前线程的任何窗口的消息，以及当前线程的消息队列上的任何消息，其 hwnd 值为 NULL ， (看到 <see cref="MSG"> 结构) 。 因此，如果 hWnd 为 NULL，则处理窗口消息和线程消息。
        /// 如果 hWnd 为 -1，则 GetMessage 仅检索当前线程的消息队列中的消息，其 hwnd 值为 NULL，也就是说，当 hWnd 参数为 NULL 或 PostThreadMessage 时发布的线程消息。
        /// </param>
        /// <param name="wMsgFilterMin">
        /// 要检索到的最低消息值的整数值。 使用 <see cref="WindowMessage.WM_KEYFIRST"> (0x0100) 指定第一个键盘消息或 <see cref="WindowMessage.WM_MOUSEFIRST"> (0x0200) 指定第一条鼠标消息。
        /// 使用此处和 <param name="wMsgFilterMax"> 中的 <see cref="WindowMessage.WM_INPUT"> 仅指定 <see cref="WindowMessage.WM_INPUT"> 消息。
        /// 如果 <param name="wMsgFilterMax"> 和 <param name="wMsgFilterMax"> 均为零， 则 <see cref="GetMessage"> 将返回所有可用消息，即不会执行范围筛选。
        /// </param>
        /// <param name="wMsgFilterMax">
        /// 要检索的最大消息值的整数值。 使用 <see cref="WindowMessage.WM_KEYFIRST"> 指定最后一条键盘消息或 <see cref="WindowMessage.WM_MOUSEFIRST"> 指定最后一条鼠标消息。
        /// 使用此处和 <param name="wMsgFilterMax"> 中的 <see cref="WindowMessage.WM_INPUT"> 仅指定 <see cref="WindowMessage.WM_INPUT"> 消息。
        /// 如果 <param name="wMsgFilterMax"> 和 <param name="wMsgFilterMax"> 均为零， 则 <see cref="GetMessage"> 将返回所有可用消息，即不会执行范围筛选。
        /// </param>
        /// <returns>
        /// 如果函数检索 非 <see cref="WindowMessage.WM_QUIT"> 的消息，则返回值为非零值。
        /// 如果函数检索 <see cref="WindowMessage.WM_QUIT"> 消息，则返回值为零。
        /// 如果出现错误，则返回值为 -1。 例如，如果 hWnd 是无效的窗口句柄或 lpMsg 是无效指针，则函数将失败。 要获得更多的错误信息，请调用 GetLastError。
        /// </returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "GetMessageW", SetLastError = true)]
        public static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, WindowMessage wMsgFilterMin, WindowMessage wMsgFilterMax);

        /// <summary>
        /// 检索创建指定窗口的线程的标识符，以及（可选）创建窗口的进程的标识符。
        /// </summary>
        /// <param name="hwnd">窗口的句柄。</param>
        /// <param name="ID">指向接收进程标识符的变量的指针。如果此参数不为 NULL，则 <see cref="GetWindowThreadProcessId"> 将进程的标识符复制到变量;否则，它不会。</param>
        /// <returns>返回值是创建窗口的线程的标识符。</returns>
        [DllImport(User32, CharSet = CharSet.Ansi, EntryPoint = "GetWindowThreadProcessId", SetLastError = false)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

        /// <summary>
        /// 加载图标、游标、动画游标或位图。
        /// </summary>
        /// <param name="hinst">DLL 或可执行文件 (.exe 模块的句柄) ，其中包含要加载的图像。</param>
        /// <param name="lpszName">要加载的图像。</param>
        /// <param name="type">要加载的图像的类型。</param>
        /// <param name="cx">图标或光标的宽度（以像素为单位）。</param>
        /// <param name="cy">图标或光标的高度（以像素为单位）。</param>
        /// <param name="fuLoad">此参数可使用以下一个或多个值。</param>
        /// <returns>如果函数成功，则返回值是新加载的图像的句柄。如果函数失败，则返回值为 NULL。</returns>
        [DllImport(User32, CharSet = CharSet.Ansi, EntryPoint = "LoadImage", SetLastError = true)]
        public static extern IntPtr LoadImage(IntPtr hinst, string lpszName, ImageType type, int cx, int cy, LoadImageFlags fuLoad);

        /// <summary>
        /// 注册一个窗口类，以便在对 CreateWindow 或 <see cref="CreateWindowEx"> 函数的调用中随后使用。
        /// </summary>
        /// <param name="lpWndClass">指向 <see cref="WindowClass"> 结构的指针。 在将结构传递给函数之前，必须用相应的类属性填充结构。</param>
        /// <return>如果函数成功，则返回值是唯一标识所注册类的类原子。 如果函数失败，则返回值为零。</return>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "RegisterClassW", SetLastError = true)]
        public static extern short RegisterClass(ref WindowClass lpWndClass);

        /// <summary>
        /// 定义保证在整个系统中唯一的新窗口消息。 发送或发布消息时可以使用消息值。
        /// </summary>
        /// <param name="lpString">要注册的消息。</param>
        /// <returns>如果成功注册消息，则返回值是范围0xC000到0xFFFF的消息标识符。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "RegisterWindowMessageW", SetLastError = false)]
        public static extern uint RegisterWindowMessage([MarshalAs(UnmanagedType.LPWStr)] string lpString);

        /// <summary>
        /// 将创建指定窗口的线程引入前台并激活窗口。 键盘输入将定向到窗口，并为用户更改各种视觉提示。 系统向创建前台窗口的线程分配略高于其他线程的优先级。
        /// </summary>
        /// <param name="hWnd">应激活并带到前台的窗口的句柄。</param>
        /// <returns>如果窗口被带到前台，则返回值为非零。如果未将窗口带到前台，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Ansi, EntryPoint = "SetForegroundWindow", SetLastError = false)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// 将指定的消息发送到窗口或窗口。 <see cref="SendMessage"> 函数调用指定窗口的窗口过程，在窗口过程处理消息之前不会返回。
        /// </summary>
        /// <param name="hWnd">
        /// 窗口过程的句柄将接收消息。 如果此参数 HWND_BROADCAST ( (HWND) 0xffff) ，则会将消息发送到系统中的所有顶级窗口，
        /// 包括已禁用或不可见的未所有者窗口、重叠窗口和弹出窗口;但消息不会发送到子窗口。消息发送受 UIPI 的约束。
        /// 进程的线程只能将消息发送到较低或等于完整性级别的线程的消息队列。
        /// </param>
        /// <param name="wMsg">要发送的消息。</param>
        /// <param name="wParam">其他的消息特定信息。</param>
        /// <param name="lParam">其他的消息特定信息。</param>
        /// <returns>返回值指定消息处理的结果;这取决于发送的消息。</returns>
        [DllImport(User32, CharSet = CharSet.Ansi, EntryPoint = "SendMessage", SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, WindowMessage wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport(User32, CharSet = CharSet.Ansi, EntryPoint = "SendMessage", SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, WindowMessage wMsg, int wParam, IntPtr lParam);

        /// <summary>
        /// 更改指定窗口的属性。 该函数还将指定偏移量处的32位（long类型）值设置到额外的窗口内存中。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，间接地是窗口所属的类</param>
        /// <param name="nIndex">要设置的值的从零开始的偏移量。 有效值的范围为零到额外窗口内存的字节数，减去整数的大小。</param>
        /// <param name="newProc">新事件处理函数（回调函数）</param>
        /// <returns>如果函数成功，则返回值是指定 32 位整数的上一个值。如果函数失败，则返回值为零。 </returns>
        [DllImport(User32, CharSet = CharSet.Ansi, EntryPoint = "SetWindowLongA", SetLastError = false)]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, WindowLongIndexFlags nIndex, WindowProc newProc);

        /// <summary>
        /// 更改指定窗口的属性。 该函数还将指定偏移量处的64位（long类型）值设置到额外的窗口内存中。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，间接地是窗口所属的类</param>
        /// <param name="nIndex">要设置的值的从零开始的偏移量。 有效值的范围为零到额外窗口内存的字节数，减去整数的大小。</param>
        /// <param name="newProc">新事件处理函数（回调函数）</param>
        /// <returns>如果函数成功，则返回值是指定偏移量的上一个值。如果函数失败，则返回值为零。 </returns>
        [DllImport(User32, CharSet = CharSet.Ansi, EntryPoint = "SetWindowLongPtrA", SetLastError = false)]
        public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, WindowLongIndexFlags nIndex, WindowProc newProc);

        /// <summary>
        /// 设置指定窗口的显示状态。
        /// </summary>
        /// <param name="hWnd">窗口的句柄。</param>
        /// <param name="nCmdShow">
        /// 控制窗口的显示方式。 如果启动应用程序的程序提供 <see cref="STARTUPINFO"> 结构，则首次调用 <see cref="ShowWindow"> 时忽略此参数。 否则，首次调用 <see cref="ShowWindow"> 时，该值应该是 WinMain 函数在其 nCmdShow 参数中获取的值。
        /// </param>
        /// <returns>如果窗口以前可见，则返回值为非零。如果窗口之前已隐藏，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "ShowWindow", ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow([In] IntPtr hWnd, WindowShowStyle nCmdShow);

        /// <summary>
        /// 将虚拟密钥消息转换为字符消息。 字符消息将发布到调用线程的消息队列，下次线程调用 <see cref="GetMessage"> 或 PeekMessage 函数时要读取。
        /// </summary>
        /// <param name="lpMsg">
        /// 指向 MSG 结构的指针，该结构包含使用 <see cref="GetMessage"> 或 PeekMessage 函数从调用线程的消息队列中检索到的消息信息。
        /// </param>
        /// <returns>
        /// 如果消息转换，则会将字符消息发布到线程的消息队列) ，则返回值为非零。
        /// 如果消息是 <see cref="WindowMessage.WM_KEYDOWN">、 <see cref="WindowMessage.WM_KEYUP">、 <see cref="WindowMessage.WM_SYSKEYDOWN"> 或 <see cref="WindowMessage.WM_SYSKEYUP">，则无论翻译如何，返回值都是非零的。
        /// </returns>
        [DllImport(User32, CharSet = CharSet.Ansi, EntryPoint = "TranslateMessage", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TranslateMessage(ref MSG lpMsg);

        /// <summary>
        /// <see cref="UpdateWindow"> 函数通过将 <see cref="WindowMessage.WM_PAINT"> 消息发送到窗口来更新指定窗口的工作区（如果窗口的更新区域不为空）。 该函数将 <see cref="WindowMessage.WM_PAINT"> 消息直接发送到指定窗口的窗口过程，绕过应用程序队列。 如果更新区域为空，则不会发送任何消息。
        /// </summary>
        /// <param name="hWnd">要更新的窗口的句柄。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "UpdateWindow", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UpdateWindow([In] IntPtr hWnd);
    }
}
