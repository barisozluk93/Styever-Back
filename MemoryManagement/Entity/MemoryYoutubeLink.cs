using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemoryManagement.Entity
{
    public class MemoryYoutubeLink
    {
        public long Id { get; set; }

        public long MemoryId { get; set; }

        [ForeignKey("MemoryId")]
        public Memory? Memory { get; set; }

        public string Link { get; set; }

        public bool IsDeleted { get; set; }
    }
}
