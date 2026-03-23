namespace Wrap.GCommon;

public static class OutputMessages
{
    // verb ending with -ed for Success Messages
    public const string CreatedMessage = "created";
    public const string AddedMessage = "added";
    public const string UpdatedMessage = "updated";
    public const string RemovedMessage = "removed";
    public const string DeletedMessage = "deleted";
    
    // verb ending with -ing for Failure Messages
    public const string CreatingMessage = "creating";
    public const string AddingMessage = "adding";
    public const string UpdatingMessage = "updating";
    public const string RemovingMessage = "removing";
    public const string DeletingMessage = "deleting";
    public const string DetailsMessage = "visualing the details of";

    // role strings
    public const string CrewString = "Crew";
    public const string CastString = "Cast";
    public const string EmptyString = "";
    
    // photo file extension and size limit
    public const string NotSupportedFileExtension = "The file extension {0} is not supported.";
    public const string ExceededFileSizeLimit = "The file size limit {0} exceeded.";
    
    // validation attribute messages
    public const string IsAfterExceptionMessage = "The date must be after {0}";
    public static class Assets
    {
        
    }

    public static class Home
    {
        
    }
    
    public static class Production
    {
        public const string NotFoundMessage = "Production's ID: {0} - is not found!";
        public const string IdIsNullOrEmptyMessage = "Production's ID: {0} is in wrong format, null or empty.";

        public const string LoadingProductionErrorMessage = "Error loading the production with ID: {0}.";
        public const string LoadingProductionErrorMessageWithException = "Error loading the production with ID: {0}. {1}";
        
        public const string LoadingManyProductionsErrorMessage = "Error loading productions.";
        public const string LoadingManyProductionsErrorMessageWithException = "Error loading productions. {0}";
        
        public const string CrudSuccessMessage = "Production is {0} successfully!";
        public const string CrudFailureMessage = "Exception occured while {0} the project. {1}";
    }

    public static class Profile
    {
        public const string UserNotIdentifiedMessage = "Invalid user identifier.";
        public const string UsernameIsNullOrEmptyMessage = "Username is null or empty.";
        public const string UserNotFoundMessage = "User with username: {0} is NOT found as crew or cast";
        public const string ErrorFindingUserMessage = "Exception occured while trying to find your profile. {0}";

        public const string DataExceptionMessage = "Exception occured while trying to get the data of your profile. {0}";
        
        public const string ErrorLoadingProfileMessage = "Error loading profile: {0}";
        public const string ErrorLoadingEditorMessage = "Error loading editor for profile: {0}";
        public const string ErrorLoadingSkillsMessage = "Error loading skills in editor: {0}";
        public const string ErrorUpdatingProfile = "Error updating profile: {0}";

        public const string NoSkillsSelected = "At least one skill must be selected.";

        public const string UpdateProfileSuccessMessage = "Profile updated successfully!";
        public const string UpdateSkillsSuccessMessage = "Skills updated successfully!";
        
        public const string CrewNotFoundMessage = "Crew member with username '{0}' not found.";
        public const string CastNotFoundMessage = "Cast member with username '{0}' not found.";
    }
    
    public static class Register
    {
        public const string CrewDraftKey = "CrewDraft";
        public const string ErrorBuildingCrewDraft = "Exception occured while building the crew draft {0}";
        public const string ErrorFoundingCrewDraft = "Error occured while finding the crew draft.";
        public const string ErrorCreatingCast = "Error occured while getting the cast data to create profile.";

        public const string ExceptionCompleteRegistrationOfCrewMessage = "Exception occured while trying to complete your registration as a filmmaker. {0}";
        public const string ExceptionCompleteRegistrationOfCastMessage = "Exception occured while trying to complete your registration as an actor. {0}";
        public const string ExceptionLogin = "Exception occured while trying to login. {0}";

        public const string IdentityCreateFailed = "Identity CreateAsync failed: {0}";
        public const string EffectedDbRowsFailure = "Expected >= {0}, got {1}.";
        public const string UserNotFound = "User with username '{0}' not found.";
        public const string LoginFailedRole = "Login failed due to wrong role!";
        public const string LoginFailedPass = "Login failed: invalid password for '{0}'.";
        public const string RegistrationTransactionFailure = "Registration failed while saving profile data.";
        public const string NotRegisteredAsCrew = "This account is not registered as Crew.";
        public const string NotRegisteredAsCast = "This account is not registered as Cast.";
        public const string InvalidUsernameOrPassword = "Invalid username or password.";
        public const string NotSelectedRole = "Please select a role to continue.";
        public const string NoSelectedSkills = "Please select at least one skill.";

        public const string SuccessMessage = "Registration successful! Welcome to Wrap!";
    }
    
    public static class Scene
    {
        
    }
    
    public static class Schedule
    {
        
    }
    
    public static class Scripts
    {
        
    }
    
    public static class Search
    {
        
    }
    
    public static class NavBar
    {
        public const string UserNotFoundMessage = "Unable to find user with ID: {0}";

        public const string UserIsNull = "User is null or empty.";
        public const string ModelIsNull = "ViewModel is null or empty. Please try again later.";
    }
}