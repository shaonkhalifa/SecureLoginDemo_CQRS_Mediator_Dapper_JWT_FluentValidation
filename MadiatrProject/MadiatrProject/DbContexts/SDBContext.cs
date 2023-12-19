using MadiatrProject.Model;
using Microsoft.EntityFrameworkCore;

namespace MadiatrProject.DbContexts;

public class SDBContext:DbContext
{
    public SDBContext(DbContextOptions<SDBContext> options):base(options)
    {
        
    }
    public DbSet<SessionTbl> SessionTbl { get; set; }
    public DbSet<Course> Course {  get; set; }
    
}
