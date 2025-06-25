using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
    
    public static void Register<T>(T service)
    {
        var t = typeof(T);
        if (services.ContainsKey(t))
            services[t] = service;
        else
            services.Add(t, service);
    }
    
    public static T Get<T>()
    {
        var t = typeof(T);
        if (services.TryGetValue(t, out var svc))
            return (T)svc;
        throw new InvalidOperationException($"Service Locator: no service {t}");
    }
    
    public static void Unregister<T>()
    {
        services.Remove(typeof(T));
    }
}
