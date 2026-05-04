using System;

/// <summary>
/// Ученик (основная таблица, сторона «много»)
/// </summary>
class Student
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public string Name { get; set; }

    private double _avgGrade;

    /// <summary>
    /// Средний балл (не может быть отрицательным)
    /// </summary>
    public double AvgGrade
    {
        get => _avgGrade;
        set
        {
            if (value < 0)
                throw new ArgumentException("Средний балл не может быть отрицательным");
            _avgGrade = value;
        }
    }

    public Student(int id, int schoolId, string name, double avgGrade)
    {
        Id = id;
        SchoolId = schoolId;
        Name = name;
        AvgGrade = avgGrade;
    }

    public Student() : this(0, 0, "", 0) { }

    public override string ToString()
        => $"[{Id}] {Name}, школа #{SchoolId}, средний балл: {AvgGrade}";
}