using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace MapEditor
{
    public partial class FrmMain : Form
    {
        public static ApplicationSetttings Settings = new ApplicationSetttings();

        MapController _mapController = new MapController();

        public FrmMain()
        {
            InitializeComponent();
            
        }
            
      
        private void Panel_Scroll(object sender, ScrollEventArgs e)
        {
            
        }

        private void creatTilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._mapController.CreateTileSet();
            if (this._mapController.TilesMap == null)
                return;
            if (this._mapController.TilesMap.TileSet == null)
                return;
            this.listView1.LargeImageList = _mapController.getImageList();

            if (listView1.Items.Count > 0)
                listView1.Items.Clear();
            this.listView1.Items.AddRange(_mapController.getListViewItem().ToArray());
        }
        
    }
}


//string path = @"C:\Users\_L_\Documents\GitHub\MapEditor\MapEditor\bin\Debug\stage1.xml";
//            string paht2 = @"C:\Users\_L_\Documents\GitHub\MapEditor\MapEditor\bin\Debug\stage1_1.xml";
           
          
//            //XmlTextReader reader = new XmlTextReader(fileName);
//            tilesMap = TilesMap.Load(path);

//            TilesMap.Save(tilesMap, paht2);
//            //TileSet.Save(
            
