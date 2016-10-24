using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

//chi tiết INotifyPropertyChanged, INotifyCollectionChanged
//http://www.codeproject.com/Articles/42536/List-vs-ObservableCollection-vs-INotifyPropertyCha
namespace MapEditor
{
    //Tilset là class quản lí các tile 
    //image dùng để quản lí image được load từ DB sau đó gán cho từng Tile
    //bao gồm 1 list các tile
    //được sử dụng trong FrmCreatTiles để load imamge sau đó
    //gán các giá trị id và name cho tile
    //đưa các tile vào danh sách
    
    //INotifyPropertyChanged cung cấp event PropertyChanged mỗi khi giá trị các properties của 
    //TileSet bị thay đổi và cập nhật lại UI
    public class TileSet : INotifyPropertyChanged, INotifyCollectionChanged
    {
        #region FIELDS_PROPERTIES
        ////FIELDS

        //chiều rộng của 1 tile
        private int _widthtile;            
        //Chiều cao của 1 tile
        private int _heighttile;              
        //tên tile
        private string _filename;
        //Số cột của tileset
        private int _columns;
        //Số hàng của tileset
        private int _rows;
        //Danh sách các Tile được cắt ra từ tileSet
        private IList<Tile> _listTiles;
        
        //PROPERTIES
        public int Widthtile
        {
            get { return _widthtile; }
            set { this.setProperty(ref _widthtile, value, "Widthtile"); }
        }

        public int Heighttile
        {
            get { return _heighttile; }
            set { this.setProperty(ref _heighttile, value, "Heighttile"); }
        }

        public string Filename
        {
            get { return _filename; }
            set { this.setProperty(ref _filename, value, "FileName"); }
        }

        //Số cột của tileSet,file image
        public int Columns
        {
            get { return _columns; }
            set { setProperty(ref _columns, value, "Columns"); }
        }
        //Số hàng của tileSet,file image
        public int Rows
        {
            get { return _rows; }
            set { this.setProperty(ref _rows, value, "Rows"); }
        }

        public Image Image { get; set; }

        public ObservableCollection<Tile> ListTiles
        {
            get { return _listTiles as ObservableCollection<Tile>; }
            set
            {
                if (_listTiles != value)
                {
                    if (_listTiles != null)
                        (_listTiles as ObservableCollection<Tile>).CollectionChanged -= this.CollectionChanged;
                    value.CollectionChanged += CollectionChanged;
                    setProperty(ref _listTiles, value, "ListTiles");
                }
            }
        }

        
        //EVENTS

        //dùng để gán event CollectionChanged cho listTiles as ObservableCollection
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        //tạo event dùng cho interface INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        
        private void setProperty<T>(ref T obj, T value, string propertyName)
        {
            if (Object.Equals(obj, value))
                return;
            obj = value;
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region CONSTRUCTOR

        public TileSet()
        {
            this.ListTiles = new ObservableCollection<Tile>();
            
            this.PropertyChanged += TileSet_PropertyChanged;
        }

        private void TileSet_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Widthtile")
            {
                if (Image != null)
                    this.Columns = (Widthtile == 0) ? 1 : (Image.Width / Widthtile);
            }
            else if (e.PropertyName == "Heighttile")
            {
                if (Image != null)
                    this.Rows = (Heighttile == 0) ? 1 : (Image.Height / Heighttile);
            }
            else if (e.PropertyName == "Image")
            {
                this.Columns = (Widthtile == 0) ? 1 : (Image.Width / Widthtile);
                this.Rows = (Heighttile == 0) ? 1 : (Image.Height / Heighttile);
            }
        }

        #endregion
        //
        #region METHODS
        //lưu thông tin xuống file xml
        //cách đọc và ghi xml file
        //https://thanhcuong.wordpress.com/2011/01/17/d%E1%BB%8Dc-v-ghi-xml-v%E1%BB%9Bi-c-read-and-write-xml-with-c/
        #region SAVE
        
        public static void Save(TileSet tileset, string filename)
        {
            XmlTextWriter wr = new XmlTextWriter(filename, Encoding.UTF8);
            wr.Formatting = Formatting.Indented;
            wr.WriteStartDocument();
            TileSet.Save(wr, tileset, filename);
            wr.WriteEndDocument();
            wr.Close();
        }

        public static void Save(XmlTextWriter writer, TileSet tileset, string filename)
        {
            writer.WriteStartElement("TileSet");
            {
                writer.WriteAttributeString("Columns", tileset.Columns.ToString());
                writer.WriteAttributeString("Rows", tileset.Rows.ToString());
                writer.WriteAttributeString("Widthtile", tileset.Widthtile.ToString());
                writer.WriteAttributeString("Heighttile", tileset.Heighttile.ToString());

                //lưu fileName dưới dạng đường dẫn tuyệt đối 
                string imageFileName = tileset.Filename.Substring(tileset.Filename.LastIndexOf(@"\") + 1);
                //lưu imageFileName
                writer.WriteAttributeString("FileName", imageFileName);

                string imagefullpath = filename.Substring(0, filename.LastIndexOf('\\')) + @"\\" + imageFileName;
                if (System.IO.File.Exists(imagefullpath) == false)
                {
                    System.IO.File.Copy(tileset.Filename, imagefullpath);
                }

                writer.WriteStartElement("Tiles");
                {
                    foreach (var tileItem in tileset.ListTiles)
                    {
                        writer.WriteStartElement("Tile");
                        {
                            writer.WriteAttributeString("Id", tileItem.Id.ToString());
                            writer.WriteAttributeString("Name", tileItem.Name);
                            writer.WriteStartElement("Rect");
                            {
                                writer.WriteAttributeString("X",tileItem.SrcRect.X.ToString());
                                writer.WriteAttributeString("Y", tileItem.SrcRect.Y.ToString());
                                writer.WriteAttributeString("Width", tileItem.SrcRect.Width.ToString());
                                writer.WriteAttributeString("Height", tileItem.SrcRect.Height.ToString());
                            }
                            writer.WriteEndElement();//rect
                        }
                        writer.WriteEndElement();//tile
                    }
                }
                writer.WriteEndElement();///tiles
            }
            writer.WriteEndElement();//tielset

        }
        #endregion

        #region LOAD

        public static TileSet Load(XmlReader reader,string fileName)
        {
            TileSet tileSet = new TileSet();
            //lấy columns rows widthtile và heighttile
            tileSet.Columns = Int32.Parse(reader.GetAttribute("Columns"));
            tileSet.Rows = Int32.Parse(reader.GetAttribute("Rows"));
            tileSet.Widthtile = Int32.Parse(reader.GetAttribute("Widthtile"));
            tileSet.Heighttile = Int32.Parse(reader.GetAttribute("Heighttile"));

            //lấy filename
            string imageFileName = reader.GetAttribute("FileName");
            string relativepath = fileName.Substring(0, fileName.LastIndexOf('\\') + 1);  // +1 là dấu \ trong path
            //cộng file name với đường dẫn
            tileSet.Filename = relativepath + imageFileName;
            //tạo image
            if (System.IO.File.Exists(tileSet.Filename))
            {
                tileSet.Image = Image.FromFile(tileSet.Filename);
            }
            else throw new Exception("Error Load Image");

            //đọc tileSet
            reader.ReadStartElement("TileSet");
            while (reader.NodeType != XmlNodeType.EndElement || reader.Name != "TileSet")
            {
                reader.Read();
                if (reader.IsStartElement())
                {
                    if (reader.IsStartElement("Tiles"))
                    {
                        reader.ReadStartElement("Tiles");
                        while (reader.NodeType != XmlNodeType.EndElement || reader.Name != "Tiles")
                        {
                            reader.Read();
                            if (reader.IsStartElement("Tile"))
                            {
                                var tile = readTile(reader);
                                tile.Image = tileSet.Image;
                                tileSet.ListTiles.Add(tile);
                            }
                        }
                        continue;
                    }
                }
            }
            return tileSet;
        }

        private static Tile readTile(XmlReader reader) 
        {
            int id = Int32.Parse(reader.GetAttribute("Id"));
            string name = reader.GetAttribute("Name");
            Rectangle rect = Rectangle.Empty;

            reader.ReadStartElement("Tile");
            while (reader.NodeType != XmlNodeType.EndElement || reader.Name != "Tile")
            {
                reader.Read();

                if (reader.IsStartElement("Rect"))
                {
                    rect.X = Int32.Parse(reader.GetAttribute("X"));
                    rect.Y = Int32.Parse(reader.GetAttribute("Y"));
                    rect.Width = Int32.Parse(reader.GetAttribute("Width"));
                    rect.Height = Int32.Parse(reader.GetAttribute("Height"));
                    continue;
                }
            }

            Tile tile = new Tile(null, rect, id);
            tile.Name = name;
            return tile;
        }
        #endregion
        #endregion
    }
}
