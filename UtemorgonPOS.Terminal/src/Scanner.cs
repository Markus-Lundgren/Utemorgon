using System.IO.Ports;
using UtemorgonPOS.Core;

/*
	WINDOWS:
	    ScannerPort=COM3
	LINUX:
	    ScannerPort=/dev/ttyACM0
*/
public static class Scanner
{
	static SerialPort _port = new SerialPort(EasyEnv.Get<string>("ScannerPort"), 9600);

	public static string? ReadForMs(int readTimeMs = SerialPort.InfiniteTimeout)
	{
		_port.ReadTimeout = readTimeMs;
		try
		{
			if (!_port.IsOpen)
				_port.Open();

			string barcode = _port.ReadLine().Trim();

			_port.Write([0x06], 0, 1); // ACK

			return barcode;
		}
		catch
		{
			return null;
		}
	}

	public static void ClearBuffer()
	{
		if (_port.IsOpen)
			_port.DiscardInBuffer();
	}

	public static void Close()
	{
		if (_port.IsOpen)
			_port.Close();

		_port.Dispose();
	}
}