using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemoryManagement.Entity
{
    public class Memory
    {
        public long Id { get; set; }
        public long CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public DateOnly BirthDate { get; set; }
        public DateOnly DeathDate { get; set; }
        public DateTime PostDate { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsLinkOnly { get; set; }

        public bool IsOpenToComment { get; set; }
        public bool IsDeleted { get; set; }
        public bool BelongingToOldPackage { get; set; }

        [NotMapped]
        public List<MemoryCandle>? Candles { get; set; }

        [NotMapped]
        public long? CandlesCount { get; set; }

        [NotMapped]
        public List<MemoryComment>? Comments { get; set; }

        [NotMapped]
        public long? CommentsCount { get; set; }

        [NotMapped]
        public List<MemoryLike>? Likes { get; set; }

        [NotMapped]
        public long? LikesCount { get; set; }

        [NotMapped]
        public List<MemoryFile>? Files { get; set; }

        [NotMapped]
        public List<MemoryYoutubeLink>? YoutubeLinks { get; set; }

        [NotMapped]
        public string? UserName { get; set; }

        [NotMapped]
        public Model.File? UserAvatar { get; set; }
        [NotMapped]
        public string? UserCityCountry { get; set; }

        [NotMapped]
        public bool HasDonation { get; set; }


    }
}
