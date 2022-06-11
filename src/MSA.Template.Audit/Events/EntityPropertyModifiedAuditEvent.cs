namespace MSA.Template.Audit.Events;

public class EntityPropertyModifiedAuditEvent : BaseAuditEvent
{
    public EntityPropertyModifiedAuditEvent(Guid correlationId, 
        string entityName, 
        string entityId, 
        string propertyName,
        string? propertyOriginalValue, 
        string? propertyCurrentValue, 
        Guid initiatorId, 
        DateTimeOffset creationDate)
    {
        CorrelationId = correlationId;
        EntityName = entityName;
        EntityId = entityId;
        PropertyName = propertyName;
        PropertyOriginalValue = propertyOriginalValue;
        PropertyCurrentValue = propertyCurrentValue;
        InitiatorId = initiatorId;
        CreationDate = creationDate;
    }

    public string EntityName { get; }
    public string EntityId { get; }
    public string PropertyName { get; }
    public string? PropertyOriginalValue { get; }
    public string? PropertyCurrentValue { get; }
}