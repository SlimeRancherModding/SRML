using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR
{
    public static class MailRegistry
    {

        internal static List<MailEntry> ModdedMails = new List<MailEntry>();

        const string MAIL_BUNDLE = "mail";

        const string MAIL_FROM_PREFIX = "m.from.";

        const string MAIL_SUBJ_PREFIX = "m.subj.";

        const string MAIL_BODY_PREFIX = "m.body.";

        public class MailEntry
        {
            public string MailKey;

            internal Action<MailDirector, MailDirector.Mail> onReadCallback;

            public MailEntry(string mailKey)
            {
                this.MailKey = mailKey;
            }

            public string FromKey => MAIL_FROM_PREFIX + MailKey;
            public string SubjectKey => MAIL_SUBJ_PREFIX + MailKey;
            public string BodyKey => MAIL_BODY_PREFIX + MailKey;

            internal SRMod GetMod()
            {
                return TranslationPatcher.GetModForKey(MAIL_BUNDLE, FromKey) ?? TranslationPatcher.GetModForKey(MAIL_BUNDLE, SubjectKey) ?? TranslationPatcher.GetModForKey(MAIL_BUNDLE, BodyKey);
            }

            public MailEntry SetReadCallback(Action<MailDirector,MailDirector.Mail> onReadCallback)
            {
                this.onReadCallback = onReadCallback;
                return this;
            }

            public MailEntry SetFromTranslation(string text)
            {
                TranslationPatcher.AddTranslationKey(MAIL_BUNDLE, FromKey, text);
                return this;
            }

            public MailEntry SetBodyTranslation(string text)
            {
                TranslationPatcher.AddTranslationKey(MAIL_BUNDLE, BodyKey, text);
                return this;
            }

            public MailEntry SetSubjectTranslation(string text)
            {
                TranslationPatcher.AddTranslationKey(MAIL_BUNDLE, SubjectKey, text);
                return this;
            }

            public override bool Equals(object obj)
            {
                return obj is MailEntry entry && entry.MailKey == MailKey;
            }

            public override int GetHashCode()
            {
                return MailKey.GetHashCode() + 112315;
            }
        }


        public static MailEntry RegisterMailEntry(MailEntry entry)
        {
            ModdedMails.Add(entry);
            return entry;
        }

        public static MailEntry RegisterMailEntry(string key)
        {
            return RegisterMailEntry(new MailEntry(key));
        }


        internal static SRMod GetModForMail(string mailKey)
        {
            return ModdedMails.FirstOrDefault((x) => x.MailKey == mailKey)?.GetMod();
        }

        
    }
}
