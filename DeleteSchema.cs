using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using System;

namespace RevitDoc_Schemas
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class DeleteSchema : IExternalCommand
    {
        string _schemaGuid = "71d24769-597b-45d6-98d6-1c14d5d2410f";

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            Guid targetSchemaGuid = new Guid(_schemaGuid);

            // Проверяем, существует ли схема
            Schema targetSchema = Schema.Lookup(targetSchemaGuid);
            if (targetSchema == null)
            {
                TaskDialog.Show("Удаление схемы", "Схема с таким GUID не найдена.");
                return Result.Failed;
            }

            int entityCount = 0;

            // Проверяем, есть ли объекты (Entities), использующие эту схему
            FilteredElementCollector collector;
            string elementNames = "Элементы:";

            try
            {
                collector = new FilteredElementCollector(doc).WhereElementIsNotElementType();
                foreach (Element elem in collector)
                {
                    Entity entity = elem.GetEntity(targetSchema);
                    if (entity.IsValid())
                    {
                        entityCount++;
                        elementNames += $"{elem.Name}\n";
                    }
                }
                collector = new FilteredElementCollector(doc).WhereElementIsElementType();
                foreach (Element elem in collector)
                {
                    Entity entity = elem.GetEntity(targetSchema);
                    if (entity.IsValid())
                    {
                        entityCount++;
                        elementNames += $"\n{elem.Name}";
                    }
                }
            }
            catch (Autodesk.Revit.Exceptions.ArgumentException ex)
            {
                TaskDialog.Show("Ошибка", $"Не удалось получить доступ к данным схемы:\n{ex.Message}");
            }


            //Проверяем, можно ли удалить
            if (entityCount > 0)
            {
                TaskDialog.Show("Ошибка", $"Схема используется в {entityCount} элементах. Очистите их перед удалением.\n{elementNames}");
                return Result.Failed;
            }
            else
            {
                TaskDialog.Show("Внимание", $"Нет подходящих элементов схемы. Схема будет удалена");
                Delete(targetSchema);
                return Result.Failed;
            }
        }

        private void Delete (Schema targetSchema)
        {
            try
            {
                Schema.EraseSchemaAndAllEntities(targetSchema, true);

                TaskDialog.Show("Удаление схемы", 
                    $"Схема {targetSchema.SchemaName} (GUID: {_schemaGuid}) успешно удалена.");
                return;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка", $"Не удалось удалить схему:\n{ex.Message}");
                return;
            }
        }
    }
}
