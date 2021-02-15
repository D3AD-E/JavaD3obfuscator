using PowerArgs;
using System;
using System.Collections.Generic;
using System.Text;

namespace JarD3obfuscator.Utils
{
    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    class ParseMain
    {
        [HelpHook, ArgShortcut("-?"), ArgDescription("Shows this help")]
        public bool Help { get; set; }

        [ArgShortcut("-p"), ArgRequired(PromptIfMissing = true), ArgDescription("Path to file to be deobfuscated"), ArgPosition(0)]
        public string PathToArchive { get; set; }

        [ArgShortcut("-spath"), ArgDescription("Path to config to be saved, by default is ReplacedConfig.txt")]
        public string PathToSave { get; set; }

        [ArgShortcut("-lpath"), ArgDescription("Path to config to be loaded, by default is ReplacedConfig.txt")]
        public string PathToLoad { get; set; }

        [ArgShortcut("-s"), ArgDescription("If provided, saves config, -spath will not work without this parameter")]
        public bool DoSave { get; set; }

        [ArgShortcut("-l"), ArgDescription("If provided, loads config, -lpath will not work without this parameter")]
        public bool DoLoad { get; set; }

        [ArgShortcut("-dj"), ArgDescription("Disable internal file reference fixes")]
        public bool DisableJavaFixes { get; set; }

        public void Main()
        {
            var deobfuscator = new Deobfuscator();
            if(DoLoad)
            {
                if (!deobfuscator.Load(PathToLoad))
                    return;
            }
            deobfuscator.Deobfuscate(PathToArchive, DisableJavaFixes);
            if (DoSave)
                deobfuscator.Save(PathToSave);
        }
    }
}
