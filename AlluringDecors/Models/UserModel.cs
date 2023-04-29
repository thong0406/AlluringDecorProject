using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AlluringDecors.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phonenumber { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public UserRoleModel UserRole { get; set; }
    }
}