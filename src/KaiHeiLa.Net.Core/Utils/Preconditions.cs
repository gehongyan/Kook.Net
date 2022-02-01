using System;

namespace KaiHeiLa;

public class Preconditions
{
    public static void NotNull<T>(T obj, string name, string msg = null) where T : class { if (obj == null) throw CreateNotNullException(name, msg); }
    
    private static ArgumentNullException CreateNotNullException(string name, string msg)
    {
        if (msg == null) return new ArgumentNullException(paramName: name);
        else return new ArgumentNullException(paramName: name, message: msg);
    }
}