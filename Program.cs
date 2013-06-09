/*

Make hideFrameworkAssemblies a parameter
Add a parameter to hide duplicates? Or optionally to expand them fully every time...

 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace asmwalk
{
    class Program
    {
        private static readonly List<string> NamesSeen = new List<string>();
        private static int _indentLevel;
        private static string _assemblySearchPath;
        private static bool _hideFrameworkAssemblies;

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Die("Pass it an assembly to walk its dependencies.");
            }
            var file = args[0];
            if (!File.Exists(file))
            {
                Die("File was not found: " + file);
            }

            _hideFrameworkAssemblies = true;

            _assemblySearchPath = Path.GetDirectoryName(file);
            AppDomain.CurrentDomain.AssemblyResolve += LoadFromSameFolder;

            //var assembly = Assembly.ReflectionOnlyLoadFrom(file); // Doesn't appear to fire AssemblyResolve
            var assembly = Assembly.LoadFrom(file);
            FollowReferences(assembly.GetName());
        }

        private static void Die(string message)
        {
            Console.Error.WriteLine(message);
            Environment.Exit(1);
        }

        private static void FollowReferences(AssemblyName assemblyName, string parentImageRuntimeVersion = null)
        {
            if (_hideFrameworkAssemblies && IsFrameworkAssembly(assemblyName))
            {
                return;
            }

            var alreadySeen = NamesSeen.Contains(assemblyName.FullName);
            Assembly assembly = null;
            string loadError = null;
            if (!alreadySeen)
            {
                NamesSeen.Add(assemblyName.FullName);
                try
                {
                    //assembly = Assembly.ReflectionOnlyLoad(assemblyName); // Doesn't appear to fire AssemblyResolve
                    assembly = Assembly.Load(assemblyName);
                    if (assembly == null)
                    {
                        loadError = "Assembly load returned null";
                    }
                }
                catch (Exception e)
                {
                    loadError = e.Message;
                }
            }

            var higherThanParentVersion = assembly != null && !string.IsNullOrEmpty(parentImageRuntimeVersion) && string.Compare(assembly.ImageRuntimeVersion, parentImageRuntimeVersion) == 1;
            var name = string.Format("{0} {1}{2}", assemblyName.Name, assemblyName.Version, assembly == null ? "" : " (" + assembly.ImageRuntimeVersion + (higherThanParentVersion ? "!!!" : "") + ")");
            Console.WriteLine("{0}{1}{2}{3}", GetIndentString(alreadySeen, !string.IsNullOrEmpty(loadError)), name, alreadySeen ? " (again)" : "", string.IsNullOrEmpty(loadError) ? "" : " FAILED: " + loadError);
            if (assembly == null)
            {
                return;
            }
            _indentLevel++;
            foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
            {
                FollowReferences(referencedAssembly, assembly.ImageRuntimeVersion);
            }
            _indentLevel--;
        }

        private static string GetIndentString(bool alreadySeen, bool error)
        {
            return new string(' ', _indentLevel) + (error ? '!' : alreadySeen ? '-' : '+');
        }

        private static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
        {
            var assemblyPath = Path.Combine(_assemblySearchPath, new AssemblyName(args.Name).Name + ".dll");
            if (File.Exists(assemblyPath) == false) return null;
            var assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }

        private static bool IsFrameworkAssembly(AssemblyName assemblyName)
        {
            var token = BitConverter.ToString(assemblyName.GetPublicKeyToken()).Replace("-", "").ToLowerInvariant();
            switch (token)
            {
                case "b77a5c561934e089":
                case "31bf3856ad364e35":
                case "b03f5f7f11d50a3a":
                case "89845dcd8080cc91":
                    return true;
            }
            return false;
        }
    }
}
