namespace Dal.Configs;

public class S3ServiceConfig(string bucketName, string prefix)
{
    public string BucketName { get; } = bucketName;

    public string Prefix { get; } = prefix;
}