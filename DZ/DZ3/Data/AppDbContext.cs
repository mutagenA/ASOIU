using Microsoft.EntityFrameworkCore;
using DZ3.Models;
using System.Linq;

namespace DZ3.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<School> Schools { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=school_data.db");

        public static void InitializeDatabase()
        {
            using var context = new AppDbContext();
            context.Database.EnsureCreated();

            if (!context.Schools.Any())
            {
                var s1 = new School { Name = "Школа num.1" };
                var s2 = new School { Name = "Лицей num.5" };
                var s3 = new School { Name = "Гимназия num.3" };
                var s4 = new School { Name = "Школа num.10" };

                context.Schools.AddRange(s1, s2, s3, s4);
                context.SaveChanges();

                context.Students.AddRange(
                    new Student { SchoolId = s1.Id, Name = "Иванов", AvgGrade = 4.5 },
                    new Student { SchoolId = s1.Id, Name = "Петров", AvgGrade = 3.8 },
                    new Student { SchoolId = s1.Id, Name = "Смирнов", AvgGrade = 4.2 },
                    new Student { SchoolId = s2.Id, Name = "Сидоров", AvgGrade = 4.7 },
                    new Student { SchoolId = s2.Id, Name = "Кузнецов", AvgGrade = 3.5 },
                    new Student { SchoolId = s2.Id, Name = "Попов", AvgGrade = 4.1 },
                    new Student { SchoolId = s3.Id, Name = "Васильев", AvgGrade = 3.9 },
                    new Student { SchoolId = s3.Id, Name = "Новиков", AvgGrade = 4.7 },
                    new Student { SchoolId = s3.Id, Name = "Морозов", AvgGrade = 3.3 },
                    new Student { SchoolId = s4.Id, Name = "Фёдоров", AvgGrade = 4.0 },
                    new Student { SchoolId = s4.Id, Name = "Алексеев", AvgGrade = 3.6 },
                    new Student { SchoolId = s4.Id, Name = "Лебедев", AvgGrade = 4.8 }
                );
                context.SaveChanges();
            }
        }
    }
}
