The BamlDump tool is a copy of the tool at \wpf\src\tools\bamldump

Several reasons for the copy to exist:
- Need to remove the custom proj references that were causing product binaries to be built
- Faster turnaround for test modifications
- Loc tests will likely take a dependency; hence good to have the tool as part of the test tree

