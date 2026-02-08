namespace Wrap.Services.Core.Utilities;

using Newtonsoft.Json;

using Microsoft.AspNetCore.Http;

public static class SessionJsonExtensions
{
    public static void SetJson<T>(ISession session, string key, T value)
    {
        string json = JsonConvert.SerializeObject(value);
        
        session.SetString(key, json);
    }

    public static T? GetJson<T>(ISession session, string key)
    {
        string? json = session.GetString(key);
        return json is null 
            ? default
            : JsonConvert.DeserializeObject<T>(json);
    }
}