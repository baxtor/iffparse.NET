using System;

namespace IffParse
{
	internal static class BitOperationExtension
	{
		public static Byte HiByte(this Byte instance)
		{
			return (Byte)((instance)>> 4);
		}

		public static Byte LoByte(this Byte instance)
		{
			return (Byte)((instance) & 0xF);
		}
	}
}

