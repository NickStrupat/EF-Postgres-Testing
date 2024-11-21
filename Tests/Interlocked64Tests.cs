namespace Tests;

public class Interlocked64Tests
{
	[Fact]
	public void AddTest()
	{
		var interlocked = new InterlockedInt64();
		Assert.Equal(1, interlocked.Add(1));
		Assert.Equal(2, interlocked.Add(1));
		Assert.Equal(3, interlocked.Add(1));
	}
	
	[Fact]
	public void IncrementTest()
	{
		var interlocked = new InterlockedInt64();
		Assert.Equal(1, interlocked.Increment());
		Assert.Equal(2, interlocked.Increment());
		Assert.Equal(3, interlocked.Increment());
	}
	
	[Fact]
	public void DecrementTest()
	{
		var interlocked = new InterlockedInt64(3);
		Assert.Equal(2, interlocked.Decrement());
		Assert.Equal(1, interlocked.Decrement());
		Assert.Equal(0, interlocked.Decrement());
	}
	
	[Fact]
	public void ReadTest()
	{
		var interlocked = new InterlockedInt64(3);
		Assert.Equal(3, interlocked.Read());
		Assert.Equal(2, interlocked.Decrement());
		Assert.Equal(2, interlocked.Read());
	}
}