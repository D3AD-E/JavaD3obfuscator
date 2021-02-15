using JarD3obfuscator.Utils;
using PowerArgs;
using System;
using System.Text.RegularExpressions;

namespace JarD3obfuscator
{
    class Program
    {
        static void Main(string[] args)
        {
            Args.InvokeMain<ParseMain>(args);
        }
    }
}
