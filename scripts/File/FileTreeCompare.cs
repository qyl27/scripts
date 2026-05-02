#!/usr/bin/env dotnet

using System.Security.Cryptography;

if (args.Length != 2)
{
    Console.WriteLine("Compare all files in 2 directories.");
    Console.WriteLine("Usage: dotnet DirectoryCompare.cs <Path A> <Path B>");
    return;
}

var dirA = args[0];
var dirB = args[1];

var hashA = GetFileHashes(dirA);
var hashB = GetFileHashes(dirB);

Dictionary<string, (string? HashA, string? HashB)> matched = new();

foreach (var (path, hash) in hashA)
{
    matched[path] = (hash, null);
}

foreach (var (path, hash) in hashB)
{
    if (!matched.TryGetValue(path, out var result))
    {
        matched[path] = (null, hash);
        continue;
    }

    var (resultA, _) = result;
    matched[path] = (resultA, hash);
}

Console.WriteLine("<Compare Result> <Relative Path>");
Console.WriteLine("Result =: Exists in both A and B, hash matched");
Console.WriteLine("Result *: Exists in both A and B, but hash mismatch");
Console.WriteLine("Result A: Exists in A only");
Console.WriteLine("Result B: Exists in B only");

Dictionary<string, List<string>> results = new()
{
    { "=", [] },
    { "*", [] },
    { "A", [] },
    { "B", [] },
};

foreach (var (path, tuple) in matched)
{
    if (tuple.HashA != null)
    {
        if (tuple.HashB != null)
        {
            if (tuple.HashA == tuple.HashB)
            {
                results["="].Add(path);
            }
            else
            {
                results["*"].Add(path);
            }
        }
        else
        {
            results["A"].Add(path);
        }
    }
    else
    {
        results["B"].Add(path);
    }
}

foreach (var (_, list) in results)
{
    list.Sort();
}

foreach (var (res, list) in results)
{
    foreach (var path in list)
    {
        Console.WriteLine($"{res} {path}");
    }
}

return;

Dictionary<string, string> GetFileHashes(string path)
{
    return Directory.GetDirectories(path, "*", SearchOption.AllDirectories)
        .Concat([path])
        .AsParallel()
        .SelectMany(Directory.GetFiles)
        .ToDictionary(f => Path.GetRelativePath(path, f), f => Sha512(f).Result);
}

async Task<string> Sha512(string file)
{
    await using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
    using var sha512 = SHA512.Create();
    var bytes = await sha512.ComputeHashAsync(fileStream);
    return Convert.ToHexString(bytes);
}