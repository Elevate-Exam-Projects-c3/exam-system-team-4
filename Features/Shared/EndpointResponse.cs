using exam_system.Features.Shared;
public class EndpointResponse<T> : EndpointResponse
{
    public T? Data { get; set; }

    public static EndpointResponse<T> Created(T data, string message = "Created successfully")
        => new(true, 201, message, data);

    public static EndpointResponse<T> Fail(string message, IDictionary<string, string[]> errors, int statusCode = 400)
        => new(false, statusCode, message, errors);
    public EndpointResponse(
       bool success,
       int statusCode,
       string message,
       T? data = default
       )
       : base(success, statusCode, message)
    {
        Data = data;
    }
    public EndpointResponse(bool success, int statusCode, string message, IDictionary<string, string[]> errors)
       : base(success, statusCode, message,errors)
    {
    }

    public static EndpointResponse<T> FromResult(RequestResponse<T> result)
    {
        if (result.Success)
            return new EndpointResponse<T>(
                result.Success,
                result.StatusCode,
                result.Message,
                result.Data
            );
        else
            return new EndpointResponse<T>(
                result.Success,
                result.StatusCode,
                result.Message,
                result.Errors
            );

    }
}

public class EndpointResponse
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public IDictionary<string, string[]>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public EndpointResponse(
       bool success,
       int statusCode,
       string message,
       IDictionary<string, string[]>? errors = null)
    {
        Success = success;
        StatusCode = statusCode;
        Message = message;
        Errors = errors;
        Timestamp = DateTime.UtcNow;
    }
   
    public static EndpointResponse FromResult(RequestResponse result)
    {

        if (result.Success)
            return new EndpointResponse(
                result.Success,
                result.StatusCode,
                result.Message
            );
        else
            return new EndpointResponse(
                result.Success,
                result.StatusCode,
                result.Message,
                result.Errors
            );
    }
}