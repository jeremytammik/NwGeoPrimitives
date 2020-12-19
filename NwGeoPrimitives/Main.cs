#region Namespaces
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.DocumentParts;
using Autodesk.Navisworks.Api.Plugins;
using System.Diagnostics;
using System.Linq;
#endregion // Namespaces

namespace NwGeoPrimitives
{
  [PluginAttribute( "NwGeoPrimitives", "JT__",
    ToolTip = "Retrieve documents and geometry primitives",
    DisplayName = "NwGeoPrimitives" )]
  class Main : AddInPlugin
  {
    public override int Execute( params string[] ps )
    {
      Document doc = Application.ActiveDocument;
      string currentFilename = doc.CurrentFileName;
      string filename = doc.FileName;
      string title = doc.Title;

      Units units = doc.Units;
      DocumentModels models = doc.Models;
      DocumentInfoPart info = doc.DocumentInfo;
      string currentSheetId = info.Value.CurrentSheetId; // "little_house_2021.rvt"
      DocumentDatabase db = doc.Database;
      bool ignoreHidden = true;
      BoundingBox3D bb = doc.GetBoundingBox( ignoreHidden );
      Point3D min = bb.Min;
      Point3D max = bb.Max;
      int nModels = models.Count;

      Debug.Print( "{0}: sheet {1}, bounding box {2}, {3} model{4}{5}",
        title, currentSheetId, Util.BoundingBoxString( bb ),
        nModels, Util.PluralSuffix( nModels ), Util.DotOrColon( nModels ) );

      // First attempt, based on Navisworks-Geometry-Primitives,
      // using walkNode oState.CurrentPartition:

      //WalkPartition wp = new WalkPartition();
      //wp.Execute();

      foreach( Model model in models )
      {
        ModelItem rootItem = model.RootItem;
        ModelItemEnumerableCollection mis = rootItem.DescendantsAndSelf;
        Debug.Print( "  {0}: {1}", model.FileName, mis.Count() );
        foreach( ModelItem mi in mis )
        {
          Debug.Print( "    {0} bb {1}", 
            mi.ClassDisplayName, 
            Util.BoundingBoxString( mi.BoundingBox() ) );
        }
      }
      return 0;
    }
  }
}
