using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.Console.Commands
{
    class FastForwardCommand : ConsoleCommand
    {
        public override string ID => "fastforward";

        public override string Usage => "fastforward [hour amount]";

        public override string Description => "Fast forwards to next dawn, or the amount of hours you request";

        public override bool Execute(string[] args)
        {
            double timeToFastForwardTo = SceneContext.Instance.TimeDirector.GetNextDawn();
            if((args?.Length ?? 0) == 1)
            {
                if(float.TryParse(args[0],out var amount))
                {
                    timeToFastForwardTo = SceneContext.Instance.TimeDirector.HoursFromNow(amount);
                }
            }

            SceneContext.Instance.TimeDirector.FastForwardTo(timeToFastForwardTo);

            return true;
        }
    }
}
