namespace JustPlatform.Domain;

public abstract class Entity<TId>
    where TId : IEquatable<TId>
{
    private int? _requestedHashCode;
    public virtual TId Id { get; protected set; }

    private List<INotification> _domainEvents = new();
    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(INotification eventItem)
    {
        _domainEvents ??= [];
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem) => _domainEvents.Remove(eventItem);

    public void ClearDomainEvents() => _domainEvents.Clear();

    public bool IsTransient() => this.Id.Equals(default);

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> item)
        {
            return false;
        }

        return ReferenceEquals(this, item)
            ? true
            : GetType() == item.GetType() && !item.IsTransient() && !IsTransient() && item.Id.Equals(Id);
    }

    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            if (!_requestedHashCode.HasValue)
            {
                _requestedHashCode = this.Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)
            }

            return _requestedHashCode.Value;
        }
        return base.GetHashCode();

    }
    public static bool operator ==(Entity<TId> left, Entity<TId> right) => Object.Equals(left, null) ? Object.Equals(right, null) : left.Equals(right);

    public static bool operator !=(Entity<TId> left, Entity<TId> right) => !(left == right);
}