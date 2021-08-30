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

namespace Dal.ServiceApi
{
    public class S3FileService : IFileService
    {
        private readonly IAmazonS3 _client;
        private readonly ILogger<S3FileService> _logger;
        private readonly S3ServiceConfig _s3ServiceConfig;

        /// <summary>
        /// Constructor that takes a S3Client and a prefix for all paths
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="client"></param>
        /// <param name="s3ServiceConfig"></param>
        public S3FileService(ILogger<S3FileService> logger, IAmazonS3 client, S3ServiceConfig s3ServiceConfig)
        {
            _logger = logger;
            _client = client;
            _s3ServiceConfig = s3ServiceConfig;
        }

        /// <summary>
        /// Upload a file to an S3, here four files are uploaded in four different ways
        /// </summary>
        /// <param name="fileKey"></param>
        /// <param name="fileName"></param>
        /// <param name="contentType"></param>
        /// <param name="data"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public async Task<GenericFileServiceResponse> Upload(string fileKey, string fileName, string contentType, Stream data, IDictionary<string, string> metadata)
        {
            try
            {
                if (await _client.DoesS3BucketExistAsync(_s3ServiceConfig.BucketName))
                {
                    var fileTransferUtility = new TransferUtility(_client);

                    var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                    {
                        Key = $"{_s3ServiceConfig.Prefix}/{fileKey}",
                        InputStream = data,
                        BucketName = _s3ServiceConfig.BucketName,
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
                throw new Exception($"Bucket: {_s3ServiceConfig.BucketName} does not exist");
            }
            // Catch specific amazon errors
            catch (AmazonS3Exception e)
            {
                _logger.LogError(e, "Failed uploading to S3 with S3 specific exception");
                
                return new GenericFileServiceResponse(e.StatusCode, e.Message);
            }
            // Catch other errors
            catch (Exception e)
            {
                _logger.LogError(e, "Failed uploading to S3 with generic exception");
                
                return new GenericFileServiceResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        /// <summary>
        /// Download S3 object
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public async Task<DownloadFileServiceResponse> Download(string keyName)
        {
            try
            {
                // Build the request with the bucket name and the keyName (name of the file)
                var request = new GetObjectRequest
                {
                    BucketName = _s3ServiceConfig.BucketName,
                    Key = $"{_s3ServiceConfig.Prefix}/{keyName}"
                };

                using var response = await _client.GetObjectAsync(request);
                await using var responseStream = response.ResponseStream;
                await using var memoryStream = new MemoryStream();
                var title = response.Metadata["x-amz-meta-title"];
                var metadata = response.Metadata.Keys.ToDictionary(x => x, x => response.Metadata[x]);

                // Copy stream to another stream
                await responseStream.CopyToAsync(memoryStream);

                var fileName = response.Headers["Name"];
                var contentType = response.Headers["Content-Type"];

                return new DownloadFileServiceResponse(HttpStatusCode.OK,
                    "Successfully downloaded S3 object",
                    memoryStream, metadata, contentType, fileName);
            }
            // Catch specific amazon errors
            catch (AmazonS3Exception e)
            {
                _logger.LogError(e, "Failed uploading from S3 with S3 specific exception");
                
                return new DownloadFileServiceResponse(e.StatusCode, e.Message);
            }
            // Catch other errors
            catch (Exception e)
            {
                _logger.LogError(e, "Failed downloading from S3 with generic exception");
                
                return new DownloadFileServiceResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        public async Task<List<string>> List()
        {
            var request = new ListObjectsV2Request
            {
                BucketName = _s3ServiceConfig.BucketName,
                Prefix = _s3ServiceConfig.Prefix
            };

            var result = await _client.ListObjectsV2Async(request);

            return result.S3Objects?.Select(x => x.Key).ToList();
        }
    }
}