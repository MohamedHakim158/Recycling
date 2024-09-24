using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectGraduation.DTO;

namespace ProjectGraduation.Models;

public partial class GraduationProjectContext : IdentityDbContext<Client>
{
    public GraduationProjectContext()
    {
    }

    public GraduationProjectContext(DbContextOptions<GraduationProjectContext> options)
        : base(options)
    {
    }
    public virtual DbSet<Client> Clients { get; set; }
    public virtual DbSet<Offer> Offers { get; set; }
    public virtual DbSet<Contact> Contacts { get; set; }
    public virtual DbSet<Post> Posts { get; set; }
    public virtual DbSet<Profile> Profiles { get; set; }
    
    public async Task<List<PostDto>> SearchPostWithTitle(string searchString)
    {
        var query = from t1 in Posts
                    join t2 in Profiles on t1.ProfileId equals t2.Id
                    join t3 in Clients on t2.ClientId equals t3.Id
                    select new PostDto
                    {
                        Id = t1.Id,
                        CreatedAt = t1.CreatedAt ,
                        Description = t1.Description ,
                        Image = t1.Image,
                        ClientId = t1.ClientId,
                        ProfileId = t1.ProfileId ,
                        Title = t1.Title ,
                        ForegroundImageOfClient = t2.ForegroundImage,
                        FullName = t3.Fname + " " + t3.Lname,
                        UserName = t3.UserName,
                        CategoryName = t1 .CategoryName 
                    };
        var list = new List<PostDto>();
        foreach (var item in query)
        {
            list.Add(item);
        }

        return list;
    }
    public async Task<List<UserDto>> SearchForUser(string searchString)
    {
        var list = Clients
    .Join(Profiles,
          Client => Client.Id,
          Profile => Profile.ClientId,
          (Client, Profile) => new UserDto
          {
              ProfileId = Profile.Id,
              Fname = Client.Fname,
              Lname = Client.Lname,
              UserName = Client.UserName,
              ForegrounImage = Profile.ForegroundImage
          })
    .Where(result => result.Fname.Contains(searchString) || result.Lname.Contains(searchString) || result.UserName.Contains(searchString))
    .ToList();
        return list;
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Contact>()
            .HasKey(C => C.Id);
        modelBuilder.Entity<Post>()
            .HasIndex(p => p.CategoryName);
    }

}
