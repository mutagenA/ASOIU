using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DZ3.Data;
using DZ3.Models;

namespace DZ3.UI
{
    public class MasterForm : Form
    {
        private DataGridView dgv = new DataGridView { Dock = DockStyle.Top, Height = 240, AutoGenerateColumns = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, ReadOnly = true };
        private TextBox txtName = new TextBox { Width = 200 };

        public MasterForm()
        {
            Text = "Управление школами (Master)"; Width = 520; Height = 430; StartPosition = FormStartPosition.CenterParent;

            var lbl = new Label { Text = "Название школы:", Width = 110 };
            var btnAdd = new Button { Text = "Добавить", Width = 90 };
            var btnEdit = new Button { Text = "Изменить", Width = 90 };
            var btnDelete = new Button { Text = "Удалить", Width = 90 };

            var flow = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 130, Padding = new Padding(15) };
            flow.Controls.AddRange(new Control[] { lbl, txtName, btnAdd, btnEdit, btnDelete });
            Controls.AddRange(new Control[] { dgv, flow });

            LoadData();

            btnAdd.Click += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtName.Text)) { MessageBox.Show("Поле пустое!"); return; }
                using (var db = new AppDbContext())
                {
                    db.Schools.Add(new School { Name = txtName.Text.Trim() });
                    db.SaveChanges();
                }
                LoadData(); txtName.Clear();
            };

            btnEdit.Click += (s, e) => {
                if (dgv.SelectedRows.Count == 0) return;
                int id = (int)dgv.SelectedRows[0].Cells["Id"].Value;
                if (string.IsNullOrWhiteSpace(txtName.Text)) { MessageBox.Show("Название пустое!"); return; }
                using (var db = new AppDbContext())
                {
                    var school = db.Schools.Find(id);
                    if (school != null) { school.Name = txtName.Text.Trim(); db.SaveChanges(); }
                }
                LoadData(); txtName.Clear();
            };

            btnDelete.Click += (s, e) => {
                if (dgv.SelectedRows.Count == 0) return;
                int id = (int)dgv.SelectedRows[0].Cells["Id"].Value;
                if (MessageBox.Show("Удалить выбранную запись?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.No) return;

                using (var db = new AppDbContext())
                {
                    if (db.Students.Any(st => st.SchoolId == id))
                    {
                        MessageBox.Show("Ошибка: Невозможно удалить школу! В ней числятся ученики.", "Нарушение целостности");
                        return;
                    }
                    var school = db.Schools.Find(id);
                    if (school != null) { db.Schools.Remove(school); db.SaveChanges(); }
                }
                LoadData(); txtName.Clear();
            };

            dgv.SelectionChanged += (s, e) => {
                if (dgv.SelectedRows.Count > 0) txtName.Text = dgv.SelectedRows[0].Cells["Name"].Value.ToString();
            };
        }

        private void LoadData()
        {
            using var db = new AppDbContext();
            dgv.DataSource = db.Schools.OrderBy(s => s.Name).Select(s => new { s.Id, s.Name }).ToList();
        }
    }
}
