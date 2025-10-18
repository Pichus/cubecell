using Avalonia.ReactiveUI;
using CubeCell.ViewModels;

namespace CubeCell.Views;

public partial class About : ReactiveUserControl<AboutPageViewModel>
{
    public About()
    {
        InitializeComponent();
    }
}