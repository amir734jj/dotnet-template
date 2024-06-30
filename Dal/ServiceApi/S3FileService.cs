using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Dal.Configs;
using Dal.Interfaces;
using Microsoft.Extensions.Logging;
using Models.ViewModels.S3;

namespace Dal.ServiceApi;

public class S3FileService(
    ILogger<S3FileService> logger,
    IAmazonS3 client,
    S3ServiceConfig s3ServiceConfig) : IFileService
{
    public async Task<GenericFileServiceResponse> Upload(
        string fileKey,
        string fileName,
        string contentType,
        Stream data,
        IDictionary<string, string> metadata)
    {
        try
        {
            if (await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(client, s3ServiceConfig.BucketName))
            {
                var fileTransferUtility = new TransferUtility(client);

                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    Key = $"{s3ServiceConfig.Prefix}/{fileKey}",
                    InputStream = data,
                    BucketName = s3ServiceConfig.BucketName,
                    CannedACL = S3CannedACL.PublicRead
                };

                foreach (var (key, value) in metadata)
                {
                    fileTransferUtilityRequest.Metadata.Add(key, value);
                }

                metadata["Name"] = fileName;
                metadata["Content-Type"] = contentType;

                await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);

                return new GenericFileServiceResponse(HttpStatusCode.OK, "Successfully uploaded to S3");
            }

            // Bucket not found
            throw new Exception($"Bucket: {s3ServiceConfig.BucketName} does not exist");
        }
        // Catch specific amazon errors
        catch (AmazonS3Exception e)
        {
            logger.LogError(e, "Failed uploading to S3 with S3 specific exception");
                
            return new GenericFileServiceResponse(e.StatusCode, e.Message);
        }
        // Catch other errors
        catch (Exception e)
        {
            logger.LogError(e, "Failed uploading to S3 with generic exception");
                
            return new GenericFileServiceResponse(HttpStatusCode.BadRequest, e.Message);
        }
    }

    public async Task<DownloadFileServiceResponse> Download(string keyName)
    {
        try
        {
            // Build the request with the bucket name and the keyName (name of the file)
            var request = new GetObjectRequest
            {
                BucketName = s3ServiceConfig.BucketName,
                Key = $"{s3ServiceConfig.Prefix}/{keyName}"
            };

            using var response = await client.GetObjectAsync(request);
            await using var responseStream = response.ResponseStream;
            await using var memoryStream = new MemoryStream();
            var metadata = response.Metadata.Keys.ToDictionary(x => x, x => response.Metadata[x]);

            // Copy stream to another stream
            await responseStream.CopyToAsync(memoryStream);

            var fileName = response.Headers["Name"];
            var contentType = response.Headers["Content-Type"];

            return new DownloadFileServiceResponse(
                HttpStatusCode.OK,
                "Successfully downloaded S3 object",
                memoryStream, metadata, contentType, fileName);
        }
        // Catch specific amazon errors
        catch (AmazonS3Exception e)
        {
            logger.LogError(e, "Failed uploading from S3 with S3 specific exception");
                
            return new DownloadFileServiceResponse(e.StatusCode, e.Message);
        }
        // Catch other errors
        catch (Exception e)
        {
            logger.LogError(e, "Failed downloading from S3 with generic exception");
                
            return new DownloadFileServiceResponse(HttpStatusCode.BadRequest, e.Message);
        }
    }

    public async Task<GenericFileServiceResponse> Delete(string keyName)
    {
        try
        {
            // Build the request with the bucket name and the keyName (name of the file)
            var request = new DeleteObjectRequest
            {
                BucketName = s3ServiceConfig.BucketName,
                Key = $"{s3ServiceConfig.Prefix}/{keyName}"
            };

            var response = await client.DeleteObjectAsync(request);
            return new GenericFileServiceResponse(
                response.HttpStatusCode,
                $"Deleting S3 object with key: {keyName}");
        }
        // Catch specific amazon errors
        catch (AmazonS3Exception e)
        {
            logger.LogError(e, "Failed uploading from S3 with S3 specific exception");
                
            return new GenericFileServiceResponse(e.StatusCode, e.Message);
        }
        // Catch other errors
        catch (Exception e)
        {
            logger.LogError(e, "Failed downloading from S3 with generic exception");
                
            return new GenericFileServiceResponse(HttpStatusCode.BadRequest, e.Message);
        }
    }

    public async Task<List<string>> List()
    {
        var request = new ListObjectsV2Request
        {
            BucketName = s3ServiceConfig.BucketName,
            Prefix = s3ServiceConfig.Prefix
        };

        var result = await client.ListObjectsV2Async(request);

        return result.S3Objects?.Select(x => x.Key).ToList();
    }
}