using System.ComponentModel.DataAnnotations;

namespace MadiatrProject.Model
{
    public class Students
    {
        [Key]
        public int StudentId { get; set; }
        public string? StudentName { get; set;}
        public string? StudentEmail { get; set;}
        public string? FatherName { get; set; }
        public string? MotherName { get; set; }
        public int RollNo { get; set; }
    }
}
