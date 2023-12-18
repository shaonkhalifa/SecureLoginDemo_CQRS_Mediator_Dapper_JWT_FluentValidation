using Azure.Core;
using Dapper;
using MadiatrProject.DbContexts;
using MadiatrProject.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MadiatrProject.Command
{
    public class RolePermissionSetupCommand : IRequest<int>
    {
        public string RoleName { get; set; }
        public int PermissionAssignId { get; set; }
        public int RoleId { get; set; }
        public List<int> PermissionIds { get; set; }
        private class RolePermissionSetupCommandHandler : IRequestHandler<RolePermissionSetupCommand, int>
        {
            private readonly MDBContext _dbContext;
            public RolePermissionSetupCommandHandler(MDBContext dbContext)
            {
                _dbContext = dbContext;
            }
            public async Task<int> Handle(RolePermissionSetupCommand request, CancellationToken cancellationToken)
            {
                var Roledata = IsRoleNameExistsAsync(request.RoleName);
                if (Roledata != null)
                {
                    throw new InvalidOperationException("Rolename already exists.");
                }
                int insertedRoleId = await InsertRole(request.RoleName);


                var permissionAssignments = request.PermissionIds
                         .Select(permissionId => new PermissionAssign
                         {
                             RoleId = insertedRoleId,
                             PermissionId = permissionId
                         })
                         .ToList();

                _dbContext.PermissionAssign.AddRange(permissionAssignments);
                await _dbContext.SaveChangesAsync();


                return insertedRoleId;
            }
            private async Task<int> IsRoleNameExistsAsync(string roleName)
            {
                var connection = _dbContext.GetSqlConnection();
                var checkRolenameQuery = "SELECT RoleId FROM [User] WHERE UserName = @RoleName";
                int RoleId = await connection.QueryFirstOrDefaultAsync<int>(checkRolenameQuery, new { RoleName = roleName });

                return RoleId;
            }

            private async Task<int> InsertRole(string roleName)
            {
                var role = new Role
                {
                    RoleName = roleName
                };
                _dbContext.Role.Add(role);
                await _dbContext.SaveChangesAsync();
                int roleId = role.RoleId;
                return roleId;
            }
        }
    }
}
