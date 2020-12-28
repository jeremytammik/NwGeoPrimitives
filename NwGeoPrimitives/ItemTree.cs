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

    string Indent( int level )
    {
      const int indent_step = 2;
      return new string( ' ', level * indent_step );
    }

    public void WriteTo( StreamWriter w )
    {
      int indentation_level = 0;

      int n = _layers.Count();
      Debug.Print(
        "{0}{1} layers containing {2} hierarchical model items",
        Indent( indentation_level ), n, ItemData.InstanceCount );

      int iLayer = 0;

      foreach( ItemData layer in _layers )
      {
        List<ItemData> cats = layer.Children;
        n = cats.Count();
        w.Write( "    {0}{1}: {2} has {3} categories",
          Indent( indentation_level ), 
          iLayer++, layer, n );

        int iCategory = 0;

        foreach( ItemData cat in cats )
        {
          List<ItemData> fams = cat.Children;
          n = fams.Count();
          w.Write( "    {0}{1}: {2} has {3} families",
            Indent( indentation_level ), 
            iCategory++, cat, n );

          int iFamily = 0;

          foreach( ItemData fam in fams )
          {
            List<ItemData> types = fam.Children;
            n = types.Count();
            w.Write( "    {0}{1}: {2} has {3} types",
              Indent( indentation_level ),
              iFamily++, fam, n );

            int iType = 0;

            foreach( ItemData typ in types )
            {
              List<ItemData> instances = typ.Children;
              n = instances.Count();
              w.Write( "    {0}{1}: {2} has {3} instances",
                Indent( indentation_level ),
                iType++, typ, n );

              int iInst = 0;

              foreach( ItemData inst in instances )
              {
                List<ItemData> subinsts = inst.Children;
                n = subinsts.Count();
                w.Write( "    {0}{1}: {2} has {3} subinstances",
                  Indent( indentation_level ),
                  iInst++, inst, n );

                int iSubinst = 0;

                foreach( ItemData subinst in subinsts )
                {
                  List<ItemData> children = subinst.Children;
                  n = children.Count();
                  w.Write( "    {0}{1}: {2} has {3} children",
                    Indent( indentation_level ),
                    iSubinst++, inst, n );
                }
              }
            }
          }
        }
      }
    }
  }
}
