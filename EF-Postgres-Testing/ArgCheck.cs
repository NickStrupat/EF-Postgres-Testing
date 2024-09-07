using System.Runtime.CompilerServices;

public static class ArgCheck
{
    public static void ThrowIfNull<T>(
        T? value,
        [CallerArgumentExpression(nameof(value))] String? name = null
    )
        where T : class
    {
        ArgumentNullException.ThrowIfNull(value, name);
    }

    public static void ThrowIfNull<T1, T2>(
        T1? value1,
        T2? value2,
        [CallerArgumentExpression(nameof(value1))] String? name1 = null,
        [CallerArgumentExpression(nameof(value2))] String? name2 = null
    ) 
        where T1 : class
        where T2 : class
    {
        ArgumentNullException.ThrowIfNull(value1, name1);
        ArgumentNullException.ThrowIfNull(value2, name2);
    }
    
    public static void ThrowIfNull<T1, T2, T3>(
        T1? value1,
        T2? value2,
        T3? value3,
        [CallerArgumentExpression(nameof(value1))] String? name1 = null,
        [CallerArgumentExpression(nameof(value2))] String? name2 = null,
        [CallerArgumentExpression(nameof(value3))] String? name3 = null
    )
        where T1 : class
        where T2 : class
        where T3 : class
    {
        ArgumentNullException.ThrowIfNull(value1, name1);
        ArgumentNullException.ThrowIfNull(value2, name2);
        ArgumentNullException.ThrowIfNull(value3, name3);
    }
    
    public static void ThrowIfNull<T1, T2, T3, T4>(
        T1? value1,
        T2? value2,
        T3? value3,
        T4? value4,
        [CallerArgumentExpression(nameof(value1))] String? name1 = null,
        [CallerArgumentExpression(nameof(value2))] String? name2 = null,
        [CallerArgumentExpression(nameof(value3))] String? name3 = null,
        [CallerArgumentExpression(nameof(value4))] String? name4 = null
    )
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
    {
        ArgumentNullException.ThrowIfNull(value1, name1);
        ArgumentNullException.ThrowIfNull(value2, name2);
        ArgumentNullException.ThrowIfNull(value3, name3);
        ArgumentNullException.ThrowIfNull(value4, name4);
    }
    
    public static void ThrowIfNull<T1, T2, T3, T4, T5>(
        T1? value1,
        T2? value2,
        T3? value3,
        T4? value4,
        T5? value5,
        [CallerArgumentExpression(nameof(value1))] String? name1 = null,
        [CallerArgumentExpression(nameof(value2))] String? name2 = null,
        [CallerArgumentExpression(nameof(value3))] String? name3 = null,
        [CallerArgumentExpression(nameof(value4))] String? name4 = null,
        [CallerArgumentExpression(nameof(value5))] String? name5 = null
    )
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
    {
        ArgumentNullException.ThrowIfNull(value1, name1);
        ArgumentNullException.ThrowIfNull(value2, name2);
        ArgumentNullException.ThrowIfNull(value3, name3);
        ArgumentNullException.ThrowIfNull(value4, name4);
        ArgumentNullException.ThrowIfNull(value5, name5);
    }
}