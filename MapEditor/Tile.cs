using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

//Tile là class dùng để vẻ các ô vuông được cắt từ image tileSet
//với id được đánh dấu từ trái sang phải ,trên xuống
//name được gán bằng "tile_" + id
//image tileSet là hình được dùng để cắt
//srcRect là khu vực của tile theo hình vuông hay chữ nhật trên image TileSet


namespace MapEditor
{
    //INotifyPropertyChanged chỉ có duy nhất một thành viên là event mang tên PropertyChanged. 
    //Khi định nghĩa một class để dùng cho binding,
    //cần kích hoạt event này trong setter cho mỗi property trong class đó.
    //https://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged(v=vs.110).aspx
    public class Tile : INotifyPropertyChanged
    {
        #region FIELDS_PROPERTIES
        //id cho tile
        private int _id;

        public int Id
        {
            get { return _id; }
            set { setProperty(ref _id, value, "Id"); }
        }

        //name Tile
        private string _name;

        public string Name
        {
            get { return _name; }
            set { setProperty(ref _name, value, "Name"); }
        }

        //srcRect dùng để lưu thông tin để vẻ của tile lên màn hình
        private Rectangle _srcRect;

        public Rectangle SrcRect
        {
            get { return _srcRect; }
            set { setProperty(ref _srcRect, value, "SrcRect"); }
        }

        //ảnh gồm nhiều tile,khi vẽ cắt ảnh bằng srcRect như sprite
        Image _image;

        public Image Image
        {
            get { return _image; }
            set { setProperty(ref _image, value, "Image"); }
        }

        //implment event PropertyChanged cho class
        public event PropertyChangedEventHandler PropertyChanged;

        private void setProperty<T>(ref T obj, T value, string propertyName)
        {
            if (object.Equals(obj, value))
                return;
            obj = value;
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region CONSTRCUTOR 
        public Tile(Image image, Rectangle srcRect, int id)
        {
            Id = id;
            SrcRect = srcRect;
            Image = image;
            Name = "tile_" + id.ToString();
        }
        #endregion

        #region METHODS
        //trả về ảnh đã cắt bằng srcRect
        public Bitmap getBitmap()
        { 
            return ((_image as Bitmap).Clone(SrcRect,_image.PixelFormat));
        }
        
        //vẽ bitmap ra màn hình
        public void draw(Graphics graphics, Point position, Size size) 
        {
            graphics.DrawImage(_image,new Rectangle(position,size),SrcRect,GraphicsUnit.Pixel);
        }
        #endregion

    }
}
