namespace Wrap.ViewModels.LoginAndRegistration.Helpers;

using System.Text.RegularExpressions;

using GCommon.Enums;

/// <summary>
/// Groups CrewRoleType enum values by department
/// Used for rendering accordion UI with bubble buttons
/// </summary>
public static class CrewRolesDepartments
{
    /// <summary>
    /// Categorizing the roles by their department name for easily use in registration form.
    /// </summary>
    /// <returns>Dictionary with key - department and value - enum values</returns>
    public static IReadOnlyDictionary<string, IReadOnlyCollection<CrewRoleType>> GetRolesByDepartment()
    {
        return new Dictionary<string, IReadOnlyCollection<CrewRoleType>>
        {
            ["Direction & Production"] =
            [
                CrewRoleType.Director,
                CrewRoleType.FirstAssistantDirector,
                CrewRoleType.SecondAssistantDirector,
                CrewRoleType.ThirdAssistantDirector,
                CrewRoleType.UnitProductionManager,
                CrewRoleType.LineProducer,
                CrewRoleType.Producer,
                CrewRoleType.ExecutiveProducer,
                CrewRoleType.ProductionSupervisor
            ],

            ["Writing & Development"] =
            [
                CrewRoleType.Screenwriter,
                CrewRoleType.ScriptEditor,
                CrewRoleType.ScriptSupervisor,
                CrewRoleType.DialogueCoach
            ],

            ["Camera Department"] =
            [
                CrewRoleType.DirectorOfPhotography,
                CrewRoleType.CameraOperator,
                CrewRoleType.FirstAssistantCamera,
                CrewRoleType.SecondAssistantCamera,
                CrewRoleType.DigitalImagingTechnician,
                CrewRoleType.SteadicamOperator,
                CrewRoleType.CameraLoader,
                CrewRoleType.VideoAssistOperator
            ],

            ["Lighting Department"] =
            [
                CrewRoleType.Gaffer,
                CrewRoleType.BestBoyElectric,
                CrewRoleType.LightingTechnician,
                CrewRoleType.RiggingElectrician,
                CrewRoleType.Spark,
                CrewRoleType.LampOperator,
                CrewRoleType.GeneratorOperator,
                CrewRoleType.Electrician,
                CrewRoleType.LightingDesigner
            ],

            ["Grip Department"] =
            [
                CrewRoleType.KeyGrip,
                CrewRoleType.BestBoyGrip,
                CrewRoleType.Grip,
                CrewRoleType.DollyGrip,
                CrewRoleType.CraneOperator,
                CrewRoleType.RiggingGrip,
                CrewRoleType.SteadicamGrip
            ],

            ["Art Department"] =
            [
                CrewRoleType.ProductionDesigner,
                CrewRoleType.ArtDirector,
                CrewRoleType.SetDecorator,
                CrewRoleType.PropMaster,
                CrewRoleType.PropsAssistant,
                CrewRoleType.SetDresser,
                CrewRoleType.ScenicArtist,
                CrewRoleType.ConstructionCoordinator,
                CrewRoleType.Leadman,
                CrewRoleType.Carpenter
            ],

            ["Costume & Makeup"] =
            [
                CrewRoleType.CostumeDesigner,
                CrewRoleType.WardrobeSupervisor,
                CrewRoleType.MakeupArtist,
                CrewRoleType.HairStylist,
                CrewRoleType.CostumeAssistant,
                CrewRoleType.MakeupAssistant,
                CrewRoleType.HairAssistant,
                CrewRoleType.WigMaster,
                CrewRoleType.SpecialEffectsMakeupArtist
            ],

            ["Sound Department"] =
            [
                CrewRoleType.ProductionSoundMixer,
                CrewRoleType.BoomOperator,
                CrewRoleType.SoundRecordist,
                CrewRoleType.SoundDesigner,
                CrewRoleType.FoleyArtist,
                CrewRoleType.SoundEditor,
                CrewRoleType.ReRecordingMixer,
                CrewRoleType.ADRSupervisor,
                CrewRoleType.SoundEngineer
            ],

            ["Post-Production"] =
            [
                CrewRoleType.Editor,
                CrewRoleType.AssistantEditor,
                CrewRoleType.Colorist,
                CrewRoleType.VisualEffectsSupervisor,
                CrewRoleType.VFXArtist,
                CrewRoleType.PostProductionSupervisor,
                CrewRoleType.TitlesDesigner
            ],

            ["Music Department"] =
            [
                CrewRoleType.Composer,
                CrewRoleType.MusicSupervisor,
                CrewRoleType.MusicEditor,
                CrewRoleType.Orchestrator,
                CrewRoleType.Conductor,
                CrewRoleType.Musician,
                CrewRoleType.Singer,
                CrewRoleType.SoundtrackCoordinator
            ],

            ["Locations"] =
            [
                CrewRoleType.LocationManager,
                CrewRoleType.AssistantLocationManager,
                CrewRoleType.LocationScout,
                CrewRoleType.LocationCoordinator
            ],

            ["Logistics & Transportation"] =
            [
                CrewRoleType.LogisticsManager,
                CrewRoleType.TransportCoordinator,
                CrewRoleType.TransportationCaptain,
                CrewRoleType.Driver,
                CrewRoleType.TravelCoordinator,
                CrewRoleType.AccommodationCoordinator,
                CrewRoleType.EquipmentManager,
                CrewRoleType.InventoryClerk
            ],

            ["Set Operations & Support"] =
            [
                CrewRoleType.ProductionAssistant,
                CrewRoleType.CateringManager,
                CrewRoleType.CraftServices,
                CrewRoleType.SecurityPersonnel,
                CrewRoleType.SetMedic,
                CrewRoleType.WeatherConsultant,
                CrewRoleType.AnimalTrainer,
                CrewRoleType.StuntCoordinator,
                CrewRoleType.ExtrasCastingDirector,
                CrewRoleType.ExtrasCoordinator
            ],

            ["Other"] = [CrewRoleType.Other]
        };
    }

    /// <summary>
    /// Get friendly display name for CrewRoleType
    /// Converts PascalCase to "Spaced Case"
    /// </summary>
    public static string GetDisplayName(CrewRoleType role)
    {
        return Regex.Replace(role.ToString(), "([A-Z])", " $1").Trim();
    }
}
