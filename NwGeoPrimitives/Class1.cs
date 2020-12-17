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
        long subNodesCount = group.Children().Count;

        for( long subNodeIndex = 1; subNodeIndex <= subNodesCount; subNodeIndex++ )
        {
          COMApi.InwOaNode newNode = group.Children()[ subNodeIndex ];

          if( (!bFoundFirst) && (subNodesCount > 1) )
          {
            bFoundFirst = true;
          }
          walkNode( newNode, bFoundFirst );
        }
      }
      else if( parentNode.IsGeometry )
      {
        long fragsCount = parentNode.Fragments().Count;
        Debug.WriteLine( "frags count:" + fragsCount.ToString() );

        for( long fragindex = 1; fragindex <= fragsCount; fragindex++ )
        {
          CallbackGeomListener callbkListener = new CallbackGeomListener();

          COMApi.InwNodeFragsColl fragsColl = parentNode.Fragments();
          COMApi.InwOaFragment3 frag = fragsColl[ fragindex ];

          frag.GenerateSimplePrimitives(
            COMApi.nwEVertexProperty.eNORMAL,
            callbkListener );
        }
        fragCount += fragsCount;
        geoNodeCount += 1;
      }
    }

    DateTime dt = DateTime.Now;
    long geoNodeCount = 0;
    long fragCount = 0;

    public override int Execute( params string[] ps )
    {
      geoNodeCount = 0;
      fragCount = 0;
      dt = DateTime.Now;

      //convert to COM selection 
      COMApi.InwOpState oState = ComBridge.State;
      walkNode( oState.CurrentPartition, false );

      TimeSpan ts = DateTime.Now - dt;
      double ms = ts.TotalMilliseconds;

      string s = string.Format(
        "Retrieved {0} geometry nodes and {1} fragments in {2} milliseconds.",
        geoNodeCount, fragCount, ms.ToString( "0.##" ) );
      Debug.Print( s );
      return 0;
    }
  }
}
