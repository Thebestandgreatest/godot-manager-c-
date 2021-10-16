extends Control

#HACK refactor this mess of code

var versionName
var jsonData

onready var dropDown = $"HBoxContainer/Version Selector"
onready var versionRequest = $"Versions Request"


func _ready():
	#Requests the released versions of Godot from github api
	print("Requesting Releases")
	#TODO: save downloaded version names to a file for quick reference
	
	#TODO: have a downloaded and not downloaded versions section to the dropdown
	$"Versions Request".request("https://api.github.com/repos/godotengine/godot/releases")

func _on_HTTPRequest_request_completed(_result, response_code, _headers, body):
	# after the request completes, parse and check the JSON, then populate the list
	print("Request Complete")
	if response_code != 200:
		push_error("Unable to get JSON data error %d" % [response_code])
		return
	jsonData = JSON.parse(body.get_string_from_utf8())
	
	if typeof(jsonData.result) != TYPE_ARRAY || jsonData.error != OK:
		push_error("JSON data malformed")
		return
	
	for i in jsonData.result.size():
		dropDown.add_item(jsonData.result[i].name)
	dropDown.select(0)
	versionName = dropDown.get_item_text(0)

func _on_Version_Selector_item_selected(index):
	#FIXME: can't decide if this is a useless function
	#could just do dropDown.get_item_text(dropDown.get_selected())
	versionName = dropDown.get_item_text(index)

func _on_Launch_Version_pressed():
	#checks if the requested version is already downloaded or not
	#launches it if it is, and confirms download if it isn't
	#FIXME: check for OS verison and download the correct file for the OS version (windows/linux/mac)
	var file = File.new()
	if file.file_exists("user://%s.exe" % [versionName]):
		var output = OS.execute(ProjectSettings.globalize_path("user://%s" % [versionName]), ["-p"], true)
		print(output)
	else:
		$"Confirm Download".set_text("Are you sure you want to download %s?" % [versionName])
		$"Confirm Download".popup_centered()

func _on_ConfirmationDialog_confirmed():
	#self explanatory
	#calls the download function when the accept dialog is confirmed
	print("confirmed download")
	downloadFile(versionName)

func downloadFile(version):
	#downloads the specified version of Godot using a cURL command by 
	#TODO: test using httpRequest to make the solution completely cross-platform
	
	for i in range(jsonData.result.size()):
		if jsonData.result[i].name == version:
			#godot httpRequest to download the release asset
			$"Version Download".set_download_file(ProjectSettings.globalize_path("user://%s.exe.zip" % [version]))
			$"Version Download".request("http://docs.godotengine.org/en/stable/_static/docs_logo.png")
			
			pass
			#cURL command to download the release asset
			#var output = OS.execute('curl', [str("%s" % [jsonData.result[i].assets[20].browser_download_url]), "-L", "-O", '"', ProjectSettings.globalize_path("user://%s.exe.zip" % [version]), '"'], true, [])
			#print(output)

func _on_Version_Download_request_completed(result, response_code, _headers, _body):
	print("Request Complete ", result, ", ", response_code)
