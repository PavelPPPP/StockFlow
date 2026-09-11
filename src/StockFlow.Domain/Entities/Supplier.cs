using StockFlow.Domain.Common;
using StockFlow.Domain.ValueObjects;

namespace StockFlow.Domain.Entities
{
    public class Supplier : Entity<Guid>
    {
        public string Name { get; private set; }
        public EmailValue ContactEmail { get; private set; }
        public string Phone {  get; private set; }

        private Supplier(Guid id, string name, EmailValue contactEmail, string phone)
            : base(id)
        {
            Name = name;
            ContactEmail  = contactEmail;
            Phone = phone;
        }

        public static Supplier Create(string name, EmailValue contactEmail, string phone)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Supplier name cannot be empty.", nameof(name));
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                throw new ArgumentException("Supplier phone cannot be empty.", nameof(phone));
            }

            return new Supplier(Guid.NewGuid(), name.Trim(), contactEmail, phone.Trim());
        }
    }
}