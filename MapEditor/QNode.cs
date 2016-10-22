using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;

namespace MapEditor
{
    //link tham khảo cách sử dụng quadtree
    //http://www.codeproject.com/Articles/30535/A-Simple-QuadTree-Implementation-in-C
    //http://blog.notdot.net/2009/11/Damn-Cool-Algorithms-Spatial-indexing-with-Quadtrees-and-Hilbert-Curves
    class QNode
    {
        #region FIELDS
        //FIELDS
        private int _level;
        private Int64 _id;
        private List<GameObject> _listObject;
        private Rectangle _bound;
        private QNode[] _childs;
        private QNode _parent;
        #endregion
        //4 con của một node,nếu node là leaf thì con sẽ có giá trị là null
        public QNode[] Childs
        {
            get { return _childs; }
            set { _childs = value; }
        }

        //Danh sách các đối tượng mà node quản lý

        public List<GameObject> ListObjects
        {
            get { return _listObject; }
            set { _listObject = value; }
        }

        /// <summary>
        /// South West Bắc Nam
        /// 10h30
        /// </summary>
        public QNode SW
        {
            get 
            { 
                return (Childs[0] == null) ? null : Childs[0]; 
            }
            set 
            {
                Childs[0] = value;
            }
        }
        /// <summary>
        /// South East
        /// 1h30
        /// </summary>
        public QNode SE
        {
            get
            {
                return (Childs[1] == null) ? null : Childs[1];
            }
            set
            {
                Childs[1] = value;
            }
        }
        /// <summary>
        /// North East
        /// 4h30
        /// </summary>
        public QNode NE
        {
            get
            {
                return (Childs[2] == null) ? null : Childs[2];
            }
            set
            {
                Childs[2] = value;
            }
        }
        /// <summary>
        /// North West
        /// </summary>
        public QNode NW
        {
            get
            {
                return (Childs[3] == null) ? null : Childs[3];
            }
            set
            {
                Childs[3] = value;
            }
        }

        //Bậc của Node,nếu là gốc thì bậc là 0
        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }

        //Id của node,dịch bit để định danh vd top-left 00,top right 01 
        public Int64 Id
        {
            get { return _id; }
            set { _id = value; }
        }

        //khung hình của node
        public Rectangle Bound
        {
            get { return _bound; }
            set { _bound = value; }
        }


        public QNode(int level, Rectangle bound, QNode parent)
        {
            this._level = level;
            this._bound = bound;
            this._parent = parent;
            this.Id = (parent == null) ? 0 : parent.Id;
            this.ListObjects = new List<GameObject>();
            this._childs = new QNode[4];
        }

        //IMPORTANT
        //tạo node con từ các object vào bound
        public void InitChild()
        {
            //Chỉ tạo node khi level ko quá cao và ko có quá nhiều Object
            if (this._level >= FrmMain.Settings.MaxLevelQuadTree
                || this.ListObjects.Count <= FrmMain.Settings.MaxObjectQuadTree)
                return;

            //Tính 4 hình chữ nhật con của node này
            Rectangle[] rects = this.devideBound();

            //khởi tạo các childNode
            for (int i = 0; i < 4; i++)
            {
                this.Childs[i] = new QNode(this.Level + 1, rects[i], this);
            }

            //Tính Id  cho Node
            QNode.GenerateId(this);

            for (int i = 0; i < 4; i++)
            {
                foreach(GameObject gameObj in this.ListObjects)
                {
                    Childs[i].insertObject(gameObj);
                }
            }

            //Node ko phải lá ko được giữ lại object
            this.removeAll();

            //Recurse cho tất cả các child
            for (int i = 0; i < 4; i++)
            {
                Childs[i].InitChild();
            }
        }

        //Tạo id
        public static void GenerateId(QNode parent)
        {
            //Nếu là NULL thì SW sẽ là 0000
            //Giả sử id là 0100 là phần tư thứ 2 bên phải 
            parent.SW.Id = parent.Id;
            //Lv là 0 thì offset là 2
            //Lv là 1 thì offset là 2 << 1 = 4
            int offset = 2 << parent.Level;
            //1 << 2 = 0100 | 0000 = 0100
            //1 << 4 = 0001 0000 | 0000 0100 = 0001 0100
            parent.SE.Id = Convert.ToInt64(1 << offset) | parent.Id;
            //1 << 3 = 1000 | 0000 = 1000
            //1 << 5 = 0010 0000 | 0000 0100 = 0010 0100
            parent.NW.Id = Convert.ToInt64(1 << (offset + 1)) | parent.Id;
            //0100 | 1000 = 1100
            // 0001 0100 | 0010 0100 = 0011 0100
            parent.NE.Id = parent.SE.Id | parent.NW.Id;
        }

        //Loại bỏ Object khỏi node này ,nếu nó không phải là node lá (không có object nào cả)
        //thì tìm trong tất cả con của nó
        public void removeObject(GameObject obj)
        {
            if (this.isLeaf())
            {
                this.ListObjects.Remove(obj);
            }
            else 
            {
                for (int i = 0; i < 4; i++)
                {
                    this.Childs[i].removeObject(obj);
                }
            }
        }

        //Chèn 1 object vào node này
        public bool insertObject(GameObject obj)
        { 
            //Một số xử lý liên quan đến UI
            //-------------------------------------------------
            Rectangle activeBound;
            if (FrmMain.Settings.UseTransform)
            {
                activeBound = obj.GetActiveBoundTransform(MapController.MapSize.Height);
            }
            else
                activeBound = obj.ActiveBound;
            //==================================================
            //return true nếu obj nằm gọn trong node ,bên ngoài thấy true sẽ không insert 
            //vào node khác nữa,để tăng hiệu suất
            if (this.Bound.Contains(activeBound))
            {
                this.ListObjects.Add(obj);
                return true;
            }
            else if (this.Bound.IntersectsWith(activeBound))
            {
                this.ListObjects.Add(obj);
                return false;
            }
            return false;
        }

        public void removeAll()
        {
            this.ListObjects.Clear();
        }

        //Chia bound hiện tại của node thành 4 node con,chia đôi 2 cạnh hcn
        public Rectangle[] devideBound()
        {
            Rectangle[] result = new Rectangle[4];
            return result;

            //dịch sang bit sang phải để chia đôi
            int halfWidth = this.Bound.Width >> 1;
            int halfHeight = this.Bound.Height >> 1;

            //SW
            result[0] = new Rectangle()
            {
                X = this.Bound.X,
                Y = this.Bound.Y,
                Width = halfWidth,
                Height = halfHeight,
            };
            //SE
            result[0] = new Rectangle()
            {
                X = this.Bound.X + halfWidth,
                Y = this.Bound.Y,
                Width = halfWidth,
                Height = halfHeight,
            };
            //NE
            result[0] = new Rectangle()
            {
                X = this.Bound.X + halfWidth,
                Y = this.Bound.Y + halfHeight,
                Width = halfWidth,
                Height = halfHeight,
            };
            //NW
            result[0] = new Rectangle()
            {
                X = this.Bound.X,
                Y = this.Bound.Y + halfHeight,
                Width = halfWidth,
                Height = halfHeight,
            };

            return result;
        }

        // Kiểm tra bound của nođe này có giao nhau không.
        public bool isIntersect(Rectangle rect)
        {
            return this.Bound.IntersectsWith(rect);
        }

        // Kiểm tra bound của node này có chứa hình chữ nhật rect không.
        public bool isContains(Rectangle rect)
        {
            return this.Bound.Contains(rect);
        }
        //return true nếu là lá
        public bool isLeaf()
        {
            if (this.Childs[0] == null)
                return true;
            return false;
        }

        //Lấy danh sách các đối tượng mà node của nó giao với hình chữ nhật rect,thông thường rect là khung màn hình
        public void getListObject(ref List<GameObject> return_listObject, Rectangle rect)
        {
            //Nếu là node lá thì tiến hành kiểm tra chèn object vào return_list nếu ko thì bắt đầu kiểm tra con của nó
            if (this.isLeaf())
            {
                //Nếu return list có Object thì tiến hành kiểm tra từng phần tử xem có trùng ko,sau đó chèn vào
                //Nếu return list rỗng thì chèn cả listObject vào
                if (return_listObject.Any())
                {
                    foreach (var obj in this.ListObjects)
                    {
                        if (!return_listObject.Contains(obj))
                        {
                            return_listObject.Add(obj);
                        }
                    }
                }
                else
                {
                    return_listObject.AddRange(this.ListObjects);
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    _childs[i].getListObject(ref return_listObject, rect);
                }
            }
        }
    }
}
