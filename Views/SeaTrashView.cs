using StardewRaft.Core.Factories;
using StardewRaft.Image.Structs;
using StardewRaft.Models;
using StardewRaft.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewRaft.Views
{
    public class SeaTrashView
    {
        //private SpriteSheetAnimateStruct SpriteSheetInventoryItem = new SpriteSheetAnimateStruct(Resources.sea_trash,
        //    21, 21,
        //    new() { { 0, 6 }, { 1, 6 }, { 2, 6 } });

        public static SpiteSheetObjectsStruct<string> SpriteSheetSeaTrash { get; } =  new (Resources.sea_trash,
            new Dictionary<string, Rectangle>() 
            {
                { "plastic_white_0" , new Rectangle(0, 0, 21, 21)},
                { "plastic_white_1" , new Rectangle(21, 0, 21, 21)},
                { "plastic_red_0" , new Rectangle(42, 5, 21, 16)},
                { "plastic_red_1" , new Rectangle(63, 5, 21, 16)},
                { "plastic_bottle_0" , new Rectangle(84, 9, 21, 12)},
                { "plastic_bottle_1" , new Rectangle(105, 9, 21, 12)},

                { "leaf_0" , new Rectangle(0, 24, 21, 18)},
                { "leaf_1" , new Rectangle(21, 22, 21, 20)},
                { "leaf_2" , new Rectangle(42, 22, 21, 20)},
                { "leaf_3" , new Rectangle(63, 25, 21, 17)},
                { "leaf_4" , new Rectangle(84,25, 21, 17)},
                { "leaf_5" , new Rectangle(105, 26, 21, 16)},


                { "wood_0" , new Rectangle(0, 42, 30, 10)},
                { "wood_1" , new Rectangle(32, 42, 30, 10)},
                { "wood_2" , new Rectangle(0, 53, 33, 10)},
                { "wood_3" , new Rectangle(35, 53, 33, 10)},
                { "wood_4" , new Rectangle(64, 42, 29, 7)},
                { "wood_5" , new Rectangle(95, 42, 29, 7)},

                { "barrel_0" , new Rectangle(0, 63, 24, 31)},
                { "barrel_1" , new Rectangle(26, 63, 24, 31)},
                { "barrel_2" , new Rectangle(52, 63, 22, 34)},
            });

        private Random _rnd = new();

        private SeaTrashModel _model;
        private Form _renderForm;
        
        public SeaTrashView(SeaTrashModel model, Form renderForm)
        {
            _model = model;
            _renderForm = renderForm;

            _model.ModelUpdated += OnModelUpdated;
        }

        private void OnModelUpdated()
        {
            _renderForm.Invalidate();
        }


        public void Draw(Graphics graphics)
        {

            foreach(var trash in _model.SeaTrashList.OrderBy(m => m.Collider.Bottom))
            {
                var _frameSeaTrashRactangle = SpriteSheetSeaTrash.GetFrameRectangle(trash.Skin);


                graphics.DrawImage(SpriteSheetSeaTrash.Texture.Image, trash.Collider, _frameSeaTrashRactangle, GraphicsUnit.Pixel);
                //graphics.DrawRectangle(new Pen(Brushes.Red, 1), trash.Collider);

            }

        }

       


    }
}
