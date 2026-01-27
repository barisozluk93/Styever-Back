using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Entity
{
    public class UserVoucher
    {
        public long Id { get; set; }

        public long PlanId { get; set; }

        public long? UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        public double Price { get; set; }

        public string? SenderEmail {  get; set; }
        public string ReceiverEmail { get; set; }
        public string Message { get; set; }
        public Guid? Voucher { get; set; }

        public DateTime Date { get; set; }

        public bool IsDeleted { get; set; }
    }
}
