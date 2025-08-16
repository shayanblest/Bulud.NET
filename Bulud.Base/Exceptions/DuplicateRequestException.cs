namespace Bulud.Base.Exceptions;

public class DuplicateRequestException(string message = "این درخواست پیش تر در سامانه ثبت شده است.") : Exception(message);