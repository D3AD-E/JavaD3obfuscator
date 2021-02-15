# JavaD3obfuscator
>.NET 5.0
### Are you tired of nasty guys that put files with names that windows cannot handle(ex. con, com, nul) into zip or jar archives? Me too, that is why I created this app, that will automatically fix an archive for you!
## Features
- Console app
- Auto internal files references fixes
- Save or load config to save the same replacements in multiple files
- Logs will let you know what happened during the process
- Error logger will provide you with all information on fails
- Command line arguments

## Dependencies
- PowerArgs
## Params
- -?          Shows help
- -p          Path to file to be deobfuscated, can also be provided as the 1st argument, without -p
- -spath      Path to config to be saved, by default is ReplacedConfig.txt
- -lpath      Path to config to be loaded, by default is ReplacedConfig.txt
- -s          If provided, saves config, -spath will not work without this parameter
- -l          If provided, loads config, -lpath will not work without this parameter
- -dj         Disable internal file reference fixes
## Screenshots
![alt text](https://github.com/D3AD-E/JavaD3obfuscator/blob/master/Img1.png?raw=true)
