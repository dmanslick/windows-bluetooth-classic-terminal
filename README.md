# Windows Bluetooth Classic Terminal

This is a basic terminal made using WinForms to allow sending of plain text data to a device using Bluetooth Classic

## NOTE

To build this project in a way that includes all dependencies, run the following command:

```
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

The contents of the "/bin/Release/net8.0-windows/win-x64/publish" directory are all that are needed to run this app once built (so zip that folder to distribute this)
