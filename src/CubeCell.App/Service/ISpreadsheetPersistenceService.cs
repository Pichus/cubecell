namespace CubeCell.App.Service;

public interface ISpreadsheetPersistenceService
{
    public void CreateSpreadsheetAndSave(string filePath, string fileName);
    public void UpdateExistingSpreadsheetAndSave(string filepath);
}
