using System;
using System.Text.Json;
using DurableMultiAgentTemplate.Shared.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DurableMultiAgentTemplate.Model; // 追加

namespace DurableMultiAgentTemplate.Test.Model;

[TestClass]
public class AdditionalInfoSerializationTest
{
    [TestMethod]
    public void AdditionalMarkdownInfo_SerializeDeserialize_WorksCorrectly()
    {
        var original = new AdditionalMarkdownInfo("**bold text**");
        IAdditionalInfo info = original;
        var options = new JsonSerializerOptions { TypeInfoResolver = SourceGenerationContext.Default };
        string json = JsonSerializer.Serialize(info, typeof(IAdditionalInfo), options);
        var deserialized = (IAdditionalInfo)JsonSerializer.Deserialize(json, typeof(IAdditionalInfo), options)!;
        Assert.IsInstanceOfType(deserialized, typeof(AdditionalMarkdownInfo));
        Assert.AreEqual(original.MarkdownText, ((AdditionalMarkdownInfo)deserialized).MarkdownText);
    }

    [TestMethod]
    public void AdditionalLinkInfo_SerializeDeserialize_WorksCorrectly()
    {
        var original = new AdditionalLinkInfo("Google", new Uri("https://www.google.com/"));
        IAdditionalInfo info = original;
        var options = new JsonSerializerOptions { TypeInfoResolver = SourceGenerationContext.Default };
        string json = JsonSerializer.Serialize(info, typeof(IAdditionalInfo), options);
        var deserialized = (IAdditionalInfo)JsonSerializer.Deserialize(json, typeof(IAdditionalInfo), options)!;
        Assert.IsInstanceOfType(deserialized, typeof(AdditionalLinkInfo));
        var link = (AdditionalLinkInfo)deserialized;
        Assert.AreEqual(original.LinkText, link.LinkText);
        Assert.AreEqual(original.Uri, link.Uri);
    }
}
