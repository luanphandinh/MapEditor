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
        //dùng để vẻ hình,lấy graphics từ tablelayout trên view 
        //sau đó vẽ lên
        public Graphics Graphics
        {
            get { return _graphics; }
            set { _graphics = value; }
        }

        public ObjectEditor ObjectEditor { get; set; }
        
        public TilesMap TilesMap
        {
            get { return _tilesMap; }
            //Khi set TIle map hàm PropertyCHanged sẽ được gọi
            //từ đó tính luôn MapSize mà ko cần phải gọi hàm để tính
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

        ///OPen FrmCreateTiles,Sau khi tạo tileSet thì TileMap nhận TileSet 
        ///từ FrmCreateTiles
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
                    //gán TileSet của tile map sau khi craete từ Frm
                }
                this.TilesMap.TileSet = _createTileFrm.Tileset;
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
        //lấy danh sách các image của tile để gán cho listView trên FrmMain
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

        protected void OnDraw(EventArgs e)
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

            //vẽ map từ i sang phải và j xuống dưới
            //vì trên View ta sẽ scroll Map,nên chỉ vẻ những phần hiển thị 
            //lấy Max từ index (visibleRectangle.X / tileSize.Width) - 1 
            //vì nếu X tại vị trí 0 ,tức là ko có scroll sang ngang thì 
            //ta sẽ vẽ từ index 0 trong TileSet với phép toán lấy Max mà ko 
            //phải là lấy -1
            int iBegin = Math.Max(visibleRectangle.X / tileSize.Width - 1, 0);
            //tương tự iEnd chỉ vẽ được hết phần Width của màn hình
            //được vẽ từ vị trí iBegin 
            int iEnd = Math.Min(iBegin + visibleRectangle.Width / tileSize.Width + 2, TilesMap.Columns);
            int jBegin = Math.Max(visibleRectangle.Y / tileSize.Height - 1, 0);
            int jEnd = Math.Min(jBegin + visibleRectangle.Height / tileSize.Height + 2, TilesMap.Rows);

            for (int i = iBegin; i < iEnd; i++)
            {
                for (int j = jBegin; j < jEnd; j++)
                {
                    Tile tile = TilesMap.TileSet.ListTiles.ToList().Find(t => t.Id == TilesMap[i, j]);
                    if (tile == null)
                        continue;

                    tile.draw(
                        Graphics,
                        new Point(tileSize.Width * i,tileSize.Height * j),
                        tileSize);
                }
            }
            //perform vẽ Objects sẽ làm sau

            if (FrmMain.Settings.UseTransform == true)
            {
                int worldHeight = this.TilesMap.GetMapHeight();
                if (this.ObjectEditor.QuadTree == null)
                    this.ObjectEditor.Draw(Graphics, worldHeight);
                else
                    this.ObjectEditor.Draw(Graphics, visibleRectangle, worldHeight);
            }
            else
            {
                if (this.ObjectEditor.QuadTree == null)
                    this.ObjectEditor.Draw(Graphics);
                else 
                    this.ObjectEditor.Draw(Graphics, visibleRectangle);
            }

            _lastVisibleRect = visibleRectangle;
            OnDraw(null);
        }

        public void InitObjectEditor()
        {
            ObjectEditor = new ObjectEditor(TilesMap.ListObject);
            ObjectEditor.ListItem.ListChanged += ListItem_ListChanged;
        }

        private void ListItem_ListChanged(object sender, ListChangedEventArgs e)
        { 
            //nếu thêm item vào thì add gameObject_PropertyChanged cho item đó
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                (sender as BindingList<GameObject>).ElementAt(e.NewIndex).PropertyChanged += gameObject_PropertyChanged;
            }
            //vẽ lại  map khi add item
            this.Draw(_lastVisibleRect);
        }

        private void gameObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        { 
            if(this.Graphics != null)
            {
                //vẽ lại map mỗi khi item thay đổi property
                this.Draw(_lastVisibleRect);
            }
        }
        /// <summary>
        /// Vẽ tile lên một tạo độ trên map
        /// </summary>
        /// <param name="map_coordinate"></param>
        /// <param name="tile"></param>
        public void DrawTile(Point map_coordinate,Tile tile)
        {
            tile.draw(this.Graphics, map_coordinate, FrmMain.Settings.TileSize);
            OnDraw(null);
        }

        /// <summary>
        /// vẽ quadTree
        /// </summary>
        public void RenderQuadTree()
        {
            if (this.ObjectEditor != null && this.Graphics != null)
            {
                this.ObjectEditor.RenderQuadTree(this.Graphics);
                OnDraw(null);
            }
        }
    }
}
