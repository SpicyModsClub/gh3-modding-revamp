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

        public static FileStream CreateTempFile()
        {
            return new FileStream(
                Path.GetTempFileName(),
                FileMode.Truncate,
                FileAccess.ReadWrite,
                FileShare.Read,
                4096,
                FileOptions.DeleteOnClose);
        }


        public static FileStream CreateTempCopy(string resourceName) {
            var tempFile = CreateTempFile();

            using (var sample = OpenSample(resourceName)) {
                sample.CopyTo(tempFile);
                tempFile.Position = 0;
            }

            return tempFile;
        }
    }
}
