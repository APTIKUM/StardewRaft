using StardewRaft.Core.Feature;
using StardewRaft.Models;

public class PlayerStatsView : UserControl
{
    private readonly PlayerModel _player;

    private readonly Form _parentForm;

    private readonly int _barHeight = 35;
    private readonly int _barWidth = 300;
    private readonly int _barSpacing = 10;
    private readonly int _barPadding = 5;
    private readonly int _margin = 10;

    private readonly Font _titleFont = new Font("Arial", 12, FontStyle.Bold);


    private readonly Brush _barBorderBrush = new SolidBrush(Color.FromArgb(255, 90, 75, 60));
    private readonly Brush _barStatBackgroundBrush = new SolidBrush(Color.FromArgb(80, 180, 130, 100));
    private readonly Color _backgroundColor = Color.FromArgb(255, 90, 75, 60);

    private readonly Brush _healthBrush = new SolidBrush(Color.Red);
    private readonly Brush _hungerBrush = new SolidBrush(Color.Orange);
    private readonly Brush _thirstBrush = new SolidBrush(Color.DodgerBlue);

    public PlayerStatsView(PlayerModel player, Form parentForm)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));

        _parentForm = parentForm;

        _parentForm.Controls.Add(this);
        _parentForm.Load += ParentForm_Load;

        _player.StatsUpdated += OnStatsUpdated;

        SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        SetStyle(ControlStyles.Selectable, false);

        BackColor = Color.Transparent;

        DoubleBuffered = true;

        Height = _barHeight * 3 + _barSpacing * 2 + _margin * 2 + 6 * _barPadding;
        Width = _barWidth + (_margin + _barPadding) * 2;

        DoubleBuffered = true;
    }

    private void ParentForm_Load(object? sender, EventArgs e)
    {
        Location = new Point(0, _parentForm.Height - Height);
    }

    private void OnStatsUpdated()
    {
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var graphics = e.Graphics;
        int yPos = _margin;

        BringToFront();


        DrawProgressBar(graphics, _margin, yPos,
                      _player.Thirst, _player.MaxThirst,
                      _thirstBrush, "Жажда"
                      , _player.DangerThirst);
        yPos += _barHeight + _barSpacing + 2 * _barPadding;

        DrawProgressBar(graphics, _margin, yPos,
                      _player.Hunger, _player.MaxHunger,
                      _hungerBrush, "Голод"
                      , _player.DangerHunger);
        yPos += _barHeight + _barSpacing + 2 * _barPadding;

        DrawProgressBar(graphics, _margin, yPos,
              _player.Health, _player.MaxHealth,
              _healthBrush, "Здоровье");
    }

    private void DrawProgressBar(Graphics graphics, int x, int y, int value, int maxValue, Brush brush, string label, int dangerValue = 0)
    {
        var backRect = new Rectangle(x, y, _barWidth + _barPadding * 2, _barHeight + _barPadding * 2);

        RoundedShapes.DrawRoundedRect(backRect, 3, graphics, _backgroundColor);

        


        float fillWidth = (float)value / maxValue * _barWidth;
        var fillRect = new Rectangle(x + _barPadding, y + _barPadding, _barWidth, _barHeight);
        graphics.FillRectangle(_barStatBackgroundBrush, fillRect);
        fillRect.Width = (int)fillWidth;
        graphics.FillRectangle(brush, fillRect);

        if (dangerValue != 0)
        {
            var size = 5;
            float xDangerStat = (float)dangerValue / maxValue * _barWidth;
            var dangerBorder = new Rectangle(x + _barPadding + (int)xDangerStat, y + _barPadding, size, size);
            graphics.FillRectangle(_barBorderBrush, dangerBorder);
            
            dangerBorder.Y += _barHeight - size;
            
            graphics.FillRectangle(_barBorderBrush, dangerBorder);

        }




        var text = $"{label}";
        var textSize = graphics.MeasureString(text, _titleFont);
        var textPos = new PointF(_margin + _barPadding, y + _barHeight / 2 - textSize.Height / 2 + _barPadding);
        graphics.DrawString(text, _titleFont, Brushes.White, textPos);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}