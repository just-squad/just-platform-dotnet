namespace JustPlatform.DataAccess.Npgsql.Models;

public readonly struct Xid(uint value) : IEquatable<Xid>
{
    public uint Value { get; } = value;

    public static explicit operator uint(Xid xid) => xid.Value;
    public static explicit operator Xid(uint xid) => new(xid);

    public bool Equals(Xid other) => Value == other.Value;

    public override bool Equals(object? obj) => obj is Xid other && Equals(other);

    public override int GetHashCode() => (int)Value;

    public static bool operator ==(Xid left, Xid right) => left.Equals(right);

    public static bool operator !=(Xid left, Xid right) => !(left == right);
}