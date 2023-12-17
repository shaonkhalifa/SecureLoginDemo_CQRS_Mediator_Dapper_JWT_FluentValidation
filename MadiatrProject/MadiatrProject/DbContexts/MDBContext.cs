using System.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;
using MadiatrProject.Model;
using Microsoft.EntityFrameworkCore.Storage;

namespace MadiatrProject.DbContexts
{
    public class MDBContext : DbContext
    {
        public MDBContext(DbContextOptions<MDBContext> options) : base(options)
        {

        }
        public DbSet<Students> Students { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<RoleAssain> RoleAssain { get; set; }
        public IDbConnection GetSqlConnection() => (IDbConnection)Database.GetDbConnection();
        //public IDbConnection GetSqlConnection()
        //{
        //    var dbConnection = Database.GetDbConnection();
        //    if (dbConnection.State != ConnectionState.Open)
        //    {
        //        dbConnection.Open();
        //    }
        //    return dbConnection;
        //}


    }

}
