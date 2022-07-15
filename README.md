# TrimVideo
------

A lightweight utility for trimming videos quickly.

Open the exe from command line with the arguments `TrimVideo.exe <Path to folder>` or `TrimVideo.exe <Path to video file> <Path to video file> ... <Path to video file>`

I tried to make the UI as simple as possible. The two green markers on the timeline mark the start and end points of the trimmed video. Drag the markers or click and drag on the outside of the range to adjust the trimmed video range. The black marker represents the playhead.
Press CTRL+S to export the video. You must have ffmpeg added to your PATH variable in order for it to export correctly. 
If you opened a folder, the program will close, and reopen with the next video in the folder. This will repeat until you close the program or run out of videos in teh folder. 
Saved files will be written to the same folder as the original files, and will have the pattern "_FILENAME_Trim.EXT"

At some point I might add a way to drag and drop in files, but that's not implemented yet. I might also improve the UI a little bit at some point...
