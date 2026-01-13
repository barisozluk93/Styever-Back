using MemoryManagement.Model;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemoryManagement.Entity
{
    public class MemoryFile
    {
        public long Id { get; set; }

        public long MemoryId { get; set; }

        [ForeignKey("MemoryId")]
        public Memory? Memory { get; set; }

        public long FileId { get; set; }

        public bool IsPrimary { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public string? FileName { get; set; }


        [NotMapped]
        public Model.File? File { get; set; }
    }
}
