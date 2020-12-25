﻿#region Namespaces
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.DocumentParts;
using Autodesk.Navisworks.Api.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#endregion // Namespaces

namespace NwGeoPrimitives
{
  class ItemData
  {
    //Layers:
    //  ClassDisplayName "Levels: ..."
    //  ClassName LcRevitLayer
    //  DisplayName Level 1
    //  Children 5 Categories
    //    ClassDisplayName Category
    //    ClassName LcRevitCollection
    //    DisplayName Doors
    //    Children 1
    //      ClassDisplayName Family
    //      ClassName LcRevitCollection
    //      DisplayName M_Single_Flush
    //      Children 1
    //        ClassDisplayName Type
    //        ClassName LcRevitCollection
    //        DisplayName 0915 x 2134 mm
    //        Children 1
    //          ClassDisplayName Doors: M_Single_Flush: 0915 x 2134 mm
    //          ClassName LcRevitInstance
    //          DisplayName M_Single_Flush
    //          InstanceGuid Revit UniqueId, more or less

    public string DisplayName { get; }
    public string ClassName { get; }
    public string ClassDisplayName { get; }
    public IEnumerable<ItemData> Children { get; }
    public bool HasGeometry { get; }
    public Guid InstanceGuid { get; }

    public ItemData( ModelItem mi )
    {
      DisplayName = mi.DisplayName;
      ClassName = mi.ClassName;
      ClassDisplayName = mi.ClassDisplayName;
      HasGeometry = mi.HasGeometry;
      InstanceGuid = mi.InstanceGuid;

      Debug.Assert( ClassName.StartsWith( "LcRevit" ), 
        "expected Revit object ClassName prefix" );
    }

    public override string ToString()
    {
      string typ = ClassName.Equals( "LcRevitCollection" )
        ? ClassDisplayName
        : ClassName.Substring( 7 );
      return typ + " " + DisplayName;
    }
  }
}
