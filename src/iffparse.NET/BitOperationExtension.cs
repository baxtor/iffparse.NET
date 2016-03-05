using System;

namespace Net.Iffparse
{
	internal static class BitOperationExtension
	{
		public static Int16 Reverse(this Int16 instance)
		{
			byte[] array = BitConverter.GetBytes(instance);
			Array.Reverse(array);

			return BitConverter.ToInt16(array, 0);
		}

		public static Int32 Reverse(this Int32 instance)
		{
			byte[] array = BitConverter.GetBytes(instance);
			Array.Reverse(array);
			
			return BitConverter.ToInt32(array, 0);
		}

		public static Int64 Reverse(this Int64 instance)
		{
			byte[] array = BitConverter.GetBytes(instance);
			Array.Reverse(array);
			
			return BitConverter.ToInt64(array, 0);
		}

		public static UInt16 Reverse(this UInt16 instance)
		{
			byte[] array = BitConverter.GetBytes(instance);
			Array.Reverse(array);
			
			return BitConverter.ToUInt16(array, 0);
		}

		public static UInt32 Reverse(this UInt32 instance)
		{
			byte[] array = BitConverter.GetBytes(instance);
			Array.Reverse(array);
			
			return BitConverter.ToUInt32(array, 0);
		}

		public static UInt64 Reverse(this UInt64 instance)
		{
			byte[] array = BitConverter.GetBytes(instance);
			Array.Reverse(array);
			
			return BitConverter.ToUInt64(array, 0);
		}

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

