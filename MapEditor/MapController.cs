using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace MapEditor
{
    class MapController
    {
        public static Size MapSize { get; set; }

        //FIELDS

        //Tham chiếu đến buffer của tablelayout.creategraphics
        private Graphics _graphics;

        //hình chữ nhật gần nhất dùng làm khung để vẽ map
        private Rectangle _lastVisibleRect;


        //tileMap
        private TilesMap _tilesMap;


        //PROPERTIES
        public Graphics Graphics
        {
            get { return _graphics; }
            set { _graphics = value; }
        }

        

        public TilesMap TilesMap
        {
            get { return _tilesMap; }
            set 
            {
                if (_tilesMap != value)
                {
                    _tilesMap = value;
                    if (value != null)
                    {
                        value.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
                            {
                                if (e.PropertyName == "Columns" || e.PropertyName == "Rows")
                                {
                                    MapController.MapSize = new Size(
                                        _tilesMap.Columns * FrmMain.Settings.TileSize.Width,
                                        _tilesMap.Rows * FrmMain.Settings.TileSize.Height);
                                }
                            };
                    }
                }
            }
        }
        

        public MapController()
        { 
        }

        ///OPen FrmCreateTiles
        public void CreateTileSet()
        {
            var _createTileFrm = new FrmCreateTiles();
            DialogResult result = _createTileFrm.ShowDialog();

            //lấy dữ liệu từ frm
            if (result == DialogResult.OK)
            { 
                if(this.TilesMap == null)
                {
                    this.TilesMap = new TilesMap(10, 10);
                    this.TilesMap.TileSet = _createTileFrm.Tileset;
                }
            }
        }

        //lấy listViewItem để gán cho ListView tren FrmMain
        public IList<ListViewItem> getListViewItem()
        {
            if (this.TilesMap == null)
                return null;
            if (this.TilesMap.TileSet == null)
                return null;
            List<ListViewItem> result = new List<ListViewItem>();
            foreach (Tile tile in this.TilesMap.TileSet.ListTiles)
            {
                result.Add(new TileItem(tile));
            }
            return result;
        }
        //lấy danh sách các tile để gán cho listView trên FrmMain
        public ImageList getImageList()
        {
            if (this.TilesMap == null)
                return null;
            if (this.TilesMap.TileSet == null)
                return null;
            ImageList imageList = new ImageList();
            foreach (var tile in _tilesMap.TileSet.ListTiles)
            {
                imageList.Images.Add(tile.getBitmap());
            }
            imageList.ImageSize = new Size(40, 40);
            return imageList;
        }

        //Sự kiện được kích hoạt mỗi khi vẽ lại map hoặc vẽ 1 tile mới lên map
        //https://msdn.microsoft.com/en-us/library/aa645739(v=vs.71).aspx
        //https://msdn.microsoft.com/en-us/library/ms173171.aspx
        public event EventHandler Drawn;

        protected void OnDrawn(EventArgs e)
        {
            if (Drawn != null)
            {
                Drawn(this, e);
            }
        }

        //Duyệt matrixindex và vẽ map
        //Chỉ vẽ các tile nằm trong màn hình
        //Vẽ tất cả các Object
        public void Draw(Rectangle visibleRectangle)
        {
            if (this._graphics == null)
                return;
            if (this.TilesMap == null)
                return;
            if (this.TilesMap.TileSet == null)
                return;
            var tileSize = FrmMain.Settings.TileSize;
        }



    }
}
