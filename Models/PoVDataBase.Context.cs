﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PoVWebsite.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class PoVEntities : DbContext
    {
        public PoVEntities()
            : base("name=PoVEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Available> Availables { get; set; }
        public DbSet<User> Users { get; set; }

        //Test
        public DbSet<Picture> Pictures { get; set; }

        public DbSet<PushContact> PushContacts { get; set; }

        public DbSet<Token> Tokens { get; set; }
    }
}
