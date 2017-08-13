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
			Console.Title = "Newgrounds歌曲下载器";
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine("感谢使用由WEGFan制作的Newgrounds歌曲下载器!\n");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("输入歌曲的编号或艺术家名称来获取下载地址。 (例如： '233333 f-777 666666 robtop)\n输入 '/a' 来复制所有的地址到剪切板, 输入 '/c' 来清除列表.");

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
								Console.WriteLine("列表是空的");
							}
							else
							{
								string cb = "";
								foreach (string url in urlList)
									cb += url + "\n";
								Clipboard.SetText(cb);
								Console.ForegroundColor = ConsoleColor.Yellow;
								Console.WriteLine(urlList.Count + "个地址已复制到剪切板");
							}
							break;
						}

					case "/c":
						{
							urlList.Clear();
							Console.ForegroundColor = ConsoleColor.Yellow;
							Console.WriteLine("完成!");
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
					Console.WriteLine("无法为编号 " + songID + "找到歌曲. 此歌曲可能已经被移除.");
				}
			}
			catch (WebException e)
			{
				if (e.Status == WebExceptionStatus.Timeout)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("连接超时.");
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
				Console.WriteLine("无法找到艺术家" + author + "的歌曲. 这可能是因为输入错误，或这个艺术家暂未上传任何歌曲");
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