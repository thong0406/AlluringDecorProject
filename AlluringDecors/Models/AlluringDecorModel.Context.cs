﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class AlluringDecorEntities : DbContext
    {
        public AlluringDecorEntities()
            : base("name=AlluringDecorEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<bill> bills { get; set; }
        public DbSet<domain> domains { get; set; }
        public DbSet<feedback> feedbacks { get; set; }
        public DbSet<project> projects { get; set; }
        public DbSet<role> roles { get; set; }
        public DbSet<service> services { get; set; }
        public DbSet<service_domain> service_domain { get; set; }
        public DbSet<service_example> service_example { get; set; }
        public DbSet<service_request> service_request { get; set; }
        public DbSet<user> users { get; set; }
        public DbSet<user_role> user_role { get; set; }
    }
}