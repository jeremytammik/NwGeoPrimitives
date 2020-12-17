#region Namespaces
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.DocumentParts;
using Autodesk.Navisworks.Api.Plugins;
using System.Diagnostics;
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

      Debug.Print( "{0}: sheet {1}, bounding box {2}",
        title, currentSheetId, Util.BoundingBoxString( bb ) );

      GeoPrimitives gp = new GeoPrimitives();
      gp.Execute();
      return 0;
    }
  }
}
