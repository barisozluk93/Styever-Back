using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemoryManagement.Entity
{
    public class MemoryComment
    {
        public long Id { get; set; }

        public string? Comment { get; set; }

        public long MemoryId { get; set; }

        [ForeignKey("MemoryId")]
        public Memory? Memory { get; set; }

        public long UserId { get; set; }

        public DateTime? Date { get; set; }

        public bool IsDeleted { get; set; }

        [NotMapped]
        public string? UserName { get; set; }

        [NotMapped]
        public Model.File? UserAvatar { get; set; }

    }
}
