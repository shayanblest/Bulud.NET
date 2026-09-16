namespace Bulud.Core.Exceptions;

public class NotFoundException(string message = "منبع مورد نظر در سامانه یافت نشد.") : Exception(message);
