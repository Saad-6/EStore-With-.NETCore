namespace EStore.Models.Order
{
    public class Address : BaseEntity
    {
        public virtual string? FirstName { get; set; }
        public virtual string? LastName { get; set; }
        public virtual string? City { get; set; }
        public virtual string? ZipCode { get; set; }
        public virtual string? StreetAddress { get; set; }
        public virtual string? PhoneNumber { get; set; }
    }
}
