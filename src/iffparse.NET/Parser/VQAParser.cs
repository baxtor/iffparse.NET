using System;
using System.IO;

namespace Net.Iffparse
{

//	public class VQAParser : IParser
//	{
//		UInt32[] offsets;
//		public byte[] audioData;
//
//		public VQAParser(string fileName)
//		{
//			this.FileName = fileName;
//
//			if (!File.Exists(fileName)) 
//			{
//				throw new FileNotFoundException (string.Format ("File {0} not found", fileName));
//			}
//		}
//
//		public byte AudioBits {
//			get;
//			set;
//		}
//
//		public byte AudioChannels {
//			get;
//			set;
//		}
//
//		public ushort AudioFrequency {
//			get;
//			set;
//		}
//
//		public ushort Frames {
//			get;
//			set;
//		}
//
//		public byte FrameRate {
//			get;
//			set;
//		}
//
//		public ushort Height {
//			get;
//			set;
//		}
//
//		public ushort Width {
//			get;
//			set;
//		}
//
//		private void CollectAudioData()
//		{
//			var ms = new MemoryStream();
//			var adpcmIndex = 0;
//
//			bool compressed = false;
//			for (var i = 0; i < Frames; i++)
//			{
//				Stream.Seek(offsets[i], SeekOrigin.Begin);
//				BinaryReader reader = new BinaryReader(Stream);
//				var end = (i < Frames - 1) ? offsets[i + 1] : Stream.Length;
//				
//				while (reader.BaseStream.Position < end)
//				{
//					var type = new String(reader.ReadChars(4));
//					var length = Swap(reader.ReadUInt32());
//
//					//Result.ChildNodes.Add(new StructureTreeNode(Result,type,string.Empty,length,Stream.Position));
//
//					switch (type)
//					{
//						case "SND0":
//						case "SND2":
//							var rawAudio = reader.ReadBytes((int)length);
//							ms.Write(rawAudio,0,rawAudio.Length);
//							compressed = (type == "SND2");
//							break;
//						default:
//							reader.ReadBytes((int)length);
//							break;
//					}
//					
//					if (reader.PeekChar() == 0) reader.ReadByte();
//				}
//			}
//
//			audioData = ms.ToArray();
//		}
//
//		public static uint Swap(uint orig)
//		{
//			return (uint)((orig & 0xff000000) >> 24) | ((orig & 0x00ff0000) >> 8) | ((orig & 0x0000ff00) << 8) | ((orig & 0x000000ff) << 24);
//		}
//
//		#region IParser implementation
//
//		public string FileName {
//			get;
//			private set;
//		}
//		
//		public bool IsParsed {
//			get;
//			private set;
//		}
//		
//		public StructureTreeNode Result {
//			get;
//			private set;
//		}
//		
//		public Stream Stream {
//			get;
//			private set;
//		}
//
//		public void Parse()
//		{
//			Stream = new FileStream (FileName, FileMode.Open, FileAccess.Read);
//			BinaryReader reader = new BinaryReader( Stream );
//			
//			// Decode FORM chunk
//			if (new String(reader.ReadChars(4)) != "FORM")
//				throw new InvalidDataException("Invalid vqa (invalid FORM section)");
//			/*var length =*/ reader.ReadUInt32();
//
//			//Result = new StructureTreeNode("FORM","WVQA",0,Stream.Position);
//
//			if (new String(reader.ReadChars(8)) != "WVQAVQHD")
//				throw new InvalidDataException("Invalid vqa (not WVQAVQHD)");
//			/*length =*/ reader.ReadUInt32();
//
//			//Result.ChildNodes.Add(new StructureTreeNode(Result,"VQHD",string.Empty,0,Stream.Position));
//
//			/*var version = */reader.ReadUInt16();
//			/*var flags = */reader.ReadUInt16();
//			Frames = reader.ReadUInt16();
//			Width = reader.ReadUInt16();
//			Height = reader.ReadUInt16();
//			
//			/*blockWidth = */reader.ReadByte();
//			/*blockHeight = */reader.ReadByte();
//			FrameRate = reader.ReadByte();
//			/*cbParts = */reader.ReadByte();
//			//blocks = new int2(Width / blockWidth, Height / blockHeight);
//			
//			/*numColors =*/ reader.ReadUInt16();
//			/*var maxBlocks = */reader.ReadUInt16();
//			/*var unknown1 = */reader.ReadUInt16();
//			/*var unknown2 = */reader.ReadUInt32();
//			
//			// Audio
//			AudioFrequency = reader.ReadUInt16();
//			AudioChannels = reader.ReadByte();
//			AudioBits = reader.ReadByte();
//			/*var unknown3=*/  reader.ReadChars(12);
//			
//			
//			//var frameSize = Exts.NextPowerOf2(Math.Max(Width,Height));
//			//cbf = new byte[Width*Height];
//			//cbp = new byte[Width*Height];
//			//palette = new uint[numColors];
//			//origData = new byte[2*blocks.X*blocks.Y];
//			//frameData = new uint[frameSize,frameSize];
//			
//			var type = new String(reader.ReadChars(4));
//			if (type != "FINF")
//			{
//				reader.ReadBytes(0x94);
//				type = new String(reader.ReadChars(4));
//			}
//			
//			/*var length = */reader.ReadUInt16();
//			/*var unknown4 = */reader.ReadUInt16();
//			
//			// Frame offsets
//			offsets = new UInt32[Frames];
//			for (int i = 0; i < Frames; i++)
//			{
//				offsets[i] = reader.ReadUInt32();
//				if (offsets[i] > 0x40000000) offsets[i] -= 0x40000000;
//				offsets[i] <<= 1;
//			}
//			
//			CollectAudioData();
//			IsParsed = true;
//		}
//
//		#endregion
//
//		#region IDisposable implementation
//
//		public void Dispose()
//		{
//			Dispose(true);
//			GC.SuppressFinalize(this);
//		}
//		
//		protected void Dispose(bool disposing)
//		{
//			if (disposing) {
//				if (Stream != null) {
//					Stream.Dispose();
//				}
//				Result = null;
//			}
//		}
//
//		#endregion
//	}

}

