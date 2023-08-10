// Paste.exe returns text from the clipboard.
// Copyright( C ) 2023 Timothy J. Bruce

/*
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

namespace Icod.Paste {

	public static class Program {

		private const System.Int32 theBufferSize = 16384;


		[System.STAThread]
		public static System.Int32 Main( System.String[] args ) {
			if ( null == args ) {
				args = new System.String[ 0 ];
			}
			var len = args.Length;
			if ( 4 < len ) {
				PrintUsage();
				return 1;
			}
			System.String? outputPathName = null;
			if ( 0 < len ) {
				len = len - 1;
				System.String @switch;
				System.Int32 i = -1;
				do {
					@switch = args[ ++i ];
					if ( new System.String[] { "--help", "-h", "/h" }.Contains( @switch, System.StringComparer.OrdinalIgnoreCase ) ) {
						PrintUsage();
						return 1;
					} else if ( new System.String[] { "--copyright", "-c", "/c" }.Contains( @switch, System.StringComparer.OrdinalIgnoreCase ) ) {
						PrintCopyright();
						return 1;
					} else if ( "--output".Equals( @switch, System.StringComparison.OrdinalIgnoreCase ) ) {
						outputPathName = args[ ++i ].TrimToNull();
					} else {
						PrintUsage();
						return 1;
					}
				} while ( i < len );
			}

			System.Action<System.String?, System.Collections.Generic.IEnumerable<System.String>> writer;
			if ( System.String.IsNullOrEmpty( outputPathName ) ) {
				writer = ( a, b ) => WriteStdOut( b );
			} else {
				writer = ( a, b ) => WriteFile( a, b );
			}
			var text = new TextCopy.Clipboard().GetText() ?? System.String.Empty;
			writer( outputPathName, new System.String[ 1 ] { text } );
			return 0;
		}

		private static void PrintUsage() {
			System.Console.Error.WriteLine( "No, no, no! Use it like this, Einstein:" );
			System.Console.Error.WriteLine( "Paste.exe --help" );
			System.Console.Error.WriteLine( "Paste.exe [--output outputFilePathName]" );
			System.Console.Error.WriteLine( "Paste.exe returns text from the clipboard." );
			System.Console.Error.WriteLine( "outputFilePathName may be relative or absolute path." );
			System.Console.Error.WriteLine( "If outputFilePathName is omitted then output is written to StdOut." );
		}

		private static void PrintCopyright() {
			var copy = new System.String[] {
				"Paste.exe returns text from the clipboard.",
				"",
				"Copyright( C ) 2023 Timothy J. Bruce",
				"",
				"This program is free software: you can redistribute it and / or modify",
				"it under the terms of the GNU General Public License as published by",
				"the Free Software Foundation, either version 3 of the License, or",
				"( at your option ) any later version.",
				"",
				"",
				"This program is distributed in the hope that it will be useful,",
				"but WITHOUT ANY WARRANTY; without even the implied warranty of",
				"",
				"MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the",
				"",
				"GNU General Public License for more details.",
				"",
				"",
				"You should have received a copy of the GNU General Public License",
				"",
				"along with this program.If not, see < https://www.gnu.org/licenses/>."
			};
			foreach ( var line in copy ) {
				System.Console.WriteLine( line );
			}
		}

		#region io
		private static void WriteStdOut( System.Collections.Generic.IEnumerable<System.String> data ) {
			foreach ( var datum in data ) {
				System.Console.Out.WriteLine( datum );
			}
		}
		private static void WriteFile( System.String? filePathName, System.Collections.Generic.IEnumerable<System.String> data ) {
			filePathName = filePathName?.TrimToNull();
			if ( System.String.IsNullOrEmpty( filePathName ) ) {
				throw new System.ArgumentNullException( nameof( filePathName ) );
			}
			using ( var file = System.IO.File.Open( filePathName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write, System.IO.FileShare.None ) ) {
				file.Seek( 0, System.IO.SeekOrigin.Begin );
				using ( var writer = new System.IO.StreamWriter( file, System.Text.Encoding.UTF8, theBufferSize, true ) ) {
					foreach ( var datum in data ) {
						writer.WriteLine( datum );
					}
					writer.Flush();
					writer.Close();
				}
				file.Flush();
				file.SetLength( file.Position );
				file.Close();
			}
		}
		#endregion io

		private static System.String? TrimToNull( this System.String @string ) {
			if ( System.String.IsNullOrEmpty( @string ) ) {
				return null;
			}
			@string = @string.Trim();
			return System.String.IsNullOrEmpty( @string )
				? null
				: @string
			;
		}

	}

}