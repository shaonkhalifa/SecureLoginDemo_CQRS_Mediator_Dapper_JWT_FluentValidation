using MadiatrProject.DbContexts;
using MadiatrProject.Enums;
using MadiatrProject.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;

namespace MadiatrProject.Services;

public class DbInitializer : IHostedService
{
    // public readonly MDBContext _dbcontext;
    private readonly IServiceScopeFactory _scopeFactory;
    public DbInitializer(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {

        var scope = _scopeFactory.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<MDBContext>();
        List<(string Name, int Value)> enumList = Enum.GetValues(typeof(PermissionEnum))
                                                      .Cast<PermissionEnum>()
                                                      .Select(permission => (permission.ToString(), (int)permission))
                                                      .ToList();


        foreach (var item in enumList)
        {
            var permissionCheck = await _context.Permission.AnyAsync(a => a.PermissionName == item.Name);
            if (permissionCheck == false)
            {
                Permission permission = new Permission()
                {
                    PermissionId = item.Value,
                    PermissionName = item.Name
                };
                await _context.Permission.AddAsync(permission);
            }
        }
        //await _context.SaveChangesAsync();
    }

    //public void Configure(DbContextOptions<MDBContext> options)
    //{
    //    using (var context = new MDBContext(options))
    //    {
    //        SeedPermissions(context);
    //    }
    //}

    //public async Task StartAsync(CancellationToken cancellationToken)
    //{
    //    if (!_dbcontext.Permission.Any())
    //    {
    //        var permissions = PermissionEnum.GetValues(typeof(Permission))
    //                    .Cast<Permission>()
    //                    .Select((value, index) => new { Value = value, Index = index });
    //        foreach (var permission in permissions)
    //        {
    //            _dbcontext.Permission.Add(new Permission
    //            {
    //                PermissionId = permission.Index,
    //                PermissionName = permission.Value.ToString(),
    //            });
    //        }

    //    }
    //   await _dbcontext.SaveChangesAsync();
    //}

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var scope = _scopeFactory.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<MDBContext>();
        if (_context != null)
        {
            await _context.DisposeAsync();
        }
    }
}
