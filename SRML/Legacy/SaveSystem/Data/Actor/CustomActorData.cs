using System;
using System.IO;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using SRML.SR.SaveSystem.Data.Partial;
using SRML.SR.SaveSystem.Registry;
using VanillaActorData = MonomiPark.SlimeRancher.Persist.ActorDataV09;
namespace SRML.SR.SaveSystem.Data.Actor
{
    internal abstract class CustomActorData<T> : CustomActorData where T : ActorModel
    {

        public override Type GetModelType()
        {
            return typeof(T);
        }



        public abstract void PullCustomModel(T model);

        public abstract void PushCustomModel(T model);

        public sealed override void PushCustomModel(ActorModel model)
        {
            this.PushCustomModel((T) model);
        }

        public sealed override void PullCustomModel(ActorModel model)
        {
            this.PullCustomModel((T) model);
        }

        public sealed override void Load(Stream stream, bool skipPayloadEnd)
        {
            base.Load(stream,false);
            var reader = new BinaryReader(stream);
            LoadCustomData(reader);
            if(!skipPayloadEnd) ReadDataPayloadEnd(reader);
        }

        public sealed override void WriteData(BinaryWriter writer)
        {
            base.WriteData(writer);
            WriteDataPayloadEnd(writer);
            WriteCustomData(writer);
        }

    }
    public abstract class CustomActorData : VanillaActorData, IDataRegistryMember
    {
        public abstract void PullCustomModel(ActorModel model);

        public abstract void PushCustomModel(ActorModel model);

        public abstract void WriteCustomData(BinaryWriter writer);

        public abstract void LoadCustomData(BinaryReader reader);

        public abstract Type GetModelType();

        static CustomActorData()
        {
            RegisterPickers();
        }
        public static void RegisterPickers()
        {
            EnumTranslator.RegisterEnumFixer((EnumTranslator translator, EnumTranslator.TranslationMode mode, VanillaActorData v) =>
            {
                v.typeId = (int)translator.TranslateEnum(mode, ((Identifiable.Id)v.typeId));
                translator.FixEnumValues(mode, v.fashions);
                v.fashions.RemoveAll(x => x == Identifiable.Id.NONE);
                translator.FixEnumValues(mode, v.emotions.emotionData);
            });
            CustomChecker.RegisterCustomChecker((VanillaActorData data) =>
            {
                if (SaveRegistry.IsCustom(data)) return CustomChecker.CustomLevel.FULL;
                if (data.fashions.Any((x) => ModdedIDRegistry.IsModdedID<Identifiable.Id>(x)))
                    return CustomChecker.CustomLevel.PARTIAL;
                if (data.emotions.emotionData.Any((x) => ModdedIDRegistry.IsModdedID(x.Key)))
                    return CustomChecker.CustomLevel.PARTIAL;

                return CustomChecker.CustomLevel.VANILLA;
            });
            PartialData.RegisterPartialData(() => new PartialActorData());
        }
    }
}