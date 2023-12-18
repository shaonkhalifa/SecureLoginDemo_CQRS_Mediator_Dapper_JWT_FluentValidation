using MadiatrProject.DbContexts;
using MadiatrProject.Enums;
using MadiatrProject.Model;
using Microsoft.EntityFrameworkCore;

namespace MadiatrProject.Services;

public class PermissionInitializerService
{
    public readonly MDBContext _dbContext;
    public PermissionInitializerService(MDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    public void permissioninsert()
    {


        //    //var _context = _scopeFactory.ServiceProvider.GetRequiredService<MDBContext>();
        //    List<(string Name, int Value)> enumList = Enum.GetValues(typeof(PermissionEnum))
        //                                                  .Cast<PermissionEnum>()
        //                                                  .Select(permission => (permission.ToString(), (int)permission))
        //                                                  .ToList();


        //    foreach (var item in enumList)
        //    {
        //        var permissionCheck = await _context.Permission.AnyAsync(a => a.PermissionName == item.Name);
        //        if (permissionCheck == false)
        //        {
        //            Permission permission = new Permission()
        //            {
        //                PermissionId = item.Value,
        //                PermissionName = item.Name
        //            };
        //            await _context.Permission.AddAsync(permission);
        //        }
        //    }
        //    await _context.SaveChangesAsync();
        //}
    }
}
