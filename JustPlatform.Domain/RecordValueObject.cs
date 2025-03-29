namespace JustPlatform.Domain;

public abstract record RecordValueObject
{
    protected static bool EqualOperator(ValueObject left, ValueObject right)
    {
        if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
        {
            return false;
        }
        return ReferenceEquals(left, null) || left.Equals(right);
    }

    protected static bool NotEqualOperator(ValueObject left, ValueObject right)
    {
        return !(EqualOperator(left, right));
    }

    protected abstract IEnumerable<object?> GetEqualityComponents();

    public virtual bool Equals(RecordValueObject? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;

        return this.GetEqualityComponents().SequenceEqual(obj.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }

    public RecordValueObject? GetCopy()
    {
        return this.MemberwiseClone() as RecordValueObject;
    }
}