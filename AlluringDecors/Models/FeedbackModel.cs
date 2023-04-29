using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AlluringDecors.Models
{
    public class FeedbackModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int? UserId { get; set; }
        [Required]
        public string Comment { get; set; }
        public DateTime DateSent { get; set; }
        public UserModel User { get; set; }
    }
}