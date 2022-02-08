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
}