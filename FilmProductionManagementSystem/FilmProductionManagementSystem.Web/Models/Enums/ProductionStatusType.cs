namespace FilmProductionManagementSystem.Web.Models.Enums;

public enum ProductionStatusType
{
    // Early development
    Concept = 1, // Initial idea, pitch phase
    Development = 2, // Script writing, financing, planning

    // Pre-production
    Preproduction = 3, // Casting, locations, scheduling
    Financing = 4, // Budget secured / in progress
    Casting = 5, // Actors selection
    LocationScouting = 6, // Finding and securing locations
    Rehearsals = 7, // Actor rehearsals

    // Production
    Production = 8, // Active shooting
    OnHold = 9, // Temporarily paused
    Reshoots = 10, // Additional shooting after main unit

    // Post-production
    PostProduction = 11, // Editing, sound, VFX
    PictureLock = 12, // Final edit locked
    SoundDesign = 13, // Sound editing & design
    ColorGrading = 14, // Color correction / grading
    VisualEffects = 15, // VFX production
    MusicComposition = 16, // Score composition

    // Distribution & release
    Marketing = 17, // Trailers, posters, festivals
    Distribution = 18, // Sales, delivery to distributors
    FestivalCircuit = 19, // Film festivals run
    Released = 20, // Public release

    // Final states
    Completed = 21, // Fully finished and archived
    Cancelled = 22 // Production cancelled
}