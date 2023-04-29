using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AlluringDecors.Models
{
    public class BillModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ServiceRequestId { get; set; }
        [Required]
        public double Price { get; set; }
        public string Note { get; set; }
        public DateTime DateSent { get; set; }
        public ServiceRequestModel ServiceRequest { get; set; }
    }
}