﻿using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Home;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Home
{
    public sealed partial class ResultControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public ResultViewModel ViewModel { get; } = IOCHelper.GetService<ResultViewModel>();

        public string Copy => ResourceService.GetLocalized("/Home/Copy");

        public string CopyOptionsToolTip => ResourceService.GetLocalized("/Home/CopyOptionsToolTip");

        public string CopyLink => ResourceService.GetLocalized("/Home/CopyLink");
        public string CopyLinkToolTip => ResourceService.GetLocalized("/Home/CopyLinkToolTip");

        public string CopyContent => ResourceService.GetLocalized("/Home/CopyContent");

        public string CopyContentToolTip => ResourceService.GetLocalized("/Home/CopyContentToolTip");

        public ResultControl()
        {
            InitializeComponent();
        }

        public string LocalizedCategoryId(string categoryId)
        {
            return string.Format(ResourceService.GetLocalized("/Home/categoryId"), categoryId);
        }

        public string LocalizedResultCountInfo(int count)
        {
            return string.Format(ResourceService.GetLocalized("/Home/ResultCountInfo"), count);
        }
    }
}
