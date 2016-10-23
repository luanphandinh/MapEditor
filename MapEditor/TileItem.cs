using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapEditor
{
    class TileItem : ListViewItem
    {
        private Tile _tile;

        public Tile Tile
        {
            get { return _tile; }
        }
        //FrmMain sẽ có ListVIewItems dưới dạng largeImage
        //TileItem sẽ là ListViewItem để đưa vào Items của ListView trên form Main
        //TileItem sẽ chứ Tile được lấy từ mapcontroller.tilemap.tileset.tilelist.item
        //base của listviewItem bao gồm tring text và imageIndex
        //vì imageindex trên View được lưu từ index 0
        //nên khi add id Image vào thì phải trừ 1 do idTile được lưu từ 1
        //Name bằng tileName
        //gán tile = tile
        public TileItem(Tile tile)
            : base(tile.Name,tile.Id-1)
        {
            _tile = tile;
        }
        
    }
}
