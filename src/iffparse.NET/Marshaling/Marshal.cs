using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;

namespace Net.Iffparse.Marshaling
{
	[CLSCompliant(true)]
	public static class Marshal
	{
		public static int SizeOf<T>()
		{
			return SizeOf (typeof(T));
		}

		public static int SizeOf(Type t)
		{
			if (t == null)
				throw new ArgumentNullException("t");
			var ti = t.GetTypeInfo ();
			if (!ti.IsPrimitive && !ti.IsValueType)
				throw new ArgumentException("Argument must be a value type.", "t");
			if (ti.IsGenericType)
				throw new ArgumentException("Only non generic types are supported.", "t");

			if (typeof(Byte) == t)
				return sizeof(Byte);
			if (typeof(SByte) == t)
				return sizeof(SByte);
			if (typeof(Int16) == t)
				return sizeof(Int16);
			if (typeof(UInt16) == t)
				return sizeof(UInt16);
			if (typeof(Int32) == t)
				return sizeof(Int32);
			if (typeof(UInt32) == t)
				return sizeof(UInt32);
			if (typeof(Int64) == t)
				return sizeof(Int64);
			if (typeof(UInt64) == t)
				return sizeof(UInt64);
			if (typeof(Single) == t)
				return sizeof(Single);
			if (typeof(Double) == t)
				return sizeof(Double);
			if (typeof(Decimal) == t)
				return sizeof(Decimal);
			
			var tmi = GetStructTree (t);
			return tmi.Size;
		}

		public static T BytesToStruct<T>(byte[] buffer) where T : struct
		{
			// 1. through reflection get Marshal.StructLayout Attribute
			// if not found > exception
			// 2. iterate through every public field/property marked with Marshal.FieldOffsetAttribute
			// save them in temporary dictionary
			// 2.1 if field type is another struct go to 1. recursively
			// 3. order temp dict by offset
			// 4. create struct type
			// 5. iterate over the temp dict and read each compatible type from stream using ReadByte, ReadInt and so on,
			//    and assign value to struct.field
			var structInfoTree = GetStructTree<T>();
			if (structInfoTree.HasChildren) {
				object structValue = Activator.CreateInstance<T> ();
				var typeInfo = typeof(T).GetTypeInfo ();
				using(var ms = new MemoryStream(buffer,false)) {
					using (var br = new BinaryReader (ms)) {
						foreach (var memberInfo in structInfoTree.Children) {
							var fieldInfo = typeInfo.GetDeclaredField (memberInfo.FieldName);
							var fieldValue = ReadValueByType (memberInfo.FieldType, br);
							fieldInfo.SetValue (structValue, fieldValue);
						}
					}
				}
				return (T)structValue;
			}
			return default(T);
		}

		public static byte[] StructToBytes<T>(T structType) where T : struct
		{
			// see BytesToStruct in reverse order
			//AdjustEndianness(typeof(T),new byte[0]);
			return new byte[0];
		}

		private static object ReadValueByType(Type type,  BinaryReader br)
		{
			if (typeof(Byte) == type)
				return br.ReadByte ();
			if (typeof(SByte) == type)
				return br.ReadSByte ();
			if (typeof(Int16) == type)
				return ReadInt16 (br);
			if (typeof(UInt16) == type)
				return ReadUInt16 (br);
			if (typeof(Int32) == type)
				return ReadInt32 (br);
			if (typeof(UInt32) == type)
				return ReadUInt32 (br);
			if (typeof(Int64) == type)
				return ReadInt64 (br);
			if (typeof(UInt64) == type)
				return ReadUInt64 (br);
			if (typeof(Single) == type)
				return br.ReadSingle ();
			if (typeof(Double) == type)
				return br.ReadDouble ();
			if (typeof(Decimal) == type)
				return br.ReadDecimal ();
			return new object();
		}

		private static Int16 ReadInt16(BinaryReader br){
			Int16 value = br.ReadInt16 ();
			if (!BitConverter.IsLittleEndian) {
				return value;
			}
			var buffer = BitConverter.GetBytes(value);
			Array.Reverse(buffer);
			value = BitConverter.ToInt16(buffer, 0);
			return value;
		}

		private static UInt16 ReadUInt16(BinaryReader br){
			UInt16 value = br.ReadUInt16 ();
			if (!BitConverter.IsLittleEndian) {
				return value;
			}
			var buffer = BitConverter.GetBytes(value);
			Array.Reverse(buffer);
			value = BitConverter.ToUInt16(buffer, 0);
			return value;
		}

		private static Int32 ReadInt32(BinaryReader br){
			Int32 value = br.ReadInt32 ();
			if (!BitConverter.IsLittleEndian) {
				return value;
			}
			var buffer = BitConverter.GetBytes(value);
			Array.Reverse(buffer);
			value = BitConverter.ToInt32(buffer, 0);
			return value;
		}

		private static UInt32 ReadUInt32(BinaryReader br){
			UInt32 value = br.ReadUInt32 ();
			if (!BitConverter.IsLittleEndian) {
				return value;
			}
			var buffer = BitConverter.GetBytes(value);
			Array.Reverse(buffer);
			value = BitConverter.ToUInt32(buffer, 0);
			return value;
		}

		private static Int64 ReadInt64(BinaryReader br){
			Int64 value = br.ReadInt64 ();
			if (!BitConverter.IsLittleEndian) {
				return value;
			}
			var buffer = BitConverter.GetBytes(value);
			Array.Reverse(buffer);
			value = BitConverter.ToInt64(buffer, 0);
			return value;
		}

		private static UInt64 ReadUInt64(BinaryReader br){
			UInt64 value = br.ReadUInt64 ();
			if (!BitConverter.IsLittleEndian) {
				return value;
			}
			var buffer = BitConverter.GetBytes(value);
			Array.Reverse(buffer);
			value = BitConverter.ToUInt64(buffer, 0);
			return value;
		}

		private static MemberInfoNode GetStructTree(Type t)
		{
			if (t == null)
				throw new ArgumentNullException ("t");
			
			var ti = t.GetTypeInfo ();

			if (ti.IsClass || ti.IsGenericType || ti.IsInterface || ti.IsPrimitive || ti.IsEnum)
				throw new ArgumentException ("Only struct are allowed");
			var memberTree = new MemberInfoNode ();
			var offset = 0;

			var isExplicitLayout = !ti.IsAutoLayout || ti.IsExplicitLayout;
			foreach (var fieldInfo in ti.DeclaredFields) {
				var childInfo = new MemberInfoNode ();
				childInfo.FieldName = fieldInfo.Name;
				childInfo.FieldType = fieldInfo.FieldType;
				if (fieldInfo.IsStatic) {
					throw new InvalidOperationException ("Struct type must not contain static fields " + fieldInfo.Name);
				} else if (isExplicitLayout) {
					// get fieldOffset
					var foa = fieldInfo.GetCustomAttribute<FieldOffsetAttribute>(true);
					if (foa == null)
						throw new InvalidOperationException ("FieldOffsetAttribute has to be declared on struct type with layout explicit");
					var fieldOffset = foa.Value;
					childInfo.Offset = fieldOffset;
					childInfo.Size = SizeOf (fieldInfo.FieldType);
					memberTree.Children.Add (childInfo);
				} else {
					var size = SizeOf (fieldInfo.FieldType);
					childInfo.Offset = offset;
					childInfo.Size = size;
					memberTree.Children.Add (childInfo);
					offset += size;

				}
			}

			return memberTree;
		}

		private static MemberInfoNode GetStructTree<T>() where T : struct
		{
			return GetStructTree (typeof(T));
		}

		internal class MemberInfoNode
		{
			public MemberInfoNode ()
			{
				Children = new System.Collections.Generic.List<MemberInfoNode>();
			}

			public MemberInfoNode Parent {
				get;
				set;
			}

			public ICollection<MemberInfoNode> Children {
				get;
				private set;
			}

			public bool HasChildren {
				get {
					return Children.Count > 0;
				}
			}

			public int Offset {
				get;
				set;
			}

			public Type FieldType {
				get;
				set;
			}

			public string FieldName {
				get;
				set;
			}

			int size;
			public int Size {
				get {
					if (HasChildren)
						return Children.Aggregate (0, (i, c) => i += c.Size);
					return size;
				}
				set {
					size = value;
				}
			}
		}
	} 
}

