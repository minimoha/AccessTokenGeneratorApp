namespace AccessTokenGeneratorApp.Models;

public class Result
{
    private Result() { }

    internal Result(bool successful, string message, object data = default, string responseCode = null)
    {
        Successful = successful;
        Message = message;
        Data = data;
        ResponseCode = responseCode;
    }

    internal Result(bool successful, object data = default)
    {
        Successful = successful;
        Data = data;
    }

    public bool Successful { get; set; }
    public string Message { get; set; }
    public string ResponseCode { get; set; }
    public object Data { get; set; }


    public static Result Success(object data = null, string message = "Operation successful", string responseCode = default)
    {
        return new Result(true, message, data, ResponseCodes.SUCCESS);
    }

    public static Result Success(object data, string message)
    {
        return new Result(true, message, data, ResponseCodes.SUCCESS);
    }

    public static Result Success(string message)
    {
        return new Result(true, message, null, ResponseCodes.SUCCESS);
    }

    public static Result Error(string message, object data = null)
    {
        return new Result(false, message, data);
    }

    public static Result Error(string message, string responseCode, object data = null)
    {
        return new Result(false, message, data, responseCode);
    }

    public static Result Failure(string message, object data = null)
    {
        return new Result(false, message, data, ResponseCodes.UNSUCCESSFUL);
    }

    public static Result Failure(object data = null)
    {
        return new Result(false, data);
    }

    public static Result Forbidden(string message, object data = null)
    {
        return new Result(false, message, data);
    }

    public static Result Exception(Exception ex, string customMessage = null)
    {
        const string defaultErrorMessage = "An error occurred. Please try again later or contact admin for resolution.";
        var message = string.IsNullOrWhiteSpace(customMessage)
            ? $"{defaultErrorMessage} | {ex.Message}"
            : $"An exception occurred. | {customMessage}";

        Log.Information(ex.Message + ex.StackTrace, ex);

        return new Result(false, message, null, ResponseCodes.ERROR);
    }
}
