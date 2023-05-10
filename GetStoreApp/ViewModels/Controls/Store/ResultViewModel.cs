﻿using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Controls.Settings.Experiment;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.UI.Notifications;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Windows.System;

namespace GetStoreApp.ViewModels.Controls.Store
{
    /// <summary>
    /// 微软商店页面：请求结果用户控件视图模型
    /// </summary>
    public sealed class ResultViewModel : ViewModelBase
    {
        public ObservableCollection<ResultModel> ResultDataList { get; } = new ObservableCollection<ResultModel>();

        private bool _resultCotnrolVisable = false;

        public bool ResultControlVisable
        {
            get { return _resultCotnrolVisable; }

            set
            {
                _resultCotnrolVisable = value;
                OnPropertyChanged();
            }
        }

        private string _categoryId;

        public string CategoryId
        {
            get { return _categoryId; }

            set
            {
                _categoryId = value;
                OnPropertyChanged();
            }
        }

        private bool _isSelectMode = false;

        public bool IsSelectMode
        {
            get { return _isSelectMode; }

            set
            {
                _isSelectMode = value;
                OnPropertyChanged();
            }
        }

        // 复制CategoryID
        public IRelayCommand CopyIDCommand => new RelayCommand(() =>
        {
            CopyPasteHelper.CopyToClipBoard(CategoryId);
            new ResultIDCopyNotification(true).Show();
        });

        // 进入多选模式
        public IRelayCommand SelectCommand => new RelayCommand(() =>
        {
            foreach (ResultModel resultItem in ResultDataList)
            {
                resultItem.IsSelected = false;
            }

            IsSelectMode = true;
        });

        // 全选
        public IRelayCommand SelectAllCommand => new RelayCommand(() =>
        {
            foreach (ResultModel resultItem in ResultDataList)
            {
                resultItem.IsSelected = true;
            }
        });

        // 全部不选
        public IRelayCommand SelectNoneCommand => new RelayCommand(() =>
        {
            foreach (ResultModel resultItem in ResultDataList)
            {
                resultItem.IsSelected = false;
            }
        });

        // 复制选定项目的内容
        public IRelayCommand CopySelectedCommand => new RelayCommand(async () =>
        {
            List<ResultModel> SelectedResultDataList = ResultDataList.Where(item => item.IsSelected is true).ToList();

            // 内容为空时显示空提示对话框
            if (SelectedResultDataList.Count is 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            };

            StringBuilder stringBuilder = new StringBuilder();

            SelectedResultDataList.ForEach(resultItem =>
            {
                stringBuilder.AppendLine(string.Format("[\n{0}\n{1}\n{2}\n{3}\n]",
                    resultItem.FileName,
                    resultItem.FileLink,
                    resultItem.FileSHA1,
                    resultItem.FileSize)
                    );
            });

            CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());

            new ResultContentCopyNotification(true, true, SelectedResultDataList.Count).Show();
        });

        // 复制选定项目的链接
        public IRelayCommand CopySelectedLinkCommand => new RelayCommand(async () =>
        {
            List<ResultModel> SelectedResultDataList = ResultDataList.Where(item => item.IsSelected is true).ToList();

            // 内容为空时显示空提示对话框
            if (SelectedResultDataList.Count is 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            };

            StringBuilder stringBuilder = new StringBuilder();

            SelectedResultDataList.ForEach(resultItem => { stringBuilder.AppendLine(string.Format("{0}", resultItem.FileLink)); });

            CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());

            new ResultLinkCopyNotification(true, true, SelectedResultDataList.Count).Show();
        });

        // 下载选定项目
        public IRelayCommand DownloadSelectedCommand => new RelayCommand(async () =>
        {
            // 查看是否开启了网络监控服务
            if (NetWorkMonitorService.NetWorkMonitorValue)
            {
                NetWorkStatus NetStatus = NetWorkHelper.GetNetWorkStatus();

                // 网络处于未连接状态，不再进行下载，显示通知
                if (NetStatus is NetWorkStatus.None || NetStatus is NetWorkStatus.Unknown)
                {
                    new NetWorkErrorNotification().Show();
                    return;
                }
            }

            List<ResultModel> SelectedResultDataList = ResultDataList.Where(item => item.IsSelected is true).ToList();

            // 内容为空时显示空提示对话框
            if (SelectedResultDataList.Count is 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            };

            // 使用应用内提供的下载方式
            if (DownloadOptionsService.DownloadMode.InternalName == DownloadOptionsService.DownloadModeList[0].InternalName)
            {
                List<BackgroundModel> duplicatedList = new List<BackgroundModel>();

                bool IsDownloadSuccessfully = false;

                SelectedResultDataList.ForEach(async resultItem =>
                {
                    string DownloadFilePath = string.Format("{0}\\{1}", DownloadOptionsService.DownloadFolder.Path, resultItem.FileName);

                    BackgroundModel backgroundItem = new BackgroundModel
                    {
                        DownloadKey = UniqueKeyHelper.GenerateDownloadKey(resultItem.FileName, DownloadFilePath),
                        FileName = resultItem.FileName,
                        FileLink = resultItem.FileLink,
                        FilePath = DownloadFilePath,
                        TotalSize = 0,
                        FileSHA1 = resultItem.FileSHA1,
                        DownloadFlag = 1
                    };

                    DuplicatedDataInfoArgs CheckResult = await DownloadXmlService.CheckDuplicatedAsync(backgroundItem.DownloadKey);

                    if (CheckResult is DuplicatedDataInfoArgs.None)
                    {
                        await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Add");
                        IsDownloadSuccessfully = true;
                    }
                    else
                    {
                        duplicatedList.Add(backgroundItem);
                    }
                });

                if (duplicatedList.Count > 0)
                {
                    ContentDialogResult result = await new DownloadNotifyDialog(DuplicatedDataInfoArgs.MultiRecord).ShowAsync();

                    if (result is ContentDialogResult.Primary)
                    {
                        foreach (BackgroundModel backgroundItem in duplicatedList)
                        {
                            try
                            {
                                if (File.Exists(backgroundItem.FilePath))
                                {
                                    File.Delete(backgroundItem.FilePath);
                                }
                            }
                            catch (Exception) { }
                            finally
                            {
                                await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Update");
                                IsDownloadSuccessfully = true;
                            }
                        }
                    }
                    else if (result is ContentDialogResult.Secondary)
                    {
                        NavigationService.NavigateTo(typeof(DownloadPage));
                    }
                }

                // 显示下载任务创建成功消息
                new DownloadCreateNotification(IsDownloadSuccessfully).Show();
            }

            // 使用浏览器下载
            else if (DownloadOptionsService.DownloadMode == DownloadOptionsService.DownloadModeList[1])
            {
                SelectedResultDataList.ForEach(async resultItem =>
                {
                    await Launcher.LaunchUriAsync(new Uri(resultItem.FileLink));
                });
            }
        });

        // 退出多选模式
        public IRelayCommand CancelCommand => new RelayCommand(() =>
        {
            IsSelectMode = false;
        });

        // 根据设置存储的文件链接操作方式操作获取到的文件链接
        public IRelayCommand DownloadCommand => new RelayCommand<ResultModel>(async (resultItem) =>
        {
            // 查看是否开启了网络监控服务
            if (NetWorkMonitorService.NetWorkMonitorValue)
            {
                NetWorkStatus NetStatus = NetWorkHelper.GetNetWorkStatus();

                // 网络处于未连接状态，不再进行下载，显示通知
                if (NetStatus is NetWorkStatus.None || NetStatus is NetWorkStatus.Unknown)
                {
                    new NetWorkErrorNotification().Show();
                    return;
                }
            }

            // 使用应用内提供的下载方式
            if (DownloadOptionsService.DownloadMode.InternalName == DownloadOptionsService.DownloadModeList[0].InternalName)
            {
                string DownloadFilePath = string.Format("{0}\\{1}", DownloadOptionsService.DownloadFolder.Path, resultItem.FileName);

                BackgroundModel backgroundItem = new BackgroundModel
                {
                    DownloadKey = UniqueKeyHelper.GenerateDownloadKey(resultItem.FileName, DownloadFilePath),
                    FileName = resultItem.FileName,
                    FileLink = resultItem.FileLink,
                    FilePath = DownloadFilePath,
                    TotalSize = 0,
                    FileSHA1 = resultItem.FileSHA1,
                    DownloadFlag = 1
                };

                // 检查是否存在相同的任务记录
                DuplicatedDataInfoArgs CheckResult = await DownloadXmlService.CheckDuplicatedAsync(backgroundItem.DownloadKey);

                switch (CheckResult)
                {
                    case DuplicatedDataInfoArgs.None:
                        {
                            bool AddResult = await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Add");
                            new DownloadCreateNotification(AddResult).Show();
                            break;
                        }

                    case DuplicatedDataInfoArgs.Unfinished:
                        {
                            ContentDialogResult result = await new DownloadNotifyDialog(DuplicatedDataInfoArgs.Unfinished).ShowAsync();

                            if (result is ContentDialogResult.Primary)
                            {
                                try
                                {
                                    if (File.Exists(backgroundItem.FilePath))
                                    {
                                        File.Delete(backgroundItem.FilePath);
                                    }
                                }
                                catch (Exception) { }
                                finally
                                {
                                    bool AddResult = await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Update");
                                    new DownloadCreateNotification(AddResult).Show();
                                }
                            }
                            else if (result is ContentDialogResult.Secondary)
                            {
                                NavigationService.NavigateTo(typeof(DownloadPage));
                            }
                            break;
                        }

                    case DuplicatedDataInfoArgs.Completed:
                        {
                            ContentDialogResult result = await new DownloadNotifyDialog(DuplicatedDataInfoArgs.Completed).ShowAsync();

                            if (result is ContentDialogResult.Primary)
                            {
                                try
                                {
                                    if (File.Exists(backgroundItem.FilePath))
                                    {
                                        File.Delete(backgroundItem.FilePath);
                                    }
                                }
                                catch (Exception) { }
                                finally
                                {
                                    bool AddResult = await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Update");
                                    new DownloadCreateNotification(AddResult).Show();
                                }
                            }
                            else if (result is ContentDialogResult.Secondary)
                            {
                                NavigationService.NavigateTo(typeof(DownloadPage));
                            }
                            break;
                        }
                }
            }

            // 使用浏览器下载
            else if (DownloadOptionsService.DownloadMode == DownloadOptionsService.DownloadModeList[1])
            {
                await Launcher.LaunchUriAsync(new Uri(resultItem.FileLink));
            }
        });

        // 右键单击打开链接
        public IRelayCommand OpenLinkCommand => new RelayCommand<ResultModel>(async (resultItem) =>
        {
            // 查看是否开启了网络监控服务
            if (NetWorkMonitorService.NetWorkMonitorValue)
            {
                NetWorkStatus NetStatus = NetWorkHelper.GetNetWorkStatus();

                // 网络处于未连接状态，不再进行下载，显示通知
                if (NetStatus is NetWorkStatus.None || NetStatus is NetWorkStatus.Unknown)
                {
                    new NetWorkErrorNotification().Show();
                    return;
                }
            }

            await Launcher.LaunchUriAsync(new Uri(resultItem.FileLink));
        });

        // 复制指定项目的链接
        public IRelayCommand CopyLinkCommand => new RelayCommand<string>((fileLink) =>
        {
            CopyPasteHelper.CopyToClipBoard(fileLink);

            new ResultLinkCopyNotification(true, false).Show();
        });

        // 复制指定项目的内容
        public IRelayCommand CopyContentCommand => new RelayCommand<ResultModel>((copyContent) =>
        {
            string CopyContent = string.Format("[\n{0}\n{1}\n{2}\n{3}\n]\n",
                copyContent.FileName,
                copyContent.FileLink,
                copyContent.FileSHA1,
                copyContent.FileSize
                );

            CopyPasteHelper.CopyToClipBoard(CopyContent);

            new ResultContentCopyNotification(true, false).Show();
        });

        public ResultViewModel()
        {
            Messenger.Default.Register<bool>(this, MessageToken.ResultControlVisable, (resultControlVisableMessage) =>
            {
                ResultControlVisable = resultControlVisableMessage;
            });

            Messenger.Default.Register<string>(this, MessageToken.ResultCategoryId, (resultCategoryIdMessage) =>
            {
                CategoryId = resultCategoryIdMessage;
            });

            Messenger.Default.Register<List<ResultModel>>(this, MessageToken.ResultDataList, (resultDataListMessage) =>
            {
                ResultDataList.Clear();

                foreach (ResultModel resultItem in resultDataListMessage)
                {
                    resultItem.IsSelected = false;
                    ResultDataList.Add(resultItem);
                }
            });

            Messenger.Default.Register<bool>(this, MessageToken.WindowClosed, (windowClosedMessage) =>
            {
                if (windowClosedMessage)
                {
                    Messenger.Default.Unregister(this);
                }
            });
        }

        /// <summary>
        /// 在多选模式下点击项目选择相应的条目
        /// </summary>
        public void OnItemClick(object sender, ItemClickEventArgs args)
        {
            ResultModel resultItem = (ResultModel)args.ClickedItem;
            int ClickedIndex = ResultDataList.IndexOf(resultItem);

            ResultDataList[ClickedIndex].IsSelected = !ResultDataList[ClickedIndex].IsSelected;
        }
    }
}