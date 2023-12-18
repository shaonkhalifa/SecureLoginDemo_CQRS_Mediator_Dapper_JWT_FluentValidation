using MadiatrProject.Enums;
using MadiatrProject.Model;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MadiatrProject.DbContexts;

public class MDBContext : DbContext
{
    public MDBContext(DbContextOptions<MDBContext> options) : base(options)
    {
        Console.WriteLine("MDBContext constructor called.");
        InsertPredefinedData();
    }
    public DbSet<Students> Students { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<Role> Role { get; set; }
    public DbSet<RoleAssain> RoleAssain { get; set; }
    public DbSet<PermissionAssign> PermissionAssign { get; set; }
    public DbSet<Permission> Permission { get; set; }
    public IDbConnection GetSqlConnection() => (IDbConnection)Database.GetDbConnection();

    private void InsertPredefinedData()
    {
        var existingPermissionNames = Permission.Select(a => a.PermissionName).ToList();

        List<Permission> permissions = Enum.GetValues(typeof(PermissionEnum))
            .Cast<PermissionEnum>()
            .Select(permission => new Permission
            {
                PermissionId = (int)permission,
                PermissionName = permission.ToString()
            })
             .Where(permission =>
        !existingPermissionNames.Contains(permission.PermissionName))
            .ToList();
        Permission.AddRange(permissions);
        SaveChanges();


    }
}
