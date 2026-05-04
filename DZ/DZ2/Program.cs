using System;
using System.IO;
using System.Linq;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        string dbPath = "school.db";
        string schoolCsv = Path.Combine(AppContext.BaseDirectory, "school.csv");
        string studentCsv = Path.Combine(AppContext.BaseDirectory, "student.csv");

        var db = new DatabaseManager(dbPath);
        db.InitializeDatabase(schoolCsv, studentCsv);

        string choice;
        do
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║         УПРАВЛЕНИЕ УЧЕНИКАМИ         ║");
            Console.WriteLine("╠══════════════════════════════════════╣");
            Console.WriteLine("║ 1 — Показать все школы               ║");
            Console.WriteLine("║ 2 — Показать всех учеников           ║");
            Console.WriteLine("║ 3 — Добавить ученика                 ║");
            Console.WriteLine("║ 4 — Редактировать ученика            ║");
            Console.WriteLine("║ 5 — Удалить ученика                  ║");
            Console.WriteLine("║ 6 — Отчёты                           ║");
            Console.WriteLine("║ 0 — Выход                            ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.Write("Ваш выбор: ");
            choice = Console.ReadLine()?.Trim() ?? "";
            Console.WriteLine();

            switch (choice)
            {
                case "1": ShowSchools(db); break;
                case "2": ShowStudents(db); break;
                case "3": AddStudent(db); break;
                case "4": EditStudent(db); break;
                case "5": DeleteStudent(db); break;
                case "6": ReportsMenu(db); break;
                case "0": Console.WriteLine("До свидания!"); break;
                default: Console.WriteLine("Неверный пункт."); break;
            }

            if (choice != "0" && choice != "6")
            {
                Console.WriteLine("\nНажмите Enter для продолжения...");
                Console.ReadLine();
            }
        } while (choice != "0");
    }

    static void ShowSchools(DatabaseManager db)
    {
        var list = db.GetAllSchools();
        foreach (var s in list)
            Console.WriteLine($"[{s.Id}] {s.Name}");
        Console.WriteLine($"Итого: {list.Count} школ");
    }

    static void ShowStudents(DatabaseManager db)
    {
        var list = db.GetFormattedStudentsList();
        foreach (var s in list) Console.WriteLine(s);
        Console.WriteLine($"Итого: {list.Count} учеников");
    }

    static void AddStudent(DatabaseManager db)
    {
        Console.Write("ID школы: ");
        if (!int.TryParse(Console.ReadLine(), out int schoolId)) return;

        Console.Write("Имя ученика: ");
        string name = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Средний балл: ");
        if (!double.TryParse(Console.ReadLine(), out double grade)) return;

        db.AddStudent(new Student(0, schoolId, name, grade));
    }

    static void EditStudent(DatabaseManager db)
    {
        Console.Write("ID ученика для редактирования: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) return;

        var student = db.GetAllStudents().FirstOrDefault(x => x.Id == id);
        if (student == null)
        {
            Console.WriteLine("Ученик не найден.");
            return;
        }

        Console.Write($"Новое имя (текущее: {student.Name}): ");
        string newName = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(newName)) newName = student.Name;

        Console.Write($"Новый ID школы (текущий: {student.SchoolId}): ");
        if (!int.TryParse(Console.ReadLine(), out int newSchoolId))
            newSchoolId = student.SchoolId;

        Console.Write($"Новый средний балл (текущий: {student.AvgGrade}): ");
        if (!double.TryParse(Console.ReadLine(), out double newGrade))
            newGrade = student.AvgGrade;

        db.UpdateStudent(id, newSchoolId, newName, newGrade);
    }

    static void DeleteStudent(DatabaseManager db)
    {
        Console.Write("ID ученика для удаления: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            db.DeleteStudent(id);
            Console.WriteLine("Ученик удалён.");
        }
    }

    static void ReportsMenu(DatabaseManager db)
    {
        string rep;
        do
        {
            Console.Clear();
            Console.WriteLine("\n=== ОТЧЁТЫ ===");
            Console.WriteLine("1 — Ученики по школам (список)");
            Console.WriteLine("2 — Количество учеников по школам");
            Console.WriteLine("3 — Средний балл по школам");
            Console.WriteLine("0 — Назад");
            Console.Write("Выбор: ");
            rep = Console.ReadLine()?.Trim() ?? "";

            if (rep == "0") break;

            var builder = new ReportBuilder(db);
            switch (rep)
            {
                case "1":
                    builder.Query(@"SELECT s.student_name, sc.school_name, s.avg_grade 
                                     FROM student s 
                                     JOIN school sc ON s.school_id = sc.school_id 
                                     ORDER BY sc.school_name, s.student_name")
                            .Title("Ученики по школам")
                            .Header("Имя ученика", "Школа", "Средний балл")
                            .ColumnWidths(30, 25, 15)
                            .Print();
                    break;

                case "2":
                    builder.Query(@"SELECT sc.school_name, COUNT(s.student_id) 
                                     FROM school sc 
                                     LEFT JOIN student s ON sc.school_id = s.school_id 
                                     GROUP BY sc.school_name 
                                     ORDER BY sc.school_name")
                            .Title("Количество учеников по школам")
                            .Header("Название школы", "Кол-во учеников")
                            .ColumnWidths(40, 20)
                            .Print();
                    break;

                case "3":
                    builder.Query(@"SELECT sc.school_name, ROUND(AVG(s.avg_grade), 2) 
                                     FROM school sc 
                                     JOIN student s ON sc.school_id = s.school_id 
                                     GROUP BY sc.school_name 
                                     ORDER BY sc.school_name")
                            .Title("Средний балл по школам")
                            .Header("Название школы", "Средний балл")
                            .ColumnWidths(40, 20)
                            .Print();
                    break;
            }
            Console.WriteLine("\nНажмите Enter для возврата к отчетам...");
            Console.ReadLine();
        } while (rep != "0");
    }
}
