using System.ComponentModel.DataAnnotations;

namespace MadiatrProject.Model
{
    public class PermissionAssign
    {
        [Key]
        public int PermissionAssignId { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set;}
    }
}
