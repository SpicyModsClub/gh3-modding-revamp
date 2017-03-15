using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GuitarHero.Tests
{
    public static class TestHelpers
    {
        public static Stream OpenSample(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream("GuitarHero.Tests.Samples." + path);
        }
    }
}
