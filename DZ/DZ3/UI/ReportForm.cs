using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using DZ3.Data;

namespace DZ3.UI
{
    public class ReportForm : Form
    {
        public ReportForm()
        {
            Text = "Сводные отчёты (LINQ to Entities)"; Width = 750; Height = 520; StartPosition = FormStartPosition.CenterParent;

            var tc = new TabControl { Dock = DockStyle.Fill };
            var tp1 = new TabPage("Раздел 1. Полный список");
            var tp2 = new TabPage("Раздел 2. Кол-во учеников");
            var tp3 = new TabPage("Раздел 3. Средний показатель");

            var dgv1 = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false };
            var dgv2 = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false };
            var dgv3 = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false };

            tp1.Controls.Add(dgv1); tp2.Controls.Add(dgv2); tp3.Controls.Add(dgv3);
            tc.TabPages.AddRange(new TabPage[] { tp1, tp2, tp3 });
            Controls.Add(tc);

            using var context = new AppDbContext();

            dgv1.DataSource = context.Students
                .Include(st => st.School)
                .OrderBy(st => st.Name)
                .Select(st => new {
                    ФИО_Ученика = st.Name,
                    Наименование_Школы = st.School!.Name,
                    Средний_Балл = st.AvgGrade
                }).ToList();

            dgv2.DataSource = context.Students
                .GroupBy(st => st.School!.Name)
                .Select(g => new {
                    Школа = g.Key,
                    Количество_Учеников = g.Count()
                })
                .OrderBy(r => r.Школа)
                .ToList();

            dgv3.DataSource = context.Students
                .GroupBy(st => st.School!.Name)
                .Select(g => new {
                    Школа = g.Key,
                    Средний_Балл_По_Школе = Math.Round(g.Average(st => st.AvgGrade), 2)
                })
                .OrderByDescending(r => r.Средний_Балл_По_Школе)
                .ToList();
        }
    }
}
