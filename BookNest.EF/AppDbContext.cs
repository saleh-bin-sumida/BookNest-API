﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RepositoryWithUOW.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryWithUWO.EF;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>(
            entity => entity
            .HasOne(x => x.Author)
            .WithMany(x => x.Books)
            .HasForeignKey(x => x.AuthorId)
            .IsRequired(true)
            );


        modelBuilder.Entity<ApplicationUser>(
           entity => entity
           .Property(x => x.FullName).HasMaxLength(200)
           );

    }


}
