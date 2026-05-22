using System;
using System.IO;


namespace kfutils {

    

    public abstract class PathUtils    
    {
        public static bool IsDirectorySeparator(char c)
        {
            return (c == Path.PathSeparator) || (c == Path.AltDirectorySeparatorChar);
        }
    }

}