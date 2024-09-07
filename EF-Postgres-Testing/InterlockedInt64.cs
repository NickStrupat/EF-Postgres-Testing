using System.Runtime.CompilerServices;

public struct InterlockedInt64
{
    private Int64 value;
    
    /// <summary><inheritdoc cref="Interlocked.Add(ref Int64, Int64)"/></summary>
    /// <param name="value"><inheritdoc cref="Interlocked.Add(ref Int64, Int64)"/></param>
    /// <returns><inheritdoc cref="Interlocked.Add(ref Int64, Int64)"/></returns>
    public Int64 Add(Int64 value) => Interlocked.Add(ref this.value, value);
    
    /// <summary><inheritdoc cref="Interlocked.Increment(ref Int64)"/></summary>
    /// <returns><inheritdoc cref="Interlocked.Increment(ref Int64)"/></returns>
    public Int64 Increment() => Interlocked.Increment(ref value);
    
    /// <summary><inheritdoc cref="Interlocked.Decrement(ref Int64)"/></summary>
    /// <returns><inheritdoc cref="Interlocked.Decrement(ref Int64)"/></returns>
    public Int64 Decrement() => Interlocked.Decrement(ref value);
    
    /// <summary><inheritdoc cref="Interlocked.Read(ref readonly Int64)"/></summary>
    /// <returns><inheritdoc cref="Interlocked.Read(ref readonly Int64)"/></returns>
    public readonly Int64 Read() => Interlocked.Read(ref Unsafe.AsRef(in value));
    
    /// <summary><inheritdoc cref="Interlocked.Exchange(ref Int64, Int64)"/></summary>
    /// <param name="value"><inheritdoc cref="Interlocked.Exchange(ref Int64, Int64)"/></param>
    /// <returns><inheritdoc cref="Interlocked.Exchange(ref Int64, Int64)"/></returns>
    public Int64 Exchange(Int64 value) => Interlocked.Exchange(ref this.value, value);
    
    /// <summary><inheritdoc cref="Interlocked.CompareExchange(ref Int64, Int64, Int64)"/></summary>
    /// <param name="value"><inheritdoc cref="Interlocked.CompareExchange(ref Int64, Int64, Int64)"/></param>
    /// <param name="comparand"><inheritdoc cref="Interlocked.CompareExchange(ref Int64, Int64, Int64)"/></param>
    /// <returns><inheritdoc cref="Interlocked.CompareExchange(ref Int64, Int64, Int64)"/></returns>
    public Int64 CompareExchange(Int64 value, Int64 comparand) => Interlocked.CompareExchange(ref this.value, value, comparand);
    
    /// <summary><inheritdoc cref="Interlocked.And(ref Int64, Int64)"/></summary>
    /// <param name="value"><inheritdoc cref="Interlocked.And(ref Int64, Int64)"/></param>
    /// <returns><inheritdoc cref="Interlocked.And(ref Int64, Int64)"/></returns>
    public Int64 And(Int64 value) => Interlocked.And(ref this.value, value);
    
    /// <summary><inheritdoc cref="Interlocked.Or(ref Int64, Int64)"/></summary>
    /// <param name="value"><inheritdoc cref="Interlocked.Or(ref Int64, Int64)"/></param>
    /// <returns><inheritdoc cref="Interlocked.Or(ref Int64, Int64)"/></returns>
    public Int64 Or(Int64 value) => Interlocked.Or(ref this.value, value);
    
    /// <summary><inheritdoc cref="Int64.ToString()"/></summary>
    /// <returns><inheritdoc cref="Int64.ToString()"/></returns>
    public readonly override String ToString() => Read().ToString();
}