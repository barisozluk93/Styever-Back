using System.ComponentModel.DataAnnotations.Schema;

namespace MemoryManagement.Model
{
        public class UserAddress
        {
        public long Id { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public string Address { get; set; }
        public string AddressHeader { get; set; }
        public long UserId { get; set; }

        public bool IsPrimary { get; set; }
        public bool IsDeleted { get; set; }
    }
}
