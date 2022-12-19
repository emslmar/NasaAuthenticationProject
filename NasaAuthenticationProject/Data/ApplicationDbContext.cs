using Microsoft.EntityFrameworkCore;
using NasaAuthenticationProject.Models;
using System.Collections.Generic;

namespace Nasa_Application.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }  //create table Users


    }
}
