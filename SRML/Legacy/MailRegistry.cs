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

            // TODO: Upgrade to new system
            internal SRMod GetMod()
            {
                return null;
                //return TranslationPatcher.GetModForKey(MAIL_BUNDLE, FromKey) ?? TranslationPatcher.GetModForKey(MAIL_BUNDLE, SubjectKey) ?? TranslationPatcher.GetModForKey(MAIL_BUNDLE, BodyKey);
            }

            /// <summary>
            /// Sets the callback of the <see cref="MailEntry"/>.
            /// </summary>
            /// <param name="onReadCallback">The callback</param>
            /// <returns></returns>
            public MailEntry SetReadCallback(Action<MailDirector,MailDirector.Mail> onReadCallback)
            {
                this.onReadCallback = onReadCallback;
                return this;
            }

            /// <summary>
            /// Sets the from translation.
            /// </summary>
            /// <param name="text">The translated text.</param>
            /// <returns></returns>
            public MailEntry SetFromTranslation(string text)
            {
                TranslationPatcher.AddTranslationKey(MAIL_BUNDLE, FromKey, text);
                return this;
            }

            /// <summary>
            /// Sets the body translation.
            /// </summary>
            /// <param name="text">The translated text.</param>
            public MailEntry SetBodyTranslation(string text)
            {
                TranslationPatcher.AddTranslationKey(MAIL_BUNDLE, BodyKey, text);
                return this;
            }

            /// <summary>
            /// Sets the subject translation.
            /// </summary>
            /// <param name="text">The translated text.</param>
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

        /// <summary>
        /// Registers a <see cref="MailEntry"/>.
        /// </summary>
        /// <param name="entry">The <see cref="MailEntry"/> to register.</param>
        /// <returns>The registered <see cref="MailEntry"/></returns>
        public static MailEntry RegisterMailEntry(MailEntry entry)
        {
            ModdedMails.Add(entry);
            return entry;
        }

        /// <summary>
        /// Registers a <see cref="MailEntry"/>.
        /// </summary>
        /// <param name="key">The key of the <see cref="MailEntry"/> to register.</param>
        /// <returns>The registered <see cref="MailEntry"/></returns>
        public static MailEntry RegisterMailEntry(string key) => RegisterMailEntry(new MailEntry(key));

        internal static SRMod GetModForMail(string mailKey) => ModdedMails.FirstOrDefault((x) => x.MailKey == mailKey)?.GetMod();
    }
}
