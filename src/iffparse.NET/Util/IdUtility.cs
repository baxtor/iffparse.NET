using System;

namespace IffParse.Util
{
	/// <summary>
	/// IFF Identifier utility.
	/// </summary>
	public static class IdUtility
	{
		/// <summary>
		/// Identifiers to string.
		/// </summary>
		/// <returns>The to string.</returns>
		/// <param name="id">Identifier.</param>
		public static string IdToString(uint id)
		{
			return string.Format(
				"{0}{1}{2}{3}", 
				(char)((id >> 24) & 0x7f), 
				(char)((id >> 16) & 0x7F), 
				(char)((id >> 8) & 0x7F), 
				(char)(id & 0x7F));
		}

		/// <summary>
		/// Determines if is generic identifier the specified id.
		/// </summary>
		/// <returns><c>true</c> if is generic identifier the specified id; otherwise, <c>false</c>.</returns>
		/// <param name="id">Identifier.</param>
		public static bool IsGenericId(uint id)
		{
			return (id == GenericChunkIds.ID_FORM 
				 || id == GenericChunkIds.ID_LIST 
				 || id == GenericChunkIds.ID_CAT 
				 || id == GenericChunkIds.ID_PROP);
		}

		/// <summary>
		/// Determines if is good identifier the specified id.
		/// </summary>
		/// <returns><c>true</c> if is good identifier the specified id; otherwise, <c>false</c>.</returns>
		/// <param name="id">Identifier.</param>
		public static bool IsGoodId(uint id)
		{
			var idArray = BitConverter.GetBytes(id);
			if (((char)idArray[0]) == ' ')
			{
				if (id == GenericChunkIds.ID_NULL)
				{
					return true;
				}
				return false;
			}
			for (int i = 0; i < idArray.Length; i++)
			{
				if ((((char)idArray[i]) < ' ') || ((char)idArray[i] > '~'))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Determines if is good type the specified id.
		/// </summary>
		/// <returns><c>true</c> if is good type the specified id; otherwise, <c>false</c>.</returns>
		/// <param name="id">Identifier.</param>
		public static bool IsGoodType(uint id)
		{
			if (!IsGoodId(id))
			{
				return false;
			}
			var idArray = BitConverter.GetBytes(id);

			for (int i = 0; i < idArray.Length; i++)
			{
				char current = (char)idArray[i];
				if (((current < 'A') || (current > 'Z'))
					&& ((current < '0') || (current > '9')))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Makes the identifier.
		/// </summary>
		/// <returns>The identifier.</returns>
		/// <param name="a">The alpha component.</param>
		/// <param name="b">The blue component.</param>
		/// <param name="c">C.</param>
		/// <param name="d">D.</param>
		public static uint MakeId(char a, char b, char c, char d)
		{
			return (((uint)(a) << 24) | ((uint)(b) << 16) | ((uint)(c) << 8) | ((uint)(d)));
		}

	}
}

