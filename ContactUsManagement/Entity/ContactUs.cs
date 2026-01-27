using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContactUsManagement.Entity
{
    public class ContactUs
    {
        public long Id { get; set; }
        public string Fullname { get; set; }

        public string Message { get; set; }
        public string Subject { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; }

    }
}
