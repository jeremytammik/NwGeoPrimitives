#region Namespaces
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ComBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;
using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;
using NwVertex = Autodesk.Navisworks.Api.Interop.ComApi.InwSimpleVertex;
using System.Collections.Generic;
using Autodesk.Navisworks.Api;
#endregion // Namespaces

namespace NwGeoPrimitives
{
  static class Extension
  {
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static T[] ToArray<T>( this Array arr )
      where T : struct
    {
      T[] result = new T[ arr.Length ];
      Array.Copy( arr, result, result.Length );
      return result;
    }
  }

  public class GeoPrimitives
  {
    #region GetSortedFragments
    // from thread on Faster primitive data extraction:
    // https://forums.autodesk.com/t5/navisworks-api/faster-primitive-data-extraction/td-p/9298425
    Dictionary<int[], Stack<ComApi.InwOaFragment3>> 
      GetSortedFragments( ModelItemCollection modelItems )
    {
      ComApi.InwOpState oState = ComBridge.State;
      ComApi.InwOpSelection oSel = ComBridge.ToInwOpSelection( modelItems );

      // To be most efficient, you need to lookup an efficient 
      // EqualityComparer for the int[] key

      var pathDict = new Dictionary<int[], Stack<ComApi.InwOaFragment3>>();

      foreach( ComApi.InwOaPath3 path in oSel.Paths() )
      {
        // This yields ONLY unique fragments
        // ordered by geometry they belong to

        foreach( ComApi.InwOaFragment3 frag in path.Fragments() )
        {
          int[] pathArr = ((Array) frag.path.ArrayData).ToArray<int>();
          if( !pathDict.TryGetValue( pathArr, 
            out Stack<ComApi.InwOaFragment3> frags ) )
          {
            frags = new Stack<ComApi.InwOaFragment3>();
            pathDict[ pathArr ] = frags;
          }
          frags.Push( frag );
        }
      }
      return pathDict;
    }
    #endregion // GetSortedFragments

    class CallbackGeomListener : ComApi.InwSimplePrimitivesCB
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
      ComApi.InwOaNode parentNode,
      bool bFoundFirst )
    {
      if( parentNode.IsGroup )
      {
        ComApi.InwOaGroup group = (ComApi.InwOaGroup) parentNode;
        long n = group.Children().Count;

        for( long i = 1; i <= n; ++i )
        {
          ComApi.InwOaNode newNode = group.Children()[i];

          if( (!bFoundFirst) && (n > 1) )
          {
            bFoundFirst = true;
          }
          walkNode( newNode, bFoundFirst );
        }
      }
      else if( parentNode.IsGeometry )
      {
        ComApi.InwNodeFragsColl fragsColl
          = parentNode.Fragments();

        CallbackGeomListener cbl 
          = generate_primitives_for_all_fragments
            ? new CallbackGeomListener()
            : null;

        long nFrags = fragsColl.Count;

        Debug.WriteLine( "frags count:"
          + nFrags.ToString() );

        for( long i = 1; i <= nFrags; ++i )
        {
          ComApi.InwOaFragment3 frag = fragsColl[ i ];

          if( generate_primitives_for_all_fragments )
          {
            frag.GenerateSimplePrimitives(
              ComApi.nwEVertexProperty.eNORMAL, cbl );
          }
          else
          {
            // Collect single instance of each fragment
            // in lookup dictionary for later call to
            // GenerateSimplePrimitives

            int[] pathArr = ((Array) frag.path.ArrayData).ToArray<int>();
            if( !_pathDict.TryGetValue( pathArr,
              out Stack<ComApi.InwOaFragment3> frags ) )
            {
              frags = new Stack<ComApi.InwOaFragment3>();
              _pathDict[ pathArr ] = frags;
            }
            frags.Push( frag );
          }
          _nFragsTotal += nFrags;
        }
        ++_nNodesTotal;
      }
    }

    long _nNodesTotal;
    long _nFragsTotal;

    /// <summary>
    /// If true, naively call GenerateSimplePrimitives 
    /// on every fragment encountered by `walkNode`; 
    /// if false, collect them in a dictionary instead,
    /// and generate primitives once only for each fragment.
    /// </summary>
    bool generate_primitives_for_all_fragments = false;

    /// <summary>
    /// Dictionary mapping fragment paths to fragments
    /// used to collect only one fragment instance for each path
    /// </summary>
    Dictionary<int[], Stack<ComApi.InwOaFragment3>> _pathDict;

    public void Execute()
    {
      _nNodesTotal = 0;
      _nFragsTotal = 0;
      _pathDict = new Dictionary<int[], Stack<ComApi.InwOaFragment3>>();

      DateTime dt = DateTime.Now;

      CallbackGeomListener.Init();

      // Convert to COM selection

      ComApi.InwOpState oState = ComBridge.State;
      walkNode( oState.CurrentPartition, false );

      // Generate primitives for collected fragments

      CallbackGeomListener cbl
        = new CallbackGeomListener();

      foreach( var kvp in _pathDict )
      {
        var frags = kvp.Value;
        while( frags.Count > 0 )
        {
          ComApi.InwOaFragment3 frag = frags.Pop();
          frag.GenerateSimplePrimitives(
            ComApi.nwEVertexProperty.eNORMAL, cbl );
        }
      }

      TimeSpan ts = DateTime.Now - dt;
      double ms = ts.TotalMilliseconds;

      string s = string.Format(
        "Retrieved {0} geometry nodes and {1} fragments in {2} milliseconds.",
        _nNodesTotal, _nFragsTotal, ms.ToString( "0.##" ) );
      Debug.Print( s );

      Debug.Print( "Line, point, snappoint and triangle counts: {0}, {1}, {2}, {3}",
        CallbackGeomListener.LineCount, CallbackGeomListener.PointCount,
        CallbackGeomListener.SnapPointCount, CallbackGeomListener.TriangleCount );
    }
  }
}
