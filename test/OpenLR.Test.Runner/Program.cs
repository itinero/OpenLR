using NUnit.Common;
using NUnitLite;
using OpenLR.Tests;
using System;
using System.Reflection;

namespace OpenLR.Test.Runner
{
    class Program
    {
        static int Main(string[] args)
        {
            var res = new AutoRun(typeof(LocationExtensionTests).GetTypeInfo().Assembly)
                .Execute(args, new ExtendedTextWrapper(Console.Out), Console.In);

#if DEBUG
            Console.ReadLine();
#endif
            return res;
        }
    }
}