using System.Data.SqlClient;
using System.Data;

namespace MadiatrProject.DbContext
{
    public class MDBContext
    {
        private readonly IConfiguration _configuration;
        private readonly string databaseName;
        public MDBContext(IConfiguration configuration)
        {
            _configuration = configuration;
            databaseName = _configuration.GetConnectionString("defaultconnections");

        }
        public IDbConnection CreateConnection() => new SqlConnection(databaseName);
    }

}
