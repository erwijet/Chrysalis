using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChrysalisLib;

namespace cenc
{
    class Program
    {

        static string HelpMenu =
            @"
Chrysalis Encoding Tool. Help Menu.

cenc.exe and Chrysalis are avalible under the GNU AGPLv3

The Chrysalis library is avalible at http://erwijet.github.com/Chrysalis

Chrysalis v1.1.0.1[α] October 15 2018
cenc.exe  v1.0.1[α] 16 August 2018

Usage: cenc [mode] <input type> filename

Modes:
-e: Encode
-d: Decode

Input Types:
-f: File
-d: Directory / Folder'

Password Protection

Examples:
cenc -e -f unencodedFile.txt
cenc -d encodedFile.png";
        static void Main(string[] args)
        {
            string cmdString = GetCommandString(args) ?? "NaN";
            if (cmdString == "NaN")
            {
                Console.WriteLine(HelpMenu);
                //Console.ReadKey();
                return;
            }
            if (cmdString.Substring(0, 1) == "+")
            {
                var flag = cmdString.Substring(1, 1);
                if (flag == "%")
                    Chrysalis.DoFileEncode(new System.IO.FileInfo(cmdString.Substring(2)), false);
                else
                    Chrysalis.DoFolderEncode(new System.IO.DirectoryInfo(cmdString.Substring(2)));
            }
            else
            {
                Chrysalis.DoFileDecode(new System.IO.FileInfo(cmdString.Substring(1)));
            }
            
        }

        //+: encode, -:decode %: file, &: directory

        static string GetCommandString(params string[] args)
        {
            string commandString = null;

            switch (args.Length)
            {
                case 3:
                    if (args[0] == "-e" && (args[1] == "-f" || args[1] == "-d") && args[2] != "")
                        commandString = "+" + ((args[1] == "-f") ? "%" : "&") + args[2];
                    break;
                case 2:
                    if (args[0] == "-d" && args[1] != "")
                        commandString = "-" + args[1];
                    break;
            }

            if (commandString != null)
                return commandString;
            return null;
        }
    }
}

