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

        public FrmMain()
        {
            InitializeComponent();
            
        }
            
      
        private void Panel_Scroll(object sender, ScrollEventArgs e)
        {
            
        }
        
    }
}


//string path = @"C:\Users\_L_\Documents\GitHub\MapEditor\MapEditor\bin\Debug\stage1.xml";
//            string paht2 = @"C:\Users\_L_\Documents\GitHub\MapEditor\MapEditor\bin\Debug\stage1_1.xml";
           
          
//            //XmlTextReader reader = new XmlTextReader(fileName);
//            tilesMap = TilesMap.Load(path);

//            TilesMap.Save(tilesMap, paht2);
//            //TileSet.Save(
            
