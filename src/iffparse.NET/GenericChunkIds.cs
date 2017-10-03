using IffParse.Lists;
using IffParse.Util;

namespace IffParse
{
	/// <summary>
    /// Helper class for chunk id's 
    /// </summary>
    public static class GenericChunkIds
	{
		/// <summary>
        /// Collection chunk identifier for <see cref="ContextInfoNode"/>
        /// </summary>
		public static readonly uint CI_COLLECTIONCHUNK = IdUtility.MakeId('c', 'o', 'l', 'l');
		/// <summary>
        /// Entry handler identifier for <see cref="ContextInfoNode"/>
        /// </summary>
		public static readonly uint CI_ENTRYHANDLER = IdUtility.MakeId('e', 'n', 'h', 'd');
		/// <summary>
        /// Exit handler identifier for <see cref="ContextInfoNode"/>
        /// </summary>
		public static readonly uint CI_EXITHANDLER = IdUtility.MakeId('e', 'x', 'h', 'd');
		/// <summary>
        /// Property chunk identifier for <see cref="ContextInfoNode"/>
        /// </summary>
		public static readonly uint CI_PROPCHUNK = IdUtility.MakeId('p', 'r', 'o', 'p');
		/// <summary>
        /// Entry handler identifier for <see cref="ContextInfoNode"/>
        /// </summary>
		public static readonly uint ID_CAT = IdUtility.MakeId('C', 'A', 'T', ' ');
		/// <summary>
        /// Form chunk identifier
        /// </summary>
		public static readonly uint ID_FORM = IdUtility.MakeId('F', 'O', 'R', 'M');
		/// <summary>
        /// List chunk identifier
        /// </summary>
		public static readonly uint ID_LIST = IdUtility.MakeId('L', 'I', 'S', 'T');
		/// <summary>
        /// NULL chunk identifier
        /// </summary>
		public static readonly uint ID_NULL = IdUtility.MakeId(' ', ' ', ' ', ' ');
		/// <summary>
        /// Property chunk identifier
        /// </summary>
		public static readonly uint ID_PROP = IdUtility.MakeId('P', 'R', 'O', 'P');
		/// <summary>
        /// Container chunk identifier
        /// </summary>
		internal static readonly uint ID_CONTAINER = IdUtility.MakeId('\0', '\0', '\0', '{');
		/// <summary>
        /// Padding chunk identifier
        /// </summary>
		internal static readonly uint ID_PAD = IdUtility.MakeId('!', 'P', 'A', 'D');
	}
}

