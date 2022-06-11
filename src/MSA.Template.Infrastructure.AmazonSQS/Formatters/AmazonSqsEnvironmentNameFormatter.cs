using MassTransit;

namespace MSA.Template.Infrastructure.AmazonSQS.Formatters;

public class AmazonSqsEnvironmentNameFormatter : IEntityNameFormatter
{
    private readonly IEntityNameFormatter _original;
    private readonly string _environmentName;

    public AmazonSqsEnvironmentNameFormatter(IEntityNameFormatter original, string environmentName)
    {
        _original = original;
        _environmentName = environmentName;
    }

    public string FormatEntityName<T>()
    {
        return $"{_original.FormatEntityName<T>()}_{_environmentName}";
    }
}