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

        //base của listviewItem bao gồm tring text và imageIndex
        //vì id của tile được lưu từ 1 nên imgae index sẽ lưu từ 0
        //gán tile = tile
        public TileItem(Tile tile)
            : base(tile.Name,tile.Id-1)
        {
            _tile = tile;
        }
        
    }
}
