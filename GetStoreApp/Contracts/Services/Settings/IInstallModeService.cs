﻿using GetStoreApp.Models.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Settings
{
    public interface IInstallModeService
    {
        InstallModeModel DefaultInstallMode { get; set; }

        InstallModeModel InstallMode { get; set; }

        List<InstallModeModel> InstallModeList { get; set; }

        Task InitializeInstallModeAsync();

        Task SetInstallModeAsync(InstallModeModel installMode);
    }
}
