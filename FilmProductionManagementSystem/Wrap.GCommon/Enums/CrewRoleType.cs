namespace Wrap.GCommon.Enums;

public enum CrewRoleType
{
    // ─────────────────────────────
    // Direction / Production Leadership
    // ─────────────────────────────
    Director = 1,
    FirstAssistantDirector = 2, // Manages on-set workflow
    SecondAssistantDirector = 3, // Manages scheduling and call sheets
    ThirdAssistantDirector = 4, // Manages cast and background actors
    UnitProductionManager = 5,
    LineProducer = 6,
    Producer = 7,
    ExecutiveProducer = 8,
    ProductionSupervisor = 9,

    // ─────────────────────────────
    // Writing / Development
    // ─────────────────────────────
    Screenwriter = 10,
    ScriptEditor = 11,
    ScriptSupervisor = 12,
    DialogueCoach = 13,

    // ─────────────────────────────
    // Camera Department
    // ─────────────────────────────
    DirectorOfPhotography = 20,
    CameraOperator = 21,
    FirstAssistantCamera = 22, // Focus Puller
    SecondAssistantCamera = 23, // Clapper / Loader
    DigitalImagingTechnician = 24, // DIT
    SteadicamOperator = 25,
    CameraLoader = 26,
    VideoAssistOperator = 27,

    // ─────────────────────────────
    // Lighting Department
    // ─────────────────────────────
    Gaffer = 30,
    BestBoyElectric = 31,
    LightingTechnician = 32,
    RiggingElectrician = 33,
    Spark = 34,
    LampOperator = 35,
    GeneratorOperator = 36,
    Electrician = 37,
    LightingDesigner = 38,

    // ─────────────────────────────
    // Grip Department
    // ─────────────────────────────
    KeyGrip = 40,
    BestBoyGrip = 41,
    Grip = 42,
    DollyGrip = 43,
    CraneOperator = 44,
    RiggingGrip = 45,
    SteadicamGrip = 46,

    // ─────────────────────────────
    // Art Department
    // ─────────────────────────────
    ProductionDesigner = 50,
    ArtDirector = 51,
    SetDecorator = 52,
    PropMaster = 53,
    PropsAssistant = 54,
    SetDresser = 55,
    ScenicArtist = 56,
    ConstructionCoordinator = 57,
    Leadman = 58,
    Carpenter = 59,

    // ─────────────────────────────
    // Costume & Makeup
    // ─────────────────────────────
    CostumeDesigner = 60,
    WardrobeSupervisor = 61,
    MakeupArtist = 62,
    HairStylist = 63,
    CostumeAssistant = 64,
    MakeupAssistant = 65,
    HairAssistant = 66,
    WigMaster = 67,
    SpecialEffectsMakeupArtist = 68,

    // ─────────────────────────────
    // Sound Department
    // ─────────────────────────────
    ProductionSoundMixer = 70,
    BoomOperator = 71,
    SoundRecordist = 72,
    SoundDesigner = 73,
    FoleyArtist = 74,
    SoundEditor = 75,
    ReRecordingMixer = 76,
    ADRSupervisor = 77,
    SoundEngineer = 78,

    // ─────────────────────────────
    // Post-Production
    // ─────────────────────────────
    Editor = 80,
    AssistantEditor = 81,
    Colorist = 82,
    VisualEffectsSupervisor = 83,
    VFXArtist = 84,
    PostProductionSupervisor = 85,
    TitlesDesigner = 86,

    // ─────────────────────────────
    // Music Department
    // ─────────────────────────────
    Composer = 90,
    MusicSupervisor = 91,
    MusicEditor = 92,
    Orchestrator = 93,
    Conductor = 94,
    Musician = 95,
    Singer = 96,
    SoundtrackCoordinator = 97,

    // ─────────────────────────────
    // Locations Department
    // ─────────────────────────────
    LocationManager = 100,
    AssistantLocationManager = 101,
    LocationScout = 102,
    LocationCoordinator = 103,

    // ─────────────────────────────
    // Logistics & Transportation
    // ─────────────────────────────
    LogisticsManager = 110,
    TransportCoordinator = 111,
    TransportationCaptain = 112,
    Driver = 113,
    TravelCoordinator = 114,
    AccommodationCoordinator = 115,
    EquipmentManager = 116,
    InventoryClerk = 117,

    // ─────────────────────────────
    // Set Operations & Support
    // ─────────────────────────────
    ProductionAssistant = 120,
    CateringManager = 121,
    CraftServices = 122,
    SecurityPersonnel = 123,
    SetMedic = 124,
    WeatherConsultant = 125,
    AnimalTrainer = 126,
    StuntCoordinator = 127,
    ExtrasCastingDirector = 128,
    ExtrasCoordinator = 129,

    // ─────────────────────────────
    // Misc / Fallback
    // ─────────────────────────────
    Other = 999
}