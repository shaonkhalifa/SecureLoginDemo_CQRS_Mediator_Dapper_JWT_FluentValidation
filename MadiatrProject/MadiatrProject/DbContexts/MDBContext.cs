using System.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;
using MadiatrProject.Model;

namespace MadiatrProject.DbContexts
{
    public class MDBContext: DbContext
    {
        public MDBContext(DbContextOptions<MDBContext> options):base(options) 
        {
                
        }
        public DbSet<Students> Students { get; set; }
        
        public IDbConnection GetSqlConnection()
        {
            var dbConnection = Database.GetDbConnection();
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }
            return dbConnection;
        }


    }

}
