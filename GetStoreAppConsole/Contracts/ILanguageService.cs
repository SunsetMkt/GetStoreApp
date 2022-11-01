﻿namespace GetStoreAppConsole.Contracts
{
    public interface ILanguageService
    {
        string DefaultConsoleLanguage { get; }

        string ConsoleLanguage { get; }

        List<string> LanguageList { get; set; }

        Task InitializeLanguageAsync();
    }
}