#region Namespaces
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.DocumentParts;
using Autodesk.Navisworks.Api.Plugins;
using System.Collections.Generic;
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
      int i, n;
      
      n = models.Count;

      Debug.Print( "{0}: sheet {1}, bounding box {2}, {3} model{4}{5}",
        title, currentSheetId, Util.BoundingBoxString( bb ),
        n, Util.PluralSuffix( n ), Util.DotOrColon( n ) );

      // First attempt, based on Navisworks-Geometry-Primitives,
      // using walkNode oState.CurrentPartition:

      //WalkPartition wp = new WalkPartition();
      //wp.Execute();

      // Second attempt, retrieving root item descendants:

      //List<string> Categories = new List<string>();

      Debug.Print( "Revit model item tree structure:" );

      foreach( Model model in models )
      {
        ModelItem rootItem = model.RootItem;

        ModelItemEnumerableCollection mis 
          = rootItem.DescendantsAndSelf;

        n = mis.Count();
        Debug.Print( "  {0}: {1} model items", 
          model.FileName, n );

        ItemData.InstanceCount = 0;

        List<ItemData> layers
          = new List<ItemData>( mis
            .Where<ModelItem>( mi => mi.IsLayer )
            .Select<ModelItem, ItemData>( 
              mi => new ItemData( mi ) ) );

        n = layers.Count();
        Debug.Print( 
          "  {0} layers containing {1} hierarchical model items", 
          n, ItemData.InstanceCount );

        int iLayer = 0;

        foreach( ItemData layer in layers )
        {
          List<ItemData> cats = layer.Children;
          n = cats.Count();
          Debug.Print(
            "    {0}: {1} has {2} categories",
            iLayer++, layer, n );

          int iCategory = 0;

          foreach( ItemData cat in cats )
          {
            List<ItemData> fams = cat.Children;
            n = fams.Count();
            Debug.Print(
              "      {0}: {1} has {2} families",
              iCategory++, cat, n );

            int iFamily = 0;

            foreach( ItemData fam in fams )
            {
              List<ItemData> types = fam.Children;
              n = types.Count();
              Debug.Print(
                "        {0}: {1} has {2} types",
                iFamily++, fam, n );

              int iType = 0;

              foreach( ItemData typ in types )
              {
                List<ItemData> instances = typ.Children;
                n = instances.Count();
                Debug.Print(
                  "          {0}: {1} has {2} instances",
                  iType++, typ, n );

                int iInst = 0;

                foreach( ItemData inst in instances )
                {
                  List<ItemData> subinsts = inst.Children;
                  n = subinsts.Count();
                  Debug.Print(
                    "            {0}: {1} has {2} subinstances",
                    iInst++, inst, n );

                  int iSubinst = 0;

                  foreach( ItemData subinst in subinsts )
                  {
                    List<ItemData> children = subinst.Children;
                    n = children.Count();
                    Debug.Print(
                      "            {0}: {1} has {2} children",
                      iSubinst++, inst, n );
                  }

                }
              }
            }
          }
        }

        if( 50 > n )
        {
          List<ModelItem> migeos 
            = new List<ModelItem>();

          foreach( ModelItem mi in mis )
          {
            Debug.Print(
              "    '{0}' '{1}' '{2}' has geo {3}",
              mi.DisplayName, mi.ClassDisplayName,
              mi.ClassName, mi.HasGeometry );

            if( mi.HasGeometry )
            {
              migeos.Add( mi );
            }
          }
          Debug.Print( "  {0} model items have geometry:", migeos.Count() );
          foreach( ModelItem mi in migeos )
          {
            Debug.Print(
              "    '{0}' '{1}' '{2}' {3} bb {4}",
              mi.DisplayName, mi.ClassDisplayName,
              mi.ClassName, mi.HasGeometry,
              Util.BoundingBoxString( mi.BoundingBox() ) );

            if( "Floor" == mi.DisplayName )
            {
              RvtProperties.DumpProperties( mi );
              RvtProperties props = new RvtProperties( mi );
              int id = props.ElementId;
            }
          }
        }
      }
      return 0;
    }
  }
}
