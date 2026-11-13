namespace UtemorgonPOS.Core;

public readonly struct Response(int code, string message)
{
	public readonly int Code = code;
	public readonly string Message = message;
}