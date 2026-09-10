using StockFlow.Domain.Common;

namespace StockFlow.Domain.Entities
{
    public class Warehouse : Entity<Guid>
    {
        public string Name { get; private set; }
        public string Address { get; private set; }
        public bool IsActive { get; private set; }

        private Warehouse(Guid id, string name, string address, bool isActive)
            : base(id)
        {
            Name = name;
            Address = address;
            IsActive = isActive;
        }

        public static Warehouse Create(string name, string address)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Warehouse name cannot be empty.", nameof(name));
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Warehouse address cannot be empty.", nameof(address));
            }

            return new Warehouse(Guid.NewGuid(), name.Trim(), address.Trim(), isActive: true);
        }
    }
}