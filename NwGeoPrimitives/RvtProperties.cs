#region Namespaces
using Autodesk.Navisworks.Api;
using System.Diagnostics;
using System.Linq;
#endregion // Namespaces

namespace NwGeoPrimitives
{
  class RvtProperties
  {
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

      ElementId = dp_ElementId.Value.ToInt32();
    }

    #region Sample Code from 'Navisworks .NET API Properties' by Xiaodong Liang
    // https://adndevblog.typepad.com/aec/2012/05/navisworks-net-api-properties.html

    public static void DumpProperties( ModelItem mi )
    {
      PropertyCategoryCollection pcs = mi.PropertyCategories;
      int nPropertyCategories = pcs.Count<PropertyCategory>();
      Debug.Print( "{0} property categories:" );
      foreach( PropertyCategory pc in pcs )
      {
        Debug.Print( "{0} ({1})", pc.DisplayName, pc.Name );

        foreach( DataProperty dp in pc.Properties )
        {
          VariantData v = dp.Value;
          if( v.IsDateTime )
          {
            Debug.Print( "  {0} ({1}) = {2}",
              dp.DisplayName, dp.Name,
              v.ToDateTime().ToShortTimeString() );
          }
          else
          { // if( v.IsDisplayString )
            {
              Debug.Print( "  {0} ({1}) = {2}",
                dp.DisplayName, dp.Name, v );
            }
          }
        }
      }
    }

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
    #endregion // Sample Code from 'Navisworks .NET API Properties' by Xiaodong Liang
  }
}
