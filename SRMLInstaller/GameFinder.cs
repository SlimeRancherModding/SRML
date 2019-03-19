using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRMLInstaller
{
    static class GameFinder
    {
        private const String gameDLL = "Assembly-CSharp.dll";
        
        private const String gameName = "SlimeRancher";

        private static String dataFolder = gameName + "_Data";

        private const String epicPath = "C:/Program Files/Epic Games/";

        private const String steamPath32 = "C:/Program Files (x86)/Steam/steamapps/common/";

        private const String steamPath64 = "C:/Program Files/Steam/steamapps/common/";

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

            
            List<String> candidates = new List<String>();
            void AddIfCandidate(String path) { if(CheckPathForGame(path)) candidates.Add(path);}
            AddIfCandidate(epicPath);
            AddIfCandidate(steamPath32);
            AddIfCandidate(steamPath64);

            if (candidates.Count == 0) throw new Exception("Could not auto-locate game folder!");
            var candidatetoDLL = Path.Combine(gameName, exeToDLL);
            if (candidates.Count > 1)
            {
                Console.WriteLine("Found multiple candidates for the game path!");

                return Path.Combine(SelectFromList(candidates), candidatetoDLL);
            }

            return Path.Combine(candidates[0], candidatetoDLL);
        }

        static bool CheckPathForGame(String path)
        {
            return (File.Exists(Path.Combine(path, Path.Combine(gameName, exeToDLL))));
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
