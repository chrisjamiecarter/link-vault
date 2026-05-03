namespace LinkVault.Core.Domain;

public abstract class Entity<TId>
    : IEquatable<Entity<TId>>
    where TId : notnull
{
    // default! required for ORM hydration; Id is set immediately after construction.
    public TId Id { get; protected set; } = default!;

    // default! required for ORM hydration; Sequence is set immediately after construction.
    public long Sequence { get; protected set; } = default!;

    // Required for ORM hydration.
    protected Entity() { } 

    protected Entity(TId id) => Id = id;

    public override bool Equals(object? obj) =>
        obj is Entity<TId> other && Equals(other);

    public bool Equals(Entity<TId>? other) =>
        other is not null
        && GetType() == other.GetType()
        && EqualityComparer<TId>.Default.Equals(Id, other.Id);

    public override int GetHashCode() =>
        EqualityComparer<TId>.Default.GetHashCode(Id);

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) =>
        Equals(left, right);

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) =>
        !Equals(left, right);
}
