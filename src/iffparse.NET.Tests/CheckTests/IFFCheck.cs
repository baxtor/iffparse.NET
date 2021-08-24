using System;
using System.IO;
using IffParse.Parser;
using IffParse.Util;
using NUnit.Framework;

namespace CheckTests
{
	/// <summary>
	/// Original IFFCheck.c
	/// </summary>
	[TestFixture]
	public class IFFCheck
	{
		private readonly string[] _errorMessages =
		{
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
		/// amiga iff file (IFF) little endian
		/// </summary>
		private readonly string _SampleFile = "sample.lbm";

		[Test]
		public void Check()
		{
			CheckInternal(_SampleFile,
				error => Assert.That(
					error == (int)ParserStatus.EndOfFile,
					"File scan aborted, error {0}: {1}",
					error,
					_errorMessages[-error - 1])
			);
		}

		private void CheckInternal(string fileName, Action<int> assertAction)
		{
			using (var fs = new FileStream(Path.Combine(TestContext.CurrentContext.TestDirectory, fileName),FileMode.Open))
			{
				using (var iffParser = new IFFParser(fs, false))
				{
					var error = 0;
					while (true)
					{
						error = iffParser.Parse(ParseMode.RawStep);
						if (error == (int)ParserStatus.EndOfContext)
						{
							continue;
						}
						else if (error != 0)
						{
							break;
						}

						var top = iffParser.GetCurrentContext();
						if (top == null)
						{
							continue;
						}

						for (var parent = iffParser.GetParentContext(top);
							parent != null;
							parent = iffParser.GetParentContext(parent))
							Console.Write(".");
						Console.Write("{0} {1} ", IdUtility.IdToString(top.Id), top.Size);
						Console.Write("{0}\n", IdUtility.IdToString(top.Type));
					}

					assertAction(error);
				}
			}
		}
	}
}
