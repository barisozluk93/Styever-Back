using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArticleManagement.Entity
{
    public class Article
    {
        public long Id { get; set; }
        public long FileId { get; set; }

        public string Header { get; set; }
        public string HeaderEn { get; set; }
        public string SubHeader { get; set; }

        public string SubHeaderEn { get; set; }
        public string Content { get; set; }

        public string ContentEn { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public Model.File? File { get; set; }




    }
}
