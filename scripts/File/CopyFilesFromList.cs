#!/usr/bin/env dotnet

if (args.Length != 3)
{
    Console.WriteLine("Copy files relative to source directory to destination direction by a list of text.");
    Console.WriteLine("Usage: dotnet CopyFilesFromList.cs <File List> <Source Directory> <Destination Directory>");
    return;
}

var fileList = args[0];
var sourceDir = args[1];
var destinationDir = args[2];

if (!File.Exists(fileList))
{
    Console.WriteLine($"File list file {fileList} doesn't exist.");
    return;
}

if (!Directory.Exists(sourceDir))
{
    Console.WriteLine($"Source directory {sourceDir} doesn't exist.");
    return;
}

if (!Directory.Exists(destinationDir))
{
    Directory.CreateDirectory(destinationDir);
}

var files = File.ReadAllLines(fileList);
files.AsParallel().ForAll(file =>
{
    var relativePath = Path.GetRelativePath(sourceDir, file);
    var targetPath = Path.GetFullPath(relativePath, destinationDir);
    File.Copy(file, targetPath);
});
Console.WriteLine($"Copied {files.Length} files.");