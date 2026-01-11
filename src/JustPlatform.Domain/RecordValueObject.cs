namespace JustPlatform.Domain;

public abstract record RecordValueObject
{
    protected static bool EqualOperator(ValueObject left, ValueObject right) => !(left is null ^ right is null) && (left is null || left.Equals(right));

    protected static bool NotEqualOperator(ValueObject left, ValueObject right) => !(EqualOperator(left, right));

    protected abstract IEnumerable<object?> GetEqualityComponents();

    public virtual bool Equals(RecordValueObject? obj) => obj != null && obj.GetType() == GetType() && this.GetEqualityComponents().SequenceEqual(obj.GetEqualityComponents());

    public override int GetHashCode() => GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);

    public RecordValueObject? GetCopy() => this.MemberwiseClone() as RecordValueObject;
}