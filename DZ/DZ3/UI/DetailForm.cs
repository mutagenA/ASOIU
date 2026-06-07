using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using DZ3.Data;
using DZ3.Models;

namespace DZ3.UI
{
    public class DetailForm : Form
    {
        private DataGridView dgv = new DataGridView { Dock = DockStyle.Top, Height = 240, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, ReadOnly = true };
        private TextBox txtName = new TextBox { Width = 140 };
        private TextBox txtGrade = new TextBox { Width = 50 };
        private ComboBox cbSchools = new ComboBox { Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };

        public DetailForm()
        {
            Text = "Управление учениками (Detail)"; Width = 680; Height = 440; StartPosition = FormStartPosition.CenterParent;

            var flow = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 140, Padding = new Padding(10) };
            flow.Controls.AddRange(new Control[] {
                new Label { Text = "Имя:", Width = 40 }, txtName,
                new Label { Text = "Школа:", Width = 50 }, cbSchools,
                new Label { Text = "Балл:", Width = 40 }, txtGrade
            });

            var btnAdd = new Button { Text = "Добавить", Width = 90 };
            var btnEdit = new Button { Text = "Изменить", Width = 90 };
            var btnDelete = new Button { Text = "Удалить", Width = 90 };
            flow.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete });
            Controls.AddRange(new Control[] { dgv, flow });

            LoadSchools(); LoadData();

            btnAdd.Click += (s, e) => {
                if (!ValidateInput(out double grade)) return;
                using (var db = new AppDbContext())
                {
                    db.Students.Add(new Student { Name = txtName.Text.Trim(), SchoolId = (int)cbSchools.SelectedValue, AvgGrade = grade });
                    db.SaveChanges();
                }
                LoadData(); ClearInputs();
            };

            btnEdit.Click += (s, e) => {
                if (dgv.SelectedRows.Count == 0) return;
                if (!ValidateInput(out double grade)) return;
                int id = (int)dgv.SelectedRows[0].Cells["Id"].Value;

                using (var db = new AppDbContext())
                {
                    var student = db.Students.Find(id);
                    if (student != null)
                    {
                        student.Name = txtName.Text.Trim();
                        student.SchoolId = (int)cbSchools.SelectedValue;
                        student.AvgGrade = grade;
                        db.SaveChanges();
                    }
                }
                LoadData(); ClearInputs();
            };

            btnDelete.Click += (s, e) => {
                if (dgv.SelectedRows.Count == 0) return;
                int id = (int)dgv.SelectedRows[0].Cells["Id"].Value;
                if (MessageBox.Show("Удалить ученика?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.No) return;

                using (var db = new AppDbContext())
                {
                    var student = db.Students.Find(id);
                    if (student != null) { db.Students.Remove(student); db.SaveChanges(); }
                }
                LoadData(); ClearInputs();
            };

            dgv.SelectionChanged += (s, e) => {
                if (dgv.SelectedRows.Count > 0 && cbSchools.Items.Count > 0)
                {
                    txtName.Text = dgv.SelectedRows[0].Cells["Имя_Ученика"].Value.ToString();
                    txtGrade.Text = dgv.SelectedRows[0].Cells["Средний_Балл"].Value.ToString();
                    cbSchools.Text = dgv.SelectedRows[0].Cells["Школа"].Value.ToString();
                }
            };
        }

        private bool ValidateInput(out double grade)
        {
            grade = 0;
            if (string.IsNullOrWhiteSpace(txtName.Text)) { MessageBox.Show("Укажите имя!"); return false; }
            if (cbSchools.SelectedValue == null) { MessageBox.Show("Выберите учреждение!"); return false; }

            if (!double.TryParse(txtGrade.Text.Replace('.', ','), out grade) || grade < 0)
            {
                MessageBox.Show("Валидация провалена: Значение балла не может быть отрицательным числом!", "Ошибка ввода");
                return false;
            }
            return true;
        }

        private void LoadSchools()
        {
            using var db = new AppDbContext();
            cbSchools.DataSource = db.Schools.OrderBy(s => s.Name).ToList();
            cbSchools.DisplayMember = "Name";
            cbSchools.ValueMember = "Id";
        }

        private void LoadData()
        {
            using var db = new AppDbContext();
            dgv.DataSource = db.Students
                .Include(st => st.School)
                .OrderBy(st => st.Name)
                .Select(st => new {
                    st.Id,
                    Имя_Ученика = st.Name,
                    Школа = st.School != null ? st.School.Name : "",
                    Средний_Балл = st.AvgGrade
                }).ToList();
        }

        private void ClearInputs() { txtName.Clear(); txtGrade.Clear(); }
    }
}
