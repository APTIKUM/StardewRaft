using StardewRaft.Core.Feature;
using StardewRaft.Properties;
using StardewRaft.Core.Factories;
using StardewRaft.Models;
using StardewRaft.Core.Factories.InventoryItem;
using StardewRaft.Models.Craft;
using StardewRaft.Image.Structs;

namespace StardewRaft.Views.Inventory
{
    public class InventoryView : UserControl
    {
        public InventoryModel Inventory { get; private set; }

        private readonly int _columnsCountClosed = 10;
        private readonly int _columnsCountOpen = 5;

        public static SpiteSheetObjectsStruct<InventoryItemType> SpriteSheetInventoryItem { get; } = new(Resources.inventory_items_icon,
new Dictionary<InventoryItemType, Rectangle>()
{
             { InventoryItemType.Wood , new Rectangle(0, 0, 64, 64)},
             { InventoryItemType.Leaf , new Rectangle(64, 0, 64, 64)},
             { InventoryItemType.Plastic , new Rectangle(128, 0, 64, 64)},
             { InventoryItemType.Rope , new Rectangle(128 + 64, 0, 64, 64)},
             { InventoryItemType.Potato , new Rectangle(0, 64, 64, 64)},
             { InventoryItemType.Apple , new Rectangle(64, 64, 64, 64)},
             { InventoryItemType.Banana , new Rectangle(128, 64, 64, 64)},
             { InventoryItemType.Hook , new Rectangle(0, 128, 64, 64)},
             { InventoryItemType.BuildingHammer , new Rectangle(64, 128, 64, 64)},
});

        public static Font TitleFont { get; } = new Font("Arial", 12, FontStyle.Bold);
        public static Font HeaderFont { get; } = new Font("Arial", 16, FontStyle.Bold);

        public static Brush TitleBrush { get; } = new SolidBrush(Color.FromArgb(255, 190, 175, 165));
        public static Brush SlotBackgroundBrush { get; } = new SolidBrush(Color.FromArgb(140, 90, 65, 45));
        public static Brush SlotBorderBrush { get; } = new SolidBrush(Color.FromArgb(180, 140, 115, 95));
        public static Brush SlotActiveBorderBrush { get; } = new SolidBrush(Color.FromArgb(200, 180, 130, 100));
        public static Color BackgroundColor { get; } = Color.FromArgb(255, 90, 75, 60);

        private List<InventorySlot> _quickSlots;
        private List<InventorySlot> _fullSlots;

        private InventorySlot? _hoveredSlot = null;
        private InventorySlot? _dragDropSlot = null;

        private int _headerHeight = InventorySlot.FullSlotSize;
        private int _footerHeight = InventorySlot.FullSlotSize;

        private int _deleteBoxSize = (int)(InventorySlot.FullSlotSize / 2.0);
        private Rectangle _deleteBoxRect => new Rectangle(
            (Width - _deleteBoxSize) / 2,
            (Height + InventorySize.Height) / 2 - _deleteBoxSize - InventoryPadding - (_footerHeight - _deleteBoxSize) / 2,
            _deleteBoxSize,
            _deleteBoxSize);

        public event Action<int, int> RequestDragAndDropItem;
        public event Action<int> RequestDeleteItemAt;
        public event Action<int> RequestSetActiveSlot;

        public bool IsOpen { get; private set; } = false;

        public int InventoryPadding => InventorySlot.SlotSpacing;

        public int SlotsCount => IsOpen ? Inventory.MaxCapacity : _columnsCountClosed;
        public int ColumnsCount => IsOpen ? _columnsCountOpen : _columnsCountClosed;

        private int _rowsCountOpen;
        private int _rowsCountClosed;
        public int RowsCount => IsOpen ? _rowsCountOpen : _rowsCountClosed;

        public Size InventorySize => IsOpen ? _inventoryOpenSize : _inventoryClosedSize;
        private Size _inventoryOpenSize;
        private Size _inventoryClosedSize;


        public void CalculateInventorySizes()
        {
            _rowsCountOpen = (int)Math.Ceiling((double)Inventory.MaxCapacity / _columnsCountOpen);
            _rowsCountClosed = 1;

            var inventoryWidthOpen = _columnsCountOpen * InventorySlot.FullSlotSize - InventorySlot.SlotSpacing
                + InventoryPadding * 2;

            var inventoryWidthClosed = _columnsCountClosed * InventorySlot.FullSlotSize - InventorySlot.SlotSpacing
                + InventoryPadding * 2;

            var inventoryHeightOpen = _rowsCountOpen * InventorySlot.FullSlotSize
                - InventorySlot.SlotSpacing + _headerHeight + _footerHeight
                + InventoryPadding * 2;

            var inventoryHeightClosed = InventorySlot.FullSlotSize - InventorySlot.SlotSpacing
                + InventoryPadding * 2;

            _inventoryOpenSize = new Size(inventoryWidthOpen, inventoryHeightOpen);
            _inventoryClosedSize = new Size(inventoryWidthClosed, inventoryHeightClosed);
        }

        public InventoryView(InventoryModel inventory, Form parentForm)
        {
            Inventory = inventory;

            parentForm.Controls.Add(this);
            parentForm.Load += ParentForm_Load;

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Selectable, false);

            Dock = DockStyle.None;
            BackColor = Color.Transparent;
            DoubleBuffered = true;

            Inventory.ModelUpdated += Invalidate;

            MouseMove += OnMouseMove;
            MouseLeave += OnMouseLeave;

            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;

            InventorySlot.BaseInventory = this;
            CalculateInventorySizes();
        }

        private void ParentForm_Load(object? sender, EventArgs e)
        {
            UpdateSizeAndPosition();
            UpdateSlots();
        }

        public void UpdateSlots()
        {
            var oldState = IsOpen;

            IsOpen = false;
            _quickSlots = CreateSlots();

            IsOpen = true;
            _fullSlots = CreateSlots();

            IsOpen = oldState;
        }

        private List<InventorySlot> CreateSlots()
        {
            var resultSlots = new List<InventorySlot>();

            var startY = IsOpen
                ? (Parent.Height - InventorySize.Height) / 2 + InventoryPadding + _headerHeight
                : InventoryPadding;

            var startX = IsOpen
                ? (Parent.Width - InventorySize.Width) / 2 + InventoryPadding
                : InventoryPadding;

            for (int row = 0; row < RowsCount; row++)
            {
                var y = startY + row * InventorySlot.FullSlotSize;

                for (int column = 0; column < ColumnsCount; column++)
                {
                    var x = startX + column * InventorySlot.FullSlotSize;

                    var indexItem = row * ColumnsCount + column;
                    resultSlots.Add(new InventorySlot(indexItem, x, y));
                }
            }

            return resultSlots;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var graphics = e.Graphics;

            RoundedShapes.DrawRoundedRect(
                new Rectangle(
                    (Width - InventorySize.Width) / 2,
                    (Height - InventorySize.Height) / 2,
                    InventorySize.Width,
                    InventorySize.Height),
                20, graphics, BackgroundColor);

            if (IsOpen)
            {
                SendToBack();
                DrawInventoryHeader(graphics);

                DrawPanelSlots(graphics, _fullSlots);

                DrawInventoryFooter(graphics);
            }
            else
            {
                //BringToFront();
                DrawPanelSlots(graphics, _quickSlots);
            }


            if (_dragDropSlot != null)
            {
                var location = PointToClient(MousePosition);

                var renderRect = new Rectangle(location, _dragDropSlot.SlotRect.Size);
                renderRect.X -= renderRect.Width / 2;
                renderRect.Y -= renderRect.Height / 2;

                DrawSlotData(graphics, _dragDropSlot, renderRect);
            }
        }

        private void DrawInventoryFooter(Graphics graphics)
        {
            graphics.DrawImage(Resources.sprite_sheet_interface_icon, _deleteBoxRect);
        }

        private void DrawInventoryHeader(Graphics graphics)
        {
            var headerText = "Инвентарь";
            var headerTextSize = graphics.MeasureString(headerText, HeaderFont);
            graphics.DrawString(headerText, HeaderFont, TitleBrush,
                (Width - headerTextSize.Width) / 2,
                (Height - InventorySize.Height) / 2 + InventoryPadding);

            var selectedSlot = _dragDropSlot ?? _hoveredSlot;

            if (selectedSlot != null)
            {
                var item = selectedSlot.InventoryItem;
                if (item != null)
                {
                    var text = item.Title;

                    if (item is IInvetoryTool tool)
                    {
                        text += $" {(int)(tool.NowDurability / (float)tool.MaxDurability * 100)}%";
                    }

                    var titleSize = graphics.MeasureString(text, TitleFont);

                    graphics.DrawString(text, TitleFont, TitleBrush,
                        (Width - titleSize.Width) / 2,
                        (Height - InventorySize.Height) / 2 + InventoryPadding + (_headerHeight + headerTextSize.Height - titleSize.Height) / 2);
                }
            }
        }

        private void DrawSlotData(Graphics graphics, InventorySlot slot, Rectangle? drawRect = null)
        {
            var rect = drawRect ?? slot.SlotRect;

            var item = slot.InventoryItem;

            if (item != null)
            {
                var _frameInventoryItemRect = SpriteSheetInventoryItem.GetFrameRectangle(item.Type);

                graphics.DrawImage(SpriteSheetInventoryItem.Texture.Image, rect, _frameInventoryItemRect, GraphicsUnit.Pixel);

                if (item.Count > 1)
                {
                    var countText = item.Count.ToString();
                    var countSize = graphics.MeasureString(countText, TitleFont);

                    graphics.DrawString(countText, TitleFont, TitleBrush,
                        rect.Right - countSize.Width + InventorySlot.SlotBorderWidth,
                        rect.Bottom - countSize.Height + InventorySlot.SlotBorderWidth);
                }
                if (item is IInvetoryTool tool && tool.NowDurability < tool.MaxDurability)
                {
                    var margin = 0;
                    var height = 5;

                    float fillWidth = (float)tool.NowDurability / tool.MaxDurability * rect.Width;

                    rect.Y = rect.Bottom - height - margin;
                    rect.Height = height;
                    rect.Width = (int)fillWidth;

                    graphics.FillRectangle(Brushes.YellowGreen, rect);
                }
            }

        }

        private void DrawPanelSlots(Graphics graphics, List<InventorySlot> slots)
        {

            foreach (var slot in slots)
            {
                if ((slot.IndexItem == Inventory.IndexActiveSlot && !IsOpen)
                    || slot == _hoveredSlot)
                {
                    graphics.FillRectangle(SlotActiveBorderBrush, slot.BorderRect);

                }
                else
                {
                    graphics.FillRectangle(SlotBorderBrush, slot.BorderRect);
                }

                graphics.FillRectangle(SlotBackgroundBrush, slot.SlotRect);

                if (slot != _dragDropSlot)
                {
                    DrawSlotData(graphics, slot);
                }

            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            SelectHoveredSlot(e.Location);
        }

        private void OnMouseLeave(object? sender, EventArgs e)
        {
            _hoveredSlot = null;
            _dragDropSlot = null;
        }

        private InventorySlot? GetSlotByLocation(Point location)
        {
            var nowSlots = IsOpen ? _fullSlots : _quickSlots;
            var resultSlot = nowSlots.FirstOrDefault(s => s.BorderRect.Contains(location));

            return resultSlot;
        }

        private void SelectHoveredSlot(Point location)
        {
            var oldHovered = _hoveredSlot;

            _hoveredSlot = GetSlotByLocation(location);

            if (oldHovered != _hoveredSlot)
            {
                Invalidate();
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (IsOpen)
            {
                if (_hoveredSlot != null && _hoveredSlot.InventoryItem != null)
                {
                    if (_dragDropSlot == null)
                    {
                        _dragDropSlot = _hoveredSlot;
                    }
                }
            }
            else
            {
                if (_hoveredSlot != null && _hoveredSlot.IndexItem != Inventory.IndexActiveSlot)
                {
                    RequestSetActiveSlot(_hoveredSlot.IndexItem);
                }
            }

        }

        private void OnMouseUp(object? sender, MouseEventArgs e)
        {
            if (_dragDropSlot != null)
            {

                if (_hoveredSlot != null)
                {
                    RequestDragAndDropItem?.Invoke(_dragDropSlot.IndexItem, _hoveredSlot.IndexItem);
                }
                else if (_deleteBoxRect.Contains(e.Location))
                {
                    RequestDeleteItemAt(_dragDropSlot.IndexItem);
                }

                _dragDropSlot = null;
            }

        }

        public void ToggleInventory()
        {
            IsOpen = !IsOpen;

            _hoveredSlot = null;
            _dragDropSlot = null;

            UpdateSizeAndPosition();
            Invalidate();
        }

        public void UpdateSizeAndPosition()
        {
            if (Parent == null) return;


            Width = IsOpen ? Parent.Width : InventorySize.Width;
            Height = IsOpen ? Parent.Height : InventorySize.Height;

            Top = IsOpen ? (Parent.Height - Height) / 2 : Parent.Height - Height - InventoryPadding;
            Left = (Parent.Width - Width) / 2;

        }


    }
}