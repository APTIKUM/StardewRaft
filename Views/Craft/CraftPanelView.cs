using StardewRaft.Core.Factories;
using StardewRaft.Models.Craft;
using StardewRaft.Views.Craft;
using StardewRaft.Core.Feature;
using StardewRaft.Properties;
using StardewRaft.Image.Structs;

namespace StardewRaft.Views.Inventory
{
    public class CraftPanelView : UserControl
    {
        public static SpiteSheetObjectsStruct<InventoryItemType> SpriteSheetInventoryItem { get; } = InventoryView.SpriteSheetInventoryItem;
        public static Font TitleFont { get; } = InventoryView.TitleFont;
        public static Font HeaderFont { get; } = InventoryView.HeaderFont;
        public static Brush TitleBrush { get; } = InventoryView.TitleBrush;
        public static Brush SlotBackgroundBrush { get; } = InventoryView.SlotBackgroundBrush;
        public static Brush SlotBorderBrush { get; } = InventoryView.SlotBorderBrush;
        public static Brush SlotActiveBorderBrush { get; } = InventoryView.SlotActiveBorderBrush;
        public static Color BackgroundColor { get; } = InventoryView.BackgroundColor;
        public Form ParentForm { get; private set; }

        public event Action<CraftRecipe> RequestCraftItem;

        public static SpiteSheetObjectsStruct<string> SpriteSheetGroupIcon { get; } = new(Resources.inventory_items_icon,
new ()
{
             { "Инструменты" , new Rectangle(0, 3 * 64, 64, 64)},
             { "Расходники" , new Rectangle(64, 3 * 64, 64, 64)},
             
});

        private bool _isOpen = false;
        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                _isOpen = value;
                UpdateSize();
                Invalidate();

            }
        }

        private int _btnMargin = 20;
        private int _btnHeight = 40;

        private Rectangle _panelSelectedItemRect;

        private Rectangle _btnCreate;

        public int CraftPanelPadding => 10;
        public int CraftPanelSpacing => 10;

        public int WidthGroupPanel { get; } = CraftPanelGroupSlot.FullSlotSize + CraftPanelGroupSlot.SlotSpacing;
        public int WidthItemsPanel { get; } = CraftPanelItemSlot.FullSlotSize + CraftPanelItemSlot.SlotSpacing + 150;

        private List<CraftPanelGroupSlot> _groupSlots;

        private Dictionary<CraftPanelGroupSlot, List<CraftPanelItemSlot>> _itemSlots = new();

        private CraftPanelGroupSlot? _hoveredGroupSlot;
        private CraftPanelGroupSlot? _selectedGroupSlot;

        private CraftPanelItemSlot? _hoveredItemSlot;
        private CraftPanelItemSlot? _selectedItemSlot;


        public Size CraftPanelSize { get; private set; }

        public CraftPanelView(Form parentForm)
        {

            ParentForm = parentForm;

            ParentForm.Controls.Add(this);
            ParentForm.Load += ParentForm_Load;

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Selectable, false);

            Location = new(CraftPanelPadding, CraftPanelPadding);

            Size = new(0, 0);

            InitializeUISlots();
            _selectedGroupSlot = _groupSlots?.FirstOrDefault();
            _selectedItemSlot = _itemSlots[_selectedGroupSlot].FirstOrDefault();

            SendToBack();

            Dock = DockStyle.None;
            BackColor = Color.Transparent;
            DoubleBuffered = true;

            MouseMove += OnMouseMove;
            MouseLeave += OnMouseLeave;

            MouseClick += OnMouseClick;

            _panelSelectedItemRect = new Rectangle(
                WidthGroupPanel + WidthItemsPanel + CraftPanelSpacing * 2,
                0,
                WidthItemsPanel,
                CraftPanelItemSlot.FullSlotSize + CraftPanelItemSlot.SlotSpacing + _btnMargin + _btnHeight + 45);

            _btnCreate = new Rectangle(
               _panelSelectedItemRect.X + _btnMargin,
               CraftPanelItemSlot.FullSlotSize + _btnMargin,
               _panelSelectedItemRect.Width - _btnMargin * 2,
               _btnHeight);
        }



        public void UpdateSize()
        {
            Size = IsOpen ? CraftPanelSize : new Size(0, 0);
        }

        private void InitializeUISlots()
        {
            _groupSlots = new();

            var x = CraftPanelGroupSlot.SlotSpacing;
            var y = CraftPanelGroupSlot.SlotSpacing;

            foreach (var group in CraftPanelModel.CraftGroups)
            {
                var groupSlot = new CraftPanelGroupSlot(group.Title, x, y);
                _groupSlots.Add(groupSlot);
                y += CraftPanelGroupSlot.FullSlotSize;

                _itemSlots[groupSlot] = CreateItemSlotsByGroup(group);
            }
        }

        private List<CraftPanelItemSlot> CreateItemSlotsByGroup(CraftGroup group)
        {
            var x = WidthGroupPanel + CraftPanelItemSlot.SlotSpacing + CraftPanelSpacing;
            var y = CraftPanelItemSlot.SlotSpacing;

            var slots = new List<CraftPanelItemSlot>();

            foreach (var item in group.CraftRecipes)
            {
                slots.Add(new(item, x, y));
                y += CraftPanelItemSlot.FullSlotSize;
            }

            return slots;
        }


        private void ParentForm_Load(object? sender, EventArgs e)
        {
            CraftPanelSize = new(WidthGroupPanel + WidthItemsPanel * 2 + CraftPanelSpacing * 2, ParentForm.Height);
        }



        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;

            if (IsOpen)
            {
                DrawGroups(g);
                DrawItems(g);
                DrawSelectedItem(g);
            }
        }

        private void DrawSelectedItem(Graphics g)
        {
            if (_selectedItemSlot is null)
            {
                return;
            }

            var panelRect = _panelSelectedItemRect;
            panelRect.Height += _selectedItemSlot.Recipe.Ingredients.Count * CraftPanelItemSlot.FullSlotSize;

            RoundedShapes.DrawRoundedRect(panelRect, 20, g, BackgroundColor);

            var selectedSlot = new CraftPanelItemSlot(_selectedItemSlot.Recipe,
                _panelSelectedItemRect.X + CraftPanelItemSlot.SlotSpacing,
                _panelSelectedItemRect.Y + CraftPanelItemSlot.SlotSpacing);

            g.FillRectangle(SlotBorderBrush, selectedSlot.BorderRect);
            g.FillRectangle(SlotBackgroundBrush, selectedSlot.SlotRect);
            g.DrawImage(SpriteSheetInventoryItem.Texture.Image, selectedSlot.SlotRect,
                SpriteSheetInventoryItem.GetFrameRectangle(_selectedItemSlot.Recipe.CraftItem), GraphicsUnit.Pixel);

            var itemText = $"{InventoryItemFactory.ItemsTitles[_selectedItemSlot.Recipe.CraftItem]} {_selectedItemSlot.Recipe.CraftCount} шт.";
            g.DrawString(itemText, TitleFont, TitleBrush,
                new RectangleF(selectedSlot.SlotRect.Right + 10, selectedSlot.SlotRect.Top, 150, selectedSlot.SlotRect.Height),
                new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });

            var padding = CraftPanelItemSlot.SlotBorderWidth;
            RoundedShapes.DrawRoundedRect(_btnCreate, 10, g, Color.FromArgb(180, 140, 115, 95));

            var buttonRect = new Rectangle(_btnCreate.X + padding, _btnCreate.Y + padding, _btnCreate.Width - padding * 2, _btnCreate.Height - padding * 2);
            RoundedShapes.DrawRoundedRect(buttonRect, 10, g, Color.FromArgb(140, 90, 65, 45));
            
            g.DrawString("Создать", TitleFont, TitleBrush, buttonRect,
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

            int requirementsY = _btnCreate.Bottom + 20;
            g.DrawString("Требуется:", TitleFont, TitleBrush, _panelSelectedItemRect.X + 15, requirementsY);

            int slotY = requirementsY + 30;
            int slotX = _panelSelectedItemRect.X + CraftPanelItemSlot.SlotSpacing;

            foreach (var ingredient in _selectedItemSlot.Recipe.Ingredients)
            {
                var ingredientSlot = new CraftPanelItemSlot(
                    new CraftRecipe(ingredient.ItemType, [], 1),
                    slotX,
                    slotY);

                g.FillRectangle(SlotBorderBrush, ingredientSlot.BorderRect);
                g.FillRectangle(SlotBackgroundBrush, ingredientSlot.SlotRect);


                g.DrawImage(SpriteSheetInventoryItem.Texture.Image, ingredientSlot.SlotRect,
                    SpriteSheetInventoryItem.GetFrameRectangle(ingredient.ItemType), GraphicsUnit.Pixel);

                var ingredientText = $"{InventoryItemFactory.ItemsTitles[ingredient.ItemType]} {ingredient.Amount} шт.";
                g.DrawString(ingredientText, TitleFont, TitleBrush,
                    new RectangleF(ingredientSlot.SlotRect.Right + 10, ingredientSlot.SlotRect.Top, 150, ingredientSlot.SlotRect.Height),
                    new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });

                slotY += CraftPanelItemSlot.FullSlotSize;
            }
        }

        private void DrawItems(Graphics g)
        {
            var nowItemsSlots = _itemSlots[_selectedGroupSlot];

            if (nowItemsSlots.Count == 0)
            {
                return;
            }

            RoundedShapes.DrawRoundedRect(
               new Rectangle(
                   WidthGroupPanel + CraftPanelSpacing,
                   0,
                   WidthItemsPanel,
                   CraftPanelItemSlot.FullSlotSize * nowItemsSlots.Count + CraftPanelItemSlot.SlotSpacing),
                   20, g, BackgroundColor);

            foreach (var item in nowItemsSlots)
            {
                var brushBorder = SlotBorderBrush;

                if (item == _hoveredItemSlot || item == _selectedItemSlot)
                {
                    brushBorder = SlotActiveBorderBrush;
                }

                g.FillRectangle(brushBorder, item.BorderRect);

                g.FillRectangle(SlotBackgroundBrush, item.SlotRect);

                g.DrawImage(SpriteSheetInventoryItem.Texture.Image, item.SlotRect,
                    SpriteSheetInventoryItem.GetFrameRectangle(item.Recipe.CraftItem), GraphicsUnit.Pixel);

                var itemName = InventoryItemFactory.ItemsTitles[item.Recipe.CraftItem];

                RectangleF textRect = new RectangleF(
                    item.SlotRect.Right + 10, 
                    item.SlotRect.Top,
                    150,
                    item.SlotRect.Height);

                var stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center
                };

                g.DrawString(itemName, TitleFont, TitleBrush, textRect, stringFormat);   
            }
        }


        private void DrawGroups(Graphics g)
        {
            RoundedShapes.DrawRoundedRect(
               new Rectangle(
                   0,
                   0,
                   WidthGroupPanel,
                   CraftPanelGroupSlot.FullSlotSize * _groupSlots.Count + CraftPanelGroupSlot.SlotSpacing),
               20, g, BackgroundColor);

            foreach (var group in _groupSlots)
            {
                var brushBorder = SlotBorderBrush;
                
                if (group == _hoveredGroupSlot || group == _selectedGroupSlot)
                {
                    brushBorder = SlotActiveBorderBrush;
                }
                
                g.FillRectangle(brushBorder, group.BorderRect);                

                g.FillRectangle(SlotBackgroundBrush, group.SlotRect);

                g.DrawImage(SpriteSheetGroupIcon.Texture.Image, group.SlotRect,
                   SpriteSheetGroupIcon.GetFrameRectangle(group.Title), GraphicsUnit.Pixel);


            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            SelectHoveredSlots(e.Location);
        }

        private void OnMouseLeave(object? sender, EventArgs e)
        {
            _hoveredGroupSlot = null;
            _hoveredItemSlot = null;
        }


        private void SelectHoveredSlots(Point location)
        {
            _hoveredGroupSlot = _groupSlots.FirstOrDefault(s => s.BorderRect.Contains(location));

            _hoveredItemSlot = _itemSlots[_selectedGroupSlot].FirstOrDefault(s => s.BorderRect.Contains(location));
        }

        private void OnMouseClick(object? sender, MouseEventArgs e)
        {
            if (_selectedItemSlot is not null)
            {
                if (_btnCreate.Contains(e.Location))
                {
                    RequestCraftItem?.Invoke(_selectedItemSlot.Recipe);
                }
            }

            if (_hoveredGroupSlot is not null)
            {
                _selectedGroupSlot = _hoveredGroupSlot;
                _selectedItemSlot = _itemSlots[_selectedGroupSlot].FirstOrDefault();
            }
            else
            {
                _selectedItemSlot = _hoveredItemSlot ?? _selectedItemSlot;
            }
        }
    }
}