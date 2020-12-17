#region Namespaces
using Autodesk.Navisworks.Api.Plugins;
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
      GeoPrimitives gp = new GeoPrimitives();
      gp.Execute();
      return 0;
    }
  }
}
