using System.Collections.Generic;
using System.Threading.Tasks;

using Google.Apis.Drive.v3.Data;

namespace CubeCell.App.Services;

public interface ICloudSpreadsheetPersistenceService
{
    public Task<IEnumerable<File>> GetSpreadsheetsStoredInCloudByUserAsync();
    public Task DownloadSpreadsheetFromCloud(string fileId, string savePath);
    public Task UploadSpreadsheetToCloud(string localPath, string fileName);
}
