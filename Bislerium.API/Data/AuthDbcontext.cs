using Bislerium.API.Model.Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bislerium.API.Data
{
    public class AuthDbcontext : IdentityDbContext<IdentityUser>
    {
        public AuthDbcontext(DbContextOptions<AuthDbcontext> options) : base(options)
        {
        }
        public DbSet<Blogs> Blogs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<CommentReaction> CommentReactions { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var AdminRoleId = "[f9f81ba6-afd4-465e-b2d6-4239239ccf30]";
            var BloggerRoleId = "[07cdf48b-3bc2-4c16-b06e-51f45e180ce4]";
            var SurferRoleId = "[95f9f19c-d556-400c-9e3c-d19218ad8b5a]";
            var roles = new List<IdentityRole>
            {
                new IdentityRole()
                {
                    Id = AdminRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper(),
                    ConcurrencyStamp = AdminRoleId
                },
                new IdentityRole()
                {
                    Id = BloggerRoleId,
                    Name = "Blogger",
                    NormalizedName = "Blogger".ToUpper(),
                    ConcurrencyStamp = BloggerRoleId
                },
                new IdentityRole()
                {
                    Id = SurferRoleId,
                    Name = "Surfer",
                    NormalizedName = "Surfer".ToUpper(),
                    ConcurrencyStamp = SurferRoleId
                }
            };
            //seed the role
            builder.Entity<IdentityRole>().HasData(roles);
            //Self creating on admin User
            var adminUserId = "edc57c69-7805-46a5-9956-d5fd2f7cf8d3";
            var admin = new IdentityUser()
            {
                Id = adminUserId,
                UserName = "Admin",
                Email = "Wcaavash@gmail.com",
                NormalizedEmail = "Wcaavash@gmail.com".ToUpper(),
                NormalizedUserName = "Admin".ToUpper(),
                PhoneNumber= "9822895932"
            };
            admin.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(admin, "Admin@123");
            builder.Entity<IdentityUser>().HasData(admin);
            //Giving Role to Admin
            var AdminRoles = new List<IdentityUserRole<String>>()
            {
                new()
                {
                    UserId = adminUserId,
                    RoleId = AdminRoleId
                },
                new()
                {
                    UserId = adminUserId,
                    RoleId = BloggerRoleId
                },
                new()
                {
                    UserId = adminUserId,
                    RoleId = SurferRoleId
                }
            };
            builder.Entity<IdentityUserRole<String>>().HasData(AdminRoles);
        }

    }
}
