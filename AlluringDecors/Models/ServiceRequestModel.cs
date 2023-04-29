using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AlluringDecors.Models
{
    public class ServiceRequestModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int ServiceDomainId { get; set; }
        [Required]
        public string Receiver { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Contact { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public DateTime DateSent { get; set; }
        public BillModel Bill { get; set; }
        public UserModel User { get; set; }
        public ServiceDomainModel ServiceDomain { get; set; }
    }
}