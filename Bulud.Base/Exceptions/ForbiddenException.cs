namespace Bulud.Base.Exceptions;

public class ForbiddenException(string message = "دسترسی به این بخش امکان پذیر نمی باشد.") : Exception(message);
