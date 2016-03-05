using System;

namespace Net.Iffparse
{
	public static class GenericChunkIds
	{
		public static readonly uint CI_COLLECTIONCHUNK = IFFUtility.MakeId('c', 'o', 'l', 'l');
		public static readonly uint CI_ENTRYHANDLER = IFFUtility.MakeId('e', 'n', 'h', 'd');
		public static readonly uint CI_EXITHANDLER = IFFUtility.MakeId('e', 'x', 'h', 'd');
		public static readonly uint CI_PROPCHUNK = IFFUtility.MakeId('p', 'r', 'o', 'p');
		public static readonly uint ID_CAT = IFFUtility.MakeId('C', 'A', 'T', ' ');
		public static readonly uint ID_FORM = IFFUtility.MakeId('F', 'O', 'R', 'M');
		public static readonly uint ID_LIST = IFFUtility.MakeId('L', 'I', 'S', 'T');
		public static readonly uint ID_NULL = IFFUtility.MakeId(' ', ' ', ' ', ' ');
		public static readonly uint ID_PROP = IFFUtility.MakeId('P', 'R', 'O', 'P');
		internal static readonly uint ID_CONTAINER = IFFUtility.MakeId('\0', '\0', '\0', '{');
		internal static readonly uint ID_PAD = IFFUtility.MakeId('!', 'P', 'A', 'D');
	}
}

