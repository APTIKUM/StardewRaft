using Timer = System.Windows.Forms.Timer;
using System.Numerics;
using StardewRaft.Views;
using StardewRaft.Models;
using StardewRaft.Controllers;
using StardewRaft.Managers;
using StardewRaft.Models.Raft;
using StardewRaft.Views.Inventory;
using StardewRaft.Core.Factories;
using System.Diagnostics;
using StardewRaft.Core.Feature;
using StardewRaft.Models.Craft;
using StardewRaft.Properties;
using StardewRaft.Core.Factories.Inventory;


namespace StardewRaft
{
    public partial class GameForm : Form
    {
        public int CenterX => ClientSize.Width / 2;
        public int CenterY => ClientSize.Height / 2;

        private readonly WindModel _windModel;
        private readonly WindManager _windManager;

        private readonly InventoryModel _inventoryModel;
        private readonly InventoryView _inventoryView;
        private readonly InventoryController _inventoryController;

        private readonly CraftPanelView _craftPanelView;

        private readonly CameraModel _cameraModel;
        private readonly CameraManager _cameraManager;

        private readonly SeaBackgroundModel _seaModel;
        private readonly SeaBackgroundView _seaView;
        private readonly SeaBackgroundManager _seaManager;

        private readonly SharkModel _sharkModel;
        private readonly SharkView _sharkView;
        private readonly SharkManager _sharkManager;

        private readonly RaftModel _raftModel;
        private readonly RaftView _raftView;
        private readonly RaftController _raftController;

        private readonly SeaTrashModel _seaTrashModel;
        private readonly SeaTrashView _seaTrashView;
        private readonly SeaTrashManager _seaTrashManager;

        private readonly PlayerModel _playerModel;
        private readonly PlayerController _playerController;
        private readonly PlayerView _playerView;
        private readonly PlayerStatsView _playerStatsView;

        private readonly Timer _gameMainTimer;

        private readonly Label _label;

        private Stopwatch _stopwatchPowerClick = new();

        private CustomMouseEventArgs _customMouseEventArgs;

        private Button _gameOverBtn;
        private bool _isGameOver = false;

        private void InitializeGameOverLabel()
        {
            _gameOverBtn = new Button
            {
                Visible = false,
                Text = "ИГРА ОКОНЧЕНА",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(200, 180, 130, 100),
                BackColor = InventoryView.BackgroundColor,
                AutoSize = true, 
                AutoSizeMode = AutoSizeMode.GrowAndShrink, 
                MinimumSize = new Size(250, 60), 
                Padding = new Padding(20, 10, 20, 10),
                Location = new Point(25, 100),
                FlatStyle = FlatStyle.Flat,
            };

            _gameOverBtn.Click += _gameOverBtn_Click;

            _gameOverBtn.Location = new Point(
                (ClientSize.Width - _gameOverBtn.Width) / 2,
                (ClientSize.Height - _gameOverBtn.Height) / 2);

            Controls.Add(_gameOverBtn);
            _gameOverBtn.BringToFront();
        }

        private void _gameOverBtn_Click(object? sender, EventArgs e)
        {
            Close();

            var mainMenu = new MainMenuForm();
            mainMenu.Show();
        }

        private bool _isMouseHold = false;
        public double PowerClick => Math.Clamp(_stopwatchPowerClick.ElapsedMilliseconds / 1000.0, 0, 1.0);

        private bool[] _pressedKeys = new bool[256];
        public GameForm()
        {
            InitializeComponent();
            DoubleBuffered = true; 

            _gameMainTimer = new Timer { Interval = 1000 / 60 };

            _windModel = new WindModel(new Vector2(1f, -0.7f)); 
            _windManager = new WindManager(_windModel);

            _raftModel = new RaftModel(CenterX, CenterY);
            _raftView = new RaftView(_raftModel, this);
            _raftController = new RaftController(_raftModel);


            _inventoryModel = new InventoryModel();
            _inventoryView = new InventoryView(_inventoryModel, this);
            _inventoryController = new InventoryController(_inventoryModel, _inventoryView);


            _playerModel = new PlayerModel(CenterX, CenterY, _raftModel, _inventoryModel);
            _playerView = new PlayerView(_playerModel, this);
            _playerController = new PlayerController(_playerModel);
            _playerStatsView = new PlayerStatsView(_playerModel, this);

            _craftPanelView = new CraftPanelView(this);

            _cameraModel = new CameraModel(_playerModel, this);
            _cameraManager = new CameraManager(_cameraModel);

            _seaModel = new SeaBackgroundModel(this, _cameraModel,_windModel);
            _seaView = new SeaBackgroundView(_seaModel, this);
            _seaManager = new SeaBackgroundManager(_seaModel);

            _sharkModel = new SharkModel(_seaModel);
            _sharkView = new SharkView(_sharkModel, this);
            _sharkManager = new SharkManager(_sharkModel, _raftModel);

            _seaTrashModel = new SeaTrashModel(_seaModel, _raftModel);
            _seaTrashView = new SeaTrashView(_seaTrashModel, this);
            _seaTrashManager = new SeaTrashManager(_seaTrashModel);

            HookModel.PlayerModel = _playerModel;
            HookModel.SeaTrashModel = _seaTrashModel;
            HookModel.RaftModel = _raftModel;

            BuildingHammerModel.RaftController = _raftController;
            BuildingHammerModel.PlayerModel = _playerModel;

            CraftPanelModel.Player = _playerModel;
            CraftPanelController.View = _craftPanelView;

            _label = new Label();
            _label.AutoSize = true;
            _label.Font = new Font("Arial", 20);
            _label.Location = new Point(0, 0);
            Controls.Add(_label);


            InitializeGameOverLabel();

            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;

            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;

            _gameMainTimer.Tick += MainGameLoop; 
            _gameMainTimer.Start();

            MouseWheel += OnMouseWheel;

            KeyPreview = true; 
        }

        public void ShowGameOver(bool show)
        {
            _gameOverBtn.Visible = show;

            if (show)
            {
                _gameOverBtn.Location = new Point(
                    (ClientSize.Width - _gameOverBtn.Width) / 2,
                    (ClientSize.Height - _gameOverBtn.Height) / 2);
            }
        }

        private void EndGame()
        {
            _isGameOver = true;

            MouseDown -= OnMouseDown;
            MouseUp -= OnMouseUp;

            KeyDown -= OnKeyDown;
            KeyUp -= OnKeyUp;

            ShowGameOver(true);
        }

        private void OnMouseUp(object? sender, MouseEventArgs e)
        {
            _isMouseHold = false;

            _customMouseEventArgs = CustomMouseEventArgs.CreateByParent(e, PowerClick, _cameraModel.ScreenToWorld(e.Location));

            _stopwatchPowerClick.Stop();
            _stopwatchPowerClick.Reset();


            //if (_pressedKeys[(int)Keys.B])
            //{
            //    _raftController.RequestChangeRaft(_customMouseEventArgs);
            //    return;
            //}

            if (TryTakeSeaTrashByClick(_customMouseEventArgs))
            {
                return;
            }

            if (_playerController.TryUseActiveInventoryItem(_customMouseEventArgs))
            {
                return;
            }
        }

        private void OnMouseDown(object? sender, MouseEventArgs e)
        {
            _stopwatchPowerClick.Restart();
            
            _isMouseHold = true;

            _customMouseEventArgs = CustomMouseEventArgs.CreateByParent(e, PowerClick, _cameraModel.ScreenToWorld(e.Location));
        }

        private void OnMouseWheel(object? sender, MouseEventArgs e)
        {
            if (_pressedKeys[(int)Keys.Z])
            {
                _cameraManager.ChangeScale(e.Delta > 0);
            }
            else
            {
                var indexSlot = _inventoryModel.IndexActiveSlot + (Math.Sign(e.Delta));

                indexSlot = Math.Clamp(indexSlot, 0, 9);

                _inventoryController.SetActiveSlot(indexSlot);
            }
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.I)
            {
                _inventoryView.ToggleInventory();
                CraftPanelController.ChangeOpen(_inventoryView.IsOpen);
                e.Handled = true;
            }
            else if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
            {
                var indexSlot = (int)e.KeyCode - (int)Keys.D0 - 1;
                if (indexSlot < 0)
                {
                    indexSlot = 9;
                }

                _inventoryController.SetActiveSlot(indexSlot);
            }
            else if (e.KeyCode == Keys.Q)
            {
                _playerModel.TryAddToInventory(InventoryItemFactory.CreateResourcesBySeaTrash(new SeaTrash(SeaTrashType.Barrel, "", new(), new())).First());
            }
            
            _pressedKeys[e.KeyValue] = true;
        }

        private void OnKeyUp(object? sender, KeyEventArgs e)
        {
            _pressedKeys[e.KeyValue] = false;
        } 

        private bool TryTakeSeaTrashByClick(CustomMouseEventArgs e)
        {
            if (!_playerController.IsInInteractionRange(e.Location))
            {
                return false;
            }

            var trash = _seaTrashManager.TryGetTrashByPosition(e.Location);

            if (trash is null)
            {
                return false;
            }

            var inventoryItems = InventoryItemFactory.CreateResourcesBySeaTrash(trash);

            var successTakeSeaTrash = false;
            foreach (var item in inventoryItems)
            {
                if (_playerController.TryAddToInventory(item))
                {
                    successTakeSeaTrash = true;
                }
            }

            if (successTakeSeaTrash)
            {
                _seaTrashManager.RemoveTrash(trash);
            }

            return successTakeSeaTrash;
        }

        private void MainGameLoop(object? sender, EventArgs e)
        {
            if (_playerModel.Health <= 0)
            {
                EndGame();
            }
            else
            {
                _playerController.UpdateModel(_pressedKeys);
            }

            _windManager.UpdateModel();
            _seaManager.UpdateModel();
            _raftController.UpdateModel();
            
            _sharkManager.UpdateModel();
            _cameraManager.UpdateMovel();
            _seaTrashManager.UpdateModel();
            HookController.UpdateModel();

            if (_isMouseHold)
            {

                var localEventArgs = new CustomMouseEventArgs(_customMouseEventArgs.Button,
                                        _customMouseEventArgs.Clicks,
                                        _customMouseEventArgs.X,
                                        _customMouseEventArgs.Y,
                                        _customMouseEventArgs.Delta,
                                        PowerClick);

                _playerController.TryUseActiveInventoryItemOnMouseDown(localEventArgs);
                
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            var originalState = g.Save();

            g.Clip = new Region(ClientRectangle);

            g.TranslateTransform(CenterX, CenterY);
            g.ScaleTransform(_cameraModel.Scale, _cameraModel.Scale);
            g.TranslateTransform(-CenterX, -CenterY);
            g.TranslateTransform(-_cameraModel.Position.X, -_cameraModel.Position.Y);

            _seaView.Draw(g);

            if (!_sharkModel.IsEatingRaft)
            {
                _sharkView.DrawFin(g);
            }

            _raftView.DrawBottom(g);

            _seaTrashView.Draw(g);
            _raftView.Draw(g);

            if (_inventoryModel.ActiveSlot is InventoryBuildingHammer && !_inventoryView.IsOpen && !_isGameOver)
            {
                var RaftTile = new RaftTile(BuildingHammerModel.RaftController.GetTileOnRaftPosition(_cameraModel.ScreenToWorld(MousePosition)));
                g.DrawImage(Resources.raft_tile_sprite_0_building_hammer, RaftTile.Collider);
            }

            HookView.Draw(g);
            _playerView.Draw(g);

            if (!HookModel.IsActive && _customMouseEventArgs is not null && _customMouseEventArgs.Button == MouseButtons.Left)
            {
                _playerView.DrawClickPowerByHook(g, PowerClick);
            }

            if (_sharkModel.IsEatingRaft)
            {
                _sharkView.DrawEatShark(g);
            }

            g.Restore(originalState);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.None;
            Bounds = Screen.PrimaryScreen.Bounds;
        }
    }
}
