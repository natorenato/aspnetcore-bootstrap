using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Domains.Models
{
    public class Contact
    {
        public int ContactId { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
    }
}