using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;


namespace MapEditor
{
    class ObjectEditor
    {
        #region FIELDS
        private BindingList<GameObject> _listItem;
        //
        private static Brush brush = new SolidBrush(Color.FromArgb(125, 205, 205, 155));
        private static Pen pen = new Pen(brush);
        private static Brush brush_initBound = new SolidBrush(Color.FromArgb(175, 50, 50, 50));
        private static Brush brush_font = new SolidBrush(Color.FromArgb(220, 35, 35, 35));
        private static Brush brush_quadnode = new SolidBrush(Color.FromArgb(160, 130, 210, 250));
        private static Brush brush_quadnode_noobject = new SolidBrush(Color.FromArgb(160, 240, 240, 240));
        #endregion
        #region PROPERTIES
        //QuadTree
        public QNode QuadTree { get; set; }

        //Dùng bindingList để bind với listBox
        public BindingList<GameObject> ListItem
        {
            get { return _listItem; }
            set { _listItem = value; }
        }
        
        //Lưu lại tọa độ khi chuột click.Dùng để khởi tạo đối tượng Object
        //kết hợp với tạo đô Mouseup tạo thành Rectangle
        public Point MouseDown { get; set; }

        //Lưu lại tọa độ khi chuột click.Dùng để khởi tạo đối tượng Object
        //kết hợp với tạo đô MouseDown tạo thành Rectangle
        public Point MouseUp { get; set; }
        #endregion

        //Constructor 1 arg nhận sourceList Object vào để quản lý
        public ObjectEditor(BindingList<GameObject> sourceList)
        {
            ListItem = sourceList;
            MouseDown = new Point(-1, -1);
        }

        public void Bind(ListBox listBox)
        {
            BindingSource bs = new BindingSource(this,"ListItem");
            //cho phép thêm item mới vào bindinglist
            bs.AllowNew = true;
            listBox.DataSource = bs;
            listBox.DisplayMember = "Name";
            _listItem.ListChanged += (object sender, ListChangedEventArgs e) =>
                {
                    (listBox.DataSource as BindingSource).ResetBindings(true);
                };
        }

        public void InitGameObject()
        {
            if (this.MouseDown.X == -1 && this.MouseDown.Y == -1)
                return;
            //x đi từ trái sang phải,lấy min giữa mouseup và mousedown để 
            //lấy giá trị x nhỏ hơn để làm điểm vẽ rectangle
            int x = Math.Min(MouseDown.X, MouseUp.X);
            int y = Math.Min(MouseDown.Y, MouseUp.Y);
            //ngược lại với x thì width lấy max
            int width = Math.Max(MouseDown.X, MouseUp.X) - x;
            int height = Math.Max(MouseDown.Y, MouseUp.Y) - y;
            if (width == 0 && height == 0)
                return;
            #region chưa hiểu rõ chỗ này,liên quan tới hàn useTransform trong GameObject
            if (FrmMain.Settings.UseTransform == true)
            {
                y += height;
            }
            #endregion
            var obj = new GameObject(x, y, width, height);
            obj.Name = "object" + ListItem.Count.ToString();
            obj.Id = 0;
            ListItem.Add(obj);

            //gán lại mousedowwn để vẽ object mới
            this.MouseDown = new Point(-1, -1);
        }
        /// <summary>
        /// Vẽ tất cả object
        /// </summary>
        /// <param name="graphics"></param>
        public void Draw(Graphics graphics)
        {
            foreach (GameObject GObject in ListItem)
            {
                if (GObject.Image == null)
                {
                    graphics.FillRectangle(brush_initBound, GObject.InitBound);
                }
                else
                {
                    graphics.DrawImage(GObject.Image, GObject.InitBound);
                }
                //
                graphics.FillRectangle(brush, GObject.ActiveBound);

                //Vẽ tên object
                graphics.DrawString(
                    GObject.Name,
                    SystemFonts.IconTitleFont,
                    brush_font,
                    GObject.ActiveBound.X + 3,
                    GObject.ActiveBound.Y + 3);
            }

            //Vẽ tất cả node.
            this.drawQuadTreeNode(this.QuadTree, graphics);
        }

        /// <summary>
        /// Vẽ tất cả object,sử dụng đổi hệ trục tọa độ
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="worldHeight"></param>
        public void Draw(Graphics graphics, int worldHeight)
        {
            foreach (GameObject GObject in ListItem)
            {
                var activeBound = GObject.GetActiveBoundTransform(worldHeight);
                var initBound = GObject.GetInitBoundTransform(worldHeight);
                if (GObject.Image == null)
                {
                    graphics.FillRectangle(brush_initBound, initBound);
                }
                else
                {
                    graphics.DrawImage(GObject.Image,   initBound);
                }
                //
                graphics.FillRectangle(brush, activeBound);

                //Vẽ tên object
                graphics.DrawString(
                    GObject.Name,
                    SystemFonts.IconTitleFont,
                    brush_font,
                    activeBound.X + 3,
                    activeBound.Y + 3);
            }
        }

        /// <summary>
        /// Vẽ tất cả các object trong một khung visibleRect,sử dụng đổi hệ trục tọa độ
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="visibleRect"></param>
        /// <param name="worldHeight"></param>
        public void Draw(Graphics graphics, Rectangle visibleRect,int worldHeight)
        {
            List<GameObject> listObject = new List<GameObject>();
            this.QuadTree.getListObject(ref listObject, visibleRect);
            foreach (GameObject GObject in listObject)
            {
                var activeBound = GObject.GetActiveBoundTransform(worldHeight);
                var initBound = GObject.GetInitBoundTransform(worldHeight);
                if (GObject.Image == null)
                {
                    graphics.FillRectangle(brush_initBound, initBound);
                }
                else
                {
                    graphics.DrawImage(GObject.Image, initBound);
                }
                //
                graphics.FillRectangle(brush, activeBound);

                //Vẽ tên object
                graphics.DrawString(
                    GObject.Name,
                    SystemFonts.IconTitleFont,
                    brush_font,
                    activeBound.X + 3,
                    activeBound.Y + 3);
            }
        }

        /// <summary>
        /// Vẽ tất cả các object trong một khung visibleRect,khong sử dụng đổi hệ trục tọa độ
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="visibleRect"></param>
        public void Draw(Graphics graphics, Rectangle visibleRect)
        {
            List<GameObject> listObject = new List<GameObject>();
            this.QuadTree.getListObject(ref listObject, visibleRect);
            foreach (GameObject GObject in listObject)
            {
                if (GObject.Image == null)
                {
                    graphics.FillRectangle(brush_initBound, GObject.InitBound);
                }
                else
                {
                    graphics.DrawImage(GObject.Image, GObject.InitBound);
                }
                //
                graphics.FillRectangle(brush, GObject.ActiveBound);

                //Vẽ tên object
                graphics.DrawString(
                    GObject.Name,
                    SystemFonts.IconTitleFont,
                    brush_font,
                    GObject.ActiveBound.X + 3,
                    GObject.ActiveBound.Y + 3);
            }
        }

        public void RemderQuadTree(Graphics graphics)
        {
            drawQuadTreeNode(this.QuadTree,graphics);
        }

        public void drawQuadTreeNode(QNode node, Graphics graphics)
        {
            if (node == null)
                return;
            //nếu là node lá 
            if (node.isLeaf())
            {
                if (node.ListObjects.Any())
                    graphics.FillRectangle(brush_quadnode, node.Bound);
                else
                    graphics.FillRectangle(brush_quadnode_noobject, node.Bound);
                graphics.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(255, 45, 45, 45))), node.Bound);
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    drawQuadTreeNode(node.Childs[i], graphics);
                }
            }
        }
        
    }
}
