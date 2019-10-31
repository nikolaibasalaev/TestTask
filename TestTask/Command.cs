#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace TestTask
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {


        // Private Members
        String[] m_Outputdata;
        // Properties
        /// <summary>
        /// Inform all the wall types can be created in current document
        /// </summary>

        public Command()
        {

        }

        public string[] Outputdata { get => m_Outputdata; set => m_Outputdata = value; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            //  Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // Access current selection

            Reference sel = uidoc.Selection.PickObject(ObjectType.Element, "Select element");
            Element fid = doc.GetElement(sel.ElementId);
            Family f = null;
            List<string> OutData = new List<string>();

            FamilyInstance elFamInst = fid as FamilyInstance;
            f = elFamInst.Symbol.Family;
            OutData.Add("Имя исходного семейства: " + f.Name);
            FamilySymbol mainf = elFamInst.Symbol;
            OutData.Add("Shared parameters для данного семейства:");
            ParameterSet parametersmain = mainf.Parameters;
            foreach (Parameter param in parametersmain)
            {
                if (param.IsShared)
                {
                    OutData.Add("Имя: " + param.Definition.Name + " / GUID: " + param.GUID.ToString());
                }
            }
            //нахождение вложенных семейств и их параметров

            Document famDoc = uidoc.Document.EditFamily(f);
            FilteredElementCollector filteredFamCollector = new FilteredElementCollector(famDoc);
            filteredFamCollector.OfClass(typeof(FamilySymbol));

            // Filtered element collector is iterable
           // List<string> ParamName = new List<string>();
           // List<string> ParamGuid = new List<string>();
            //FamilyInstance r = null;
            foreach (FamilySymbol e in filteredFamCollector)
            {
                //r = e as FamilyInstance;
                OutData.Add("Вложенное семейство: " + e.Name);
                OutData.Add("Shared Parameters: ");
                ParameterSet parameterss = e.Parameters;
                foreach (Parameter param in parameterss)
                {
                    if (param.IsShared)
                    {
                        OutData.Add("Имя: " + param.Definition.Name + " / GUID: " + param.GUID.ToString());

                    }
                }
            }

            Outputdata = OutData.ToArray();

            using (Output displayForm1 = new Output(this))
            {
                if (DialogResult.OK != displayForm1.ShowDialog())
                {
                    return Autodesk.Revit.UI.Result.Failed;
                }
            }

            // Modify document within a transaction

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Transaction Name");
                tx.Commit();
            }

            return Result.Succeeded;


        }




    }
}

