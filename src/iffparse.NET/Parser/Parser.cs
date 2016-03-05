/*
 * .NET IFF Parser Library
 * Version 0.7
 * 
 * MIT License
 * 
 * .NET implementation based on Jerry Morrison's
 * code from
 * https://github.com/1fish2/IFF/tree/master/IFF%20source%20code
 * 
 */

using System;
using System.IO;
using Net.Iffparse.Marshaling;

namespace Net.Iffparse
{
	/// <summary>
	/// IFF callback.
	/// </summary>
	public delegate int IFFCallBack(IFFParser iff, object userData);

	/// <summary>
	/// IFF parser.
	/// </summary>
	public class IFFParser : IDisposable
	{

		#region Private fields

		private ushort _alignment;
		private bool _disposed = false;
		private IFFIOCallbacks _streamOperations;
		private Stream _ioContext;
		private bool _newIO = false;
		private bool _paused = false;
		private List<ContextNode> _stack;
		private ContextNode _topContext;
		private bool _writeMode = false;

		#endregion
		
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Net.Iffparse.IFFParser"/> class.
		/// </summary>
		/// <param name="stream">Stream.</param>
		/// <param name="writeMode">If set to <c>true</c> write mode.</param>
		public IFFParser(Stream stream, bool writeMode)
			: this(new IFFIOCallbacks
				{
					Close = StreamClose,
					Open = (k,w) => stream,
					Read = StreamRead,
					Seek = StreamSeek,
					Write = StreamWrite
				},string.Empty, writeMode)
		{
			if (stream == null) {
				throw new ArgumentNullException ("stream");
			}

			_ioContext = stream;
		}

		internal IFFParser(IFFIOCallbacks callbacks, string openKey, bool writeMode)
		{
			_streamOperations = callbacks;
			_writeMode = writeMode;
			_newIO = true;
			_alignment = (UInt16)ChunkSize.Alignment;
			_stack = new List<ContextNode>();
			_topContext = new ContextNode(0, 0);
			_stack.AddHead(_topContext);
			try {
				_ioContext = _streamOperations.Open(openKey, writeMode);
			} catch (Exception ex) {
				throw ex;
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the offset.
		/// </summary>
		/// <value>The offset.</value>
		public long Offset {
			get { 
				return _streamOperations.Seek(_ioContext, 0);
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Attaches the context info.
		/// </summary>
		/// <param name="contextNode">Context node.</param>
		/// <param name="contextInfoNode">Context info node.</param>
		public void AttachContextInfo(ContextNode contextNode, ContextInfoNode contextInfoNode)
		{
			uint type = contextInfoNode.Type, 
				 id = contextInfoNode.Id, 
				 identifier = contextInfoNode.Identifier;

			var contextInfoNodesToDelete = new List<ContextInfoNode> ();

			foreach (var ci in contextNode.ContextInfoNodes) {
				if (ci.Id == id && ci.Type == type && ci.Identifier == identifier) {
					contextInfoNodesToDelete.AddHead (ci);
				}
			}
			foreach (var ci in contextInfoNodesToDelete) {
				contextNode.ContextInfoNodes.Remove (ci);
				PurgeContextInfoNode (ci);
			}
			contextNode.ContextInfoNodes.AddHead (contextInfoNode);
		}

		/// <summary>
		/// Return a local context item from the context stack.
		/// </summary>
		/// <param name="type">Type code to search for.</param>
		/// <param name="id">ID code to search for.</param>
		/// <param name="identifier">ident code for the class of context item to search for (ex. "exhd" -- exit handler).</param>
		/// <returns>Local context item, or NULL if nothing matched.</returns>
		public ContextInfoNode FindContextInfo(uint type, uint id, uint identifier)
		{
			foreach (var contextNode in _stack) {
				foreach (var contextInfo in contextNode.ContextInfoNodes) {
					if ((contextInfo.Identifier == identifier)
					    && (contextInfo.Id == id)
					    && (contextInfo.Type == type)) {
						return contextInfo;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Finds the property chunk.
		/// </summary>
		/// <returns>The property chunk.</returns>
		/// <param name="type">Type.</param>
		/// <param name="id">Identifier.</param>
		public ContextInfoNode FindPropChunk(uint type, uint id)
		{
			var ci = FindContextInfo(type, id, GenericChunkIds.CI_PROPCHUNK);
			if (ci != null) {
				return ci;
			}
			return null;
		}

		/// <summary>
		/// Finds the property context.
		/// </summary>
		/// <returns>The property context.</returns>
		public ContextNode FindPropContext()
		{
			foreach (var contextNode in _stack) {
				if (contextNode.Id == GenericChunkIds.ID_FORM
				 || contextNode.Id == GenericChunkIds.ID_LIST) {
					if (contextNode != _stack.Head)
						return contextNode;
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the current context.
		/// </summary>
		/// <returns>The current context.</returns>
		public ContextNode GetCurrentContext()
		{
			var contextNode = _stack.Head;
			if (contextNode != null && contextNode.Id != 0)
				return contextNode;
			return null;
		}

		/// <summary>
		/// Gets the parent context.
		/// </summary>
		/// <returns>The parent context.</returns>
		/// <param name="contextNode">Context node.</param>
		public ContextNode GetParentContext(ContextNode contextNode)
		{
			ContextNode parent;

			parent = (ContextNode)contextNode.Successor;
			if (parent != null && parent.Id != 0) {
				return parent;
			}
			return null;
		}

		/// <summary>
		/// Installs the entry handler.
		/// </summary>
		/// <returns>The entry handler.</returns>
		/// <param name="type">Type.</param>
		/// <param name="id">Identifier.</param>
		/// <param name="position">Position.</param>
		/// <param name="cb">Callback.</param>
		/// <param name="data">Data.</param>
		public int InstallEntryHandler(uint type, uint id, ContextInfoLocation position, IFFCallBack cb, object data)
		{
			return InstallHandler(type, id, GenericChunkIds.CI_ENTRYHANDLER, position, cb, data);
		}

		/// <summary>
		/// Installs the exit handler.
		/// </summary>
		/// <returns>The exit handler.</returns>
		/// <param name="type">Type.</param>
		/// <param name="id">Identifier.</param>
		/// <param name="position">Position.</param>
		/// <param name="cb">Callback</param>
		/// <param name="data">Data.</param>
		public int InstallExitHandler(uint type, uint id, ContextInfoLocation position, IFFCallBack cb, object data)
		{
			return InstallHandler(type, id, GenericChunkIds.CI_EXITHANDLER, position, cb, data);
		}

		///<summary> 
		/// Driver for the parse engine. Loops calling NextState() and acting
		/// on the next parse state. NextState() will always cause the parser
		/// to enter or exit (really pause before exiting) a chunk. In each case,
		/// either an entry handler or an exit handler will be invoked on the
		/// current context. A handler can cause the parser to return control
		/// to the calling program or it can return an error.
		/// </summary>
		public int Parse(ParseMode control)
		{
			ContextNode top;
			ContextInfoNode ci;
			int err;
			int eoc;
			int index = (int)control;
			if (index < 0 || index > (int)ParseMode.RawStep) {
				return (int)ParserStatus.BadMode;
			}

			while (true) {
				/* advance to next state and store end of chunk info. */
				eoc = NextState ();
				if ((eoc < 0) && (eoc != (int)ParserStatus.EndOfContext)) {
					return eoc;
				}

				/* user requesting manual override -- return immediately */
				if (control == ParseMode.RawStep) {
					return eoc;
				}

				if ((top = GetCurrentContext ()) == null) {
					return (int)ParserStatus.EndOfFile;
				}

				/* find chunk handlers for entering or exiting a context. */
				if (eoc == (int)ParserStatus.EndOfContext) {
					ci = FindContextInfo (top.Type, top.Id, GenericChunkIds.CI_EXITHANDLER);
				} else {
					ci = FindContextInfo (top.Type, top.Id, GenericChunkIds.CI_ENTRYHANDLER);
				}

				if (ci != null) {
					var ch = ci.ChunkHandler;
					err = ch.ChunkHandlerCallBack (this, ch.UserData);
					if (err != 1)
						return err;
				}

				if (control == ParseMode.Step) {
					return eoc;
				}
			}
		}

		/// <summary>
		/// Pops the chunk from the current context.
		/// </summary>
		/// <returns>The chunk.</returns>
		public int PopChunk()
		{
			if (_writeMode) {
				return PopChunkWrite ();
			}		
			return PopChunkRead ();
		}

		/// <summary>
		/// Pushs the chunk on the current context.
		/// </summary>
		/// <returns>The chunk.</returns>
		/// <param name="type">Type.</param>
		/// <param name="id">Identifier.</param>
		/// <param name="size">Size.</param>
		public int PushChunk(uint type, uint id, uint size)
		{
			if (_writeMode) {
				return PushChunkWrite (type, id, size);
			}
			return PushChunkRead (false);
		}

		/// <summary>
		/// Reads the chunk.
		/// </summary>
		/// <returns>The chunk.</returns>
		/// <param name="buffer">Buffer.</param>
		/// <param name="offset">Offset.</param>
		/// <param name="count">Count.</param>
		public int ReadChunk(byte[] buffer, int offset, int count)
		{
			var top = GetCurrentContext ();
			if (top == null) {
				return (int)ParserStatus.EndOfFile;
			}

			var maxBytes = top.Size - top.Offset;

			if ((ulong)count > maxBytes) {
				count = (int)maxBytes;
			}

			var result = _streamOperations.Read (_ioContext, buffer, offset, count);
			if (result != count) {
				if (result >= 0) {
					result = (int)ParserStatus.PrematureEndOfFile;
				}
			} else {
				top.Offset += (ulong)result;
			}
			return result;
		}

		/// <summary>
		/// Registers the property chunks.
		/// </summary>
		/// <returns>The property chunks.</returns>
		/// <param name="typeIds">Type identifiers.</param>
		/// <param name="ids">Identifiers.</param>
		public int RegisterPropChunks(uint[] typeIds, uint[] ids)
		{
			if (typeIds.Length != ids.Length)
				return 0;

			int result;
			for (var i = 0; i < typeIds.Length && typeIds [i] > 0; i++) {
				result = InstallEntryHandler (typeIds [i], ids [i], ContextInfoLocation.Top, new IFFCallBack (HandlePropertyChunk), null);
				if (result < 0)
					return result;
			}
			return 0;
		}

		/// <summary>
		/// Registers the stop chunks.
		/// </summary>
		/// <returns>The stop chunks.</returns>
		/// <param name="typeIds">Type identifiers.</param>
		/// <param name="ids">Identifiers.</param>
        public int RegisterStopChunks(uint[] typeIds, uint[] ids)
		{
			if (typeIds.Length != ids.Length)
				return 0;

			int result;
			for (var i = 0; i < typeIds.Length && typeIds [i] > 0; i++) {
				result = InstallEntryHandler (typeIds [i], ids [i], ContextInfoLocation.Top, new IFFCallBack (HandleStopChunk), null);
				if (result < 0)
					return result;
			}
			return 0;
		}

		/// <summary>
		/// Removes the context info.
		/// </summary>
		/// <param name="contextInfoNode">Context info node.</param>
		public void RemoveContextInfo(ContextInfoNode contextInfoNode)
		{
			if (contextInfoNode.Successor != null) {
				MinNode.Remove (contextInfoNode);
				contextInfoNode.Successor = null;
			}
		}

		/// <summary>
		/// Seeks the chunk.
		/// </summary>
		/// <returns>The chunk.</returns>
		/// <param name="position">Position.</param>
		/// <param name="mode">Mode.</param>
		public long SeekChunk(int position, SeekMode mode)
		{
			var top = GetCurrentContext ();
			if (top == null) {
				return (long)ParserStatus.EndOfFile;
			}
			/* pick offset base depending on mode */
			ulong baseOffset;
			switch (mode) {
			case SeekMode.Start:
				baseOffset = 0;
				break;
			case SeekMode.Current:
				baseOffset = top.Offset;
				break;
			case SeekMode.End:
				baseOffset = top.Size;
				break;
			default:
				return (long)ParserStatus.BadMode;
			}
			/* Clip position in range of -base..cn_Size-base. Result in newOffset is in range of 0..cn_Size */
			ulong pos;
			ulong newOffset = 0;
			if (position >= 0) {
				pos = (ulong)position;
				if (pos < top.Size - baseOffset) {
					newOffset = baseOffset + pos;
				} else {
					newOffset = top.Size;
				}
			}
			/* seek on stream */
			var seekOffset = (long)(newOffset - top.Offset);
			long newStreamPosition, oldStreamPosition;

			if ((oldStreamPosition = _streamOperations.Seek (_ioContext, seekOffset)) < 0) {
				return oldStreamPosition;
			}
			if ((newStreamPosition = _streamOperations.Seek (_ioContext, 0)) < 0) {
				return newStreamPosition;
			}
			if ((newStreamPosition - oldStreamPosition) != seekOffset) {
				return (long)ParserStatus.PrematureEndOfFile;
			}
			top.Offset = newOffset;
			return 0;
		}

		/// <summary>
		/// Stores the context info.
		/// </summary>
		/// <returns>The context info.</returns>
		/// <param name="contextInfoNode">Context info node.</param>
		/// <param name="location">Location.</param>
		public int StoreContextInfo(ContextInfoNode contextInfoNode, ContextInfoLocation location)
		{
			ContextNode contextNode;

			switch (location) {
				case ContextInfoLocation.Bottom:
					contextNode = _stack.Tail;
					break;
				case ContextInfoLocation.Top:
					contextNode = _stack.Head;
					break;
				case ContextInfoLocation.Prop:
					if ((contextNode = FindPropContext ()) != null) {
						break;
					}
					return (int)ParserStatus.NoScope;
				default:
					return (int)ParserStatus.BadLocation;
			}

			AttachContextInfo (contextNode, contextInfoNode);

			return 0;
		}

		/// <summary>
		/// Writes the chunk.
		/// </summary>
		/// <returns>The chunk.</returns>
		/// <param name="buffer">Buffer.</param>
		/// <param name="offset">Offset.</param>
		/// <param name="count">Count.</param>
		public int WriteChunk(byte[] buffer, int offset, int count)
		{
			var top = GetCurrentContext ();
			if (top == null) {
				return (int)ParserStatus.EndOfFile;
			}
			if (IFFUtility.IsGenericId (top.Id)) {
				return (int)ParserStatus.NotIFF;
			}
			return WriteChunkInternal (top, buffer, offset, count);
		}

		/// <summary>
		/// Purges the context info node.
		/// </summary>
		/// <returns>The context info node.</returns>
		/// <param name="contextInfoNode">Context info node.</param>
		protected int PurgeContextInfoNode(ContextInfoNode contextInfoNode)
		{
			if (contextInfoNode.PurgeCallBack != null)
			{
				return contextInfoNode.PurgeCallBack(this, contextInfoNode);
			}
			contextInfoNode.Dispose();
			return 0;
		}

		private void FreeContextNode(ContextNode contextNode)
		{
			_stack.Remove(contextNode);
			while (contextNode.ContextInfoNodes.Count > 0)
			{
				var contextInfoNode = contextNode.ContextInfoNodes.RemoveHead();
				PurgeContextInfoNode(contextInfoNode);
			}
			contextNode.Dispose();
		}

		/// <summary>
		/// Store contents of a chunk in a buffer and return 0 or error code
		/// </summary>
		private int BufferChunk(int size, ref byte[] buffer)
		{
			var result = ReadChunk (buffer, 0, (int)size);
			if (result != size) {
				if (result >= 0)
					result = (int)ParserStatus.PrematureEndOfFile;
				return result;
			}
			return 0;
		}

		private static int HandlePropertyChunk(IFFParser iff, object userData)
		{
			int err;

			var cn = iff.GetCurrentContext ();
			var ci = new ContextInfoNode (cn.Id, cn.Type, GenericChunkIds.CI_PROPCHUNK, cn.Size, null);
			
			if (ci != null) {
				var buffer = new byte[cn.Size];
				err = iff.BufferChunk ((int)cn.Size, ref buffer);
				if (err >= 0) {
					ci.Data = buffer;
					err = iff.StoreContextInfo (ci, ContextInfoLocation.Prop);
					if (err >= 0)
						return 1;
				}
				ci.Dispose ();
			} else {
				err = (int)ParserStatus.OutOfMemory;    
			}
			return err;
		}

        private static int HandleStopChunk(IFFParser iff, object userData)
        {
            /* stop parsing and return to client */
            return 0;
        }

		/// <summary>
		/// Install a handler node into the current context
		/// </summary>
		/// <returns></returns>
		private int InstallHandler(uint type, uint id, uint identifier, ContextInfoLocation position, IFFCallBack callbackHandler, object data)
		{
			int err;

			var ci = new ContextInfoNode(id, type, identifier, 0, null);
			if (ci != null) {
				var ch = new ChunkHandler {
					ChunkHandlerCallBack = callbackHandler,
					UserData = data
				};
				ci.ChunkHandler = ch;
				err = StoreContextInfo(ci, position);
				if (err < 0) {
					ci.Dispose();
				}
				return err;
			}
			return 0;
		}

		/// <summary>
		///Test current state and advance to the next state. State is one of:
		/// - 	Start of file if iff_NewIO is TRUE.
		///		Push initial chunk, get its info and return.
		///
		///	- 	Poised at end of chunk if iff_Paused is TRUE.
		///		PopChunk() the current context. The top context will then be a generic
		///		or end of file.  Return EOF if the latter. If this chunk is exhausted,
		///		pause again and return, otherwise push new chunk and return.
		///
		///	- 	Inside a generic chunk.
		///		Push a new context and return.
		///
		///	- 	Inside a non-generic chunk.
		///		Pause and return.
		/// </summary>
		/// <returns>The state.</returns>
		private int NextState()
		{
			ContextNode top;
			int err;
			uint topId;

			/* deal with the case of the first chunk */
			if (_newIO) {
				err = PushChunkRead (true);
				_newIO = false;
				_paused = false;
				if (err < 0) {
					return err;
				}

				top = GetCurrentContext ();

				if ((top.Id != GenericChunkIds.ID_FORM) &&
				    (top.Id != GenericChunkIds.ID_CAT) &&
				    (top.Id != GenericChunkIds.ID_LIST) &&
				    (!IsContainerId (top.Id)))
					return (int)ParserStatus.NotIFF;

				if ((top.Size & 1) == 1) {
					/* containers must inherently be word-aligned */
					return (int)ParserStatus.Mangled;
				}

				return ReadGenericType ();
			}

			/* In the PAUSE state, do the deferred PopChunk() */
			if (_paused) {
				err = PopChunk ();
				if (err < 0) {
					return err;
				}
				_paused = false;
			}

			/* If no top context node then the file is exhausted. */
			if ((top = GetCurrentContext ()) == null) {
				return (int)ParserStatus.EndOfFile;
			}

			topId = top.Id;
			if (IFFUtility.IsGenericId (topId) || IsContainerId (topId)) {
				/* If inside a generic chunk, and not exhausted, push a subchunk. */
				if (top.Offset < top.Size) {
					err = PushChunkRead (false);
					if (err < 0) {
						return err;
					}

					top = GetCurrentContext ();

					/* If non-generic, we're done, but if the containing chunk is not
					* FORM or PROP, it's an IFF syntax error.
					*/
					if (!IFFUtility.IsGenericId (top.Id)) {
						if ((topId != GenericChunkIds.ID_FORM) &&
						    (topId != GenericChunkIds.ID_PROP) &&
						    (!IsContainerId (topId)))
							return (int)ParserStatus.SyntaxError;

						return 0;
					}

					/* If this new chunk is generic, and is a PROP, test to see if it's
					* in the right place --  containing chunk should be a LIST.
					*/
					if ((top.Id == GenericChunkIds.ID_PROP) &&
					    (topId != GenericChunkIds.ID_LIST)) {
						return (int)ParserStatus.SyntaxError;
					}

					/* since it's an ok generic, get its type and return */
					return ReadGenericType ();
				} else if (top.Offset != top.Size) {
					/* If not exhaused, this is a junky IFF file */
					return (int)ParserStatus.Mangled;
				}
			}
			/* If the generic is exhausted, or this is a non-generic chunk,
			 * enter the pause state and return flag.
			 */
			_paused = true;

			return (int)ParserStatus.EndOfContext;
		}

		private int PopChunkRead()
		{
			ContextNode top;
			int error;
			ulong rsize;

			/* Get top chunk and seek past anything un-scanned including the optional pad byte
			 */
			top = GetCurrentContext ();
			if (top == null) {
				return (int)ParserStatus.EndOfFile;
			}

			rsize = (top.Size + 1) & ~(ulong)1;

			if (top.Offset < rsize) {
				error = (int)_streamOperations.Seek (_ioContext, (long)(rsize - top.Offset));
				if (error < 0) {
					return error;
				}
			}

			/* Remove and deallocate this chunk and any stored properties */
			FreeContextNode (top);

			/* Update the new top chunk, if any */
			top = GetCurrentContext ();
			if (top != null) {
				top.Offset += rsize;
			}
			return 0;
		}

		private int PopChunkWrite()
		{
			ContextNode top;
			ulong rsize;
			int err;
			ulong size;
			byte pad;
			bool unknownSize;
			bool bit64;
			uint size32;
			byte[] padBuf = new byte[2];
			uint alignment, padding, extra;
			byte[] buffer;

			if ((top = GetCurrentContext ()) == null)
				return (int)ParserStatus.EndOfFile;

			if (top.Size == (uint)ChunkSize.Unknown32Bit) {
				/* Size is unknown.  The size is therefore however many bytes the
				 * client wrote.
				 */
				size = top.CurrentSize;
				bit64 = false;
				unknownSize = true;
			} else if (top.Size == (uint)ChunkSize.Unknown64Bit) {
				/* Size is unknown.  The size is therefore however many bytes the
				 * client wrote.
				 */
				size = top.CurrentSize;
				bit64 = true;
				unknownSize = true;
			} else {
				/* Size is known.  Compare what the client said against what actually
				 * happened.
				 */
				size = top.Size;
				if (size != top.CurrentSize)
					return (int)ParserStatus.EndOfFile;

				bit64 = false;
				unknownSize = false;
			}

			/* if we're not at the end of the chunk, go there */
			if (top.Offset != top.CurrentSize) {
				err = (int)_streamOperations.Seek (_ioContext, (long)(top.CurrentSize - top.Offset));
				if (err < 0)
					return err;
			}

			/* Add a possible pad byte */
			rsize = ((size + 1) & ~((ulong)1));
			if (rsize > size) {
				pad = 0;
				var padBuffer = new byte[1];
				padBuffer [0] = pad;
				err = _streamOperations.Write (_ioContext, padBuffer, 0, padBuffer.Length);
				if (err < 0)
					return err;
			}

			/* Remove and deallocate this chunk and any stored properties */
			FreeContextNode (top);

			/* If needed, seek back and fix the chunk size */
			if (unknownSize) {
				if (bit64) {
					err = (int)_streamOperations.Seek (_ioContext, -(long)(rsize + (ulong)(size)));
					if (err >= 0) {
						Swap (ref size);
						buffer = BitConverter.GetBytes (size);
						err = _streamOperations.Write (_ioContext, buffer, 0, buffer.Length);
					}
				} else {
					if (size >= (ulong)ChunkSize.Reserved)
						return (int)ParserStatus.TooBig;

					size32 = (uint)size;

					err = (int)_streamOperations.Seek (_ioContext, -(long)(rsize + (ulong)(size32)));
					if (err >= 0) {
						buffer = BitConverter.GetBytes (size32);
						err = _streamOperations.Write (_ioContext, buffer, 0, buffer.Length);
					}
				}

				if (err >= 0)
					err = (int)_streamOperations.Seek (_ioContext, (long)rsize);

				if (err < 0)
					return err;
			}

			if ((top = GetCurrentContext ()) != null) {
				/* Update parent's count */
				top.Offset += rsize;
				top.CurrentSize += rsize;
				if ((top.Size != (uint)ChunkSize.Unknown32Bit)
				    && (top.Size != (uint)ChunkSize.Unknown64Bit)
				    && (top.Offset > top.Size))
					return (int)ParserStatus.Mangled;
			}

			alignment = _alignment;
			if ((padding = (uint)(rsize % alignment)) == 1) {
				if ((8 % alignment) == 1) {
					padding = alignment - ((8 + padding) % alignment);
				} else {
					padding = alignment - padding;
				}

				/* Update parent's count */
				top.Offset += padding + 8;
				top.CurrentSize += padding + 8;
				if ((top.Size != (ulong)ChunkSize.Unknown32Bit)
				    && (top.Size != (ulong)ChunkSize.Unknown64Bit)
				    && (top.Offset > top.Size)) {
					return (int)ParserStatus.Mangled;
				}

				extra = GenericChunkIds.ID_PAD;
				buffer = BitConverter.GetBytes (extra);
				err = _streamOperations.Write (_ioContext, buffer, 0, 4);
				if (err < 0)
					return err;

				buffer = BitConverter.GetBytes (padding);
				err = _streamOperations.Write (_ioContext, buffer, 0, 4);
				if (err < 0)
					return err;

				padBuf [0] = (byte)(alignment >> 8);
				padBuf [1] = (byte)(alignment & 0x00FF);

				err = _streamOperations.Write (_ioContext, padBuf, 0, 2);
				if (err < 0)
					return err;

				padding -= 2;

				padBuf [0] = padBuf [1] = 0;
				while (padding > 0) {
					err = _streamOperations.Write (_ioContext, padBuf, 0, 2);
					if (err < 0)
						return err;

					padding -= 2;
				}
			}

			return 0;
		}

		private int PushChunkRead(bool firstChunk)
		{
			Chunk chunk;
			int err;
			byte[] buffer;

			var top = GetCurrentContext ();
			if (top == null && !firstChunk) {
				return (int)ParserStatus.EndOfFile;
			}
			var type = top != null ? top.Type : 0;

			if (firstChunk) {
				buffer = new byte[Marshal.SizeOf<SmallChunk>()];
				err = _streamOperations.Read (_ioContext, buffer, 0, buffer.Length);
				if (err != Marshal.SizeOf<SmallChunk>()) {
					if (err >= 0) {
						err = (int)ParserStatus.NotIFF;
					}
					return err;
				}
				var smallChunk = Marshal.BytesToStruct<SmallChunk>(buffer);
				chunk.Id = smallChunk.Id;
				chunk.Size = smallChunk.Size;

				if (chunk.Size == (ulong)ChunkSize.Known64Bit) {
					buffer = new byte[chunk.Size];
					err = _streamOperations.Read (_ioContext, buffer, 0, buffer.Length);
					if ((ulong)err != chunk.Size) {
						if (err >= 0) {
							err = (int)ParserStatus.NotIFF;
						}
						return err;
					}
					chunk.Size = BitConverter.ToUInt32 (buffer, 0);
				} else if (chunk.Size >= (ulong)ChunkSize.Reserved) {
					return (int)ParserStatus.Mangled;
				}
			} else {
				/* Using ReadChunk() causes these bytes to go into the
				 * parent's scan count
				 */
				buffer = new byte[Marshal.SizeOf<SmallChunk>()];
				err = ReadChunk (buffer, 0, buffer.Length);
				if (err != Marshal.SizeOf<SmallChunk>()) {
					if (err >= 0) {
						err = (int)ParserStatus.PrematureEndOfFile;
					}
					return err;
				}

				var smallChunk = Marshal.BytesToStruct<SmallChunk>(buffer);
				chunk.Id = smallChunk.Id;
				chunk.Size = smallChunk.Size;

				if (chunk.Size == (ulong)ChunkSize.Known64Bit) {
					buffer = new byte[chunk.Size];
					err = _streamOperations.Read (_ioContext, buffer, 0, buffer.Length);
					if ((ulong)err != chunk.Size) {
						if (err >= 0) {
							err = (int)ParserStatus.NotIFF;
						}
						return err;
					}
					chunk.Size = BitConverter.ToUInt32 (buffer, 0);
				} else if (chunk.Size >= (ulong)ChunkSize.Reserved) {
					return (int)ParserStatus.Mangled;
				}
				if (chunk.Size > (top.Size - top.Offset))
					return (int)ParserStatus.Mangled;
			}
			if (IFFUtility.IsGoodId (chunk.Id)) {
				try {
					var contextNode = new ContextNode (chunk.Id, type);
					contextNode.Size = chunk.Size;
					contextNode.Offset = 0;
					contextNode.CurrentSize = chunk.Size;

					_stack.AddHead (contextNode);
					return 0;
				} catch (OutOfMemoryException) {
					return (int)ParserStatus.OutOfMemory;
				}
			}
			if (this._newIO) {
				return (int)ParserStatus.NotIFF;
			}
			return (int)ParserStatus.SyntaxError;
		}

		private int PushChunkWrite(uint type, uint id, uint size)
		{
			ContextNode cn;
			int err;
			uint parentType;
			Chunk chunk;
			SmallChunk smallChunk;
			bool firstChunk;
			byte[] buffer;

			if ((cn = GetCurrentContext ()) != null) {
				parentType = cn.Type;
				firstChunk = false;
			} else if (_newIO) {
				parentType = 0;
				firstChunk = true;
				_newIO = false;

				/* first chunk must be FORM, CAT, or LIST */
				if ((id != GenericChunkIds.ID_FORM) &&
				    (id != GenericChunkIds.ID_CAT) &&
				    (id != GenericChunkIds.ID_LIST))
					return (int)ParserStatus.NotIFF;
			} else {
				return (int)ParserStatus.EndOfFile;
			}
			if (!IFFUtility.IsGoodId (id)) {
				return (int)ParserStatus.SyntaxError;
			}
			if (IFFUtility.IsGenericId (id)) {
				if (id == GenericChunkIds.ID_PROP) {
					/* the containing context for PROP must be a LIST */
					if (cn.Id != GenericChunkIds.ID_LIST) {
						return (int)ParserStatus.SyntaxError;
					}
				}
				/* Generic ID.  Check the validity of its subtype. */
				if (!IFFUtility.IsGoodType (type)) {
					return (int)ParserStatus.NotIFF;
				}
			} else {
				/* Non-generic ID.  Containing context must be PROP or FORM or container */
				if ((cn.Id != GenericChunkIds.ID_FORM) &&
				    (cn.Id != GenericChunkIds.ID_PROP) &&
				    (!IsContainerId (cn.Id))) {
					return (int)ParserStatus.SyntaxError;
				}
			}
			/* Write the chunk header: ID and size (if IFF_SIZE_UNKNOWN, it will
			 * be patched later by PopChunkW()).
			 */
			if ((size >= (uint)ChunkSize.Reserved) &&
			    (size != (uint)ChunkSize.Unknown32Bit)) {
				chunk.Id = id;
				chunk.Filler = (uint)ChunkSize.Known64Bit;
				chunk.Size = size;

				buffer = Marshal.StructToBytes(chunk);

				if (firstChunk) {
					err = _streamOperations.Write (_ioContext, buffer, 0, buffer.Length);
				} else {
					/* Update parent's count of bytes written. */
					err = WriteChunkInternal (cn, buffer, 0, buffer.Length);
				}
			} else {
				smallChunk.Id = id;
				smallChunk.Size = size;
				buffer = Marshal.StructToBytes (smallChunk);

				if (firstChunk) {
					err = _streamOperations.Write (_ioContext, buffer, 0, buffer.Length);
				} else {
					/* Update parent's count of bytes written. */
					err = WriteChunkInternal (cn, buffer, 0, buffer.Length);
				}
			}
			if (err < 0) {
				return err;
			}
			/* Allocate and fill in a ContextNode for the new chunk */
			cn = new ContextNode (id, 0);
			cn.Size = size;
			cn.Offset = 0;
			cn.CurrentSize = 0;
			_stack.AddHead (cn);

			/* For generic chunks, write the type out now that
			 * the chunk context node is initialized.
			 */
			if (IFFUtility.IsGenericId (id) || IsContainerId (id)) {
				if (IFFUtility.IsGenericId (id)) {
					var bigEndianType = type;
					Swap (ref bigEndianType);
					buffer = BitConverter.GetBytes (bigEndianType);
					err = WriteChunkInternal (cn, buffer, 0, buffer.Length);
					cn.Type = type;
				} else {
					cn.Type = 0;
				}
			} else {
				cn.Type = parentType;
			}
			return err;
		}

		private int ReadGenericType()
		{
			ContextNode top;
			int result;

			if ((top = GetCurrentContext()) != null) {
				var buffer = new byte[sizeof(uint)];
				result = ReadChunk(buffer, 0, buffer.Length);
				var type = BitConverter.ToUInt32(buffer, 0);
				Swap(ref type);
				top.Type = type;
				if (result == sizeof(uint)) {
					if (IFFUtility.IsGoodType(top.Type)) {
						return 0;
					}
					return (int)ParserStatus.Mangled;
				}
				if (result >= 0) {
					result = (int)ParserStatus.PrematureEndOfFile;
				}
				return result;
			}
			return (int)ParserStatus.EndOfFile;
		}

		private int WriteChunkInternal(ContextNode top, byte[] buffer, int offset, int count)
		{
			ulong maxBytes;

			if ((top.Size != (ulong)ChunkSize.Unknown32Bit) && 
				(top.Size != (ulong)ChunkSize.Unknown64Bit)) {
				maxBytes = top.Size - top.Offset;
				if ((ulong)count > maxBytes) {
					count = (int)maxBytes;
				}
			}
			if (count <= 0) {
				return 0;
			}
			var result = _streamOperations.Write(_ioContext, buffer, offset, count);
			if (result >= 0) {
				top.Offset += (ulong)result;
				if (top.Offset > top.CurrentSize) {
					top.CurrentSize = top.Offset;
				}
			}
			return result;
		}

		#endregion

		#region Helper methods

		private static int StreamClose(Stream stream)
		{
			if (stream == null) {
				throw new ArgumentNullException("stream");
			}
			stream.Flush();
			stream.Dispose();
			return 0;
		}

		private static bool IsContainerId(uint id)
		{
			return ((id & GenericChunkIds.ID_CONTAINER) == GenericChunkIds.ID_CONTAINER);
		}

		private static int StreamRead(Stream stream, byte[] buffer, int offset, int count)
		{
			if (stream == null) {
				throw new ArgumentNullException("stream");
			}
			return stream.Read(buffer, offset, count);
		}

		private static long StreamSeek(Stream stream, long position)
		{
			if (stream == null) {
				throw new ArgumentNullException("stream");
			}
			return stream.Seek(position, SeekOrigin.Current);
		}

		static void Swap(ref uint size)
		{
			var buffer = BitConverter.GetBytes(size);
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(buffer);
			}
			size = BitConverter.ToUInt32(buffer, 0);
		}

		static void Swap(ref ulong size)
		{
			var buffer = BitConverter.GetBytes(size);
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(buffer);
			}
			size = BitConverter.ToUInt64(buffer, 0);
		}

		private static int StreamWrite(Stream stream, byte[] buffer, int offset, int count)
		{
			if (stream == null) {
				throw new ArgumentNullException("stream");
			}
			stream.Write(buffer, offset, count);
			return count;
		}

		#endregion

		#region IDisposable implementation

		/// <summary>
		/// Releases all resource used by the <see cref="Net.Iffparse.IFFParser"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Net.Iffparse.IFFParser"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="Net.Iffparse.IFFParser"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the <see cref="Net.Iffparse.IFFParser"/> so the garbage
		/// collector can reclaim the memory that the <see cref="Net.Iffparse.IFFParser"/> was occupying.</remarks>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Dispose the specified disposing.
		/// </summary>
		/// <param name="disposing">If set to <c>true</c> disposing.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed) {
				if (disposing) {
					var result = 0L;
					var top = (ContextNode)null;
					var ci = (ContextInfoNode)null;
					while (GetCurrentContext() != null) {
						result = PopChunkRead();
						if (result < 0) {
							while ((top = GetCurrentContext()) != null) {
								FreeContextNode(top);
							}
						}
					}
					/* also do the master context */
					while (_topContext.ContextInfoNodes.Count > 0) {
						ci = _topContext.ContextInfoNodes.RemoveHead();
						PurgeContextInfoNode(ci);
					}

					if (result >= 0) {
						result = _streamOperations.Close(_ioContext);
					} else {
						_streamOperations.Close(_ioContext);
					}
					_topContext.Dispose();
					_topContext = null;
				}
				_disposed = true;
			}
		}

		#endregion

		#region Destructor

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the <see cref="Net.Iffparse.IFFParser"/>
		/// is reclaimed by garbage collection.
		/// </summary>
		~IFFParser()
		{
			Dispose(false);
		}

		#endregion
	}
}
