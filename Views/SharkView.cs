using StardewRaft.Core.Factories;
using StardewRaft.Image.Structs;
using StardewRaft.Models;
using StardewRaft.Properties;
using Timer = System.Windows.Forms.Timer;

namespace StardewRaft.Views
{
    public class SharkView
    {
        private SharkModel _model;
        private Form _renderForm;
        private float _renderScale = 1;
        private Timer _animationFinTimer = new Timer() { Interval = 500 };
        private Timer _animationFoamTimer = new Timer() { Interval = 200 };
        private Timer _animationEatFoamTimer = new Timer() { Interval = 500 };

        private SpriteSheetAnimateStruct _spriteSheetFin = new SpriteSheetAnimateStruct(Resources.shark_fin,
            20, 15,
            new Dictionary<int, int>() { { 0, 4 }, { 1, 4 } });

        private SpriteSheetAnimateStruct _spriteSheetFoam = new SpriteSheetAnimateStruct(Resources.shark_fin_foam,
            20, 15,
            new Dictionary<int, int>() { { 0, 8 }, { 1, 8 } });


        private SpriteSheetAnimateStruct _spriteSheetEatFoam = new SpriteSheetAnimateStruct(Resources.shark_eat_foam,
            278, 137,
            new Dictionary<int, int>() { { 0, 4 }});

        private SpiteSheetObjectsStruct<_sharkEatSkins> _spriteSheetEatShark = new(Resources.shark_eat,
new Dictionary<_sharkEatSkins, Rectangle>()
{
             { _sharkEatSkins.SharkEatRight, new Rectangle(0, 0, 276, 98)},
             { _sharkEatSkins.SharkEatLeft, new Rectangle(276, 0, 276, 98)},
});

        private enum _sharkEatSkins
        {
            SharkEatLeft,
            SharkEatRight,
        }


        private Rectangle _frameFinRactangle;
        private Rectangle _frameFoamRactangle;
        private Rectangle _frameEatFoamRactangle;

        public SharkView(SharkModel model, Form renderForm)
        {
            _model = model;
            _renderForm = renderForm;

            _frameFinRactangle = _spriteSheetFin.GetFrameRectangle(0, 0);
            _frameFoamRactangle = _spriteSheetFoam.GetFrameRectangle(0, 0);
            _frameEatFoamRactangle = _spriteSheetEatFoam.GetFrameRectangle(0, 0);

            _model.ModelUpdated += OnModelUpdated;

            _animationFoamTimer.Start();
            _animationFoamTimer.Tick += AnimateFoamFrame;

            _animationFinTimer.Tick += AnimateFinFrame;
            _animationFinTimer.Start();


            _animationEatFoamTimer.Tick += AnimateEatFoamFrame;
            _animationEatFoamTimer.Start();
        }

        private void AnimateEatFoamFrame(object? sender, EventArgs e)
        {
            _frameEatFoamRactangle = _spriteSheetEatFoam.GetNextFrameRectangle();
        }

        private void OnModelUpdated()
        {
            _renderForm.Invalidate();
        }

        private void AnimateFinFrame(object? sender, EventArgs e)
        {
            //SetAnimationBySpeed();
            _frameFinRactangle = _spriteSheetFin.GetNextFrameRectangle();
        }

        private void AnimateFoamFrame(object? sender, EventArgs e)
        {
            SetAnimationBySpeed();
            _frameFoamRactangle = _spriteSheetFin.GetNextFrameRectangle();
        }

        private void SetAnimationBySpeed()
        {
            if (_model.Speed.X > 0)
            {
                _spriteSheetFin.CurrentAnimation = 1;
                _spriteSheetFoam.CurrentAnimation = 1;
            }
            else
            {
                _spriteSheetFin.CurrentAnimation = 0;
                _spriteSheetFoam.CurrentAnimation = 0;
            }
        }


        public void DrawFin(Graphics graphics)
        {
            graphics.DrawImage(_spriteSheetFoam.Texture.Image, _model.Collider, _frameFoamRactangle, GraphicsUnit.Pixel);
            graphics.DrawImage(_spriteSheetFin.Texture.Image, _model.Collider, _frameFinRactangle, GraphicsUnit.Pixel);

        }


        public void DrawEatShark(Graphics graphics)
        {
            if (_model.TargetRaftTile is null)
            {
                DrawFin(graphics);
                return;
            }

            var raftTileCollider = _model.TargetRaftTile.Collider;
            var drawLocation = raftTileCollider.Location;

            Rectangle frameRect = _spriteSheetEatShark.GetFrameRectangle(_sharkEatSkins.SharkEatLeft);
            RectangleF drawRect = new();

            var scale = 0.7;
            drawRect.Size = new SizeF((float)(frameRect.Width * scale), (float)(frameRect.Height * scale));

            drawRect.Y = drawLocation.Y + raftTileCollider.Height / 3;

            if (_model.Speed.X > 0)
            {
                drawRect.X = drawLocation.X - drawRect.Width + drawRect.Width / 7;

            }
            else
            {
                frameRect = _spriteSheetEatShark.GetFrameRectangle(_sharkEatSkins.SharkEatRight);
                drawRect.X = drawLocation.X + raftTileCollider.Width - drawRect.Width / 8;
            }


            var drawFoamrect = drawRect;
            drawFoamrect.Height = (float)(_frameEatFoamRactangle.Height * scale);

            drawFoamrect.X += _model.Speed.X < 0 ? 20: -20;

            graphics.DrawImage(_spriteSheetEatFoam.Texture.Image, drawFoamrect, _frameEatFoamRactangle, GraphicsUnit.Pixel);

            graphics.DrawImage(_spriteSheetEatShark.Texture.Image, drawRect, frameRect, GraphicsUnit.Pixel);
        }
    }
}