using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SRML
{
    internal class ModLoadException : Exception
    {
        public ModLoadException(string modId, SRModLoader.LoadingStep step, Exception innerException) :
            base(CreateMessage(modId, step), innerException)
        {
            this.modId = modId;
            this.step = step;
        }

        public string modId;
        public SRModLoader.LoadingStep step;
        
        public static string LoadingStepToVerb(SRModLoader.LoadingStep step)
        {
            string loadAction;
            switch (step)
            {
                case SRModLoader.LoadingStep.INITIALIZATION:
                    loadAction = "intializing";
                    break;
                case SRModLoader.LoadingStep.PRELOAD:
                    loadAction = "pre-loading";
                    break;
                case SRModLoader.LoadingStep.POSTLOAD:
                    loadAction = "post-loading";
                    break;
                case SRModLoader.LoadingStep.RELOAD:
                    loadAction = "reloading";
                    break;
                case SRModLoader.LoadingStep.UNLOAD:
                    loadAction = "unloading";
                    break;
                case SRModLoader.LoadingStep.UPDATE:
                    loadAction = "updating";
                    break;
                case SRModLoader.LoadingStep.FIXEDUPDATE:
                    loadAction = "fixed-updating";
                    break;
                case SRModLoader.LoadingStep.LATEUPDATE:
                    loadAction = "late-updating";
                    break;
                default:
                    loadAction = "loading";
                    break;
            }
            return loadAction;
        }

        private static string CreateMessage(string modId, SRModLoader.LoadingStep step) => 
            $"Error {LoadingStepToVerb(step)} mod '{(SRModLoader.Mods.TryGetValue(modId, out SRMod m) ? m.ModInfo.Name : modId)}'";
    }
}
