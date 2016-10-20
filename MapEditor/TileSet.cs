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
    //INotifyPropertyChanged cung cấp event PropertyChanged mỗi khi giá trị các properties của 
    //TileSet bị thay đổi và cập nhật lại UI
    class TileSet : INotifyPropertyChanged, INotifyCollectionChanged
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
            set { setProperty(ref _filename, value, "FileName"); }
        }

        public int Columns
        {
            get { return _columns; }
            set { setProperty(ref _columns, value, "Columns"); }
        }

        public int Rows
        {
            get { return _rows; }
            set { setProperty(ref _rows, value, "Rows"); }
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
    }
}
