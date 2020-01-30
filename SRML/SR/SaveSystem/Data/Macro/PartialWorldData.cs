using MonomiPark.SlimeRancher.Persist;
using SRML.SR.SaveSystem.Data.Partial;
using SRML.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR.SaveSystem.Data.Macro
{
    /// <summary>
    /// Pulls out all data in <see cref="WorldV22"/> based on if its string index is modded (note: make sure to pull custom data out of the save before using this, otherwise it'll be saved in the wrong place!)
    /// </summary>
    public class PartialWorldData : VersionedPartialData<WorldV22>
    {
        


        public override int LatestVersion => 0;

        public PartialCollection<string> activeGingerPatches = PartialDataUtils.CreatePartialModdedStringCollection();
        public AmbianceDirector.Weather weather;
        public PartialDictionary<string, EchoNoteGordoV01> echoNoteGordos = PartialDataUtils.CreatePartialDictionaryWithStringKey<EchoNoteGordoV01>();
        private static readonly SerializerPair<Identifiable.Id> identifiableIdSerializer = SerializerPair.GetEnumSerializerPair<Identifiable.Id>();
        public PartialDictionary<Identifiable.Id, float> econSaturations = new PartialDictionary<Identifiable.Id, float>((x)=>ModdedIDRegistry.IsModdedID(x.Key), identifiableIdSerializer,SerializerPair.FLOAT, checkValueValid:(x) => ModdedIDRegistry.IsModdedID(x.Key));
        public PartialGlitchSimulation glitch = new PartialGlitchSimulation();
        public PartialDictionary<string, GordoV01> gordos = PartialDataUtils.CreatePartialDictionaryWithStringKey<GordoV01>();
        public PartialDictionary<string, PlacedGadgetV08> placedGadgets = PartialDataUtils.CreatePartialDictionaryWithStringKey<PlacedGadgetV08>();

        static PartialCollection<string> MakeRanchOfferFilter() => new PartialCollection<string>(ExchangeOfferRegistry.IsCustom, SerializerPair.STRING, ExchangeOfferRegistry.IsCustom);
        public PartialCollection<string> lastOfferRancherIds = MakeRanchOfferFilter();
        public PartialCollection<string> pendingOfferRancherIds = MakeRanchOfferFilter();

        public PartialDictionary<string, float> liquidSourceUnits = PartialDataUtils.CreatePartialDictionaryWithStringKey(SerializerPair.FLOAT);

        public PartialDictionary<string, bool> oasisStates = PartialDataUtils.CreatePartialDictionaryWithStringKey<bool>(SerializerPair.BOOL);
        public PartialDictionary<string, bool> occupiedPhaseSites = PartialDataUtils.CreatePartialDictionaryWithStringKey<bool>(SerializerPair.BOOL);
        public PartialDictionary<string, bool> puzzleSlotsFilled = PartialDataUtils.CreatePartialDictionaryWithStringKey<bool>(SerializerPair.BOOL);
        public PartialDictionary<string, bool> teleportNodeActivations = PartialDataUtils.CreatePartialDictionaryWithStringKey<bool>(SerializerPair.BOOL);


        public PartialDictionary<string,QuicksilverEnergyGeneratorV02> quicksilverEnergyGenerators = PartialDataUtils.CreatePartialDictionaryWithStringKey<QuicksilverEnergyGeneratorV02>();

        private static readonly SerializerPair<SwitchHandler.State> pair = SerializerPair.GetEnumSerializerPair<SwitchHandler.State>();
        public PartialDictionary<string, SwitchHandler.State> switches = PartialDataUtils.CreatePartialDictionaryWithStringKey<SwitchHandler.State>(pair);

        public PartialDictionary<string, TreasurePodV01> treasurePods = PartialDataUtils.CreatePartialDictionaryWithStringKey<TreasurePodV01>();

        public override void Pull(WorldV22 data)
        {
            activeGingerPatches.Pull(data.activeGingerPatches);
            switches.Pull(data.switches);
            treasurePods.Pull(data.treasurePods);
            oasisStates.Pull(data.oasisStates);
            puzzleSlotsFilled.Pull(data.puzzleSlotsFilled);
            quicksilverEnergyGenerators.Pull(data.quicksilverEnergyGenerators);
            gordos.Pull(data.gordos);
            occupiedPhaseSites.Pull(data.occupiedPhaseSites);
            teleportNodeActivations.Pull(data.teleportNodeActivations);
            lastOfferRancherIds.Pull(data.lastOfferRancherIds);
            pendingOfferRancherIds.Pull(data.pendingOfferRancherIds);
            glitch.Pull(data.glitch);
            echoNoteGordos.Pull(data.echoNoteGordos);
            econSaturations.Pull(data.econSaturations);
            placedGadgets.Pull(data.placedGadgets);
            weather = ModdedIDRegistry.IsModdedID(data.weather) ? data.weather : AmbianceDirector.Weather.NONE;
        }

        public override void Push(WorldV22 data)
        {
            activeGingerPatches.Push(data.activeGingerPatches);
            switches.Push(data.switches);
            treasurePods.Push(data.treasurePods);
            oasisStates.Push(data.oasisStates);
            puzzleSlotsFilled.Push(data.puzzleSlotsFilled);
            quicksilverEnergyGenerators.Push(data.quicksilverEnergyGenerators);
            gordos.Push(data.gordos);
            occupiedPhaseSites.Push(data.occupiedPhaseSites);
            teleportNodeActivations.Push(data.teleportNodeActivations);
            lastOfferRancherIds.Push(data.lastOfferRancherIds);
            pendingOfferRancherIds.Push(data.pendingOfferRancherIds);
            glitch.Push(data.glitch);
            echoNoteGordos.Push(data.echoNoteGordos);
            econSaturations.Push(data.econSaturations);
            placedGadgets.Push(data.placedGadgets);
            data.weather = ModdedIDRegistry.IsModdedID(weather) ? weather : data.weather;

        }

        public override void ReadData(BinaryReader reader)
        {
            activeGingerPatches.Read(reader);
            switches.Read(reader);
            treasurePods.Read(reader);
            oasisStates.Read(reader);
            puzzleSlotsFilled.Read(reader);
            quicksilverEnergyGenerators.Read(reader);
            gordos.Read(reader);
            occupiedPhaseSites.Read(reader);
            teleportNodeActivations.Read(reader);
            lastOfferRancherIds.Read(reader);
            pendingOfferRancherIds.Read(reader);
            glitch.Read(reader);
            echoNoteGordos.Read(reader);
            econSaturations.Read(reader);
            placedGadgets.Read(reader);
            weather = (AmbianceDirector.Weather)reader.ReadInt32();
        }

        public override void WriteData(BinaryWriter writer)
        {
            activeGingerPatches.Write(writer);
            switches.Write(writer);
            treasurePods.Write(writer);
            oasisStates.Write(writer);
            puzzleSlotsFilled.Write(writer);
            quicksilverEnergyGenerators.Write(writer);
            gordos.Write(writer);
            occupiedPhaseSites.Write(writer);
            teleportNodeActivations.Write(writer);
            lastOfferRancherIds.Write(writer);
            pendingOfferRancherIds.Write(writer);
            glitch.Write(writer);
            echoNoteGordos.Write(writer);
            econSaturations.Write(writer);
            placedGadgets.Write(writer);
            writer.Write((int)weather);
        }

        static bool GetIsCustom<T>(KeyValuePair<string, T> pair) => ModdedStringRegistry.IsValidModdedString(pair.Key);
        static bool GetIsCustom(string pair) => ModdedStringRegistry.IsValidModdedString(pair);

        static PartialWorldData()
        {
            PartialData.RegisterPartialData<GlitchSlimulationV02, PartialGlitchSimulation>();
            PartialData.RegisterPartialData<WorldV22, PartialWorldData>();
            EnumTranslator.RegisterEnumFixer<PartialWorldData>((trans, mode, world) =>
            {
                trans.FixEnumValues(mode, world.glitch);
                world.weather = trans.TranslateEnum(mode, world.weather);
            });
            CustomChecker.RegisterCustomChecker<WorldV22>((y) =>
            {
                if (
                    ModdedIDRegistry.IsModdedID(y.weather) ||
                    y.activeGingerPatches.Any(x => GetIsCustom(x)) ||
                    y.switches.Any(x => GetIsCustom(x)) ||
                    y.treasurePods.Any(x => GetIsCustom(x)) ||
                    y.oasisStates.Any(x => GetIsCustom(x)) ||
                    y.puzzleSlotsFilled.Any(x => GetIsCustom(x)) ||
                    y.quicksilverEnergyGenerators.Any(x => GetIsCustom(x)) ||
                    y.gordos.Any(x => GetIsCustom(x)) ||
                    y.occupiedPhaseSites.Any(x => GetIsCustom(x)) ||
                    y.teleportNodeActivations.Any(x => GetIsCustom(x)) ||
                    y.lastOfferRancherIds.Any(x => GetIsCustom(x)) ||
                    y.pendingOfferRancherIds.Any(x => GetIsCustom(x)) ||
                    CustomChecker.GetCustomLevel(y.glitch) == CustomChecker.CustomLevel.PARTIAL ||
                    y.echoNoteGordos.Any(x => GetIsCustom(x)) ||
                    y.placedGadgets.Any(x => GetIsCustom(x)) ||
                    y.econSaturations.Any(x => ModdedIDRegistry.IsModdedID(x.Key))) return CustomChecker.CustomLevel.PARTIAL;
                Debug.Log("Guess we vanlla");
                return CustomChecker.CustomLevel.VANILLA;

            });

            CustomChecker.RegisterCustomChecker<GlitchSlimulationV02>((y) =>
            {
                if (y.teleporters.Any(GetIsCustom) ||
                y.nodes.Any(GetIsCustom) ||
                y.impostos.Any(GetIsCustom) ||
                y.impostoDirectors.Any(GetIsCustom) ||
                y.storage.Any(GetIsCustom)) return CustomChecker.CustomLevel.PARTIAL;
                return CustomChecker.CustomLevel.VANILLA;
            });
        }

        public class PartialGlitchSimulation : VersionedPartialData<GlitchSlimulationV02>
        {
            public override int LatestVersion => 0;

            public PartialDictionary<string, GlitchTeleportDestinationV01> teleporters = PartialDataUtils.CreatePartialDictionaryWithStringKey<GlitchTeleportDestinationV01>();
            public PartialDictionary<string, GlitchTarrNodeV01> nodes = PartialDataUtils.CreatePartialDictionaryWithStringKey<GlitchTarrNodeV01>();
            public PartialDictionary<string, GlitchImpostoDirectorV01> impostoDirectors = PartialDataUtils.CreatePartialDictionaryWithStringKey<GlitchImpostoDirectorV01>();
            public PartialDictionary<string, GlitchImpostoV01> impostos = PartialDataUtils.CreatePartialDictionaryWithStringKey<GlitchImpostoV01>();
            public PartialDictionary<string, GlitchStorageV01> storage = new PartialDictionary<string, GlitchStorageV01>((x) => ((ModdedStringRegistry.IsValidModdedString(x.Key) ? throw new Exception(x.Key) : false) || ModdedIDRegistry.IsModdedID(x.Value.id)), SerializerPair.STRING, PartialDataUtils.CreateSerializerForPersistedDataSet<GlitchStorageV01>(), checkValueValid: (x) => ModdedIDRegistry.IsModdedID(x.Value.id));
            
            public override void Pull(GlitchSlimulationV02 data)
            {
                teleporters.Pull(data.teleporters);
                nodes.Pull(data.nodes);
                impostoDirectors.Pull(data.impostoDirectors);
                impostos.Pull(data.impostos);
                storage.Pull(data.storage);
            }

            public override void Push(GlitchSlimulationV02 data)
            {
                teleporters.Push(data.teleporters);
                nodes.Push(data.nodes);
                impostos.Push(data.impostos);
                impostoDirectors.Push(data.impostoDirectors);
                storage.Push(data.storage);
            }

            public override void ReadData(BinaryReader reader)
            {
                teleporters.Read(reader);
                nodes.Read(reader);
                impostos.Read(reader);
                impostoDirectors.Read(reader);
                storage.Read(reader);
            }

            public override void WriteData(BinaryWriter writer)
            {
                teleporters.Write(writer);
                nodes.Write(writer);
                impostos.Write(writer);
                impostoDirectors.Write(writer);
                storage.Write(writer);
            }
        }
    }
}
