using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.WinGet;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Pages;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.System;

namespace GetStoreApp.UI.Controls.WinGet
{
    /// <summary>
    /// WinGet 程序包页面：搜索应用控件
    /// </summary>
    public sealed partial class SearchAppsControl : Grid, INotifyPropertyChanged
    {
        private readonly object SearchAppsLock = new object();

        private bool isInitialized = false;

        private string cachedSearchText;

        private AutoResetEvent autoResetEvent;
        private PackageManager SearchAppsManager;
        private WinGetPage WinGetInstance;

        private bool _notSearched = true;

        public bool NotSearched
        {
            get { return _notSearched; }

            set
            {
                _notSearched = value;
                OnPropertyChanged();
            }
        }

        private bool _isSearchCompleted = false;

        public bool IsSearchCompleted
        {
            get { return _isSearchCompleted; }

            set
            {
                _isSearchCompleted = value;
                OnPropertyChanged();
            }
        }

        private string _searchText = string.Empty;

        public string SearchText
        {
            get { return _searchText; }

            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }

        private List<MatchResult> MatchResultList;

        private ObservableCollection<SearchAppsModel> SearchAppsCollection { get; } = new ObservableCollection<SearchAppsModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public SearchAppsControl()
        {
            InitializeComponent();
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 复制安装命令
        /// </summary>
        private void OnCopyInstallTextExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            string appId = args.Parameter as string;
            if (appId is not null)
            {
                string copyContent = string.Format("winget install {0}", appId);
                CopyPasteHelper.CopyTextToClipBoard(copyContent);

                TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.WinGetSearchInstall));
            }
        }

        /// <summary>
        /// 安装应用
        /// </summary>
        private void OnInstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            SearchAppsModel searchApps = args.Parameter as SearchAppsModel;
            if (searchApps is not null)
            {
                Task.Run(async () =>
                {
                    AutoResetEvent autoResetEvent = new AutoResetEvent(false);
                    try
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            lock (SearchAppsLock)
                            {
                                // 禁用当前应用的可安装状态
                                foreach (SearchAppsModel searchAppsItem in SearchAppsCollection)
                                {
                                    if (searchAppsItem.AppID == searchApps.AppID)
                                    {
                                        searchAppsItem.IsInstalling = true;
                                        break;
                                    }
                                }
                            }
                        });

                        InstallOptions installOptions = WinGetService.CreateInstallOptions();

                        installOptions.PackageInstallMode = (PackageInstallMode)Enum.Parse(typeof(PackageInstallMode), WinGetConfigService.WinGetInstallMode.Value.ToString());
                        installOptions.PackageInstallScope = PackageInstallScope.Any;

                        // 更新安装进度
                        Progress<InstallProgress> progressCallBack = new Progress<InstallProgress>((installProgress) =>
                        {
                            switch (installProgress.State)
                            {
                                // 处于等待中状态
                                case PackageInstallProgressState.Queued:
                                    {
                                        DispatcherQueue.TryEnqueue(() =>
                                        {
                                            lock (WinGetInstance.InstallingAppsObject)
                                            {
                                                foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsCollection)
                                                {
                                                    if (installingItem.AppID == searchApps.AppID)
                                                    {
                                                        installingItem.InstallProgressState = PackageInstallProgressState.Queued;
                                                        break;
                                                    }
                                                }
                                            }
                                        });
                                        break;
                                    }
                                // 处于下载中状态
                                case PackageInstallProgressState.Downloading:
                                    {
                                        DispatcherQueue.TryEnqueue(() =>
                                        {
                                            lock (WinGetInstance.InstallingAppsObject)
                                            {
                                                foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsCollection)
                                                {
                                                    if (installingItem.AppID == searchApps.AppID)
                                                    {
                                                        installingItem.InstallProgressState = PackageInstallProgressState.Downloading;
                                                        installingItem.DownloadProgress = Math.Round(installProgress.DownloadProgress * 100, 2); installingItem.DownloadedFileSize = Convert.ToString(FileSizeHelper.ConvertFileSizeToString(installProgress.BytesDownloaded));
                                                        installingItem.TotalFileSize = Convert.ToString(FileSizeHelper.ConvertFileSizeToString(installProgress.BytesRequired));
                                                        break;
                                                    }
                                                }
                                            }
                                        });

                                        break;
                                    }
                                // 处于安装中状态
                                case PackageInstallProgressState.Installing:
                                    {
                                        DispatcherQueue.TryEnqueue(() =>
                                        {
                                            lock (WinGetInstance.InstallingAppsObject)
                                            {
                                                foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsCollection)
                                                {
                                                    if (installingItem.AppID == searchApps.AppID)
                                                    {
                                                        installingItem.InstallProgressState = PackageInstallProgressState.Installing;
                                                        installingItem.DownloadProgress = 100;
                                                        break;
                                                    }
                                                }
                                            }
                                        });

                                        break;
                                    }
                                // 安装完成后等待其他操作状态
                                case PackageInstallProgressState.PostInstall:
                                    {
                                        DispatcherQueue.TryEnqueue(() =>
                                        {
                                            lock (WinGetInstance.InstallingAppsObject)
                                            {
                                                foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsCollection)
                                                {
                                                    if (installingItem.AppID == searchApps.AppID)
                                                    {
                                                        installingItem.InstallProgressState = PackageInstallProgressState.PostInstall;
                                                        break;
                                                    }
                                                }
                                            }
                                        });

                                        break;
                                    }
                                // 处于安装完成状态
                                case PackageInstallProgressState.Finished:
                                    {
                                        DispatcherQueue.TryEnqueue(() =>
                                        {
                                            lock (WinGetInstance.InstallingAppsObject)
                                            {
                                                foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsCollection)
                                                {
                                                    if (installingItem.AppID == searchApps.AppID)
                                                    {
                                                        installingItem.InstallProgressState = PackageInstallProgressState.Finished;
                                                        installingItem.DownloadProgress = 100;
                                                        break;
                                                    }
                                                }
                                            }
                                        });

                                        break;
                                    }
                            }
                        });

                        // 任务取消执行操作
                        CancellationTokenSource installTokenSource = new CancellationTokenSource();

                        // 添加任务
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            lock (WinGetInstance.InstallingAppsObject)
                            {
                                WinGetInstance.InstallingAppsCollection.Add(new InstallingAppsModel()
                                {
                                    AppID = searchApps.AppID,
                                    AppName = searchApps.AppName,
                                    DownloadProgress = 0,
                                    InstallProgressState = PackageInstallProgressState.Queued,
                                    DownloadedFileSize = FileSizeHelper.ConvertFileSizeToString(0),
                                    TotalFileSize = FileSizeHelper.ConvertFileSizeToString(0)
                                });
                            }
                        });

                        WinGetInstance.InstallingStateDict.Add(searchApps.AppID, installTokenSource);

                        InstallResult installResult = await SearchAppsManager.InstallPackageAsync(MatchResultList.Find(item => item.CatalogPackage.DefaultInstallVersion.Id == searchApps.AppID).CatalogPackage, installOptions).AsTask(installTokenSource.Token, progressCallBack);

                        // 获取安装完成后的结果信息
                        if (installResult.Status is InstallResultStatus.Ok)
                        {
                            ToastNotificationService.Show(NotificationKind.WinGetInstallSuccessfully, searchApps.AppName);

                            // 检测是否需要重启设备完成应用的卸载，如果是，询问用户是否需要重启设备
                            if (installResult.RebootRequired)
                            {
                                ContentDialogResult result = ContentDialogResult.None;
                                DispatcherQueue.TryEnqueue(async () =>
                                {
                                    result = await ContentDialogHelper.ShowAsync(new RebootDialog(WinGetOptionKind.UpgradeInstall, searchApps.AppName), this);
                                    autoResetEvent.Set();
                                });

                                autoResetEvent.WaitOne();
                                autoResetEvent.Dispose();

                                if (result is ContentDialogResult.Primary)
                                {
                                    Kernel32Library.GetStartupInfo(out STARTUPINFO shutdownStartupInfo);
                                    shutdownStartupInfo.lpReserved = IntPtr.Zero;
                                    shutdownStartupInfo.lpDesktop = IntPtr.Zero;
                                    shutdownStartupInfo.lpTitle = IntPtr.Zero;
                                    shutdownStartupInfo.dwX = 0;
                                    shutdownStartupInfo.dwY = 0;
                                    shutdownStartupInfo.dwXSize = 0;
                                    shutdownStartupInfo.dwYSize = 0;
                                    shutdownStartupInfo.dwXCountChars = 500;
                                    shutdownStartupInfo.dwYCountChars = 500;
                                    shutdownStartupInfo.dwFlags = STARTF.STARTF_USESHOWWINDOW;
                                    shutdownStartupInfo.wShowWindow = WindowShowStyle.SW_HIDE;
                                    shutdownStartupInfo.cbReserved2 = 0;
                                    shutdownStartupInfo.lpReserved2 = IntPtr.Zero;

                                    shutdownStartupInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));
                                    bool createResult = Kernel32Library.CreateProcess(null, string.Format("{0} {1}", Path.Combine(InfoHelper.SystemDataPath.Windows, "System32", "Shutdown.exe"), "-r -t 120"), IntPtr.Zero, IntPtr.Zero, false, CreateProcessFlags.CREATE_NO_WINDOW, IntPtr.Zero, null, ref shutdownStartupInfo, out PROCESS_INFORMATION shutdownInformation);

                                    if (createResult)
                                    {
                                        if (shutdownInformation.hProcess != IntPtr.Zero) Kernel32Library.CloseHandle(shutdownInformation.hProcess);
                                        if (shutdownInformation.hThread != IntPtr.Zero) Kernel32Library.CloseHandle(shutdownInformation.hThread);
                                    }
                                }
                            }
                        }
                        else
                        {
                            ToastNotificationService.Show(NotificationKind.WinGetInstallFailed, searchApps.AppName, searchApps.AppID);
                        }

                        DispatcherQueue.TryEnqueue(() =>
                        {
                            lock (SearchAppsLock)
                            {
                                // 应用安装失败，将当前任务状态修改为可安装状态
                                foreach (SearchAppsModel searchAppsItem in SearchAppsCollection)
                                {
                                    if (searchAppsItem.AppID == searchApps.AppID)
                                    {
                                        searchAppsItem.IsInstalling = false;
                                        break;
                                    }
                                }
                            }

                            // 完成任务后从任务管理中删除任务
                            lock (WinGetInstance.InstallingAppsObject)
                            {
                                foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsCollection)
                                {
                                    if (installingAppsItem.AppID == searchApps.AppID)
                                    {
                                        WinGetInstance.InstallingAppsCollection.Remove(installingAppsItem);
                                        break;
                                    }
                                }
                                WinGetInstance.InstallingStateDict.Remove(searchApps.AppID);
                            }
                        });
                    }
                    // 操作被用户所取消异常
                    catch (OperationCanceledException e)
                    {
                        LogService.WriteLog(LoggingLevel.Information, "App installing operation canceled.", e);

                        DispatcherQueue.TryEnqueue(() =>
                        {
                            lock (SearchAppsLock)
                            {
                                // 应用安装失败，将当前任务状态修改为可安装状态
                                foreach (SearchAppsModel searchAppsItem in SearchAppsCollection)
                                {
                                    if (searchAppsItem.AppID == searchApps.AppID)
                                    {
                                        searchAppsItem.IsInstalling = false;
                                        break;
                                    }
                                }
                            }

                            // 完成任务后从任务管理中删除任务
                            lock (WinGetInstance.InstallingAppsObject)
                            {
                                foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsCollection)
                                {
                                    if (installingAppsItem.AppID == searchApps.AppID)
                                    {
                                        WinGetInstance.InstallingAppsCollection.Remove(installingAppsItem);
                                        break;
                                    }
                                }
                                WinGetInstance.InstallingStateDict.Remove(searchApps.AppID);
                            }
                        });
                    }
                    // 其他异常
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "App installing failed.", e);

                        DispatcherQueue.TryEnqueue(() =>
                        {
                            lock (SearchAppsLock)
                            {
                                // 应用安装失败，将当前任务状态修改为可安装状态
                                foreach (SearchAppsModel searchAppsItem in SearchAppsCollection)
                                {
                                    if (searchAppsItem.AppID == searchApps.AppID)
                                    {
                                        searchAppsItem.IsInstalling = false;
                                        break;
                                    }
                                }
                            }

                            // 完成任务后从任务管理中删除任务
                            lock (WinGetInstance.InstallingAppsObject)
                            {
                                foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsCollection)
                                {
                                    if (installingAppsItem.AppID == searchApps.AppID)
                                    {
                                        WinGetInstance.InstallingAppsCollection.Remove(installingAppsItem);
                                        break;
                                    }
                                }
                                WinGetInstance.InstallingStateDict.Remove(searchApps.AppID);
                            }
                        });

                        ToastNotificationService.Show(NotificationKind.WinGetInstallFailed, searchApps.AppName, searchApps.AppID);
                    }
                });
            }
        }

        /// <summary>
        /// 使用命令安装
        /// </summary>
        private void OnInstallWithCmdExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            string appId = args.Parameter as string;
            if (appId is not null)
            {
                Task.Run(() =>
                {
                    Kernel32Library.GetStartupInfo(out STARTUPINFO wingetStartupInfo);
                    wingetStartupInfo.lpReserved = IntPtr.Zero;
                    wingetStartupInfo.lpDesktop = IntPtr.Zero;
                    wingetStartupInfo.lpTitle = IntPtr.Zero;
                    wingetStartupInfo.dwX = 0;
                    wingetStartupInfo.dwY = 0;
                    wingetStartupInfo.dwXSize = 0;
                    wingetStartupInfo.dwYSize = 0;
                    wingetStartupInfo.dwXCountChars = 500;
                    wingetStartupInfo.dwYCountChars = 500;
                    wingetStartupInfo.dwFlags = STARTF.STARTF_USESHOWWINDOW;
                    wingetStartupInfo.wShowWindow = WindowShowStyle.SW_SHOW;
                    wingetStartupInfo.cbReserved2 = 0;
                    wingetStartupInfo.lpReserved2 = IntPtr.Zero;
                    wingetStartupInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));

                    bool createResult = Kernel32Library.CreateProcess(null, string.Format("winget install {0}", appId), IntPtr.Zero, IntPtr.Zero, false, CreateProcessFlags.CREATE_NEW_CONSOLE, IntPtr.Zero, null, ref wingetStartupInfo, out PROCESS_INFORMATION wingetInformation);

                    if (createResult)
                    {
                        if (wingetInformation.hProcess != IntPtr.Zero) Kernel32Library.CloseHandle(wingetInformation.hProcess);
                        if (wingetInformation.hThread != IntPtr.Zero) Kernel32Library.CloseHandle(wingetInformation.hThread);
                    }
                });
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：搜索应用控件——挂载的事件

        /// <summary>
        /// 初始化搜索应用内容
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!isInitialized)
            {
                try
                {
                    SearchAppsManager = WinGetService.CreatePackageManager();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Search apps information initialized failed.", e);
                    return;
                }
                finally
                {
                    isInitialized = true;
                }
            }
        }

        /// <summary>
        /// 打开临时下载目录
        /// </summary>
        private async void OnOpenTempFolderClicked(object sender, RoutedEventArgs args)
        {
            string wingetTempPath = Path.Combine(Path.GetTempPath(), "WinGet");
            if (Directory.Exists(wingetTempPath))
            {
                await Launcher.LaunchFolderPathAsync(wingetTempPath);
            }
            else
            {
                await Launcher.LaunchFolderPathAsync(Path.GetTempPath());
            }
        }

        /// <summary>
        /// 更新已安装应用数据
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            MatchResultList = null;
            IsSearchCompleted = false;
            await Task.Delay(500);
            if (string.IsNullOrEmpty(cachedSearchText))
            {
                IsSearchCompleted = true;
                return;
            }
            GetSearchApps();
            InitializeData();
        }

        /// <summary>
        /// 根据输入的内容检索应用
        /// </summary>
        private async void OnQuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                cachedSearchText = SearchText;
                NotSearched = false;
                IsSearchCompleted = false;
                await Task.Delay(500);
                GetSearchApps();
                InitializeData();
            }
        }

        #endregion 第二部分：搜索应用控件——挂载的事件

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 本地化应用数量统计信息
        /// </summary>
        private string LocalizeSearchAppsCountInfo(int count)
        {
            if (count is 0)
            {
                return ResourceService.GetLocalized("WinGet/SearchedAppsCountEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("WinGet/SearchedAppsCountInfo"), count);
            }
        }

        private bool IsSearchBoxEnabled(bool notSearched, bool isSearchCompleted)
        {
            if (notSearched)
            {
                return true;
            }
            else
            {
                if (isSearchCompleted)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void InitializeWingetInstance(WinGetPage wingetInstance)
        {
            WinGetInstance = wingetInstance;
        }

        /// <summary>
        /// 搜索应用
        /// </summary>
        private void GetSearchApps()
        {
            try
            {
                autoResetEvent ??= new AutoResetEvent(false);
                Task.Run(async () =>
                {
                    List<PackageCatalogReference> packageCatalogReferences = SearchAppsManager.GetPackageCatalogs().ToList();
                    CreateCompositePackageCatalogOptions createCompositePackageCatalogOptions = WinGetService.CreateCreateCompositePackageCatalogOptions();
                    foreach (PackageCatalogReference catalogReference in packageCatalogReferences)
                    {
                        createCompositePackageCatalogOptions.Catalogs.Add(catalogReference);
                    }
                    PackageCatalogReference catalogRef = SearchAppsManager.CreateCompositePackageCatalog(createCompositePackageCatalogOptions);
                    ConnectResult connectResult = await catalogRef.ConnectAsync();
                    PackageCatalog searchCatalog = connectResult.PackageCatalog;

                    if (searchCatalog is not null)
                    {
                        FindPackagesOptions findPackagesOptions = WinGetService.CreateFindPackagesOptions();
                        PackageMatchFilter nameMatchFilter = WinGetService.CreatePacakgeMatchFilter();
                        // 根据应用的名称寻找符合条件的结果
                        nameMatchFilter.Field = PackageMatchField.Name;
                        nameMatchFilter.Option = PackageFieldMatchOption.ContainsCaseInsensitive;
                        nameMatchFilter.Value = cachedSearchText;
                        findPackagesOptions.Filters.Add(nameMatchFilter);
                        FindPackagesResult findResult = await connectResult.PackageCatalog.FindPackagesAsync(findPackagesOptions);
                        MatchResultList = findResult.Matches.ToList();
                    }
                    autoResetEvent?.Set();
                });
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Get search apps information failed.", e);
            }
        }

        /// <summary>
        /// 初始化列表数据
        /// </summary>
        private void InitializeData()
        {
            lock (SearchAppsLock)
            {
                SearchAppsCollection.Clear();
            }

            Task.Run(() =>
            {
                autoResetEvent?.WaitOne();
                autoResetEvent?.Dispose();
                autoResetEvent = null;

                if (MatchResultList is not null)
                {
                    List<SearchAppsModel> searchAppsList = new List<SearchAppsModel>();
                    foreach (MatchResult matchItem in MatchResultList)
                    {
                        if (matchItem.CatalogPackage.DefaultInstallVersion is not null)
                        {
                            bool isInstalling = false;
                            foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsCollection)
                            {
                                if (matchItem.CatalogPackage.DefaultInstallVersion.Id == installingAppsItem.AppID)
                                {
                                    isInstalling = true;
                                    break;
                                }
                            }
                            searchAppsList.Add(new SearchAppsModel()
                            {
                                AppID = matchItem.CatalogPackage.DefaultInstallVersion.Id,
                                AppName = string.IsNullOrEmpty(matchItem.CatalogPackage.DefaultInstallVersion.DisplayName) || matchItem.CatalogPackage.DefaultInstallVersion.DisplayName.Equals("Unknown", StringComparison.OrdinalIgnoreCase) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.DefaultInstallVersion.DisplayName,
                                AppPublisher = string.IsNullOrEmpty(matchItem.CatalogPackage.DefaultInstallVersion.Publisher) || matchItem.CatalogPackage.DefaultInstallVersion.Publisher.Equals("Unknown", StringComparison.OrdinalIgnoreCase) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.DefaultInstallVersion.Publisher,
                                AppVersion = string.IsNullOrEmpty(matchItem.CatalogPackage.DefaultInstallVersion.Version) || matchItem.CatalogPackage.DefaultInstallVersion.Version.Equals("Unknown", StringComparison.OrdinalIgnoreCase) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.DefaultInstallVersion.Version,
                                IsInstalling = isInstalling,
                            });
                        }
                    }

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        lock (SearchAppsLock)
                        {
                            foreach (SearchAppsModel searchAppsItem in searchAppsList)
                            {
                                SearchAppsCollection.Add(searchAppsItem);
                            }
                        }

                        IsSearchCompleted = true;
                    });
                }
                else
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        IsSearchCompleted = true;
                    });
                }
            });
        }
    }
}
