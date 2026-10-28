using System.Text.Json;
using AzureTangyFunc;
using Microsoft.Azure.Functions.Worker.Core.FunctionMetadata;
using Xunit;

public class FunctionMetadataTests
{
    [Fact]
    public async Task WorkerDiscoversAllFunctionsAndTheirOutputBindingsWithoutStartingHost()
    {
        var providerType = typeof(GroceryAPI).Assembly.GetTypes().Single(type =>
            typeof(IFunctionMetadataProvider).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
        var provider = (IFunctionMetadataProvider)Activator.CreateInstance(providerType);
        var functions = await provider.GetFunctionMetadataAsync(null);

        Assert.Equal(10, functions.Length);
        var sales = functions.Single(function => function.Name == "OnSalesUploadWriteToQueue");
        var bindings = sales.RawBindings.Select(binding => JsonSerializer.Deserialize<JsonElement>(binding)).ToArray();
        Assert.Contains(bindings, binding => binding.GetProperty("type").GetString() == "queue"
            && binding.GetProperty("queueName").GetString() == "salesrequestinbound");
        Assert.Contains(bindings, binding => binding.GetProperty("type").GetString() == "http");
        Assert.Contains(functions.Single(function => function.Name == "ResizeImageOnBlobUpload").RawBindings,
            binding => JsonSerializer.Deserialize<JsonElement>(binding).GetProperty("type").GetString() == "blob");
        Assert.Contains(functions.Single(function => function.Name == "UpdateStatusToCompletedAndSendEmail").RawBindings,
            binding => JsonSerializer.Deserialize<JsonElement>(binding).GetProperty("type").GetString() == "sendGrid");
    }
}
