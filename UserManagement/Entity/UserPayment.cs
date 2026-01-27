using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Entity
{
    public class UserPayment
    {
        public long Id { get; set; }

        public long PlanId { get; set; }

        public long UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public double Price { get; set; }

        public DateTime PaymentDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
