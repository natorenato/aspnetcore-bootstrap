using System;
using System.Collections.Generic;

namespace API.Domains.Models
{
    public class User
    {
        
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Document { get; set; }
        public DateTime Birthdate { get; set; }
        public Country Country { get; set; }
        public Profile Profile { get; set; }
        public bool Active { get; set; }
        public IEnumerable<Contact> Contacts { get; set; }
        public User()
        {
            //Contacts = new List<Contact>();
        }
    }
}
