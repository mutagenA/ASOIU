using System.Windows.Forms;

namespace DZ3.UI
{
    public class MainForm : Form
    {
        public MainForm()
        {
            Text = "Главное меню — Ученики и Школы";
            Width = 450; Height = 320;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(60, 40, 60, 40)
            };

            var btnMaster = new Button { Text = "Справочник школ", Width = 300, Height = 45 };
            btnMaster.Click += (s, e) => new MasterForm().ShowDialog();

            var btnDetail = new Button { Text = "Список учеников", Width = 300, Height = 45 };
            btnDetail.Click += (s, e) => new DetailForm().ShowDialog();

            var btnReport = new Button { Text = "Аналитический отчет", Width = 300, Height = 45 };
            btnReport.Click += (s, e) => new ReportForm().ShowDialog();

            panel.Controls.AddRange(new Control[] { btnMaster, btnDetail, btnReport });
            Controls.Add(panel);
        }
    }
}
