#region Namespaces
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ComBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;
using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;
using NwVertex = Autodesk.Navisworks.Api.Interop.ComApi.InwSimpleVertex;
using System.Collections.Generic;
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.DocumentParts;
#endregion // Namespaces

namespace NwGeoPrimitives
{
  class IterateModels
  {
    /// <summary>
    /// Return all model items from all models in document
    /// https://forums.autodesk.com/t5/navisworks-api/getting-parameters-of-rootitem-and-subitems/m-p/9463861
    /// </summary>
    static ModelItemCollection FindEveryItem()
    {
      Document doc = Application.ActiveDocument;

      ModelItemCollection allItems = new ModelItemCollection();
      DocumentModels models = doc.Models;

      foreach( Model model in models )
      {
        ModelItem rootItem = model.RootItem;
        allItems.AddRange( rootItem.DescendantsAndSelf );
      }
      return allItems;
    }

    public void Execute( Document doc )
    {
      DateTime dt = DateTime.Now;

      DocumentModels models = doc.Models;

      int nModels = models.Count;

      foreach( Model model in models )
      {
        ModelItem rootItem = model.RootItem;
        //Examine( rootItem.DescendantsAndSelf );
      }

      TimeSpan ts = DateTime.Now - dt;
      double ms = ts.TotalMilliseconds;

    }
  }
}
