/// <summary>
/// Школа (справочная таблица, сторона «один»)
/// </summary>
class School
{
    public int Id { get; set; }
    public string Name { get; set; }

    public School(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public School() : this(0, "") { }

    public override string ToString() => $"[{Id}] {Name}";
}