using System.ComponentModel.DataAnnotations;

namespace MadiatrProject.Model
{
    public class SessionTbl
    {
        [Key]
        public Guid SessionId { get; set; }
        public DateTime? ExpireTime { get; set; }
        public DateTime? LogInTime { get; set;}
        public string? Token { get; set;}
        public int? RoleID { get; set; }
        public int? UserID { get; set; }
        public string? Permission { get; set; }
    }
}
