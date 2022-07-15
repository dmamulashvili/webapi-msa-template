using System;
using MediatR;
using System.Text.Json.Serialization;

namespace MSA.Template.SharedKernel;

public abstract class BaseCommand<TResponse> : IRequest<TResponse>
{
    [JsonIgnore]
    public Guid CorrelationId { get; set; }
}