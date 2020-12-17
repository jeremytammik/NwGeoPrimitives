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
  [PluginAttribute( "NwGeoPrimitives", "JT__", ToolTip = "NwGeoPrimitives", DisplayName = "NwGeoPrimitives" )]
  public class GeoPrimitivesPlugin : AddInPlugin
  {
    class CallbackGeomListener : COMApi.InwSimplePrimitivesCB
    {
      public void Line( NwVertex v1, NwVertex v2 )
      {
        // do your work 
      }

      public void Point( NwVertex v1 )
      {
        // do your work  
      }

      public void SnapPoint( NwVertex v1 )
      {
        // do your work  
      }

      public void Triangle(
        NwVertex v1,
        NwVertex v2,
        NwVertex v3 )
      {
        // do your work  
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
          COMApi.InwOaNode newNode = group.Children()[ i ];

          if( (!bFoundFirst) && (n > 1) )
          {
            bFoundFirst = true;
          }
          walkNode( newNode, bFoundFirst );
        }
      }
      else if( parentNode.IsGeometry )
      {
        long nFrags = parentNode.Fragments().Count;
        Debug.WriteLine( "frags count:" + nFrags.ToString() );

        for( long i = 1; i <= nFrags; i++ )
        {
          CallbackGeomListener callbkListener = new CallbackGeomListener();

          COMApi.InwNodeFragsColl fragsColl = parentNode.Fragments();
          COMApi.InwOaFragment3 frag = fragsColl[ i ];

          frag.GenerateSimplePrimitives(
            COMApi.nwEVertexProperty.eNORMAL,
            callbkListener );
        }
        _nFragsTotal += nFrags;
        ++_nNodesTotal;
      }
    }

    long _nNodesTotal = 0;
    long _nFragsTotal = 0;

    public override int Execute( params string[] ps )
    {
      _nNodesTotal = 0;
      _nFragsTotal = 0;
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
      return 0;
    }
  }
}
