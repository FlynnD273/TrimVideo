# TrimVideo

A lightweight utility for trimming videos quickly.

This project was made so I could easily cut videos to the desired start and endpoints. It's built entirely in WPF, and is essentially a wrapper for ffmpeg, which is the tool that does the actual trimming. There are lots of applications out there that can cut videos like this, but most start up a lot slower, and at least in the case of the built in Windows utility, crash a lot more. I just wanted something that makes it quick and easy to cut either individual videos or entire folders of video files. 

You can open the application normally, or open the exe from command line with the arguments `TrimVideo.exe <Path to folder>` or `TrimVideo.exe <Path to video file> <Path to video file> ... <Path to video file>`

I tried to make the UI as simple as possible. The two white markers on the timeline mark the start and end points of the trimmed video. Drag the markers or click and drag on the outside of the range to adjust the trimmed video range.
Press CTRL+S to export the video. You must have ffmpeg added to your PATH variable in order for it to export. 
If you opened a folder, the program will close, and reopen with the next video in the folder. This will repeat until you close the program or run out of videos in the folder. 
Saved files will be written to the same folder as the original files, and will have the pattern "_FILENAME_Trim.EXT".
