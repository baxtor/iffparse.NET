using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iffparse.NET.io
{
    using iffparse.NET.parser;
    /// <summary>
    /// Provides an easy to use mechanism to load a file with an expected type
    /// </summary>
    public class InterchangeFile : IDisposable
    {
        #region Fields

		private string _fileName;
        
		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="iffparse.NET.File"/> class.
		/// </summary>
		/// <param name="fileName">File name.</param>
		/// <param name="isBigEndian">If set to <c>true</c> is big endian.</param>
		public InterchangeFile (string fileName, bool isBigEndian)
		{
			Parser = new Parser (fileName, isBigEndian);
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="iffparse.NET.io.InterchangeFile"/> class.
		/// </summardy>
		/// <param name="fileName">File name.</param>
		/// <param name="isBigEndian">If set to <c>true</c> is big endian.</param>
		/// <param name="parser">Parser.</param>
		public InterchangeFile(string fileName, bool isBigEndian, IParser parser)
		{
			Parser = parser;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the file structure.
		/// </summary>
		/// <value>The file structure.</value>
		public StructureTreeNode FileStructure 
		{
			get
			{
				return Parser.Result;
			}
		}

		/// <summary>
		/// Gets the name of the file.
		/// </summary>
		/// <value>The name of the file.</value>
		public string FileName {
			get 
			{
				return _fileName;
			}
			private set 
			{
				if (string.IsNullOrEmpty(value)) 
				{
					throw new ArgumentException("file name is empty", "fileName");
				}
				_fileName = value;
			}
		}

		protected IParser Parser {
			get;
			private set;
		}

		#endregion
     	
		#region Methods

		public void Read()
		{
			try
            {
                Parser.Parse();
            } 
            catch (Exception ex)
            {
                throw; 
            }
		}

		public byte[] Read(ref StructureTreeNode chunkToRead)
		{
			byte[] data = new byte[chunkToRead.Length];
			long oldStreamPosition = Parser.Stream.Position;

			Parser.Stream.Seek(chunkToRead.Offset,System.IO.SeekOrigin.Begin);

			int bytesRead = Parser.Stream.Read(data,0,(int)chunkToRead.Length);

			if (bytesRead != chunkToRead.Length) {
				throw new ParseException("error reading chunk, could not get number of data to read from stream");
			}
			Parser.Stream.Seek(oldStreamPosition,System.IO.SeekOrigin.Begin);

			return data;
		}

        #region IDisposable implementation

        public void Dispose()
        {
			this.Dispose(true);
			GC.SuppressFinalize(this);
        }

        #endregion

		protected void Dispose(bool disposing)
		{
			if (disposing) {
				if (Parser != null) {
					Parser.Dispose();
				}
			}
		}
		#endregion

		#region Destructor

		~InterchangeFile()
		{
			Dispose(false);	
		}

		#endregion
        
    }
}
