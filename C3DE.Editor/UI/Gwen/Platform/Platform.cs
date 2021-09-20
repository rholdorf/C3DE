using System;
using System.Collections.Generic;
using System.IO;

namespace Gwen.Platform
{
	public class Platform
	{
		private static IPlatform m_Platform = null;

		/// <summary>
		/// Set the current platform.
		/// </summary>
		/// <param name="platform">Platform.</param>
		public static void Init(IPlatform platform)
		{
			m_Platform = platform;
		}

		/// <summary>
		/// Gets text from clipboard.
		/// </summary>
		/// <returns>Clipboard text.</returns>
		public static string GetClipboardText()
		{
			return null;
		}

		/// <summary>
		/// Sets the clipboard text.
		/// </summary>
		/// <param name="text">Text to set.</param>
		/// <returns>True if succeeded.</returns>
		public static bool SetClipboardText(string text)
		{
			return true;
		}


		/// <summary>
		/// Gets elapsed time since this class was initalized.
		/// </summary>
		/// <returns>Time interval in seconds.</returns>
		public static float GetTimeInSeconds()
		{
			return m_Platform.GetTimeInSeconds();
		}

		/// <summary>
		/// Changes the mouse cursor.
		/// </summary>
		/// <param name="cursor">Cursor type.</param>
		public static void SetCursor(Cursor cursor)
		{
			m_Platform.SetCursor(cursor);
		}

		/// <summary>
		/// Get special folders of the system.
		/// </summary>
		/// <returns>List of folders.</returns>
		public static IEnumerable<ISpecialFolder> GetSpecialFolders()
		{
			return m_Platform.GetSpecialFolders();
		}

		public static string GetFileName(string path)
		{
			return m_Platform.GetFileName(path);
		}

		public static string GetDirectoryName(string path)
		{
			return m_Platform.GetDirectoryName(path);
		}

		public static bool FileExists(string path)
		{
			return m_Platform.FileExists(path);
		}

		public static bool DirectoryExists(string path)
		{
			return m_Platform.DirectoryExists(path);
		}

		public static void CreateDirectory(string path)
		{
			m_Platform.CreateDirectory(path);
		}

		public static string Combine(string path1, string path2)
		{
			return m_Platform.Combine(path1, path2);
		}

		public static string Combine(string path1, string path2, string path3)
		{
			return m_Platform.Combine(path1, path2, path3);
		}

		public static string Combine(string path1, string path2, string path3, string path4)
		{
			return m_Platform.Combine(path1, path2, path3, path4);
		}

		public static string CurrentDirectory
		{
			get
			{
				return m_Platform.CurrentDirectory;
			}
		}

		public static IEnumerable<IFileSystemDirectoryInfo> GetDirectories(string path)
		{
			return m_Platform.GetDirectories(path);
		}

		public static IEnumerable<IFileSystemFileInfo> GetFiles(string path, string filter)
		{
			return m_Platform.GetFiles(path, filter);
		}

		public static Stream GetFileStream(string path, bool isWritable)
		{
			return m_Platform.GetFileStream(path, isWritable);
		}
	}
}
