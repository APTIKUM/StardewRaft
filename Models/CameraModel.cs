using System.Numerics;

namespace StardewRaft.Models;

public class CameraModel
{
    public Vector2 Position { get; private set; }
    public Form RenderForm { get; private set; }
    public Size DeadZone { get; private set; } = new Size(300, 300);
    public PlayerModel Target { get; private set; }
    public float Scale { get; private set; }

    private float _shiftScale = 0.05f;
    private float MaxScale = 2;
    private float MinScale = 1;


    public CameraModel(PlayerModel playerTarget, Form renderForm)
    {
        Target = playerTarget;
        RenderForm = renderForm;
        Scale = MinScale;
    }

    public void ChangeScale(bool toBig = true)
    {
        Scale = Math.Clamp(Scale + _shiftScale * (toBig ? 1 : -1), MinScale, MaxScale);
    }

    public void Update()
    {
        if (Target == null)
        { 
            return;
        }

        var screenCenter = new PointF(RenderForm.ClientSize.Width / 2f, RenderForm.ClientSize.Height / 2f);

        var deltaX = Target.Position.X - (Position.X + screenCenter.X);
        var deltaY = Target.Position.Y - (Position.Y + screenCenter.Y);

        var newPositionX = Position.X;
        var newPositionY = Position.Y;

        if (Math.Abs(deltaX) > DeadZone.Width / 2)
        {
            newPositionX = (int)(Target.Position.X - screenCenter.X - Math.Sign(deltaX) * DeadZone.Width / 2);
        }

        if (Math.Abs(deltaY) > DeadZone.Height / 2)
        {
            newPositionY = (int)(Target.Position.Y - screenCenter.Y - Math.Sign(deltaY) * DeadZone.Height / 2);
        }

        Position = new Vector2(newPositionX, newPositionY);
    }

    public Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        float screenCenterX = RenderForm.ClientSize.Width / 2f;
        float screenCenterY = RenderForm.ClientSize.Height / 2f;

        return new Vector2(
            (screenPosition.X - screenCenterX) / Scale + Position.X + screenCenterX,
            (screenPosition.Y - screenCenterY) / Scale + Position.Y + screenCenterY);
    }

    public Point ScreenToWorld(Point screenPosition)
    {
        float screenCenterX = RenderForm.ClientSize.Width / 2f;
        float screenCenterY = RenderForm.ClientSize.Height / 2f;

        return new Point(
            (int)((screenPosition.X - screenCenterX) / Scale + Position.X + screenCenterX),
            (int)((screenPosition.Y - screenCenterY) / Scale + Position.Y + screenCenterY));
    }
}