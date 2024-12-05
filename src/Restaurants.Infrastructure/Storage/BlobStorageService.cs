using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using Restaurants.Domain.Interfaces;
using Restaurants.Infrastructure.Configuration;

namespace Restaurants.Infrastructure.Storage;

internal class BlobStorageService(IOptions<BlobStorageSettings> blobStorageSettingsOptions) : IBlobStorageService
{
    private readonly BlobStorageSettings _blobStorageSettings = blobStorageSettingsOptions.Value; 

    public async Task<string> UploadToBlobAsync(Stream data, string fileName)
    {
        //In order to get the Blob storage settings, you can refer to this parameter
        //Inside this method we need to connect to the azure blob service using the connection string



        //assign this client to a new object
        //This should actually create a connection from our code into the azure storage account.

       var blobServiceClient =  new BlobServiceClient(_blobStorageSettings.ConnectionString);


        //Get Blob container name

        var containerClient = blobServiceClient.GetBlobContainerClient(_blobStorageSettings.LogosContainerName);

        //To upload the file to a blob container we'll have to create the blob client object

        var blobClient = containerClient.GetBlobClient(fileName);


        await blobClient.UploadAsync(data);

        var blobUrl = blobClient.Uri.ToString();

        return blobUrl;



    }

    public string? GetBlobSasUrl(string? blobUrl)
    {
        if (blobUrl == null) return null;

        var sasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = _blobStorageSettings.LogosContainerName,

            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(30),
            BlobName = GetBlobNameFromUrl(blobUrl)

        };

        //set permission to allow readonly
        sasBuilder.SetPermissions(BlobAccountSasPermissions.Read);

        //In order to generate the SAS token
        //we have to specify the shared key credential object

        var blobServiceClient = new BlobServiceClient(_blobStorageSettings.ConnectionString);


        var sasToken = sasBuilder
                        .ToSasQueryParameters(new Azure.Storage.StorageSharedKeyCredential(blobServiceClient.AccountName, _blobStorageSettings.AccountKey))
                        .ToString();

        return $"{blobUrl}?{sasToken}";

        //blob: https://restaurantsadev.blob.core.windows.net/logos/logos-public.png

        // sas sp=r&st=2024-12-04T15:34:39Z&se=2024-12-04T23:34:39Z&spr=https&sv=2022-11-02&sr=b&sig=4vPuvCgxcwmlgg%2B3rniTMDxBtlXZTkuG%2B36THI1xM78%3D

        //we have to combine both the blob URL with the sas token as the SAS URL

    }

    private string GetBlobNameFromUrl(string bloburl)
    {
        var uri = new Uri(bloburl);

        return uri.Segments.Last();
    }
}
