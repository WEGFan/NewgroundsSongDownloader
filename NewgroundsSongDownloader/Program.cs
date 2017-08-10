using System;
using System.Collections;
using System.Net;
using System.Text;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace NewgroundsSongDownloader
{
	class Program
	{
		public static ArrayList urlList = new ArrayList();
		public static string songUrl;

		[STAThread]
		static void Main(string[] args)
		{
			Console.Title = "Newgrounds Song Downloader";
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine("Thanks for using Newgrounds Song Downloader by WEGFan!\n");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Enter ID of the songs and the authors to get the download URL. (e.g. '233333 f-777 666666 robtop)\nType '/a' to copy all URLs to the clipboard, type '/c' to clear the list.");

			string input;

			while (true)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("\n> ");
				input = Console.ReadLine();

				switch (input)
				{
					case "/a":
						{
							if (urlList.Count == 0)
							{
								Console.ForegroundColor = ConsoleColor.Red;
								Console.WriteLine("The list is empty!");
							}
							else
							{
								string cb = "";
								foreach (string url in urlList)
									cb += url + "\n";
								Clipboard.SetText(cb);
								Console.ForegroundColor = ConsoleColor.Yellow;
								Console.WriteLine(urlList.Count + " URLs copied to clipboard.");
							}
							break;
						}

					case "/c":
						{
							urlList.Clear();
							Console.ForegroundColor = ConsoleColor.Yellow;
							Console.WriteLine("Done!");
							break;
						}

					default:
						foreach (string i in input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
						{
							if (IsNumber(i))
								GetUrl(Convert.ToInt32(i));
							else
								GetUrlByAuthor(i);
						}
						break;
				}
			}
		}

		static void GetUrl(int songID)
		{
			HtmlWeb web = new HtmlWeb();
			web.OverrideEncoding = Encoding.UTF8;
			web.PreRequest = delegate (HttpWebRequest webRequest)
			{
				webRequest.Timeout = 5000;
				return true;
			};
			try
			{
				HtmlAgilityPack.HtmlDocument doc = web.Load("http://www.newgrounds.com/audio/listen/" + songID);
				var hNode = doc.DocumentNode.SelectNodes("/html/body/div[3]/div[3]/div[1]/script[5]");
				try
				{
					songUrl = hNode[0].InnerText.Split(new string[] { "\":\"", "\",\"" }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("\\", "");
					urlList.Add(songUrl);
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine(songUrl);
				}
				catch
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Can't find url for " + songID + ". The song may be removed.");
				}
			}
			catch (WebException e)
			{
				if (e.Status == WebExceptionStatus.Timeout)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Timeout.");
				}
			}
		}

		static void GetUrlByAuthor(string author)
		{
			HtmlWeb web = new HtmlWeb();
			web.OverrideEncoding = Encoding.UTF8;
			try
			{
				HtmlAgilityPack.HtmlDocument doc = web.Load("http://" + author + ".newgrounds.com/audio/");
				var hNode = doc.DocumentNode.SelectNodes("//table[@class='audiolist']/tr/td/a");
				for (int i = 0; i < hNode.Count; i++)
				{
					GetUrl(Convert.ToInt32(hNode[i].OuterHtml.Split(new string[] { "/listen/", "\">" }, StringSplitOptions.RemoveEmptyEntries)[1]));
				}
			}
			catch
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Can't find songs by " + author + ". Please check your input or the author hasn't uploaded any songs yet.");
			}
		}

		static bool IsNumber(string input)
		{
			try
			{
				Convert.ToInt32(input);
			}
			catch
			{
				return false;
			}
			return true;
		}
	}
}