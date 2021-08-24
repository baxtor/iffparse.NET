
using NUnit.Framework;
using System;
using System.IO;
using IffParse.Parser;
using IffParse.Util;

namespace CheckTests
{
	/// <summary>
	/// Original IFFCheck.c
	/// </summary>
	[TestFixture ()]
	public class IFFCheck
	{
		string [] errormsgs = {
				"End of file (not an error).",
				"End of context (not an error).",
				"No lexical scope.",
				"Insufficient memory.",
				"Stream read error.",
				"Stream write error.",
				"Stream seek error.",
				"File is corrupt.",
				"IFF syntax error.",
				"Not an IFF file.",
				"Required call-back hook missing.",
				"Return to client.  You should never see this."
			};

		/// <summary>
		/// magica voxel file (RIFF) big endian
		/// </summary>
		//string RIFF_file = "castle.vox";

		/// <summary>
		/// amiga iff ile (IFF) little endian
		/// </summary>
		string IFF_file = "sample.lbm";


		[Test ()]
		public void Check ()
		{
			CheckInternal(IFF_file,
				error => Assert.That(
					error == (int) ParserStatus.EndOfFile,
					"File scan aborted, error {0}: {1}",
					error,
					errormsgs[-error - 1])
			);
		}

		private void CheckInternal(string fileName, Action<int> assertAction)
		{
			using (var fs = new FileStream (Path.Combine (TestContext.CurrentContext.TestDirectory, fileName), FileMode.Open)) {
				using (var iffParser = new IFFParser (fs, false)) {
					var error = 0;
					while (true) {
						error = iffParser.Parse (ParseMode.RawStep);
						if (error == (int)ParserStatus.EndOfContext) {
							continue;
						} else if (error != 0) {
							break;
						}
						var top = iffParser.GetCurrentContext ();
						if (top == null) {
							continue;
						}
						for (var parent = iffParser.GetParentContext (top);
							parent != null;
							parent = iffParser.GetParentContext (parent))
							Console.Write (".");
						Console.Write ("{0} {1} ", IdUtility.IdToString (top.Id), top.Size);
						Console.Write ("{0}\n", IdUtility.IdToString (top.Type));
					}

					assertAction (error);
				}
			}
		}
	}
}
