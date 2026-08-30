namespace StockFlow.Domain.Common
{
    public abstract class Entity<TId>
    {
        public TId Id { get; protected set; } = default!;

        protected Entity() { }
        
        protected Entity(TId id) => Id = id;
        
        public override bool Equals(object? obj)
        {
            if (obj is not Entity<TId> other) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id!.Equals(other.Id);
        }

        public override int GetHashCode() => Id!.GetHashCode();
    }
}