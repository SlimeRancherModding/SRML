using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SRMLInstaller
{
    static class GameFinder
    {
        private const String gameDLL = "Assembly-CSharp.dll";
        
        private const String gameName = "SlimeRancher";
            
        private const String gameNameWithSpace = "Slime Rancher";

        private static String dataFolder = gameName + "_Data";

        private const String epicPath = "C:/Program Files/Epic Games/";

        private const String steamPath32 = "C:/Program Files (x86)/Steam/steamapps/common/";

        private const String steamPath64 = "C:/Program Files/Steam/steamapps/common/";

        private const String drmfree64 = "C:/Program Files/";

        private const String drmfree32 = "C:/Program Files (x86)/";

        private static String exeToDLL = Path.Combine(dataFolder, Path.Combine("Managed", gameDLL));


        public static String FindGame()
        {
            if (File.Exists(gameDLL)) return Path.GetFullPath(gameDLL);
            var managedDLL = Path.Combine("Managed", gameDLL);
            if (File.Exists(managedDLL))
                return Path.GetFullPath(managedDLL);
            if (File.Exists(Path.Combine(dataFolder, managedDLL)))
                return Path.GetFullPath(Path.Combine(dataFolder, managedDLL));

            Console.WriteLine($"Not in a game folder! Searching for {gameName}...");

            Console.WriteLine();

            List<String> candidates = new List<String>();

            void AddIfCandidate(String path)
            {
                if(CheckPathForGame(path,gameName))
                    candidates.Add(Path.Combine(path,gameName));
                if (CheckPathForGame(path,gameNameWithSpace))
                    candidates.Add(Path.Combine(path, gameNameWithSpace));
            }
            AddIfCandidate(epicPath);
            AddIfCandidate(steamPath32);
            AddIfCandidate(steamPath64);
            AddIfCandidate(drmfree32);
            AddIfCandidate(drmfree64);

            if (candidates.Count == 0) throw new Exception($"Could not auto-locate game folder! Please move {Path.GetFileName(Assembly.GetExecutingAssembly().Location)} to a valid game folder and try again");
            var candidatetoDLL = exeToDLL;
            if (candidates.Count > 1)
            {
                Console.WriteLine("Found multiple candidates for the game path!");

                return Path.Combine(SelectFromList(candidates), candidatetoDLL);
            }


            return Path.Combine(candidates[0], candidatetoDLL);
        }

        static bool CheckPathForGame(String path,string gameName)
        {
            return (File.Exists(Path.Combine(path, gameName, exeToDLL)));
        }

        static T SelectFromList<T>(List<T> elements)
        {
            
            for (int i = 0; i < elements.Count; i++)
            {
                Console.WriteLine($"{i+1} - {elements[i]}");
            }
            restart:
            Console.Write($"Please select an option from 1 to {elements.Count}: ");
            if (Int32.TryParse(Console.ReadLine(), out int val) && val <= elements.Count && val >= 1)
                return elements[val - 1];
            goto restart;
        }
    }
}
