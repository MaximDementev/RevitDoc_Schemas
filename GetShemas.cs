using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RevitDoc_Schemas
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class GetShemas : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // Получаем все схемы в документе
            IList<Schema> schemas = Schema.ListSchemas();

            // Формируем строку с именами схем
            string schemaInfo = "Список всех схем в документе:\n\n";
            foreach (Schema schema in schemas)
            {
                schemaInfo += $"Имя: {schema.SchemaName}\nGUID: {schema.GUID}\n\n";
            }

            // Если схем нет, выводим сообщение
            if (schemas.Count == 0)
            {
                schemaInfo = "В документе нет пользовательских схем.";
            }

            // Выводим результат в TaskDialog
            TaskDialog.Show("Revit Schemas", schemaInfo);

            return Result.Succeeded;
        }
    }
}
