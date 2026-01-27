using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Entity
{
    public class User
    {
        public long Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
        
        public byte[]? Salt { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public bool IsDeleted { get; set; }
        public bool IsSystemData { get; set; }
        public long? FileId { get; set; }
        public bool IsTrial { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ExpirationDate { get; set; }

        public DateTime TrialExpirationDate { get; set; }

        [NotMapped]
        public Guid? Voucher { get; set; }

        [NotMapped]
        public UserAddress? UserAddress { get; set; }

        [NotMapped]
        public List<long> Roles { get; set; } = new List<long>();

        [NotMapped]
        public List<Permission> Permissions { get; set; } = new List<Permission>();

        [NotMapped]
        public Model.File? File { get; set; }
    }
}
