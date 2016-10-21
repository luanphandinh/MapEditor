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
        private List<GameObject> _gameObjects;
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

        public List<GameObject> GameObjects
        {
            get { return _gameObjects; }
            set { _gameObjects = value; }
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
            this.GameObjects = new List<GameObject>();
            this._childs = new QNode[4];
        }

        //IMPORTANT
        //tạo node con từ các object vào bound
    }
}
