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