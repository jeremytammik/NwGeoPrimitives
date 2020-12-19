#region Namespaces
using Autodesk.Navisworks.Api;
using System.Diagnostics;
using System.Linq;
#endregion // Namespaces

namespace NwGeoPrimitives
{
  class RvtProperties
  {
    public static object GetPropertyValue( 
      DataProperty dp, 
      bool return_string )
    {
      VariantData v = dp.Value;
      VariantDataType t = v.DataType;
      object o = null;
      switch( t )
      {
        // Empty. No data stored.
        case VariantDataType.None:
          o = return_string ? "<null>" : null;
          break;
        // Unitless double value
        case VariantDataType.Double:
          o = return_string
            ? (object) Util.RealString( v.ToDouble() )
            : v.ToDouble();
          break;
        // Unitless 32 bit integer value
        case VariantDataType.Int32:
          o = return_string
            ? (object) v.ToInt32().ToString()
            : v.ToInt32();
          break;
        // Boolean (true/false) value
        case VariantDataType.Boolean:
          o = return_string
            ? (object) (v.ToBoolean() ? "true" : "false")
            : v.ToBoolean();
          break;
        // String intended for display to the end user (normally localized)
        case VariantDataType.DisplayString:
          o = v.ToDisplayString();
          break;
        // A specific date and time (usually UTC)
        case VariantDataType.DateTime:
          o = return_string
            ? (object) v.ToDateTime().ToShortTimeString()
            : v.ToDateTime();
          break;
        // A double that represents a length (specific units depend on context)
        case VariantDataType.DoubleLength:
          o = return_string
            ? (object) Util.RealString( v.ToDoubleLength() )
            : v.ToDoubleLength();
          break;
        // A double that represents an angle in radians
        case VariantDataType.DoubleAngle:
          o = return_string
            ? (object) Util.RealString( v.ToDoubleAngle() )
            : v.ToDoubleAngle();
          break;
        // A named constant
        case VariantDataType.NamedConstant:
          o = return_string
            ? (object) v.ToNamedConstant().ToString()
            : v.ToNamedConstant();
          break;
        // String intended to be used as a programmatic identifier. 7-bit ASCII characters
        // only.
        case VariantDataType.IdentifierString:
          o = v.ToIdentifierString();
          break;
        // A double that species an area (specific units depend on context)
        case VariantDataType.DoubleArea:
          o = return_string
            ? (object) Util.RealString( v.ToDoubleArea() )
            : v.ToDoubleArea();
          break;
        // A double that species a volume (specific units depend on context)
        case VariantDataType.DoubleVolume:
          o = return_string
            ? (object) Util.RealString( v.ToDoubleVolume() )
            : v.ToDoubleVolume();
          break;
        // A 3D point value
        case VariantDataType.Point3D:
          o = return_string
            ? (object) Util.PointString( v.ToPoint3D() )
            : v.ToPoint3D();
          break;
        // A 2D point value
        case VariantDataType.Point2D:
          o = return_string
            ? (object) Util.PointString( v.ToPoint2D() )
            : v.ToPoint2D();
          break;
      }
      return o;
    }

    /// <summary>
    /// Dump all properties from the given model item
    /// </summary>
    public static void DumpProperties( ModelItem mi )
    {
      // inspired by sample code from 
      // 'Navisworks .NET API Properties' by Xiaodong Liang
      // https://adndevblog.typepad.com/aec/2012/05/navisworks-net-api-properties.html

      PropertyCategoryCollection pcs = mi.PropertyCategories;
      int nPropertyCategories = pcs.Count<PropertyCategory>();
      Debug.Print( "{0} property categories:" );
      foreach( PropertyCategory pc in pcs )
      {
        Debug.Print( "{0} ({1})", pc.DisplayName, pc.Name );

        foreach( DataProperty dp in pc.Properties )
        {
          VariantData v = dp.Value;
          VariantDataType t = v.DataType;

          Debug.Print( "  {0} ({1}) = {2}",
            dp.DisplayName, dp.Name,
            GetPropertyValue( dp, true ) );
        }
      }
    }

    // Properties seen on a Revit family instance:
    //
    // Item( LcOaNode)
    //   Name( LcOaSceneBaseUserName) : f3( Revit Family Name)
    //   Type( LcOaSceneBaseClassUserName) : Generic Models( Revit Category )
    //   Internal Type( LcOaSceneBaseClassName): LcRevitComposite( Revit family instance)
    //   Icon( LcOaBaseIcon) : Geometry
    //   Source File( LcOaNodeSourceFile) : p3.rvt(Revit BIM filename)
    //   Layer( LcOaNodeLayer) : Level 1 (Revit level)
    // Element ID( LcRevitId)
    //   Value( LcOaNat64AttributeValue) : 349818

    public int ElementId { get; }

    public RvtProperties( ModelItem mi )
    {
      PropertyCategory pc_ElementId 
        = mi.PropertyCategories.FindCategoryByName(
          PropertyCategoryNames.RevitElementId );

      DataProperty dp_ElementId 
        = pc_ElementId.Properties.FindPropertyByName( 
          DataPropertyNames.RevitElementIdValue );

      VariantData v = dp_ElementId.Value;
      Debug.Assert( v.IsDisplayString, 
        "expected Revit element id as DisplayString" );

      ElementId = int.Parse( 
        dp_ElementId.Value.ToDisplayString() );
    }

    #region Sample code
    /*
    void getProperty()
    {
      Document oDoc =
          Autodesk.Navisworks.Api.Application.ActiveDocument;
      // get the first item of the selection
      ModelItem oSelectedItem =
          oDoc.CurrentSelection.
          SelectedItems.ElementAt<ModelItem>( 0 );

      //get a property category by display name    
      PropertyCategory pc_DWGHandle =
         oSelectedItem.PropertyCategories.
         FindCategoryByDisplayName( "Entity Handle" );

      //get a property by internal name
      PropertyCategory pc_DWGHandle1 =
          oSelectedItem.PropertyCategories.
          FindCategoryByName(
          PropertyCategoryNames.AutoCadEntityHandle );

      //get a property by combined name
      PropertyCategory pc_DWGHandle2 =
           oSelectedItem.PropertyCategories.
           FindCategoryByCombinedName(
           new NamedConstant(
            PropertyCategoryNames.AutoCadEntityHandle,
            "Entity Handle" ) );

      //get a property by display name
      //(property category and property)
      DataProperty dp_DWGHandle =
          oSelectedItem.PropertyCategories.
          FindPropertyByDisplayName
          ( "Entity Handle", "Value" );

      //get a property by internal name
      DataProperty dp_DWGHandle1 =
         oSelectedItem.PropertyCategories.
         FindPropertyByName(
         PropertyCategoryNames.AutoCadEntityHandle,
         DataPropertyNames.AutoCadEntityHandleValue );

      //get a property by combined name
      DataProperty dp_DWGHandle2 =
          oSelectedItem.PropertyCategories.
          FindPropertyByCombinedName(
           new NamedConstant(
               PropertyCategoryNames.AutoCadEntityHandle,
               "Entity Handle" ),
            new NamedConstant(
                DataPropertyNames.AutoCadEntityHandleValue,
                "Value" ) );

      //display the value of the property. e.g. use one
      // DataProperty got above to access its value
      Debug.Write(
          dp_DWGHandle.Value.ToString() );
    }
    */
    #endregion // Sample Code from 'Navisworks .NET API Properties' by Xiaodong Liang
  }
}
