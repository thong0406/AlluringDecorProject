using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AlluringDecors.Models
{
    public class UserRoleModel
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int RoleId { get; set; }
        public UserModel User { get; set; }
        public RoleModel Role { get; set; }
    }
}