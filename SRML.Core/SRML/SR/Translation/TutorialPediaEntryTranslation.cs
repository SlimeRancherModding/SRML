using SRML.Core;

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
            CoreTranslator.Instance.AddPediaTranslation(Language, InstructionsKey, instructions);
            return this;
        }

        public TutorialPediaEntryTranslation SetInstructionsGamepadTranslation(string instructions)
        {
            CoreTranslator.Instance.AddPediaTranslation(Language, InstructionsGamepadKey, instructions);
            return this;
        }

        public new TutorialPediaEntryTranslation SetTitleTranslation(string name)
        {
            CoreTranslator.Instance.AddPediaTranslation(Language, TitleKey, name);
            return this;
        }

        public new TutorialPediaEntryTranslation SetIntroTranslation(string intro)
        {
            CoreTranslator.Instance.AddPediaTranslation(Language, IntroKey, intro);
            return this;
        }

        public new TutorialPediaEntryTranslation SetDescriptionTranslation(string description)
        {
            CoreTranslator.Instance.AddPediaTranslation(Language, DescriptionKey, description);
            return this;
        }
    }
}
