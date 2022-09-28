# BecomeJPEG
A tiny project powered by EgmuCV (the .NET OpenCV wrapper) to capture a webcam video stream, and JPEG compress and preview it in real time.

### Features

Upon startup, you will be presented with the Settings Panel. This is a small comprehensive window to configure the settings of the application even before it first activates your webcam.

Here's a gif showing that changing templates has become much easier as opposed to how it was in the console (no more typing, just double click one!).

![Changing Templates in GUI](images/change_template.gif)

As before, saving a template with the active settings will override old templates of the same name.

The GUI also features a small black textbox for logging. The same messages will also be saved in a log.txt next to the application (similarly to the templates.txt).

### Examples 

Here's a view of the default supplied "worst" quality template. Input is given by "Creative Live! Cam Sync 1080p V2":
![Thats a lot of compression](images/worstTemplateOut.gif)

### Notes

Depending on your CPU and the target resolution, it will demand a lot. So keeping the capture resolution low is recommended (configuration of this is a planned feature ATM).

The Settings Panel is made with basic WinForms, just because i don't really plan on making it super fancy, it should just get things done for now. It can be improved later if need be.