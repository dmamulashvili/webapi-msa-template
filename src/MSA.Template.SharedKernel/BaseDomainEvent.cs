using System;
using MediatR;

namespace MSA.Template.SharedKernel;

public class BaseDomainEvent : INotification
{
    public DateTimeOffset DateOccurred { get; protected set; } = DateTimeOffset.UtcNow;
}