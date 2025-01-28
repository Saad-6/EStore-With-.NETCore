namespace EStore.Models;

public class Response
{
    public bool Success { get; set; }
    public string Error { get; set; }
    public object? Data { get; set; }
    public Response() 
    {
    Success = true;
    }
}
