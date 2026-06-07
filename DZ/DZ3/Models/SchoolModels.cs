using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DZ3.Models
{
    public class School
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    }

    public class Student
    {
        public int Id { get; set; }
        public int SchoolId { get; set; }
        public virtual School? School { get; set; }

        [Required]
        public string Name { get; set; } = "";
        public double AvgGrade { get; set; }
    }
}
