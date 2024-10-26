using System.Text.Json;
using Amazon.S3;
using Amazon.S3.Model;
using Application.Contracts;
using Domain.Entity;
using Serilog;

namespace Infrastructure.S3Bucket;

public class StockBucket : IStockBucket
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    public StockBucket(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
        _bucketName = "stocks-index";
    }

    public async Task AddStockEntry(Stock request)
    {
        Log.Information("Started processing {@Service} with request {@Request}", nameof(AddStockEntry), request);

        var s3Request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = $"{request.Name}/{request.CreatedAt.ToString("d MMM yyyy")}/{request.CreatedAt.ToString("T")}",
            ContentBody = JsonSerializer.Serialize(request),
            ContentType = "application/json",
        };

        try
        {
            await _s3Client.PutObjectAsync(s3Request);
        }
        catch (AmazonS3Exception ex)
        {
            Log.Error("Problem occurred when saving stock entry: {@Ex}", ex);
        }
    }
}