﻿namespace DevoramUtility;

public class Singleton<T> where T : class, new()
{
    private static readonly Lazy<T> _instance = new(() => new T());
    public static T Instance { get { return _instance.Value; } }
}
