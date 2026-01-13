using Xunit;

namespace OsService.Tests;

public class WebTests
{
    [Fact]
    public void SimpleArithmetic_Passes()
    {
        Assert.Equal(4, 2 + 2);
    }
}
