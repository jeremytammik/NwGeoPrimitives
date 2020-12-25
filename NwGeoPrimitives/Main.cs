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

      foreach( Model model in models )
      {
        ModelItem rootItem = model.RootItem;

        ModelItemEnumerableCollection mis 
          = rootItem.DescendantsAndSelf;

        n = mis.Count();
        Debug.Print( "  {0}: {1} model items", 
          model.FileName, n );

        List<ModelItem> layers
          = new List<ModelItem>( 
            mis.Where<ModelItem>( 
              mi => mi.IsLayer ) );

        n = layers.Count();
        Debug.Print( "  {0} layers", n );
        int iLayer = 0;

        foreach( ModelItem layer in layers )
        {
          ModelItemEnumerableCollection cats
            = layer.Children;
          n = cats.Count();
          Debug.Print(
            "    layer {0}: '{1}' '{2}' '{3}' has {4} categories",
            iLayer++, layer.DisplayName, layer.ClassDisplayName,
            layer.ClassName, n );

          int iCategory = 0;

          foreach( ModelItem cat in cats )
          {
            ModelItemEnumerableCollection fams
              = cat.Children;
            n = fams.Count();
            Debug.Print(
              "    category {0}: '{1}' '{2}' '{3}' has {4} families",
              iCategory++, cat.DisplayName, cat.ClassDisplayName,
              cat.ClassName, n );

            int iFamily = 0;

            foreach( ModelItem fam in fams )
            {
              ModelItemEnumerableCollection types
                = fam.Children;
              n = types.Count();
              Debug.Print(
                "      family {0}: '{1}' '{2}' '{3}' has {4} types",
                iFamily++, fam.DisplayName, fam.ClassDisplayName,
                fam.ClassName, n );

              int iType = 0;

              foreach( ModelItem typ in types )
              {
                ModelItemEnumerableCollection instances
                  = typ.Children;
                n = instances.Count();
                Debug.Print(
                  "      type {0}: '{1}' '{2}' '{3}' has {4} instances",
                  iType++, typ.DisplayName, typ.ClassDisplayName,
                  typ.ClassName, n );

                int iInst = 0;

                foreach( ModelItem inst in instances )
                {
                  ModelItemEnumerableCollection children
                    = inst.Children;
                  n = children.Count();
                  Debug.Print(
                    "      instance {0}: '{1}' '{2}' '{3}' has {4} children",
                    iInst++, typ.DisplayName, typ.ClassDisplayName,
                    typ.ClassName, n );
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
