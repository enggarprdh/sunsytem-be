using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _5unSystem.Model.Entities
{
    public class EntitiesFuntion
    {
        public static void CheckTable()
        {
            var entityNamespace = typeof(EntitiesFuntion).Namespace;
            var assembly = typeof(EntitiesFuntion).Assembly;
            var entityTypes = assembly.GetTypes()
                .Where(t => t.IsClass && t.Namespace == entityNamespace && t != typeof(EntitiesFuntion))
                .ToList();
            foreach (var entityType in entityTypes)
            {
                Console.WriteLine($"Entity: {entityType.Name}");
                var properties = entityType.GetProperties();
                foreach (var property in properties)
                {
                    var columnName = property.Name;
                    var columnType = property.PropertyType.Name; // Simplified for demonstration
                    var columnExists = false; // Replace with actual check against database
                    //Console.WriteLine($"  Property: {property.Name} ({property.PropertyType.Name})");
                }
            }
        }
    }
}
