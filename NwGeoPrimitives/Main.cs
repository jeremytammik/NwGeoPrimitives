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
      Debug.Print( "{0}:{1}", doc.CurrentFileName + doc.Title );

      string filename = doc.FileName;
      Units units = doc.Units;
      DocumentModels models = doc.Models;
      DocumentInfoPart info = doc.DocumentInfo;
      DocumentDatabase db = doc.Database;
      bool ignoreHidden = true;
      BoundingBox3D bb = doc.GetBoundingBox( ignoreHidden );

      GeoPrimitives gp = new GeoPrimitives();
      gp.Execute();
      return 0;
    }
  }
}
