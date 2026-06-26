using RoverDotNet.Core.Config;
using Xunit;

namespace RoverDotNet.Core.Tests.Config;

// All test cases are ported directly from the Rust tests in
// crates/houston/src/profile/mod.rs (mask_key tests).
public class ApiKeyMaskerTests
{
    [Fact]
    public void Masks_user_key()
    {
        Assert.Equal(
            "user**************************LOLO",
            ApiKeyMasker.Mask("user:gh.foo:djru4788dhsg3657fhLOLO"));
    }

    [Fact]
    public void Masks_long_user_key()
    {
        Assert.Equal(
            "user*************************************************long",
            ApiKeyMasker.Mask("user:veryveryveryveryveryveryveryveryveryveryveryverylong"));
    }

    [Fact]
    public void Masks_graph_key()
    {
        Assert.Equal(
            "serv**************************LOLO",
            ApiKeyMasker.Mask("service:foo:djru4788dhsg3657fhLOLO"));
    }

    [Fact]
    public void Masks_nonsense_string()
    {
        Assert.Equal("some*****ense", ApiKeyMasker.Mask("some nonsense"));
    }

    [Fact]
    public void Empty_string_returns_empty()
    {
        Assert.Equal(string.Empty, ApiKeyMasker.Mask(string.Empty));
    }

    [Fact]
    public void Short_string_is_returned_unchanged()
    {
        Assert.Equal("short", ApiKeyMasker.Mask("short"));
    }
}
