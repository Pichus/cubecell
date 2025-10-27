using System.Collections.Generic;

using CubeCell.App.Models;
using CubeCell.App.Utils;

namespace CubeCell.App.ViewModels;

public class SpreadsheetViewModel
{
    private readonly Dictionary<CellCoordinates, CellViewModel> _cellViewModels = new();
    private readonly Spreadsheet _spreadsheetModel;

    public SpreadsheetViewModel(Spreadsheet spreadsheetModel)
    {
        _spreadsheetModel = spreadsheetModel;
    }

    public void SetCell(CellCoordinates coordinates, CellViewModel cell)
    {
        _cellViewModels[coordinates] = cell;
    }

    public CellViewModel? GetCell(int col, int row)
    {
        _cellViewModels.TryGetValue(new CellCoordinates(col, row), out CellViewModel? cell);

        return cell ?? null;
    }

    public CellViewModel? GetCell(string address)
    {
        (int col, int row) = CellAddressUtils.AddressToCoordinates(address);
        return GetCell(col, row) ?? null;
    }
}
