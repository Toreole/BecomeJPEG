# BecomeJPEG
A tiny project powered by EgmuCV (the .NET OpenCV wrapper) to capture a webcam video stream, and JPEG compress and preview it in real time.

### Features
On startup you will see two windows, one is the preview of the camera, the other is the console.
You can't do much with the preview window other than move it around.
But there's several commands you can use inside the console:

| Command | Description |
| --- | --- |
| quit | quits the application. |
| set quality 0-100 | Sets the target quality of the JPEG compression. 0 is most compressed, 100 is uncompressed. The quality is an integer. |
| set droprate 0-100 | Sets the % chance for the application to skip a frame of the webcam. 0 is none are skipped, 100 is all are skipped (essentially a freeze frame). Droprate is a floating point number. Note: depending on your region you may need to write "5.5" or "5,5" for it to use the correct value. | 
| set lagtime [millis] | Sets the fixed time for a freeze frame (dropped/skipped frame) to last. millis is an integer. |
| set lagrandom [millis] | Sets the randomized additional time for a freeze frame to last. millis is the exclusive upper bounds of the random range, and an integer. The duration for a freeze frame is given in lagtime + random(0, lagrandom]. If both are 0, there will be no additional lag on top of the skipped frame. |
| template apply [name] | Applies the template [name] settings. Templates are stored in a templates.txt file. By default there will be "worst", "medium", and "high". |
| template save [name] | Saves the current settings as a template with the specified name. Will override existing templates of that name. |
| template delete [name] | Deletes the template [name] |
| template list | Shows a list of available templates. |

### Examples 

Here's a view of the default supplied "worst" quality template. Input is given by "Creative Live! Cam Sync 1080p V2":
[Thats a lot of compression](images/worstTemplateOut.gif)