using System;
using System.IO;
using System.IO.Compression;

namespace winh3lixpatcher
{
    internal class Program
    {
        // based on https://gist.github.com/jakeajames/b44d8db345769a7149e97f5e155b3d46
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: winh3lixpatcher <path to h3lix rc6 ipa>");
                Console.ReadKey();
                return;
            }
            string h3lix_original_path = args[0];
            bool err = false;
            if (string.IsNullOrEmpty(h3lix_original_path))
            {
                Console.WriteLine("Usage: winh3lixpatcher <path to h3lix rc6 ipa>");
            }
            Console.WriteLine("Extracting h3lix IPA...");
            try
            {
                Directory.CreateDirectory("./h3lix-temp");
            }
            catch (Exception ex)
            {
                Console.Write("Could not create temporary directory:" + ex.Message);
                err = true;
            }
            try
            {
                ZipFile.ExtractToDirectory(h3lix_original_path, $"./h3lix-temp");
            }
            catch (Exception ex)
            {
                Console.Write("Could extract h3lix IPA:" + ex.Message);
                err = true;
            }
            Console.WriteLine("Patching h3lix binary...");
            byte[] h3lixBinary = File.ReadAllBytes("./h3lix-temp/Payload/h3lix.app/h3lix");
            for (int i = 0; i < 20; i++)
            {
                h3lixBinary[30848 + i] = 0x11;
            }
            for (int i = 0; i < 4; i++)
            {
                h3lixBinary[31790 + i] = 0x00;
            }
            for (int i = 0; i < 20; i++)
            {
                h3lixBinary[32920 + i] = 0x11;
            }
            h3lixBinary[40800] = 0x70;
            h3lixBinary[40801] = 0x47;
            File.WriteAllBytes("./h3lix-temp/Payload/h3lix.app/h3lix", h3lixBinary);
            Console.WriteLine("Recompressing h3lix IPA...");
            try
            {
                ZipFile.CreateFromDirectory("./h3lix-temp", $"./h3lix-RC6-patched.ipa");
            }
            catch (Exception ex)
            {
                Console.Write("Could recompress h3lix IPA:" + ex.Message);
                err = true;
            }
            Console.WriteLine("Removing temporary files...");
            try
            {
                Directory.Delete("./h3lix-temp", true);
            }
            catch (Exception ex)
            {
                Console.Write("Could not delete temporary directory:" + ex.Message);
                err = true;
            }
            if (!err) Console.WriteLine("completed with no errors :)");
            Console.ReadKey();
        }
    }
}
