#region Namespaces
using System;
using System.Diagnostics;
using Autodesk.Navisworks.Api.Plugins;
using ComBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;
using COMApi = Autodesk.Navisworks.Api.Interop.ComApi;
using NwVertex = Autodesk.Navisworks.Api.Interop.ComApi.InwSimpleVertex;
#endregion // Namespaces

namespace NwGeoPrimitives
{
  [PluginAttribute( "NwGeoPrimitives", "JT__",
    ToolTip = "Retrieve geometry primitives",
    DisplayName = "NwGeoPrimitives" )]
  public class GeoPrimitivesPlugin : AddInPlugin
  {
    class CallbackGeomListener : COMApi.InwSimplePrimitivesCB
    {
      static public long LineCount { get; set; }
      static public long PointCount { get; set; }
      static public long SnapPointCount { get; set; }
      static public long TriangleCount { get; set; }

      static public void Init()
      {
        LineCount = 0;
        PointCount = 0;
        SnapPointCount = 0;
        TriangleCount = 0;
      }

      public void Line( NwVertex v1, NwVertex v2 )
      {
        ++LineCount;
      }

      public void Point( NwVertex v1 )
      {
        ++PointCount; 
      }

      public void SnapPoint( NwVertex v1 )
      {
        ++SnapPointCount;
      }

      public void Triangle(
        NwVertex v1,
        NwVertex v2,
        NwVertex v3 )
      {
        ++TriangleCount;
      }
    }

    void walkNode(
      COMApi.InwOaNode parentNode,
      bool bFoundFirst )
    {
      if( parentNode.IsGroup )
      {
        COMApi.InwOaGroup group = (COMApi.InwOaGroup) parentNode;
        long n = group.Children().Count;

        for( long i = 1; i <= n; ++i )
        {
          COMApi.InwOaNode newNode = group.Children()[i];

          if( (!bFoundFirst) && (n > 1) )
          {
            bFoundFirst = true;
          }
          walkNode( newNode, bFoundFirst );
        }
      }
      else if( parentNode.IsGeometry )
      {
        CallbackGeomListener cbl 
          = new CallbackGeomListener();

        COMApi.InwNodeFragsColl fragsColl 
          = parentNode.Fragments();

        long nFrags = fragsColl.Count;

        Debug.WriteLine( "frags count:" 
          + nFrags.ToString() );

        for( long i = 1; i <= nFrags; ++i )
        {
          COMApi.InwOaFragment3 frag = fragsColl[i];

          frag.GenerateSimplePrimitives(
            COMApi.nwEVertexProperty.eNORMAL, cbl );
        }
        _nFragsTotal += nFrags;
        ++_nNodesTotal;
      }
    }

    long _nNodesTotal;
    long _nFragsTotal;

    public override int Execute( params string[] ps )
    {
      _nNodesTotal = 0;
      _nFragsTotal = 0;
      CallbackGeomListener.Init();

      DateTime dt = DateTime.Now;

      // Convert to COM selection

      COMApi.InwOpState oState = ComBridge.State;
      walkNode( oState.CurrentPartition, false );

      TimeSpan ts = DateTime.Now - dt;
      double ms = ts.TotalMilliseconds;

      string s = string.Format(
        "Retrieved {0} geometry nodes and {1} fragments in {2} milliseconds.",
        _nNodesTotal, _nFragsTotal, ms.ToString( "0.##" ) );
      Debug.Print( s );

      Debug.Print( "Line, point, snappoint and triangle counts: {0}, {1}, {2}, {3}",
        CallbackGeomListener.LineCount, CallbackGeomListener.PointCount,
        CallbackGeomListener.SnapPointCount, CallbackGeomListener.TriangleCount );

      return 0;
    }
  }
}
