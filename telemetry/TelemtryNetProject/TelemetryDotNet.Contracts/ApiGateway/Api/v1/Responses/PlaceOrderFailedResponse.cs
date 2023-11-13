namespace TelemetryDotNet.Contracts.ApiGateway.Api.v1.Responses;

public class PlaceOrderFailedResponse
{
    public string ErrorMessage { get; set; }
    public bool IsExceptionOccured { get; set; }
    
    public List<string?> ValidationErrors { get; set; } = new();

    public PlaceOrderFailedResponse(string errorMessage, bool isExceptionOccured)
    {
        ErrorMessage = errorMessage;
        IsExceptionOccured = isExceptionOccured;
    }
}