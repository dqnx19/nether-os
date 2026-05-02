# TODO: Fix Explorer Build Errors

## Steps:
- [x] 1. Edit MainWindow.xaml.cs to fix syntax errors in FileContextMenu_Opened method (remove duplicates, fix if-else braces).
- [x] 2. Run `cd e:/projects-git/nether-os/source-codes/explorer && dotnet build` to verify no errors.
  - Syntax errors fixed in MainWindow.xaml.cs.
  - Added missing ExtractHereMenuItem to ContextMenu in MainWindow.xaml.
  - Fixed selectedItem usage.
- [ ] 3. Run `dotnet run` to test the app.

