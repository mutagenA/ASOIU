using System;
using System.Collections.Generic;
using System.Text;

class ReportBuilder
{
    private readonly DatabaseManager _db;

    private string _sql = "";
    private string _title = "";
    private string[] _headers = Array.Empty<string>();
    private int[] _widths = Array.Empty<int>();
    private string _footer = "";

    public ReportBuilder(DatabaseManager db)
    {
        _db = db;
    }

    public ReportBuilder Query(string sql)
    {
        _sql = sql;
        return this;
    }

    public ReportBuilder Title(string title)
    {
        _title = title;
        return this;
    }

    public ReportBuilder Header(params string[] headers)
    {
        _headers = headers;
        return this;
    }

    public ReportBuilder ColumnWidths(params int[] widths)
    {
        _widths = widths;
        return this;
    }

    public ReportBuilder Footer(string footer)
    {
        _footer = footer;
        return this;
    }

    public void Print()
    {
        if (string.IsNullOrWhiteSpace(_sql))
        {
            Console.WriteLine("Ошибка: SQL-запрос не указан");
            return;
        }

        var (columns, rows) = _db.ExecuteQuery(_sql);
        var sb = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(_title))
            sb.AppendLine($"=== {_title} ===");

        if (_headers.Length > 0 && _widths.Length > 0)
        {
            for (int i = 0; i < _headers.Length && i < _widths.Length; i++)
                sb.Append(_headers[i].PadRight(_widths[i]));
            sb.AppendLine();

            int totalWidth = 0;
            foreach (var w in _widths) totalWidth += w;
            sb.AppendLine(new string('-', Math.Min(totalWidth, 90)));
        }

        foreach (var row in rows)
        {
            for (int i = 0; i < row.Length && i < _widths.Length; i++)
                sb.Append(row[i].PadRight(_widths[i]));
            sb.AppendLine();
        }

        if (!string.IsNullOrWhiteSpace(_footer) && rows.Count > 0)
        {
            sb.AppendLine(new string('-', 50));
            sb.AppendLine($"{_footer}: {rows.Count}");
        }

        Console.WriteLine(sb.ToString());
    }
}