using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.Config.Attributes
{
    public class ConfigCommentAttribute : Attribute
    {
        public string Comment;
        public ConfigCommentAttribute(string comment)
        {
            Comment = comment;
        }
    }
}
