using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

using Microsoft.Extensions.Configuration;

using File = Google.Apis.Drive.v3.Data.File;

namespace CubeCell.App.Services;

public class GoogleDriveSpreadsheetPersistenceService : ICloudSpreadsheetPersistenceService
{
    private const string ApplicationName = "CubeCell";
    private static readonly string[] Scopes = [DriveService.Scope.DriveFile, DriveService.Scope.Drive];

    private readonly string _credentialsPath;
    private readonly string _tokenPath;

    private DriveService? _driveService;

    public GoogleDriveSpreadsheetPersistenceService()
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        _credentialsPath = config["credentialsPath"] ??
                           throw new ArgumentException("credentialsPath not set in appsettings.json");
        _tokenPath = config["tokenPath"] ?? throw new ArgumentException("tokenPath not set in appsettings.json");
    }

    public async Task<IEnumerable<File>> GetSpreadsheetsStoredInCloudByUserAsync()
    {
        DriveService service = await GetDriveServiceAsync();

        FilesResource.ListRequest? request = service.Files.List();
        request.Q =
            "name contains '.cubecell.xlsx'";
        request.Fields = "files(id, name, modifiedTime)";
        FileList? result = await request.ExecuteAsync();

        List<File> filtered = result.Files?
                                  .Where(f => f.Name.EndsWith(".cubecell.xlsx", StringComparison.OrdinalIgnoreCase))
                                  .ToList()
                              ?? [];

        return filtered;
    }

    public async Task DownloadSpreadsheetFromCloud(string fileId, string savePath)
    {
        DriveService service = await GetDriveServiceAsync();

        FilesResource.GetRequest? request = service.Files.Get(fileId);
        await using var stream = new FileStream(savePath, FileMode.Create, FileAccess.Write);
        await request.DownloadAsync(stream);
    }

    public async Task UploadSpreadsheetToCloud(string localPath, string fileName)
    {
        DriveService service = await GetDriveServiceAsync();

        var fileMetadata = new File
        {
            Name = fileName, MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        };

        await using var stream = new FileStream(localPath, FileMode.Open);
        FilesResource.CreateMediaUpload? request = service.Files.Create(fileMetadata, stream, fileMetadata.MimeType);
        request.Fields = "id, name, webViewLink";
        await request.UploadAsync();
    }

    private async Task<DriveService> GetDriveServiceAsync()
    {
        if (_driveService != null)
        {
            return _driveService;
        }

        await using var stream = new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read);
        UserCredential? credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            (await GoogleClientSecrets.FromStreamAsync(stream)).Secrets,
            Scopes,
            "user",
            CancellationToken.None,
            new FileDataStore(_tokenPath, true));


        _driveService = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential, ApplicationName = ApplicationName
        });

        return _driveService;
    }
}
