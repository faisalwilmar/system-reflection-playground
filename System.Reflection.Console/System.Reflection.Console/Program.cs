using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Console.DataAccess.Model;
using System.Reflection.Console.Dto;
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

            var theKeyValueList = new List<KeyValuePair<string, object>>{
                new KeyValuePair<string, object>("string value", "this is string"),
                new KeyValuePair<string, object>("integer value", 3),
                new KeyValuePair<string, object>("boolean value", true)
            };

            var theValueTypeList = GetObjectPropertyType(theKeyValueList);
            foreach (var item in theValueTypeList)
            {
                Console.WriteLine(item);
            }

            WriteAllPropertyInfo<VariousProperties>();

            Console.WriteLine("GetPropertyAttribute: " + GetPropertyAttribute<VariousProperties>(p => p.CurrentDateTime));

            Console.WriteLine("GetJsonPropertyAttribute: " + GetJsonPropertyAttribute<VariousProperties>("CurrentDateTime"));

            Console.WriteLine("GetPropertyInfoByJsonPropertyAttribute: " + GetPropertyInfoByJsonPropertyAttribute<VariousProperties>(GetJsonPropertyAttribute<VariousProperties>("CurrentDateTime")).Name);
            Console.WriteLine("GetPropertyInfoByJsonPropertyAttribute: " + GetPropertyInfoByJsonPropertyAttribute<VariousProperties>("invalid"));
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

    public static List<string> GetObjectPropertyType(List<KeyValuePair<string, object>> pairs)
    {
        var @return = new List<string>();
        foreach (var item in pairs)
        {
            var itemType = item.Value.GetType();
            @return.Add(itemType.Name.ToString());
        }

        return @return;
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

    public static void WriteAllPropertyInfo<T>()
    {
        Console.WriteLine("List of Property: ");
        PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (PropertyInfo prop in props)
        {
            Console.WriteLine("Property: " + prop.Name);
            foreach (CustomAttributeData att in prop.CustomAttributes)
            {
                Console.WriteLine("\tAttribute: " + att.AttributeType.Name);
                foreach (CustomAttributeNamedArgument arg in att.NamedArguments)
                {
                    Console.WriteLine("\t\t" + arg.MemberName + ": " + arg.TypedValue);
                }
            }
        }
    }

    public static string GetPropertyAttribute<TType>(Expression<Func<TType, object>> property)
    {
        var body = property.Body;
        if (body.NodeType == ExpressionType.Convert)
            body = ((UnaryExpression)body).Operand;

        if (body is not MemberExpression memberExpression)
            throw new ArgumentException("Expression must be a property");

        var jsonPropertyAttribute = memberExpression.Member
                .GetCustomAttribute<JsonPropertyAttribute>();
        if (jsonPropertyAttribute == null) throw new ArgumentException("Model Property does not have JsonPropertyAttribute");

        return jsonPropertyAttribute.PropertyName;
    }

    public static string GetJsonPropertyAttribute<T>(string propertyName)
    {
        var property = typeof(T).GetProperty(propertyName);
        var customAtt = property.CustomAttributes.FirstOrDefault(p => p.AttributeType.Name == "JsonPropertyAttribute");
        if (customAtt == null) return null;

        var namedArgument = customAtt.NamedArguments.FirstOrDefault(p => p.MemberName == "PropertyName");
        return namedArgument.TypedValue.ToString().Trim('"');
    }

    public static PropertyInfo GetPropertyInfoByJsonPropertyAttribute<T>(string jsonPropertyName)
    {
        var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
        List<KeyValueObject> keyValueObjects = new List<KeyValueObject>();

        foreach (var property in properties)
        {
            var x = GetJsonPropertyAttribute<T>(property.Name);
            keyValueObjects.Add(new KeyValueObject
            {
                Key = x,
                Value = property
            });
        }

        var keyValObj = keyValueObjects.FirstOrDefault(p => p.Key == jsonPropertyName);
        if (keyValObj == null) throw new ArgumentException("PropertyInfo for JsonPropertyName " + jsonPropertyName + " not found");

        return (PropertyInfo)keyValObj.Value;
    }
}
