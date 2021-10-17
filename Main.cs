using Godot;
using System.Collections;
using System.Net;
using File = Godot.File;
using Octokit;

// ReSharper disable once CheckNamespace
// ReSharper disable once ClassNeverInstantiated.Global
public abstract class Main : Control
{
	private const string ReleasesUrl = "https://api.github.com/repos/godotengine/godot/releases";
	private static readonly string SavePath = ProjectSettings.GlobalizePath("user://downloadedVersions.txt");

	public override void _Ready()
	{
		var dropDown = GetNode<OptionButton>("HBoxContainer/Version Selector");

		GD.Print("Getting Downloaded Releases");

		var versionList = new ArrayList();

		var file = new File();
		file.Open(SavePath, !file.FileExists(SavePath) ? File.ModeFlags.WriteRead : File.ModeFlags.Read);

		while (file.EofReached() == false)
		{
			versionList.Add(file.GetLine());
		}

		if (!versionList[0].Equals(""))
		{
			foreach (var i in versionList)
			{
				dropDown.AddItem(i.ToString());
				dropDown.Select(0);
			}

			dropDown.AddSeparator();
		}

		dropDown.AddItem("Loading Versions...");
		dropDown.SetItemDisabled(dropDown.GetItemCount() - 1, true);

		GD.Print("Requesting Releases");
		GetGodotReleases();
	}

	private static async void GetGodotReleases()
	{
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
		
		var client = new GitHubClient(new ProductHeaderValue("Godot-Version-Manager"));
		var releases = await client.Repository.Release.GetAll("godotengine", "godot");
		GD.Print("Request Completed");
		 foreach (var i in releases)
		 {
		 	GD.Print(i.Name);
		 }
	}
}