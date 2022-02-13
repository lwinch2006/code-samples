using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace WebUI.Extensions;

public static class TempDataExtensions
{
    public static object ReadAndClear(this ITempDataDictionary tempData, string key)
    {
        var result = tempData[key];
        tempData.Remove(key);
        return result;
    }

    public static void Write<T>(this ITempDataDictionary tempData, string key, T value)
        where T : class
    {
        tempData[key] = JsonSerializer.Serialize(value);
    }

    public static T ReadAndClean<T>(this ITempDataDictionary tempData, string key)
        where T : class
    {
        var valueAsString = (string)tempData.ReadAndClear(key);

        if (valueAsString == null)
        {
            return null;
        }
        
        var value = JsonSerializer.Deserialize<T>(valueAsString);
        return value;
    }
}