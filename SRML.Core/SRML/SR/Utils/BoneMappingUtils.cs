using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.IO;
using static SlimeAppearance;

namespace SRML.SR.Utils
{
    public static class BoneMappingUtils
    {
        private static readonly Dictionary<SlimeBone, string> bones = new Dictionary<SlimeBone, string>()
        {
            {
                SlimeBone.Root,
                "bone_root"
            },
            {
                SlimeBone.Attachment,
                "bone_attachment"
            },
            {
                SlimeBone.Slime,
                "bone_slime"
            },
            {
                SlimeBone.Core,
                "bone_core"
            },
            {
                SlimeBone.JiggleBack,
                "bone_jiggle_bac"
            },
            {
                SlimeBone.JiggleBottom,
                "bone_jiggle_bot"
            },
            {
                SlimeBone.JiggleFront,
                "bone_jiggle_fro"
            },
            {
                SlimeBone.JiggleLeft,
                "bone_jiggle_lef"
            },
            {
                SlimeBone.JiggleRight,
                "bone_jiggle_rig"
            },
            {
                SlimeBone.JiggleTop,
                "bone_jiggle_top"
            },
            {
                SlimeBone.Spinner,
                "bone_spinner"
            },
            {
                SlimeBone.LeftWing,
                "bone_wing_left"
            },
            {
                SlimeBone.RightWing,
                "bone_wing_right"
            }
        };

        public static List<SlimeAppearanceApplicator.BoneMapping> GetMaps(GameObject obj)
        {
            List<SlimeAppearanceApplicator.BoneMapping> maps = new List<SlimeAppearanceApplicator.BoneMapping>();

            foreach (SlimeBone bone in bones.Keys)
            {
                maps.Add(new SlimeAppearanceApplicator.BoneMapping()
                {
                    Bone = bone,
                    BoneObject = obj.FindChild(bones[bone], true)
                });
            }

            return maps;
        }
    }
}
