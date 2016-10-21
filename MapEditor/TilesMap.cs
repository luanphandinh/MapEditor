using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MapEditor
{
    //TilesMap là class dùng để quản lý map của game
    //bao gồm 1 ma trận 2 chiều để quản lý ID các tile
    //tileSet để load các tile
    class TilesMap : INotifyPropertyChanged
    {
        //FIELDS
        //lưu trữ số dòng và cột của tileMap
        private Point _mapSize;
        //ma trận 2 chiều của id tile
        private int[,] _matrixIndex;
        //tileSet dùng để load tile
        private TileSet _tileSet;
        //

        //PROPERTIES
        #region PROPERTIES
        public TileSet TileSet
        {
            get { return _tileSet; }
            set { if (_tileSet != value) _tileSet = value; }
        }

        public int Columns
        {
            get { return _mapSize.X; }
            set 
            {
                if (_mapSize.X != value)
                {
                    _mapSize.X = value;
                    OnProPertyChanged(new PropertyChangedEventArgs("Columns"));
                }
            }
        }

        public int Rows
        {
            get { return _mapSize.Y; }
            private set
            {
                if (_mapSize.Y != value)
                {
                    _mapSize.Y = value;
                    OnProPertyChanged(new PropertyChangedEventArgs("Rows"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnProPertyChanged(PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, args);
            }
        }
        #endregion

        public TilesMap(int columns,int rows)
        {
            //lưu 2 giá trị số cột và dòng
            _mapSize = new Point(columns,rows);
            _matrixIndex = new int[columns, rows];

            _tileSet = null;
            MapController.MapSize = new Size(
                _mapSize.X * FrmMain.Settings.TileSize.Width,
                _mapSize.Y * FrmMain.Settings.TileSize.Height
                );
        }
        //mapindex đọc từ phải qua trái,trên xuống dưới
        //lấy giá trị mapindex tại i,j khi vọi tileMap[i,j]
        public int this[int i, int j]
        {
            get
            {
                if (i >= Columns || j >= Rows)
                    throw new Exception();
                return _matrixIndex[i, j];
            }
            set
            {
                if (i >= Columns || j >= Rows)
                    throw new Exception();
                _matrixIndex[i, j] = value;
            }
        }


        public static TilesMap Load(string path)
        {
            TilesMap tilesMap = null;
            int columns = 0;
            int rows = 0;
            using (XmlTextReader reader = new XmlTextReader(path))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if (reader.Name == "Tilesmap")
                        {
                            columns = Int32.Parse(reader.GetAttribute("columns"));
                            rows = Int32.Parse(reader.GetAttribute("rows"));
                            tilesMap = new TilesMap(columns, rows);
                        }
                        if (reader.Name == "Row")
                        {
                            int rowNumber = Int32.Parse(reader.GetAttribute("id"));
                            string indexes = reader.ReadString();
                            var row = indexes.Split('\t');
                            for (int i = 0; i < columns; i++)
                            {
                                tilesMap[i, rowNumber] = Int32.Parse(row[i]);
                            }
                        }
                        if (reader.Name == "TileSet")
                        {
                            tilesMap.TileSet = TileSet.Load(reader, path);
                        }
                        if (reader.Name == "Objects")
                        {
                            //do somethings
                        }
                    }
                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Tilesmap")
                        break;
                } 
            }
            return tilesMap;
        }

        public static void Save(TilesMap tilesmap, string path)
        {
            if (tilesmap == null)
                throw new ArgumentException("tilesmap does not accept null","tilesmap");
            if (path == null)
                throw new ArgumentException("Argument does not accept null", "path");

            using (XmlTextWriter wr = new XmlTextWriter(path,Encoding.UTF8))
            {
                wr.Formatting = Formatting.Indented;
                wr.WriteStartDocument();
                {
                    wr.WriteStartElement("Tilesmap");
                    {
                        wr.WriteAttributeString("columns", tilesmap.Columns.ToString());
                        wr.WriteAttributeString("rows", tilesmap.Rows.ToString());
                        wr.WriteStartElement("MatrixIndex");
                        {
                            for (int i = 0; i < tilesmap.Rows; i++)
                            {
                                wr.WriteStartElement("Row");
                                {
                                    wr.WriteAttributeString("id", i.ToString());
                                    for (int j = 0; j < tilesmap.Columns; j++)
                                    {
                                        wr.WriteString(tilesmap[j, i].ToString());
                                        if (j != tilesmap.Columns - 1)
                                            wr.WriteString("\t");
                                    }
                                }
                                wr.WriteEndElement();//Row
                            }
                        }
                        wr.WriteEndElement();//MatrixIndex
                        //lưu tileSet
                        if (tilesmap.TileSet != null)
                        {
                            TileSet.Save(wr, tilesmap.TileSet, path);
                        }
                        //lưu object
                    }
                    wr.WriteEndElement();//Tilesmap
                }
                wr.WriteEndDocument();
            }
        }

        public void Resize(int columns, int rows)
        {
            if (columns == this.Columns && rows == this.Rows)
            {
                return;
            }
            _mapSize = new Point(columns, rows);
            _matrixIndex = new int[columns, rows];
        }

        //Trả về mapheight và map width theo
        //số dòng * chiều cao 1 tile
        //số hàng * chiều rộng 1 tile
        public int GetMapHeight()
        {
            return this.Rows * FrmMain.Settings.TileSize.Height;
        }

        public int GetMapWidth()
        {
            return this.Columns * FrmMain.Settings.TileSize.Width;
        }

        public Size GetMapSize()
        {
            return new Size(GetMapWidth(), GetMapHeight());
        }

    }
}
