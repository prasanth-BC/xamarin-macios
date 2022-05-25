using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

using Xamarin;

namespace Microsoft.MaciOS.Nnyeah.Tests.SmokeTests {
	[TestFixture]
	public class CompileALibrary {
		[Test]
		public async Task BasicLibrary ()
		{
			var dir = Cache.CreateTemporaryDirectory ("BasicLibrary");
			var code = @"
using System;
public class Foo {
	public nint Ident (nint e) => e;
}
";
			var output = await TestRunning.BuildLibrary (code, "NoName", dir);
			var expectedOutputFile = Path.Combine (dir, "NoName.dll");
			Assert.IsTrue (File.Exists (expectedOutputFile));
		}

		[Test]
		public void BasicExecutable ()
		{
			var dir = Cache.CreateTemporaryDirectory ("BasicExecutable");
		}

		[Test]
		[Ignore ("This test was failing. See this issue: https://github.com/xamarin/xamarin-macios/issues/15120")]
		public async Task LibraryWithXamarinReference ()
		{
			var dir = Cache.CreateTemporaryDirectory ("LibraryWithXamarinReference");
			var code = @"
using System;
using Foundation;

public class Foo {
	public bool IsStaleHandle (NSObject o) => o.Handle != IntPtr.Zero;
}
";
			await TestRunning.BuildLibrary (code, "NoName", dir);
			var libraryFile = Path.Combine (dir, "NoName.dll");
			Assert.IsTrue (File.Exists (libraryFile));

			var convertedFile = Path.Combine (dir, "NoName-Converted.dll");
			Program.ProcessAssembly (Compiler.XamarinPlatformLibraryPath (PlatformName.macOS), Compiler.MicrosoftPlatformLibraryPath (PlatformName.macOS), libraryFile, convertedFile, true, true, false);
		}

		[Test]
		public void HasXamarinMacOSFile ()
		{
			var xamarinDll = Compiler.XamarinPlatformLibraryPath (PlatformName.macOS);
			Assert.IsTrue (File.Exists (xamarinDll), "Xamarin file doesn't exist");
		}

		[Test]
		public void HasMicrosoftMacOSFile ()
		{
			var microsoftDll = Compiler.MicrosoftPlatformLibraryPath (PlatformName.macOS);
			Assert.IsTrue (File.Exists (microsoftDll));
		}
	}
}
