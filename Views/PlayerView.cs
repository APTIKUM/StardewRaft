using StardewRaft.Core.Feature;
using StardewRaft.Models;
using StardewRaft.Properties;
using System.Numerics;
using Timer = System.Windows.Forms.Timer;
using StardewRaft.Core.Factories.InventoryItem;
using StardewRaft.Image.Structs;

namespace StardewRaft.Views
{
    public class PlayerView
    {
        private Timer _animationTimer = new Timer() { Interval = 100};

        private PlayerModel _model;

        private SpriteSheetAnimateStruct _spriteSheetBody = new SpriteSheetAnimateStruct(
                Resources.player_male_base_body,
                16, 30,
                new Dictionary<int, int> { { 0, 3 }, { 1, 3 }, { 2, 3 }, { 3, 3 } });

        private SpriteSheetAnimateStruct _spriteSheetBubble = new SpriteSheetAnimateStruct(
               Resources.player_male_base_body,
               16, 30,
               new Dictionary<int, int> { { 0, 3 } });

        private Timer _animationBubbleTimer = new Timer() { Interval = 500 };

        private Rectangle _frameBubbleRactangle;

        private TextureStruct _textureBody = new TextureStruct(Resources.player_male_base_body);
        private TextureStruct _textureBubble = new TextureStruct(Resources.player_male_bubble);
        private TextureStruct _textureHair = new TextureStruct(Resources.player_hair_short.RecolorGrayscaleImage(Color.DarkOrange));

        private readonly Brush _barStatBackgroundBrush = new SolidBrush(Color.FromArgb(80, 180, 130, 100));
        private readonly Color _backgroundColor = Color.FromArgb(255, 90, 75, 60);
        private Size RenderSize { get; set; }

        private float _renderScale;

        private Rectangle _frameRactangle;

        private Form _renderForm;

        public PlayerView(PlayerModel model, Form form)
        {
            _model = model;
            _model.ModelUpdated += OnModelUpdated;
            _renderForm = form;

            _renderScale = _model.Collider.Width / _spriteSheetBody.FrameWidth * 2;
            _frameRactangle = _spriteSheetBody.GetFrameRectangle(0, 0);
            _frameBubbleRactangle = _spriteSheetBubble.GetFrameRectangle(0, 0);

            RenderSize = new Size((int)(_spriteSheetBody.FrameWidth * _renderScale),
                (int)(_spriteSheetBody.FrameHeight * _renderScale));

            _animationTimer.Tick += AnimateFrame;
            _animationTimer.Start();

            _animationBubbleTimer.Tick += _animationBubbleTimer_Tick;
            _animationBubbleTimer.Start();
        }

        private void _animationBubbleTimer_Tick(object? sender, EventArgs e)
        {
            _frameBubbleRactangle = _spriteSheetBubble.GetNextFrameRectangle();
        }

        private void AnimateFrame(object? sender, EventArgs e)
        {
            if (_model.Speed != Vector2.Zero)
            {
                SetAnimationBySpeed();
                _frameRactangle = _spriteSheetBody.GetNextFrameRectangle();
            }
            else
            {
                _frameRactangle = _spriteSheetBody.GetFrameRectangle(0, _spriteSheetBody.CurrentAnimation);
            }
        }
        private void SetAnimationBySpeed()
        {
            if (_model.Speed.X > 0)
            {
                _spriteSheetBody.CurrentAnimation = 1;
            }
            else if (_model.Speed.X < 0)
            {
                _spriteSheetBody.CurrentAnimation = 3;
            }

            if (_model.Speed.Y > 0)
            {
                _spriteSheetBody.CurrentAnimation = 0;
            }
            else if (_model.Speed.Y < 0){
                _spriteSheetBody.CurrentAnimation = 2;
            }
        }

        private void OnModelUpdated()
        {
            _renderForm.Invalidate();
        }

        public void Draw(Graphics graphics)
        {
            

            var playerCollider = _model.Collider;
            var destRect = new Rectangle((int)(_model.Position.X - (RenderSize.Width - playerCollider.Width) / 2),
                                         (int)(_model.Position.Y - RenderSize.Height + playerCollider.Height),
                                         RenderSize.Width, RenderSize.Height);

            if (!_model.IsPlayerOnRaft(_model.Collider))
            {
                graphics.DrawImage(_textureBubble.Image, destRect, _frameBubbleRactangle, GraphicsUnit.Pixel);
                return;
            }

            graphics.FillEllipse(new SolidBrush(Color.FromArgb(255 / 3, 0, 0, 0)),
                playerCollider.X, playerCollider.Y,
                playerCollider.Width, playerCollider.Height);

            graphics.DrawImage(_textureBody.Image, destRect, _frameRactangle, GraphicsUnit.Pixel);
            graphics.DrawImage(_textureHair.Image, destRect, _frameRactangle, GraphicsUnit.Pixel);

        }

        public void DrawClickPowerByHook(Graphics graphics, double clickPower)
        {
            if (_model.InventoryModel.ActiveSlot is InventoryHook && clickPower > 0.1)
            {
                var height = 20;
                var width = 80;
                var maxValue = 1.0;
                var padding = 3;

                var backRect = new Rectangle((int)(_model.Position.X - (width - _model.Collider.Width) / 2 - padding),
                                             (int)(_model.Position.Y - RenderSize.Height + _model.Collider.Height - height - padding * 3),
                                             width + 2 * padding, height + 2 * padding);

                RoundedShapes.DrawRoundedRect(backRect, 6, graphics, _backgroundColor);


                var fillWidth = clickPower / maxValue * width ;
                var fillRect = new Rectangle(backRect.X + padding, backRect.Y + padding, width, height);

                graphics.FillRectangle(_barStatBackgroundBrush, fillRect);
                fillRect.Width = (int)fillWidth;
                graphics.FillRectangle(Brushes.GreenYellow, fillRect);
            }
        }
    }
}
