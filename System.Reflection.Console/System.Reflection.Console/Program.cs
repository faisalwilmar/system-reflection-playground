using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Console.DataAccess.Model;
using System.Text;
using System.Threading.Tasks;

static class Program
{
    static async Task Main(string[] args)
    {
        SetupDependencyInjection();

        Console.WriteLine("==== Starting Console ====");

        await StartProgram();

        Console.WriteLine("=== Console App completed ====");
        Console.WriteLine("=== Press enter to continue ====");
        Console.ReadLine();
    }

    private static async Task StartProgram()
    {
        try
        {
            var newObject = new VariousProperties
            {
                Category = "Money",
                CategoryLevel = 1,
                IsActive = true,
                CategoryTags = new List<VariousProperties.Tag>
                {
                    new VariousProperties.Tag
                    {
                        Key = "creator",
                        Value = "My Self"
                    },
                    new VariousProperties.Tag
                    {
                        Key = "created-date",
                        Value = DateTime.UtcNow.ToString()
                    }
                }
            };

            var theList = GetPropertyDetails(newObject);
            foreach (var item in theList)
            {
                Console.WriteLine(item);
            }

            var isCorrect = Program.IsCorrectType<VariousProperties>(newObject, "CategoryLevel", typeof(int));
            Console.WriteLine("This is Bool: " + isCorrect.Item1);
            Console.WriteLine("This is String: " + isCorrect.Item2);

            newObject.ConsolidatedPropertyValue = Program.ConsolidatePropertyValue(newObject, "Category,CategoryLevel,IsActive");
            Console.WriteLine(newObject.ConsolidatedPropertyValue);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.ReadLine();
        }
    }

    private static void SetupDependencyInjection()
    {
        // Setup Dependency Injection
        var serviceProvider = new ServiceCollection()
            .BuildServiceProvider();
    }

    public static List<string> GetPropertyDetails<T>(T item)
    {
        var property = typeof(T).GetProperty("Category");
        return new List<string>
        {
            property.Name.ToString(),
            property.PropertyType.ToString(),
            property.GetValue(item).ToString()
        };
    }

    public static (bool, string) IsCorrectType<T>(T item, string propertyName, Type expectedType)
    {
        var property = typeof(T).GetProperty(propertyName);
        bool isCorrect = property.PropertyType == expectedType;
        return (isCorrect, isCorrect.ToString());
    }

    public static string ConsolidatePropertyValue<T>(T item, string propertyKeys)
    {
        string result = "";

        var _partitionPropertyNames = propertyKeys.Split(",").ToList();
        List<PropertyInfo> _partitionProperties = new();

        foreach (var propertyName in _partitionPropertyNames)
        {
            _partitionProperties.Add(typeof(T).GetProperty(propertyName));
        }

        if (_partitionPropertyNames != null)
        {
            StringBuilder sb = new();
            sb.Append(typeof(T).Name.ToString());
            foreach (var p in _partitionProperties)
            {
                sb.Append("/" + p.GetValue(item));
            }

            result = sb.ToString();
        }

        return result;
    }
}
