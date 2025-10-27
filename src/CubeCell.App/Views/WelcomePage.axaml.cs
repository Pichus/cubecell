using System.Collections.Generic;
using System.Reactive.Linq;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;

using CubeCell.App.ViewModels.Pages;

using ReactiveUI;

namespace CubeCell.App.Views;

public partial class WelcomePage : ReactiveUserControl<WelcomePageViewModel>
{
    public WelcomePage()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }

    private async void OpenFileButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel is null)
        {
            return;
        }

        IReadOnlyList<IStorageFile> files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open cubecell spreadsheet",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Cubecell Spreadsheet")
                {
                    Patterns = ["*.cubecell.xlsx"],
                    AppleUniformTypeIdentifiers = ["org.openxmlformats.spreadsheetml.sheet"],
                    MimeTypes = ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"]
                }
            ]
        });

        if (files.Count == 1 && ViewModel is not null)
        {
            IStorageFile file = files[0];
            await ViewModel.OpenLocalFileCommand.Execute(file.Path.LocalPath);
        }
    }
}
