using System;

using CubeCell.App.ViewModels;
using CubeCell.App.Views;

using ReactiveUI;

namespace CubeCell.App;

public class AppViewLocator : IViewLocator
{
    public IViewFor ResolveView<T>(T viewModel, string contract = null)
    {
        return viewModel switch
        {
            WelcomePageViewModel context => new WelcomePage { DataContext = context },
            AboutPageViewModel context => new AboutPage { DataContext = context },
            SpreadsheetEditorPageViewModel context => new SpreadsheetEditorPage { DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
        };
    }
}
