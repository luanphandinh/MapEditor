using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wexman.Design;
/*
 *  Property grid không edit được trên Dictionary nên cần dùng package của Wexman để edit được dictionary.
 *  Để download package vào nuget manager tìm kiếm với từ khoá GenericDictionaryEditor
 *  Cần add reference System.Design
 *  Tham khảo:
 *  http://gendictedit.codeplex.com/SourceControl/latest#trunk/Example/FourtyTwo.cs
 */
namespace MapEditor
{
    class GameObject :INotifyPropertyChanged
    {
        #region FILEDS
        //FIELDS
        private Rectangle _activeBound;
        private Rectangle _initBound;

        private Image _image;
        //ứng với id trong game
        private int id;
        //dùng để hiển thị trên listbox,ko quan tamam trong game
        private string name;
        private Dictionary<string, string> _parameters;
        #endregion

        //PROPERTIES
        #region PROPERTIES

        public Image Image
        {
            get { return _image; }
            set 
            { 
                _image = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Image"));
            }
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set 
            { 
                name = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ActiveBound"));
            }
        }

        public Rectangle ActiveBound
        {
            get { return _activeBound; }
            set 
            {
                _activeBound = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ActiveBound"));
            }
        }


        public Rectangle InitBound
        {
            get { return _initBound; }
            set 
            { 
                _initBound = value;
                OnPropertyChanged(new PropertyChangedEventArgs("InitBound"));
            }
        }

        // Danh sách các đối số dùng để khởi tạo đối tượng, bao gồm cả Position.
        //[Editor(typeof(GenericDictionaryEditor<string, string>), typeof(UITypeEditor))]
        //[GenericDictionaryEditor(Title = "Parameters", KeyDisplayName = "Name", KeyDefaultProviderType = typeof(KeyDefault), ValueDefaultProviderType = typeof(ValueDefault), ValueDisplayName = "Value")]
        //public Dictionary<string, string> Parameters
        //{
        //    get { return _parameters; }
        //    set { _parameters = value; }
        //}

        //Được sử dụng để thay đổi giá trị property của gameObject 
        //khi property đó được bind với UI và UI thay đổi giá trị
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        #endregion

        public GameObject(int x, int y, int width, int height)
        {
            _parameters = new Dictionary<string, string>();
            this.InitBound = new Rectangle(x, y, width, height);
            this.ActiveBound = new Rectangle(x, y, width, height);
        }

        public void SetActiveBound(Rectangle rect)
        {
            this._activeBound = rect;
        }

        public void SetInitBound(Rectangle rect)
        {
            this._initBound = rect;
        }
        //worldHeigt là chiều cao của map
        public Rectangle GetInitBoundTransform(int worldheight)
        {
            return new Rectangle(
                InitBound.X,
                worldheight - this.InitBound.Y,
                InitBound.Width,
                InitBound.Height
                );
        }

        public Rectangle GetActiveBoundTransform(int worldheight)
        {
            return new Rectangle(
                this.ActiveBound.X,
                worldheight - this.ActiveBound.Y,
                ActiveBound.Width,
                ActiveBound.Height
                );
        }

        /// <summary>
        /// Key default trả về giá trị default cho dictionary
        /// </summary>
        public class KeyDefault : DefaultProvider<string>
        {
            public override string GetDefault(DefaultUsage usage)
            {
                return "params";
            }
        }
        public class ValueDefault : DefaultProvider<string>
        {
            public override string GetDefault(DefaultUsage usage)
            {
                return "0";
            }
        }
    }
}
