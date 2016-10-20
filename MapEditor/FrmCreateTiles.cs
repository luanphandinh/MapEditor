using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace MapEditor
{
    public partial class FrmCreateTiles : Form
    {
        /// <summary>
        /// PRIVATE ATTRIBUTES
        /// </summary>
        private TileSet _tileset;
        //dùng để vẽ dữ liệu đã được buffer
        //https://msdn.microsoft.com/en-us/library/system.drawing.bufferedgraphics(v=vs.110).aspx
        private BufferedGraphics _bufferedGraphics;
        private Pen _pen;
        private Graphics _graphics;
        private BufferedGraphicsContext _context;

        internal TileSet Tileset
        {
            get { return _tileset; }
            set { _tileset = value; }
        }

        //dùng để trả về danh sách các rect được cắt cho mainfrm
        public List<ListViewItem> ListViewItems { get; set; }


        public FrmCreateTiles()
        {
            InitializeComponent();
            //tạo pen để vẽ các line
            _pen = new Pen(new SolidBrush(Color.Blue), 0.5f);
            //tạo 1 graphic để vẽ được len panel_tile
            _graphics = this.panel_tile.CreateGraphics();
            //doublebuffer để tránh bị giật khi vẽ
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            ListViewItems = new List<ListViewItem>();
            //
            _context = BufferedGraphicsManager.Current;
            //tăng kích thước lớn hơn panel gốc
            _context.MaximumBuffer = this.panel_tile.Size + new Size(1, 1);
            //tạo _bufferedGraphics 
            _bufferedGraphics = _context.Allocate(_graphics, new Rectangle(Point.Empty, panel_tile.Size));

            //binding tileset
            _tileset = new TileSet();
            this.textBoxWidth.DataBindings.Add("Text", _tileset, "Widthtile");
            this.textBoxHeight.DataBindings.Add("Text", _tileset, "Heighttile");
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            var dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                //lấy tream file
                Stream filesStream = openFileDialog.OpenFile();
                //set fileName cho tileSet dưới dạng đường dẫn tương đối
                Tileset.Filename = openFileDialog.FileName;
                //tạo image từ stream
                Tileset.Image = Image.FromStream(filesStream);
                //vẽ image lên bufferd graphic
                _bufferedGraphics.Graphics.DrawImage(Tileset.Image, new Rectangle(Point.Empty, panel_tile.Size));
                //render lên graphics
                _bufferedGraphics.Render(_graphics);
                this.textBoxHeight.Enabled = true;
                this.textBoxWidth.Enabled = true;
            }
        }


        //dùng _pen để vẻ lưới kẻ lên màn hình
        private void drawGridView(int wcount,int hcount)
        {
            _bufferedGraphics.Graphics.Clear(Color.Black);
            _bufferedGraphics.Graphics.DrawImage(Tileset.Image, new Rectangle(Point.Empty, panel_tile.Size));
            //vẻ đường kẻ dọc
            float tileWidth = ((float)this.panel_tile.Width) / wcount;
            for (int i = 0; i < wcount; i++)
            {
                int drawpoint = (int)(tileWidth * i) - 1;
                _bufferedGraphics.Graphics.DrawLine(
                    _pen,
                    new Point(drawpoint,0),
                    new Point(drawpoint,panel_tile.Height)
                    );
            }

            //vẻ đường kẻ dọc
            float tileHeight = ((float)this.panel_tile.Height) / hcount;
            for (int i = 0; i < hcount; i++)
            {
                int drawpoint = (int)(tileHeight * i) - 1;
                _bufferedGraphics.Graphics.DrawLine(
                    _pen,
                    new Point(0,drawpoint),
                    new Point(panel_tile.Width,drawpoint)
                    );
            }

            _bufferedGraphics.Render(_graphics);
        }

        //vẻ lại gridview mỗi lần textChanged được gọi
        private void textBoxWidth_TextChanged(object sender, EventArgs e)
        {
            if (Tileset.Image == null)
                return;
            int h;

            if (Int32.TryParse((sender as TextBox).Text, out h) == true)
            {
                if (h == 0)
                    return;
                else h = Tileset.Image.Height / h;
            }
            else
            {
                h = 1;
            }
            drawGridView(Tileset.Columns, h);
        }

        private void textBoxHeight_TextChanged(object sender, EventArgs e)
        {
            if (Tileset.Image == null)
                return;
            int w;

            if (Int32.TryParse((sender as TextBox).Text, out w) == true)
            {
                if (w == 0)
                    return;
                else w = Tileset.Image.Width / w;
            }
            else
            {
                w = 1;
            }
            drawGridView(w, Tileset.Rows);
        }

        private void panel_tile_Paint(object sender, PaintEventArgs e)
        {
            if (Tileset.Image == null)
                return;
            _bufferedGraphics.Dispose();
            _bufferedGraphics = _context.Allocate(_graphics, new Rectangle(Point.Empty, panel_tile.Size));
            drawGridView(Tileset.Columns, Tileset.Rows);
        }

        //lưu các rectangle được cắt 
        private void buttonOK_Click(object sender, EventArgs e)
        {
            Rectangle rect = new Rectangle(0, 0, Tileset.Widthtile, Tileset.Heighttile);

            for (int i = 0; i < Tileset.Rows; i++) 
            {
                for (int j = 0; j < Tileset.Columns; j++)
                {
                    rect.Location = new Point(j * rect.Width, i * rect.Height);

                    this.Tileset.ListTiles.Add(
                        new Tile(
                            Tileset.Image,
                            new Rectangle(rect.Location,new Size(rect.Size.Width - 1,rect.Size.Height - 1)),
                            i * Tileset.Columns + j + 1
                            )
                        );
                    this.ListViewItems.Add(
                        new TileItem(Tileset.ListTiles.Last()
                        ));
                }
            }

        }

    }
}
