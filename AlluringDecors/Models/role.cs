//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AlluringDecors.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class role
    {
        public role()
        {
            this.user_role = new HashSet<user_role>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
    
        public virtual ICollection<user_role> user_role { get; set; }
    }
}
