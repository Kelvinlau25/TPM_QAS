using System;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace TPM_QAS.Helpers
{
    /// <summary>
    /// Extension methods for ISession to store/retrieve complex objects.
    /// Replaces the direct object storage available in .NET Framework sessions.
    /// </summary>
    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObject<T>(this ISession session, string key) where T : class
        {
            var value = session.GetString(key);
            return value == null ? null : JsonSerializer.Deserialize<T>(value);
        }
    }
}
