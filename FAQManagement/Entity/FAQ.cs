using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace FAQManagement.Entity
{
    public class FAQ
    {
        public long Id { get; set; }
        public string Header { get; set; }
        public string HeaderEn { get; set; }
        public string Content { get; set; }

        public string ContentEn { get; set; }
        public bool IsDeleted { get; set; }

    }
}
