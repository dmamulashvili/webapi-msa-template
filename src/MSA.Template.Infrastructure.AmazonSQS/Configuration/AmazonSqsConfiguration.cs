namespace MSA.Template.Infrastructure.AmazonSQS.Configuration;

public class AmazonSqsConfiguration
{
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    public string? RegionEndpointSystemName { get; set; }
    public string? QueueName { get; set; }
}