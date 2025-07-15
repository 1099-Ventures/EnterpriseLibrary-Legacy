using System;
using System.Collections.Generic;
using System.IO;
//using ICSharpCode.SharpZipLib.Checksums;
//using ICSharpCode.SharpZipLib.Zip;
//using ICSharpCode.SharpZipLib.GZip;

namespace Azuro.Common.Zip
{
	/// <summary>
	/// Summary description for SharpZip.
	/// </summary>
	[Obsolete("This class is now Obsolete. See System.IO.Compression for native .NET classes to work with Zip Files.", true)]
	public class SharpZip
	{
		/*
		static readonly int chunkSize = 4096;

		public static void UnZipFile(string ZippedFile, string UnzipLocation, string Password, out List<FileInfo> lfi)
		{
			lfi = new List<FileInfo>();
			ZipInputStream s = new ZipInputStream(File.OpenRead(ZippedFile));
			UnzipLocation = Path.GetFullPath(UnzipLocation) + @"\";
			if (!string.IsNullOrEmpty(Password))
				s.Password = Password;

			ZipEntry theEntry;
			while ((theEntry = s.GetNextEntry()) != null)
			{
				string directoryName = UnzipLocation + Path.GetDirectoryName(theEntry.Name);
				string fileName = Path.GetFileName(theEntry.Name);
				lfi.Add(new FileInfo(Path.GetFullPath(theEntry.Name)));

				// create directory
				DirectoryInfo di;
				if (directoryName.Length > 0)
				{
					di = Directory.CreateDirectory(directoryName);
				}

				if (fileName != String.Empty)
				{
					FileStream streamWriter = File.Create(UnzipLocation + fileName);

					int size = chunkSize;
					byte[] data = new byte[chunkSize];
					while (true)
					{
						size = s.Read(data, 0, data.Length);
						if (size > 0)
							streamWriter.Write(data, 0, size);
						else
							break;
					}

					streamWriter.Close();
				}
			}
			s.Close();
		}

		public static void UnZipFile(string ZippedFile, string UnzipLocation, string Password)
		{
			ZipInputStream s = new ZipInputStream(File.OpenRead(ZippedFile));
			UnzipLocation = Path.GetFullPath(UnzipLocation) + @"\";
			if (!string.IsNullOrEmpty(Password))
				s.Password = Password;

			ZipEntry theEntry;
			while ((theEntry = s.GetNextEntry()) != null)
			{
				string directoryName = UnzipLocation + Path.GetDirectoryName(theEntry.Name);
				string fileName = Path.GetFileName(theEntry.Name);

				// create directory
				DirectoryInfo di;
				if (directoryName.Length > 0)
				{
					di = Directory.CreateDirectory(directoryName);
				}

				if (fileName != String.Empty)
				{
					FileStream streamWriter = File.Create(UnzipLocation + fileName);

					int size = chunkSize;
					byte[] data = new byte[chunkSize];
					while (true)
					{
						size = s.Read(data, 0, data.Length);
						if (size > 0)
							streamWriter.Write(data, 0, size);
						else
							break;
					}

					streamWriter.Close();
				}
			}
			s.Close();
		}

		/// <summary>
		/// Unzip a file.
		/// </summary>
		/// <param name="ZippedFile">The file to unzip.</param>
		public static void UnZipFile(string ZippedFile)
		{
			UnZipFile(ZippedFile, ".", null);
		}

		public static void UnZipFile(string ZippedFile, out List<FileInfo> lfi)
		{
			UnZipFile(ZippedFile, ".", null, out lfi);
		}

		public static void UnZipFile(string ZippedFile, string UnzipLocation)
		{
			UnZipFile(ZippedFile, UnzipLocation, null);
		}

		public static void UnZipFile(string ZippedFile, string UnzipLocation, out List<FileInfo> lfi)
		{
			UnZipFile(ZippedFile, UnzipLocation, null, out lfi);
		}

		public static void ZipFile(string FileToZip)
		{
			string fileName = Path.GetFileNameWithoutExtension(FileToZip);
			SharpZip.ZipFile(FileToZip, fileName + ".zip", 6);
		}

		public static void ZipFile(string FileToZip, string ZippedFile)
		{
			SharpZip.ZipFile(FileToZip, ZippedFile, 6);
		}

		public static void ZipFile(byte[] input, string FileToZip, string ZippedFile)
		{
			SharpZip.ZipFile(input, Path.GetFileName(FileToZip), ZippedFile, 6);
		}

		public static void ZipFile(byte[] input, string FileToZip, string ZippedFile, int CompressionLevel)
		{
			using (MemoryStream StreamToZip = new MemoryStream(input))
			{
				SharpZip.ZipFile(StreamToZip, Path.GetFileName(FileToZip), ZippedFile, CompressionLevel);
			}
		}

		/// <summary>
		/// Zip a file.
		/// </summary>
		/// <param name="FileToZip">The file to zip.</param>
		/// <param name="ZippedFile">The destination file.</param>
		/// <param name="CompressionLevel">A compression specifier from 0-9, with 0 worst/fastest and 9 best/slowest. Default is 6.</param>
		public static void ZipFile(string FileToZip, string ZippedFile, int CompressionLevel)
		{
			if (!File.Exists(FileToZip))
				throw new FileNotFoundException("The specified file " + FileToZip + " could not be found. Zipping aborted");

			using (FileStream StreamToZip = new FileStream(FileToZip, FileMode.Open, FileAccess.Read))
			{
				SharpZip.ZipFile(StreamToZip, Path.GetFileName(FileToZip), ZippedFile, CompressionLevel);
			}
		}

		/// <summary>
		/// Zip a file.
		/// </summary>
		/// <param name="StreamToZip">The stream to zip.</param>
		/// <param name="FileToZip">The original filename to zip.</param>
		/// <param name="ZippedFile">The destination file.</param>
		/// <param name="CompressionLevel">A compression specifier from 0-9, with 0 worst/fastest and 9 best/slowest. Default is 6.</param>
		public static void ZipFile(Stream StreamToZip, string FileToZip, string ZippedFile, int CompressionLevel)
		{
			FileStream ZipFile = File.Create(ZippedFile);
			ZipOutputStream ZipStream = new ZipOutputStream(ZipFile);
			ZipEntry ZipEntry = new ZipEntry(Path.GetFileName(FileToZip));
			ZipStream.PutNextEntry(ZipEntry);
			ZipStream.SetLevel(CompressionLevel);
			byte[] buffer = new byte[chunkSize];
			System.Int32 size = StreamToZip.Read(buffer, 0, buffer.Length);
			ZipStream.Write(buffer, 0, size);
			try
			{
				while (size < StreamToZip.Length)
				{
					int sizeRead = StreamToZip.Read(buffer, 0, buffer.Length);
					ZipStream.Write(buffer, 0, sizeRead);
					size += sizeRead;
				}
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
			ZipStream.Finish();
			ZipStream.Close();
			StreamToZip.Close();
		}
		
		public static void ZipDirectory(string directory)
		{
			SharpZip.ZipDirectory(directory, "*.*");
		}

		public static void ZipDirectory(string directory, string searchPattern)
		{
			string dirName = Path.GetDirectoryName(directory);
			int sub = dirName.LastIndexOf(Path.DirectorySeparatorChar);
			string fileName = dirName;
			if (sub != -1)
				fileName = dirName.Substring(sub + 1);
			SharpZip.ZipDirectory(directory, searchPattern, fileName + ".zip", 6);
		}

		public static void ZipDirectory(string directory, string searchPattern, string zippedFile)
		{
			SharpZip.ZipDirectory(directory, searchPattern, zippedFile, 6);
		}

		/// <summary>
		/// Creates a zip file based on the directory given.
		/// </summary>
		/// <param name="directory">The directory to zip.</param>
		/// <param name="zippedFile">The destination zip file.</param>
		/// <param name="searchPattern">A search pattern such as *.xls.</param>
		/// <param name="compressionLevel">A compression specifier from 0-9, with 0 worst/fastest and 9 best/slowest. Default is 6.</param>
		public static void ZipDirectory(string directory, string searchPattern, string zippedFile, int compressionLevel)
		{
			ZipOutputStream s = new ZipOutputStream(File.Create(zippedFile));
			s.SetLevel(compressionLevel);

			ZipDirectory(directory, searchPattern, s);

			s.Finish();
			s.Close();
		}

		private static void ZipDirectory(string directory, string searchPattern, Stream stream)
		{
			string[] directories = Directory.GetDirectories(directory);
			foreach (string dir in directories)
			{
				ZipDirectory(dir, searchPattern, stream);
			}
			Crc32 crc = new Crc32();
			string[] filenames = Directory.GetFiles(directory, searchPattern);
			foreach (string file in filenames)
			{
				ZipOutputStream zos = (ZipOutputStream)stream;
				FileStream fs = File.OpenRead(file);

				byte[] buffer = new byte[fs.Length];
				fs.Read(buffer, 0, buffer.Length);
				ZipEntry entry = new ZipEntry(file);

				entry.DateTime = DateTime.Now;

				// set Size and the crc, because the information
				// about the size and crc should be stored in the header
				// if it is not set it is automatically written in the footer.
				// (in this case size == crc == -1 in the header)
				// Some ZIP programs have problems with zip files that don't store
				// the size and crc in the header.
				entry.Size = fs.Length;
				fs.Close();

				crc.Reset();
				crc.Update(buffer);

				entry.Crc = crc.Value;

				zos.PutNextEntry(entry);

				zos.Write(buffer, 0, buffer.Length);
			}
		}
		 */
	}
}
