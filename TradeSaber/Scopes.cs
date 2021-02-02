namespace TradeSaber
{
    public static class Scopes
    {
        public const string CreateCard = "create:card";
        public const string ManageCard = "manage:card";
        public const string DeleteCard = "delete:card";

        public const string CreateRarity = "create:rarity";
        public const string ManageRarity = "manage:rarity";
        public const string DeleteRarity = "delete:rarity";

        public const string CreateMutation = "create:mutation";
        public const string ManageMutation = "manage:mutation";
        public const string DeleteMutation = "delete:mutation";

        public const string CreateRole = "create:role";
        public const string ManageRole = "manage:role";

        public const string UploadFile = "upload:file";
        public const string ManageUser = "manage:user";


        public static readonly string[] AllScopes = new string[]
        {
            CreateCard,
            ManageCard,
            DeleteCard,

            CreateRarity,
            ManageRarity,
            DeleteRarity,

            CreateRole,
            ManageRole,

            UploadFile,
            ManageUser,
        };
    }
}