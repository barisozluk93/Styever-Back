using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemoryManagement.Entity
{
    public class MemoryCandle
    {
        public long Id { get; set; }

        public long MemoryId { get; set; }

        [ForeignKey("MemoryId")]
        public Memory? Memory { get; set; }

        public long UserId { get; set; }

        public DateTime? Date { get; set; }
        public string? Shelter { get; set; }
        public double? Donation { get; set; }

        public bool IsDeleted { get; set; }

        [NotMapped]
        public string? UserName { get; set; }

        [NotMapped]
        public Model.File? UserAvatar { get; set; }
    }
}
