using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace MapEditor
{
    public partial class FrmMain : Form
    {
        public static ApplicationSetttings Settings = new ApplicationSetttings();

        //Tile đang được chọn trong ListView
        private Tile _selectedTile;

        //Lưới kẻ ô
        private TableLayoutPanel _tableLayout;

        //Map controller
        private MapController _mapController;

        //Để lưu đường dẫn tạm cho file xml
        //khi lưu nếu đường này rỗng thì mở dialog yêu cầu đường dẫn,ngược lại thì ta dùng đường dẫn này
        //khi load nếu path này rỗng thì mở dialog yêu cầu đường dẫn,ngược lại thì ta dùng đường dẫn này
        private string _tilesetPath;

        private BufferedGraphics _bufferedGraphic;
        private Graphics _graphics;
        private int _scrollIndex;
        

        public FrmMain()
        {
            InitializeComponent();
            _mapController = new MapController();
            _mapController.Drawn += (object sender, EventArgs e) =>
                {
                    this._bufferedGraphic.Render(_graphics);
                };
        }
            
      
        private void Panel_Scroll(object sender, ScrollEventArgs e)
        {
            
        }


        public void InitTableLayout()
        {
            if (_tableLayout != null)
            {
                this.splitContainer2.Panel1.Controls.Remove(_tableLayout);
            }

            if (_mapController == null)
                return;
            else
            if (_mapController.TilesMap == null)
                return;
            //lấy số columns và rows từ Tilesmap để vẽ tableLayout
            int columns = this._mapController.TilesMap.Columns;
            int rows = this._mapController.TilesMap.Rows;
            _tableLayout = new TableLayoutPanel();

            
            //khởi tạo ma trận index
            //khởi tạo cột
            _tableLayout.ColumnCount = columns;
            Size tileSize = FrmMain.Settings.TileSize;
            
            for (int i = 0; i < columns; i++)
            {
                var columnStyle = new ColumnStyle(SizeType.Absolute, tileSize.Width - 1);
                _tableLayout.ColumnStyles.Add(columnStyle);
            }

            //khởi tạo dòng
            _tableLayout.RowCount = rows;
            for (int i = 0; i < rows; i++)
            {
                var rowStyle = new RowStyle(SizeType.Absolute, tileSize.Height - 1);
                _tableLayout.RowStyles.Add(rowStyle);
            }

            _tableLayout.BackColor = Color.FromArgb(205, 205, 205);
            _tableLayout.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            _tableLayout.Margin = new System.Windows.Forms.Padding(5);
            _tableLayout.Name = "map";

            //gán các Event cho tableLayout
            _tableLayout.AutoSize = true;
            //event Paint
            _tableLayout.Paint += _tableLayout_Paint;
            //event MouseClick
            _tableLayout.MouseClick += TableLayoutMouseClick;
            //MouseDown
            //MouseClick

            //add tableLayout lên panel để hiển thị
            this.splitContainer2.Panel1.Controls.Add(_tableLayout);
            //this.splitContainer2.SplitterDistance = (int)(this.splitContainer2.Size.Height * 0.75);

            
            //khởi tạo graphics
            _graphics = this._tableLayout.CreateGraphics();
            var context =  BufferedGraphicsManager.Current;
            context.MaximumBuffer = _tableLayout.Size + new Size(1, 1);
            _bufferedGraphic = context.Allocate(_graphics, new Rectangle(Point.Empty, _tableLayout.Size));
            this._mapController.Graphics = _bufferedGraphic.Graphics;
        }


        public Rectangle getVisibleMap()
        {
            //lấy phần hiểu thị trên panel
            var size = this.splitContainer2.Panel1.ClientSize;
            //lấy điểm neo của màn hình khi scroll
            Point location = new Point(
                this.splitContainer2.Panel1.HorizontalScroll.Value,
                this.splitContainer2.Panel1.VerticalScroll.Value
                );
            return new Rectangle(location, size);
        }

     
        #region mainmenu click events
        private void creatTilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._mapController.CreateTileSet();
            if (this._mapController.TilesMap == null)
                return;
            if (this._mapController.TilesMap.TileSet == null)
            {
                MessageBox.Show("Tilset NUll");
                return;
            }
            //Gán imageList cho ListView
            //với Image được đánh số từ 0 
            this.listView1.View = View.LargeIcon;
            this.listView1.LargeImageList = _mapController.getImageList();

            if (listView1.Items.Count > 0)
                listView1.Items.Clear();
            //ListView Item được đưa vào Items của View
            this.listView1.Items.AddRange(_mapController.getListViewItem().ToArray());
        }

        private void newMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var newMapFrm = new FrmNewMap();
            var result = newMapFrm.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                //Tạo TilesMap với columns và rows 
                _mapController.TilesMap = new TilesMap(newMapFrm.Columns, newMapFrm.Rows);
                
                //_mapController.TilesMap.Columns = newMapFrm.Columns;
                //_mapController.TilesMap.Rows = newMapFrm.Rows;
                this.InitTableLayout();
                //khởi tạo ObjectEditor
                //Sẽ implementsau
            }
        }
        #endregion

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as ListView).SelectedItems.Count == 0)
            {
                _selectedTile = null;
                return;
            }
            _selectedTile = ((sender as ListView).SelectedItems[0] as TileItem).Tile;
        }

        //hàm vẽ
        private void _tableLayout_Paint(object sender, PaintEventArgs e)
        {
            _mapController.Draw(getVisibleMap());
        }

        private void TableLayoutMouseClick(object sender, MouseEventArgs e)
        { 
            //kiểm tra xem đang thao tác với object hay thao tác với 
            //tile
            if (_selectedTile == null)
                return;
            Size tileSize = FrmMain.Settings.TileSize;
            //selected point là chỉ số của matrix
            Point selectedPoint = new Point(e.X / tileSize.Width, e.Y / tileSize.Height);
            if (selectedPoint.X >= _mapController.TilesMap.Columns)
                return;
            if (selectedPoint.Y >= _mapController.TilesMap.Rows)
                return;
            _mapController.TilesMap[selectedPoint.X, selectedPoint.Y] = _selectedTile.Id;

            //location là vị trí vẽ trên tableLayout
            Point location = new Point(tileSize.Width * selectedPoint.X, tileSize.Height * selectedPoint.Y);
            _mapController.DrawTile(location, _selectedTile);
            this.enableSaveButton();
        }


        private void disabeSaveButton()
        {
            this.saveMapToolStripMenuItem.Enabled = false;
            //if(this.)
        }

        private void enableSaveButton()
        {
            this.saveMapToolStripMenuItem.Enabled = true;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }
        #region Save and Load
        //sử dụng khi đường dẫn mặc định ko hợp lệ hoặc rỗng
        //yêu cầu người dùng lấy đường dẫn
        private string saveAsDialog()
        {
            var fileDlg = new SaveFileDialog();
            fileDlg.Filter = "XML Files (*xml) | *.xml";
            var result = fileDlg.ShowDialog();
            if (result != DialogResult.OK)
                return String.Empty;
            return fileDlg.FileName;
        }

        public void SaveAs()
        {
            string newPath = saveAsDialog();
            if (!String.IsNullOrEmpty(newPath))
            {
                this._tilesetPath = newPath;
            }
            else
                return;
            Cursor.Current = Cursors.WaitCursor;
            TilesMap.Save(_mapController.TilesMap, this._tilesetPath);
            Cursor.Current = Cursors.Default;
            this.disabeSaveButton();
        }

        public void Save()
        {
            if (String.IsNullOrEmpty(_tilesetPath))
            {
                string newPath = saveAsDialog();
                if (!String.IsNullOrEmpty(newPath))
                {
                    this._tilesetPath = newPath;
                }
                else return;
            }
            
            Cursor.Current = Cursors.WaitCursor;
            TilesMap.Save(_mapController.TilesMap, this._tilesetPath);
            Cursor.Current = Cursors.Default;
            this.disabeSaveButton();
        }

        public void savePreviousTileMap()
        {
            if (!String.IsNullOrEmpty(_tilesetPath))
            {
                string name = _tilesetPath.Substring(_tilesetPath.LastIndexOf('\\'));
                var result = MessageBox.Show(
                    "Do you want to save " + name + " ? ",
                    "Save TileSet", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2,
                    MessageBoxOptions.ServiceNotification
                    );
                if (result == DialogResult.OK)
                {
                    TilesMap.Save(_mapController.TilesMap, _tilesetPath);
                }
            }
            _tilesetPath = String.Empty;
        }

        public void Load()
        {
            savePreviousTileMap();
            var openDlg = new OpenFileDialog();
            openDlg.Filter = "XML Files (*xml) | *.xml";
            var result = openDlg.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }
            Cursor.Current = Cursors.WaitCursor;
            _mapController.TilesMap = TilesMap.Load(openDlg.FileName);
            this._tilesetPath = openDlg.FileName;

            listView1.LargeImageList = _mapController.getImageList();
            listView1.Items.AddRange(_mapController.getListViewItem().ToArray());
            this.InitTableLayout();
            _mapController.Draw(getVisibleMap());
            Cursor.Current = Cursors.Default;
            this.disabeSaveButton();
        }
        #endregion

        private void saveMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Save();
        }

        private void loadMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Load();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            FrmMain.Settings.PropertyChanged += (object s, PropertyChangedEventArgs property_event) =>
                {
                    if (this.Focused == true)
                        this._mapController.Draw(getVisibleMap());
                    if (_mapController.TilesMap != null)
                    {
                        MapController.MapSize = new Size(
                            _mapController.TilesMap.Columns * FrmMain.Settings.TileSize.Width,
                            _mapController.TilesMap.Rows * FrmMain.Settings.TileSize.Height
                            );
                        //var mapbound = 
                    }
                };
        }

        
    }
}