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
        MapController _mapController;

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
            int columns = this._mapController.TilesMap.Columns;
            int rows = this._mapController.TilesMap.Rows;
            _tableLayout = new TableLayoutPanel();

            _tableLayout.ColumnCount = columns;
            //khởi tạo ma trận index
            //khởi tạo cột
            Size tileSize = FrmMain.Settings.TileSize;
            
            for (int i = 0; i < columns; i++)
            {
                var columnStyle = new RowStyle(SizeType.Absolute, tileSize.Width - 1);
                _tableLayout.ColumnStyles.Add(columnStyle);
            }

            //khởi tạo dòng
            for (int i = 0; i < rows; i++)
            {
                var rowStyle = new RowStyle(SizeType.Absolute, tileSize.Height - 1);
                _tableLayout.RowStyles.Add(rowStyle);
            }

            _tableLayout.BackColor = Color.FromArgb(205, 205, 205);
            _tableLayout.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            _tableLayout.Margin = new System.Windows.Forms.Padding(5);
            _tableLayout.Name = "map";


        }

        #region mainmenu click events
        private void creatTilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._mapController.CreateTileSet();
            if (this._mapController.TilesMap == null)
                return;
            if (this._mapController.TilesMap.TileSet == null)
                return;
            //Gán imageList cho ListView
            //với Image được đánh số từ 0 
            this.listView1.LargeImageList = _mapController.getImageList();

            if (listView1.Items.Count > 0)
                listView1.Items.Clear();
            //ListView Item được đưa vào Items của View
            this.listView1.Items.AddRange(_mapController.getListViewItem().ToArray());
        }

        private void newMapToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        #endregion

    }
}