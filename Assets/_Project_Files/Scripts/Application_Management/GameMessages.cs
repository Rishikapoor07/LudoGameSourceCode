public static class GameMessages
{
    public const string NotEnoughCoins = "You do not have enough coins to play the match. Please buy coins from shop!";
    public const string SyncProfile = "Do you want to sync the guest data with this device?";
    public const string FBLogInCancelled = "Login Cancelled!";
    public const string SomethingWentWrong = "Something went wrong, Please try again.";
    public const string DisconnectionMessage = "You lost the game due to disconnection.";
    public const string GamePlayExitMessage = "Do you really want to exit from game?";
    public const string QuitGameMessage = "Do you really want to quit the game?";
    public const string SendQueryMessage = "Your message has been sent successfully. We will contact you soon.";
    public const string ItemPurchasedMessage = "Are you sure, you want to purchase this item?";
    public const string ItemPurchasedSuccessfull  = "Item purchased successfully.";
    public const string RemoveFriendMessage = "Are you sure, you want to remove this friend?";
    public const string LogOutMessage = "Do you really want to logout?";
    public const string NoInternetConnection = "Internet is not working, Please check your internet connection.";
    public const string DummyData = "Internet is not working, your game data will not be updated.";
    public const string KickedOutOfRoom = "You have been kicked out of the room due to inactivity.";
    public const string KickedOutOfRoomDueToDisconnect = "You have been kicked out of the room due to disconnection.";
    public const string Quit = "Are you sure, you want to quit?";
    public const string EmptyRoomName = "Please enter a valid room name.";
    public const string InvalidRoom = "Room not found.";
    public const string CustomRoomAlreadyExist = "Room already exists, please enter another name.";
    public const string RoomIsFull = "Room already full.";
    public const string NoPlayerAvailableToStartGame = "No player available to play, you won the game.";
    public static string EnterTextMessage(string inputfieldType) => "Please enter " + inputfieldType + ".";
    public static string GemsCoinsPurchasedMessage(string _type) => "Are you sure, you want to purchase this " + _type + "?";
    public static string PackPurchasedSuccessfull(string _type)  => _type + " purchased successfully.";
    public static string MinimumLengthTitleMessage(string inputFieldName, int minLength) => inputFieldName + " should be at least " + minLength + " characters long.";
    public static string MinimumLengthQuerryMessageMessage(string inputFieldName, int minLength) => inputFieldName + " should be at least " + minLength + " characters long.";
    public static string PlayerDicconnetedInGameMessage(string nameOfDisconnectedPlayer) => nameOfDisconnectedPlayer+" disconnected.";
}