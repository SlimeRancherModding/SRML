using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.Translation
{
    public class TutorialPediaEntryTranslation : PediaEntryTranslation
    {
        public TutorialPediaEntryTranslation(PediaDirector.Id id) : base(id)
        {
        }

        const string INSTRUCTIONS_PREFIX = "m.instructions.";
        const string INSTRUCTIONS_GAMEPAD_PREFIX = INSTRUCTIONS_PREFIX + "gamepad.";

        public string InstructionsKey => INSTRUCTIONS_PREFIX + StringKey;
        public string InstructionsGamepadKey => INSTRUCTIONS_GAMEPAD_PREFIX + StringKey;

        public TutorialPediaEntryTranslation SetInstructionsTranslation(string instructions)
        {
            TranslationPatcher.AddPediaTranslation(InstructionsKey, instructions);
            return this;
        }

        public TutorialPediaEntryTranslation SetInstructionsGamepadTranslation(string instructions)
        {
            TranslationPatcher.AddPediaTranslation(InstructionsGamepadKey, instructions);
            return this;
        }

        public new TutorialPediaEntryTranslation SetTitleTranslation(string name)
        {
            TranslationPatcher.AddPediaTranslation(TitleKey, name);
            return this;
        }

        public new TutorialPediaEntryTranslation SetIntroTranslation(string intro)
        {
            TranslationPatcher.AddPediaTranslation(IntroKey, intro);
            return this;
        }

        public new TutorialPediaEntryTranslation SetDescriptionTranslation(string description)
        {
            TranslationPatcher.AddPediaTranslation(DescriptionKey, description);
            return this;
        }
    }
}
