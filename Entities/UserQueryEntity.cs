using LinqToDB.Mapping;

namespace EStore.Entities
{
    [Table(Name = nameof(UserQueryEntity))]
    public class UserQueryEntity
    {
        [PrimaryKey, Identity]
        public new int Id { get; set; }

        [Column(Name = "Name"), NotNull]
        public string Name { get; set; }

        [Column(Name = "Email"), NotNull]
        public string Email { get; set; }

        [Column(Name = "Phone"), Nullable]
        public string Phone { get; set; } 
        
        [Column(Name = "Category"), Nullable]
        public string Category { get; set; }

        [Column(Name = "Subject"), NotNull]
        public string Subject { get; set; }

        [Column(Name = "Message"), NotNull]
        public string Message { get; set; }

        [Column(Name = "Response"), Nullable]
        public string Response { get; set; }

        [Column(Name = "IsRead"), Nullable]
        public bool IsRead { get; set; }

        [Column(Name = "IsResolved"), Nullable]
        public bool IsResolved { get; set; }

        [Column(Name = "CreatedAt"), Nullable]
        public DateTime CreatedAt { get; set; }

    }
}
