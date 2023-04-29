using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AlluringDecors.Models
{
    public class ServiceDomainModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ServiceId { get; set; }
        [Required]
        public int DomainId { get; set; }
        public ServiceModel Service { get; set; }
        public DomainModel Domain { get; set; }
    }
}