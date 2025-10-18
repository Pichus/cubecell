using System;
using CubeCell.ViewModels;
using CubeCell.Views;
using ReactiveUI;

namespace CubeCell;

public class AppViewLocator : IViewLocator
{
    public IViewFor ResolveView<T>(T viewModel, string contract = null)
    {
        return viewModel switch
        {
            WelcomePageViewModel context => new Welcome { DataContext = context },
            AboutPageViewModel context => new About { DataContext = context },
            SpreadsheetEditorPageViewModel context => new SpreadsheetEditor { DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
        };
    }
}