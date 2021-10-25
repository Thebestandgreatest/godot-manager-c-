using Godot;
using System.Collections;
using System.Net;
using File = Godot.File;
using Octokit;

// ReSharper disable once CheckNamespace
// ReSharper disable once ClassNeverInstantiated.Global
public abstract class Main : Control
{
	private static readonly string SavePath = ProjectSettings.GlobalizePath("user://downloadedVersions.txt");
	private static readonly string TokenPath = ProjectSettings.GlobalizePath("user://tokenPath.txt");
	private static string GithubToken;

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
		
		file.Close();
		file.Open(TokenPath, File.ModeFlags.Read);
		GithubToken = file.GetAsText();
		

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
		var client = new GitHubClient(new ProductHeaderValue("GodotVersionManager"))
		{
			Credentials = new Credentials(GithubToken)
		};
	}
}