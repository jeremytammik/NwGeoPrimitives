#region Namespaces
using Autodesk.Navisworks.Api;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
#endregion // Namespaces

namespace NwGeoPrimitives
{
  class ItemTree
  {
    List<ItemData> _layers;

    public int nItems { get; }

    public ItemTree( ModelItemEnumerableCollection mis )
    {
      int n = ItemData.InstanceCount;

      _layers = new List<ItemData>( mis
        .Where<ModelItem>( mi => mi.IsLayer )
        .Select<ModelItem, ItemData>(
          mi => new ItemData( mi ) ) );

      nItems = ItemData.InstanceCount - n;
    }

    static string Indent( int level )
    {
      const int indent_step = 2;
      return new string( ' ', level * indent_step );
    }

    static void Recurse( 
      StreamWriter w, 
      int level, 
      int i, 
      ItemData d )
    {
      List<ItemData> subinsts = d.Children;
      int n = subinsts.Count();
      w.WriteLine( "{0}{1}: {2} has {3} subinstances",
        Indent( level ), i, d, n );

      int iSubinst = 0;

      foreach( ItemData subinst in subinsts )
      {
        Recurse( w, level + 1, iSubinst, subinst );
      }
    }

    public void WriteTo( StreamWriter w )
    {
      //int indentation_level = 0;

      int n = _layers.Count();
      w.WriteLine(
        "{0}{1} layers containing {2} hierarchical model items",
        Indent( 0 ), n, ItemData.InstanceCount );

      //++indentation_level;
      int iLayer = 0;

      foreach( ItemData layer in _layers )
      {
        List<ItemData> cats = layer.Children;
        n = cats.Count();
        w.WriteLine( "{0}{1}: {2} has {3} categories",
          Indent( 1 ), 
          iLayer++, layer, n );

        int iCategory = 0;

        foreach( ItemData cat in cats )
        {
          List<ItemData> fams = cat.Children;
          n = fams.Count();
          w.WriteLine( "{0}{1}: {2} has {3} families",
            Indent( 2 ), 
            iCategory++, cat, n );

          int iFamily = 0;

          foreach( ItemData fam in fams )
          {
            List<ItemData> types = fam.Children;
            n = types.Count();
            w.WriteLine( "{0}{1}: {2} has {3} types",
              Indent( 3 ),
              iFamily++, fam, n );

            int iType = 0;

            foreach( ItemData typ in types )
            {
              List<ItemData> instances = typ.Children;
              n = instances.Count();
              w.WriteLine( "{0}{1}: {2} has {3} instances",
                Indent( 4 ),
                iType++, typ, n );

              int iInst = 0;

              foreach( ItemData inst in instances )
              {
                List<ItemData> subinsts = inst.Children;
                n = subinsts.Count();
                w.WriteLine( "{0}{1}: {2} has {3} subinstances",
                  Indent( 5 ),
                  iInst++, inst, n );

                int iSubinst = 0;

                foreach( ItemData subinst in subinsts )
                {
                  Recurse( w, 6, iSubinst++, subinst );
                }
              }
            }
          }
        }
      }
    }
  }
}
