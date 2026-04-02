#!/usr/bin/env dotnet

#:package System.IO.Hashing@10.0.5

// Calculate file id in Mix format for a given file name.
// Mix format: https://xhp.xwis.net/documents/MIX_Format.html

using System.IO.Hashing;
using System.Numerics;
using System.Text;
using FileId = uint;

if (args.Length != 2)
{
    Console.WriteLine("Calculate file id in Mix format for a given file name.");
    Console.WriteLine("Usage: dotnet MixFileId.cs <TD|TS> <name>");
    return;
}

var isTD = "TD".Equals(args[0], StringComparison.OrdinalIgnoreCase);
var nameArgs = args[1];

if (!Ascii.IsValid(nameArgs))
{
    Console.WriteLine("Only ascii chars behaviours are defined: treat them as a byte.");
    return;
}

Console.WriteLine($"{(isTD ? "TD" : "TS")}: {nameArgs}");

var name = nameArgs.ToUpper();
if (isTD)
{
    FileId id = 0;
    var length = name.Length;

    // Group by 4 chars
    var groupCount = length / 4;
    if (length % 4 != 0)
    {
        groupCount += 1;
    }

    for (var i = 0; i < groupCount; i++)
    {
        // Put 4 chars in a group to a temp uint, big-endian.
        uint temp = 0;
        for (var j = 0; j < 4; j++)
        {
            var index = i * 4 + j;
            if (index < length)
            {
                var ch = (int)name[index];
                temp += (uint)ch << 8 * j;
            }
        }

        // Rotate left the current id by 1 bit, then add the temp of current group.
        id = BitOperations.RotateLeft(id, 1) + temp;
    }

    var hexString = id.ToString("X8");
    Console.WriteLine(hexString);
}
else
{
    var length = name.Length;

    // File name should be aligned to a multiple of 4 chars.
    // If not, append a byte with reminder length (length % 4), then repeat the last char of the original file name.
    // E.g.: RULES.INI => RULES.INI + 0x01 + II
    var blockCount = length / 4;
    var remainderCount = length % 4;
    if (remainderCount != 0)
    {
        name += (char)remainderCount; // Add a mark for reminder length
        var toAddCharCount = 3 - remainderCount;
        var lastCharIndex = blockCount * 4;
        for (var i = 0; i < toAddCharCount; i++)
        {
            name += name[lastCharIndex];
        }
    }

    var bytes = name.Select(c => (byte)c).ToArray();
    var id = Crc32.HashToUInt32(bytes);

    var hexString = id.ToString("X8");
    Console.WriteLine(hexString);
}
