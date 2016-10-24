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

        /// <summary>
        /// Object Editor gọi hàm bind để bind với View
        /// Danh sahcs sẽ hiển thị với Name
        /// </summary>
        /// <param name="listbox"></param>
        public void Bind(ListBox listBox)
        {
            BindingSource bs = new BindingSource(this,"ListItem");
            //cho phép thêm item mới vào bindinglist
            bs.AllowNew = true;
            listBox.DataSource = bs;
            listBox.DisplayMember = "Name";
            _listItem.ListChanged += (object sender, ListChangedEventArgs e) =>
                {
                    //reset lại data displayed in listboxview
                    (listBox.DataSource as BindingSource).ResetBindings(true);
                };
        }

        /// <summary>
        /// Khởi tạo game object dựa trên giá trị mouseup và mousedown
        /// gán gameObject vào ListObject
        /// </summary>
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
        #region Draw
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
        #endregion
        #region drawQuadTree
        public void RenderQuadTree(Graphics graphics)
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
        #endregion
        #region Save
        //Save Object
        public static void Save(XmlTextWriter wr, BindingList<GameObject> listObject, string path)
        {
            wr.WriteStartElement("Objects");
            {
                foreach (GameObject item in listObject)
                {
                    wr.WriteStartElement("Item");
                    {
                        wr.WriteAttributeString("Id", item.Id.ToString());
                        wr.WriteAttributeString("Name", item.Name.ToString());
                        wr.WriteAttributeString("X", item.InitBound.X.ToString());
                        wr.WriteAttributeString("Y", item.InitBound.Y.ToString());
                        wr.WriteAttributeString("Width", item.InitBound.Width.ToString());
                        wr.WriteAttributeString("Height", item.InitBound.Height.ToString());

                        //tạo thêm folder res để lưu image nếu object có image
                        if (Directory.Exists("res"))
                        {
                            Directory.CreateDirectory("res");
                        }
                        if (item.Image != null)
                        {
                            item.Image.Save("res\\" + item.Name + ".png");
                            wr.WriteAttributeString("Image", "res\\" + item.Name + ".png");
                        }

                        wr.WriteStartElement("ActiveBound");
                        {
                            wr.WriteAttributeString("X", item.ActiveBound.X.ToString());
                            wr.WriteAttributeString("Y", item.ActiveBound.Y.ToString());
                            wr.WriteAttributeString("Width", item.ActiveBound.Width.ToString());
                            wr.WriteAttributeString("Height", item.ActiveBound.Height.ToString());
                        }
                        wr.WriteEndElement();//ActiveBound
                        //lưu params
                        if (item.Parameters.Any())
                        {
                            wr.WriteStartElement("Params");
                            {
                                foreach (KeyValuePair<string, string> elem in item.Parameters.ToList())
                                {
                                    wr.WriteStartElement("Elem");
                                    wr.WriteAttributeString("Key", elem.Key);
                                    wr.WriteAttributeString("Value", elem.Value);
                                    wr.WriteEndElement();
                                }
                            }
                            wr.WriteEndElement();
                        }
                    }
                    wr.WriteEndElement();//Item
                }
            }
            wr.WriteEndElement();//Objects
        }

        #endregion
        #region Load
        //Load list game objects
        public static IList<GameObject> Load(XmlTextReader reader,string path)
        { 
            List<GameObject> listGameObject = new List<GameObject>();
            reader.ReadStartElement("Objects");
            while(reader.NodeType != XmlNodeType.EndElement || reader.Name != "Objects")
            {
                reader.Read();
                if (reader.IsStartElement("Item"))
                {
                    GameObject _obj = readGameObject(reader);
                    listGameObject.Add(_obj);
                }
            }
            return listGameObject;
        }

        private static GameObject readGameObject(XmlTextReader reader)
        {
            GameObject obj = null;
            int id = Int32.Parse(reader.GetAttribute("Id"));
            string name = reader.GetAttribute("Name");
            int x = Int32.Parse(reader.GetAttribute("X"));
            int y = Int32.Parse(reader.GetAttribute("Y"));
            int width = Int32.Parse(reader.GetAttribute("Width"));
            int height = Int32.Parse(reader.GetAttribute("Height"));
            string img = reader.GetAttribute("Image");
            obj = new GameObject(x, y, width, height);
            obj.Id = id;
            obj.Name = name;
            if (!String.IsNullOrEmpty(img))
            { 
                if(File.Exists(img))
                {
                    obj.Image = Image.FromFile(img);
                }
            }

            //khi chưa đến end Element của Object
            
            while (reader.NodeType != XmlNodeType.EndElement || reader.Name != "Item")
            {
                reader.Read();
                if (reader.IsStartElement("ActiveBound"))
                {
                    x = Int32.Parse(reader.GetAttribute("X"));
                    y = Int32.Parse(reader.GetAttribute("Y"));
                    width = Int32.Parse(reader.GetAttribute("Width"));
                    height = Int32.Parse(reader.GetAttribute("Height"));
                    obj.ActiveBound = new Rectangle(x, y, width, height);
                }
                if (reader.IsStartElement("Params"))
                {
                    string key = String.Empty;
                    string value = String.Empty;
                    while (reader.NodeType != XmlNodeType.EndElement || reader.Name != "Params")
                    {
                        reader.Read();
                        if (reader.IsStartElement("Elem"))
                        {
                            key = reader.GetAttribute("Key");
                            value = reader.GetAttribute("Value");
                            obj.Parameters.Add(key, value);
                        }
                    }
                }
            }

            return obj;
        }

        #endregion

        /// <summary>
        /// Khởi tạo QuadTree
        /// </summary>
        /// <param name="level"></param>
        /// <param name="bound"></param>
        public void InitQuadTree(int level, Rectangle bound)
        { 
            //lấy hình vuông dựa trên max kích thước của map
            int edge = Math.Max(bound.Width, bound.Height);

            bound.Size = new Size(edge, edge);

            Thread thread = new Thread(new ThreadStart(() =>
            {
                //tạo node gốc với level 0 và bound là edge * edge
                this.QuadTree = new QNode(0, bound, null);
                //gán listItems cho quadtree
                this.QuadTree.ListObjects = this.ListItem.ToList();
                //bắt đầu tạo các node con cho quadtree dựa trên listItem
                this.QuadTree.InitChild();
            }));
            thread.Start();
        }

        /// <summary>
        /// lưu quadTree
        /// </summary>
        /// <param name="root"></param>
        /// <param name="path"></param>
        public static void SaveQuadTree(QNode root, string path)
        {
            using (XmlTextWriter wr = new XmlTextWriter(path, Encoding.UTF8))
            {
                wr.Formatting = Formatting.Indented;
                wr.WriteStartDocument();
                Save(wr, root, path);
                wr.WriteEndDocument();
            }
        }
        //lưu quad tree
        private static void Save(XmlTextWriter wr, QNode qnode, string path)
        {
            if (qnode == null)
                return;
            wr.WriteStartElement("QNode");
            {
                wr.WriteAttributeString("Id", qnode.Id.ToString());
                wr.WriteAttributeString("Level", qnode.Level.ToString());
                wr.WriteAttributeString("X", qnode.Bound.X.ToString());
                wr.WriteAttributeString("Y", qnode.Bound.Y.ToString());
                wr.WriteAttributeString("Width", qnode.Bound.Width.ToString());
                wr.WriteAttributeString("Height", qnode.Bound.Height.ToString());
                if (qnode.isLeaf() && qnode.ListObjects.Any())
                {
                    string str = String.Empty;
                    foreach (var obj in qnode.ListObjects)
                    {
                        str += obj.Name + " ";
                    }
                    wr.WriteStartElement("Objects");
                    wr.WriteString(str);
                    wr.WriteEndElement();
                }
                else
                {
                    for (int i = 0; i < 4; i++) 
                    {
                        Save(wr, qnode.Childs[i], path);
                    }
                }

            }
            wr.WriteEndElement();//Objects
        }

    }//class
}//namespace
