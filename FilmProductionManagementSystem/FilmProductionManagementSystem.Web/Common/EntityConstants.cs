namespace FilmProductionManagementSystem.Web.Common;

public static class EntityConstants
{
    public static class Production
    {
        public const int TitleMinLength = 2;
        public const int TitleMaxLength = 200;
        
        public const int DescriptionMaxLength = 2000;
        
        public const int ThumbnailMaxLength = 500;
        
        public const decimal BudgetMinValue = 0;
        public const decimal BudgetMaxValue = 100_000_000;
    }
    
    public static class Scene
    {
        public const int SceneNameMinLength = 2;
        public const int SceneNameMaxLength = 200;
        
        public const int LocationMinLength = 2;
        public const int LocationMaxLength = 300;
        
        public const int DescriptionMaxLength = 2000;
    }
    
    public static class ProductionAsset
    {
        public const int TitleMinLength = 2;
        public const int TitleMaxLength = 200;
        
        public const int DescriptionMaxLength = 500;
        
        public const int FilePathMaxLength = 500;
        
        public const int FileTypeMaxLength = 100;
    }
    
    public static class Crew
    {
        public const int ProfileImagePathMaxLength = 500;
        
        public const int FirstNameMinLength = 2;
        public const int FirstNameMaxLength = 150;
        
        public const int LastNameMinLength = 2;
        public const int LastNameMaxLength = 150;
        
        public const int NicknameMaxLength = 100;
        
        public const int BiographyMaxLength = 4000;
    }
    
    public static class Cast
    {
        public const int ProfileImagePathMaxLength = 500;
        
        public const int FirstNameMinLength = 2;
        public const int FirstNameMaxLength = 150;
        
        public const int LastNameMinLength = 2;
        public const int LastNameMaxLength = 150;
        
        public const int NicknameMaxLength = 100;
        
        public const int RoleMaxLength = 200;
        
        public const int BiographyMaxLength = 4000;
    }
    
    public static class Script
    {
        public const int TitleMinLength = 2;
        public const int TitleMaxLength = 200;
    }
    
    public static class ShootingDay
    {
        public const int NotesMaxLength = 400;
    }
}