using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SQLite;

class DatabaseManager
{
    private string _connectionString;

    public DatabaseManager(string dbPath)
    {
        _connectionString = $"Data Source={dbPath};Version=3;";
    }

    public void InitializeDatabase(string schoolCsv, string studentCsv)
    {
        CreateTables();

        if (GetAllSchools().Count == 0 && File.Exists(schoolCsv))
            ImportSchoolsFromCsv(schoolCsv);

        if (GetAllStudents().Count == 0 && File.Exists(studentCsv))
            ImportStudentsFromCsv(studentCsv);
    }

    private void CreateTables()
    {
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS school (
            school_id INTEGER PRIMARY KEY AUTOINCREMENT,
            school_name TEXT NOT NULL
        );
        CREATE TABLE IF NOT EXISTS student (
            student_id INTEGER PRIMARY KEY AUTOINCREMENT,
            school_id INTEGER NOT NULL,
            student_name TEXT NOT NULL,
            avg_grade REAL NOT NULL,
            FOREIGN KEY (school_id) REFERENCES school(school_id)
        );";
        cmd.ExecuteNonQuery();
    }

    private void ImportSchoolsFromCsv(string path)
    {
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();
        var lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var p = lines[i].Split(';');
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO school (school_id, school_name) VALUES (@id,@name)";
            cmd.Parameters.AddWithValue("@id", int.Parse(p[0]));
            cmd.Parameters.AddWithValue("@name", p[1]);
            cmd.ExecuteNonQuery();
        }
    }

    private void ImportStudentsFromCsv(string path)
    {
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();
        var lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var p = lines[i].Split(';');
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO student (student_id, school_id, student_name, avg_grade) VALUES (@id,@sid,@name,@grade)";
            cmd.Parameters.AddWithValue("@id", int.Parse(p[0]));
            cmd.Parameters.AddWithValue("@sid", int.Parse(p[1]));
            cmd.Parameters.AddWithValue("@name", p[2]);
            cmd.Parameters.AddWithValue("@grade", double.Parse(p[3].Replace('.', ',')));
            cmd.ExecuteNonQuery();
        }
    }

    public List<School> GetAllSchools()
    {
        var list = new List<School>();
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM school ORDER BY school_id ASC";
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add(new School(r.GetInt32(0), r.GetString(1)));
        return list;
    }

    public List<Student> GetAllStudents()
    {
        var list = new List<Student>();
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM student ORDER BY student_id ASC";
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add(new Student(r.GetInt32(0), r.GetInt32(1), r.GetString(2), r.GetDouble(3)));
        return list;
    }

    public List<string> GetFormattedStudentsList()
    {
        var list = new List<string>();
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT s.student_id, s.student_name, sch.school_name, s.avg_grade 
            FROM student s 
            JOIN school sch ON s.school_id = sch.school_id 
            ORDER BY s.student_id ASC";

        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            string row = $"[{r.GetInt32(0)}] {r.GetString(1)}, {r.GetString(2)}, средний балл: {r.GetDouble(3):F1}";
            list.Add(row);
        }
        return list;
    }

    public void AddStudent(Student s)
    {
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();

        var checkCmd = conn.CreateCommand();
        checkCmd.CommandText = "SELECT COUNT(*) FROM school WHERE school_id = @sid";
        checkCmd.Parameters.AddWithValue("@sid", s.SchoolId);
        long count = (long)checkCmd.ExecuteScalar();

        if (count == 0)
        {
            Console.WriteLine($"\n[ОШИБКА] Школы с ID {s.SchoolId} не существует. Ученик не добавлен.");
            return;
        }

        var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO student (school_id, student_name, avg_grade) VALUES (@s,@n,@g)";
        cmd.Parameters.AddWithValue("@s", s.SchoolId);
        cmd.Parameters.AddWithValue("@n", s.Name);
        cmd.Parameters.AddWithValue("@g", s.AvgGrade);
        cmd.ExecuteNonQuery();

        Console.WriteLine("\n[УСПЕХ] Ученик добавлен.");
    }

    public void UpdateStudent(int id, int newSchoolId, string newName, double newGrade)
    {
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();

        var checkCmd = conn.CreateCommand();
        checkCmd.CommandText = "SELECT COUNT(*) FROM school WHERE school_id = @sid";
        checkCmd.Parameters.AddWithValue("@sid", newSchoolId);
        long count = (long)checkCmd.ExecuteScalar();

        if (count == 0)
        {
            Console.WriteLine($"\n[ОШИБКА] Школы с ID {newSchoolId} не существует. Изменения не сохранены.");
            return;
        }

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE student 
            SET school_id = @sid, 
                student_name = @name, 
                avg_grade = @grade 
            WHERE student_id = @id";
        cmd.Parameters.AddWithValue("@sid", newSchoolId);
        cmd.Parameters.AddWithValue("@name", newName);
        cmd.Parameters.AddWithValue("@grade", newGrade);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();

        Console.WriteLine("\n[УСПЕХ] Данные обновлены.");
    }

    public void DeleteStudent(int id)
    {
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM student WHERE student_id=@id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public (string[], List<string[]>) ExecuteQuery(string sql)
    {
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        using var r = cmd.ExecuteReader();

        string[] cols = new string[r.FieldCount];
        for (int i = 0; i < cols.Length; i++)
            cols[i] = r.GetName(i);

        var rows = new List<string[]>();
        while (r.Read())
        {
            string[] row = new string[r.FieldCount];
            for (int i = 0; i < row.Length; i++)
                row[i] = r.GetValue(i).ToString();
            rows.Add(row);
        }
        return (cols, rows);
    }
}
