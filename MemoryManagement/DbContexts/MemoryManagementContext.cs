using MemoryManagement.Entity;
using Microsoft.EntityFrameworkCore;


namespace MemoryManagement.DbContexts

{
    public class MemoryManagementContext : DbContext
    {
        public MemoryManagementContext(DbContextOptions<MemoryManagementContext> options) : base(options)
        {
           
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Memory> Memories { get; set; }
        public DbSet<MemoryComment> MemoryComments { get; set; }
        public DbSet<MemoryLike> MemoryLikes { get; set; }
        public DbSet<MemoryFile> MemoryFiles { get; set; }
        public DbSet<MemoryYoutubeLink> MemoryYoutubeLinks { get; set; }
        public DbSet<MemoryCandle> MemoryCandles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Kuş", EnName = "Bird", IsDeleted = false },
                new Category { Id = 2, Name = "Kedi", EnName = "Cat", IsDeleted = false },
                new Category { Id = 3, Name = "Köpek", EnName = "Dog", IsDeleted = false },
                new Category { Id = 4, Name = "Balık", EnName = "Fish", IsDeleted = false },
                new Category { Id = 5, Name = "Fare", EnName = "Hamster", IsDeleted = false },
                new Category { Id = 6, Name = "At", EnName = "Horse", IsDeleted = false },
                new Category { Id = 7, Name = "Kaplumbağa", EnName = "Turtle", IsDeleted = false }
            );

            //modelBuilder.Entity<Memory>().HasData(
            //    new Memory
            //    {
            //        Id = 1,
            //        Name = "Queenie",
            //        CategoryId = 2,
            //        BirthDate = new DateOnly(2023, 11, 12),
            //        DeathDate = new DateOnly(2024, 8, 28),
            //        IsDeleted = false,
            //        PostDate = DateTime.UtcNow,
            //        Text = "Our precious Queenie crossed the Rainbow Bridge at 13 years old, leaving behind gentle pawprints on our hearts. Graceful, kind and full of quiet love, she filled our home with warmth and calm. Her soft purrs, curious eyes and tender ways brought comfort to every day and joy to every corner. Though the house feels still without her, we see her everywhere :- in the sunlight on the floor, in the quiet of the evening, and in every happy memory she gave us. Sleep softly, dear Queenie. You’ll always be part of our hearts and our home. Forever loved and sadly missed by Bryan, Lisa and Sarah\r\n",
            //        UserId = 2,
            //        IsOpenToComment = true,
            //        IsPrivate = false,
            //    },
            //    new Memory
            //    {
            //        Id = 2,
            //        Name = "Ringo",
            //        CategoryId = 3,
            //        BirthDate = new DateOnly(2020, 4, 4),
            //        DeathDate = new DateOnly(2025, 5, 5),
            //        IsDeleted = false,
            //        PostDate = DateTime.UtcNow,
            //        Text = "Our precious Queenie crossed the Rainbow Bridge at 13 years old, leaving behind gentle pawprints on our hearts. Graceful, kind and full of quiet love, she filled our home with warmth and calm. Her soft purrs, curious eyes and tender ways brought comfort to every day and joy to every corner. Though the house feels still without her, we see her everywhere :- in the sunlight on the floor, in the quiet of the evening, and in every happy memory she gave us. Sleep softly, dear Queenie. You’ll always be part of our hearts and our home. Forever loved and sadly missed by Bryan, Lisa and Sarah\r\n",
            //        UserId = 3,
            //        IsOpenToComment = true,
            //        IsPrivate = false,
            //    },
            //    new Memory
            //    {
            //        Id = 3,
            //        Name = "Bubbles",
            //        CategoryId = 4,
            //        BirthDate = new DateOnly(2024, 1, 12),
            //        DeathDate = new DateOnly(2024, 8, 18),
            //        IsDeleted = false,
            //        PostDate = DateTime.UtcNow,
            //        Text = "Our precious Queenie crossed the Rainbow Bridge at 13 years old, leaving behind gentle pawprints on our hearts. Graceful, kind and full of quiet love, she filled our home with warmth and calm. Her soft purrs, curious eyes and tender ways brought comfort to every day and joy to every corner. Though the house feels still without her, we see her everywhere :- in the sunlight on the floor, in the quiet of the evening, and in every happy memory she gave us. Sleep softly, dear Queenie. You’ll always be part of our hearts and our home. Forever loved and sadly missed by Bryan, Lisa and Sarah\r\n",
            //        UserId = 4,
            //        IsOpenToComment = false,
            //        IsPrivate = false,
            //    },
            //    new Memory
            //    {
            //        Id = 4,
            //        Name = "George",
            //        CategoryId = 6,
            //        BirthDate = new DateOnly(2018, 2, 14),
            //        DeathDate = new DateOnly(2024, 11, 24),
            //        IsDeleted = false,
            //        PostDate = DateTime.UtcNow,
            //        Text = "Our precious Queenie crossed the Rainbow Bridge at 13 years old, leaving behind gentle pawprints on our hearts. Graceful, kind and full of quiet love, she filled our home with warmth and calm. Her soft purrs, curious eyes and tender ways brought comfort to every day and joy to every corner. Though the house feels still without her, we see her everywhere :- in the sunlight on the floor, in the quiet of the evening, and in every happy memory she gave us. Sleep softly, dear Queenie. You’ll always be part of our hearts and our home. Forever loved and sadly missed by Bryan, Lisa and Sarah\r\n",
            //        UserId = 4,
            //        IsOpenToComment = true,
            //        IsPrivate = true,
            //    },
            //    new Memory
            //    {
            //        Id = 5,
            //        Name = "Fredy",
            //        CategoryId = 6,
            //        BirthDate = new DateOnly(2018, 2, 14),
            //        DeathDate = new DateOnly(2024, 11, 24),
            //        IsDeleted = false,
            //        PostDate = DateTime.UtcNow,
            //        Text = "Our precious Queenie crossed the Rainbow Bridge at 13 years old, leaving behind gentle pawprints on our hearts. Graceful, kind and full of quiet love, she filled our home with warmth and calm. Her soft purrs, curious eyes and tender ways brought comfort to every day and joy to every corner. Though the house feels still without her, we see her everywhere :- in the sunlight on the floor, in the quiet of the evening, and in every happy memory she gave us. Sleep softly, dear Queenie. You’ll always be part of our hearts and our home. Forever loved and sadly missed by Bryan, Lisa and Sarah\r\n",
            //        UserId = 3,
            //        IsOpenToComment = true,
            //        IsPrivate = false,
            //    },
            //    new Memory
            //    {
            //        Id = 6,
            //        Name = "Hato",
            //        CategoryId = 6,
            //        BirthDate = new DateOnly(2018, 2, 14),
            //        DeathDate = new DateOnly(2024, 11, 24),
            //        IsDeleted = false,
            //        PostDate = DateTime.UtcNow,
            //        Text = "Our precious Queenie crossed the Rainbow Bridge at 13 years old, leaving behind gentle pawprints on our hearts. Graceful, kind and full of quiet love, she filled our home with warmth and calm. Her soft purrs, curious eyes and tender ways brought comfort to every day and joy to every corner. Though the house feels still without her, we see her everywhere :- in the sunlight on the floor, in the quiet of the evening, and in every happy memory she gave us. Sleep softly, dear Queenie. You’ll always be part of our hearts and our home. Forever loved and sadly missed by Bryan, Lisa and Sarah\r\n",
            //        UserId = 3,
            //        IsOpenToComment = true,
            //        IsPrivate = false,
            //    },
            //    new Memory
            //    {
            //        Id = 7,
            //        Name = "Aaaaa",
            //        CategoryId = 6,
            //        BirthDate = new DateOnly(2018, 2, 14),
            //        DeathDate = new DateOnly(2024, 11, 24),
            //        IsDeleted = false,
            //        PostDate = DateTime.UtcNow,
            //        Text = "Our precious Queenie crossed the Rainbow Bridge at 13 years old, leaving behind gentle pawprints on our hearts. Graceful, kind and full of quiet love, she filled our home with warmth and calm. Her soft purrs, curious eyes and tender ways brought comfort to every day and joy to every corner. Though the house feels still without her, we see her everywhere :- in the sunlight on the floor, in the quiet of the evening, and in every happy memory she gave us. Sleep softly, dear Queenie. You’ll always be part of our hearts and our home. Forever loved and sadly missed by Bryan, Lisa and Sarah\r\n",
            //        UserId = 3,
            //        IsOpenToComment = true,
            //        IsPrivate = true,
            //    },
            //    new Memory
            //    {
            //        Id = 8,
            //        Name = "Bbbbbb",
            //        CategoryId = 6,
            //        BirthDate = new DateOnly(2018, 2, 14),
            //        DeathDate = new DateOnly(2024, 11, 24),
            //        IsDeleted = false,
            //        PostDate = DateTime.UtcNow,
            //        Text = "Our precious Queenie crossed the Rainbow Bridge at 13 years old, leaving behind gentle pawprints on our hearts. Graceful, kind and full of quiet love, she filled our home with warmth and calm. Her soft purrs, curious eyes and tender ways brought comfort to every day and joy to every corner. Though the house feels still without her, we see her everywhere :- in the sunlight on the floor, in the quiet of the evening, and in every happy memory she gave us. Sleep softly, dear Queenie. You’ll always be part of our hearts and our home. Forever loved and sadly missed by Bryan, Lisa and Sarah\r\n",
            //        UserId = 3,
            //        IsOpenToComment = true,
            //        IsPrivate = false,
            //    },
            //    new Memory
            //    {
            //        Id = 9,
            //        Name = "Cccccc",
            //        CategoryId = 6,
            //        BirthDate = new DateOnly(2018, 2, 14),
            //        DeathDate = new DateOnly(2024, 11, 24),
            //        IsDeleted = false,
            //        PostDate = DateTime.UtcNow,
            //        Text = "Our precious Queenie crossed the Rainbow Bridge at 13 years old, leaving behind gentle pawprints on our hearts. Graceful, kind and full of quiet love, she filled our home with warmth and calm. Her soft purrs, curious eyes and tender ways brought comfort to every day and joy to every corner. Though the house feels still without her, we see her everywhere :- in the sunlight on the floor, in the quiet of the evening, and in every happy memory she gave us. Sleep softly, dear Queenie. You’ll always be part of our hearts and our home. Forever loved and sadly missed by Bryan, Lisa and Sarah\r\n",
            //        UserId = 2,
            //        IsOpenToComment = true,
            //        IsPrivate = false,
            //    },
            //    new Memory
            //    {
            //        Id = 10,
            //        Name = "Ddddddd",
            //        CategoryId = 6,
            //        BirthDate = new DateOnly(2018, 2, 14),
            //        DeathDate = new DateOnly(2024, 11, 24),
            //        IsDeleted = false,
            //        PostDate = DateTime.UtcNow,
            //        Text = "Our precious Queenie crossed the Rainbow Bridge at 13 years old, leaving behind gentle pawprints on our hearts. Graceful, kind and full of quiet love, she filled our home with warmth and calm. Her soft purrs, curious eyes and tender ways brought comfort to every day and joy to every corner. Though the house feels still without her, we see her everywhere :- in the sunlight on the floor, in the quiet of the evening, and in every happy memory she gave us. Sleep softly, dear Queenie. You’ll always be part of our hearts and our home. Forever loved and sadly missed by Bryan, Lisa and Sarah\r\n",
            //        UserId = 2,
            //        IsOpenToComment = true,
            //        IsPrivate = false,
            //    },
            //    new Memory
            //    {
            //        Id = 11,
            //        Name = "Eeeeeee",
            //        CategoryId = 6,
            //        BirthDate = new DateOnly(2018, 2, 14),
            //        DeathDate = new DateOnly(2024, 11, 24),
            //        IsDeleted = false,
            //        PostDate = DateTime.UtcNow,
            //        Text = "Our precious Queenie crossed the Rainbow Bridge at 13 years old, leaving behind gentle pawprints on our hearts. Graceful, kind and full of quiet love, she filled our home with warmth and calm. Her soft purrs, curious eyes and tender ways brought comfort to every day and joy to every corner. Though the house feels still without her, we see her everywhere :- in the sunlight on the floor, in the quiet of the evening, and in every happy memory she gave us. Sleep softly, dear Queenie. You’ll always be part of our hearts and our home. Forever loved and sadly missed by Bryan, Lisa and Sarah\r\n",
            //        UserId = 3,
            //        IsOpenToComment = true,
            //        IsPrivate = false,
            //    },
            //    new Memory
            //    {
            //        Id = 12,
            //        Name = "Ffffff",
            //        CategoryId = 6,
            //        BirthDate = new DateOnly(2018, 2, 14),
            //        DeathDate = new DateOnly(2024, 11, 24),
            //        IsDeleted = false,
            //        PostDate = DateTime.UtcNow,
            //        Text = "Our precious Queenie crossed the Rainbow Bridge at 13 years old, leaving behind gentle pawprints on our hearts. Graceful, kind and full of quiet love, she filled our home with warmth and calm. Her soft purrs, curious eyes and tender ways brought comfort to every day and joy to every corner. Though the house feels still without her, we see her everywhere :- in the sunlight on the floor, in the quiet of the evening, and in every happy memory she gave us. Sleep softly, dear Queenie. You’ll always be part of our hearts and our home. Forever loved and sadly missed by Bryan, Lisa and Sarah\r\n",
            //        UserId = 3,
            //        IsOpenToComment = true,
            //        IsPrivate = false,
            //    }
            //);

            //modelBuilder.Entity<MemoryFile>().HasData(
            //    new MemoryFile { Id = 1, FileId = 1, IsPrimary = true, MemoryId = 1 },
            //    new MemoryFile { Id = 2, FileId = 2, IsPrimary = true, MemoryId = 2 },
            //    new MemoryFile { Id = 3, FileId = 3, IsPrimary = true, MemoryId = 3 },
            //    new MemoryFile { Id = 4, FileId = 4, IsPrimary = true, MemoryId = 4 },
            //    new MemoryFile { Id = 5, FileId = 5, IsPrimary = false, MemoryId = 4 },
            //    new MemoryFile { Id = 6, FileId = 6, IsPrimary = false, MemoryId = 4 },
            //    new MemoryFile { Id = 7, FileId = 2, IsPrimary = true, MemoryId = 6 },
            //    new MemoryFile { Id = 8, FileId = 3, IsPrimary = true, MemoryId = 7 },
            //    new MemoryFile { Id = 9, FileId = 1, IsPrimary = true, MemoryId = 8 },
            //    new MemoryFile { Id = 10, FileId = 4, IsPrimary = true, MemoryId = 9 },
            //    new MemoryFile { Id = 11, FileId = 6, IsPrimary = true, MemoryId = 10 },
            //    new MemoryFile { Id = 12, FileId = 5, IsPrimary = true, MemoryId = 11 },
            //    new MemoryFile { Id = 13, FileId = 6, IsPrimary = true, MemoryId = 12 },
            //    new MemoryFile { Id = 14, FileId = 6, IsPrimary = true, MemoryId = 5 }
            //);

            //modelBuilder.Entity<MemoryComment>().HasData(
            //    new MemoryComment { Id = 1, Comment = "Bir bir yorumdur.", Date = DateTime.UtcNow, IsDeleted = false, MemoryId = 1, UserId = 3 },
            //    new MemoryComment { Id = 2, Comment = "Bir bir yorumdur. (2)", Date = DateTime.UtcNow, IsDeleted = false, MemoryId = 1, UserId = 1 },
            //    new MemoryComment { Id = 3, Comment = "Bir bir yorumdur. (3)", Date = DateTime.UtcNow, IsDeleted = false, MemoryId = 1, UserId = 4 },
            //    new MemoryComment { Id = 4, Comment = "Bir bir yorumdur. (4)", Date = DateTime.UtcNow, IsDeleted = false, MemoryId = 1, UserId = 1 }
            //);

            //modelBuilder.Entity<MemoryLike>().HasData(
            //    new MemoryLike { Id = 1, Date = DateTime.UtcNow, IsDeleted = false, MemoryId = 1, UserId = 1 },
            //    new MemoryLike { Id = 2, Date = DateTime.UtcNow, IsDeleted = false, MemoryId = 1, UserId = 3 },
            //    new MemoryLike { Id = 3, Date = DateTime.UtcNow, IsDeleted = false, MemoryId = 1, UserId = 4 }
            //);
        }
    }
}
