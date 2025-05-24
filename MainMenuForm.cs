using StardewRaft.Views.Inventory;

namespace StardewRaft
{
    public partial class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
             ControlStyles.AllPaintingInWmPaint |
             ControlStyles.UserPaint, true);

            Text = "StardewRaft";
            WindowState = FormWindowState.Maximized;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.White;

            InitializeComponent();

            var panel = new Panel
            {
                Size = new Size(300, 200),
                BackColor = Color.Transparent,
                Location = new Point(
                    (ClientSize.Width - 300) / 2,
                    (ClientSize.Height - 200) / 2)
            };

            var playButton = new Button
            {
                Text = "ИГРАТЬ",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(200, 180, 130, 100),
                BackColor = InventoryView.BackgroundColor,
                Size = new Size(250, 60),
                Location = new Point(25, 20),
                FlatStyle = FlatStyle.Flat
            };
            playButton.FlatAppearance.BorderSize = 0;
            playButton.Click += (s, e) =>
            {
                var gameForm = new GameForm();
                gameForm.Show();
                Hide();
                gameForm.FormClosed += (sender, args) => Show();
            };

            var exitButton = new Button
            {
                Text = "ВЫЙТИ",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(200, 180, 130, 100),
                BackColor = InventoryView.BackgroundColor,
                Size = new Size(250, 60),
                Location = new Point(25, 100),
                FlatStyle = FlatStyle.Flat
            };

            exitButton.FlatAppearance.BorderSize = 0;
            exitButton.Click += (s, e) => Application.Exit();

            panel.Controls.Add(playButton);
            panel.Controls.Add(exitButton);
            Controls.Add(panel);

            Resize += (s, e) =>
            {
                panel.Location = new Point(
                    (ClientSize.Width - panel.Width) / 2,
                    (ClientSize.Height - panel.Height) / 2);
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {

            var g = e.Graphics;
        }
    }
}