﻿using FakeEstate.ListingManager.Models.EFHelpers;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace FakeEstate.ListingManager.Models.Listings
{
    public class FakeEstateContext : DbContext
    {
        public FakeEstateContext() 
            : base("name=FakeEstateContext")
        {
        }

        public virtual DbSet<Listing> Listings { get; set; }
        public virtual DbSet<ListingPhoto> ListingPhotos { get; set; }
        public virtual DbSet<Agent> Agents { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Map using TPT - this makes soft delete more complicated since not
            // all tables have the IsDeleted column
            modelBuilder.Entity<Listing>().ToTable("dbo.Listings");
            modelBuilder.Entity<CommercialListing>().ToTable("dbo.CommercialListings");

            modelBuilder.Entity<Listing>()
                .HasRequired(l => l.SellingAgent)
                .WithMany(a => a.Listings)
                .HasForeignKey(a => a.SellingAgentId);

            modelBuilder.Entity<ListingPhoto>()
                .ToTable("ListingPhoto");

            var conv = new AttributeToTableAnnotationConvention<SoftDeleteAttribute, string>(
                "SoftDeleteColumnName",
                (type, attributes) => attributes.Single().ColumnName);

            modelBuilder.Conventions.Add(conv);
        }
    }
}
