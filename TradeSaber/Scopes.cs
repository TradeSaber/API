namespace TradeSaber
{
    public static class Scopes
    {
        public const string Mascot = "mascot:descriptor";

        public const string CreatePack = "create:pack";
        public const string ManagePack = "manage:pack";
        public const string DeletePack = "delete:pack";

        public const string CreateCard = "create:card";
        public const string ManageCard = "manage:card";
        public const string DeleteCard = "delete:card";

        public const string CreateSeries = "create:series";
        public const string ManageSeries = "manage:series";
        public const string DeleteSeries = "delete:series";

        public const string CreateRarity = "create:rarity";
        public const string ManageRarity = "manage:rarity";
        public const string DeleteRarity = "delete:rarity";

        public const string CreateMutation = "create:mutation";
        public const string ManageMutation = "manage:mutation";
        public const string DeleteMutation = "delete:mutation";

        public const string CreateObjective = "create:objective";
        public const string ManageObjective = "manage:objective";
        public const string DeleteObjective = "delete:objective";

        public const string CreateRole = "create:role";
        public const string ManageRole = "manage:role";

        public const string UploadFile = "upload:file";
        public const string ManageUser = "manage:user";


        public static readonly string[] AllScopes = new string[]
        {
            Mascot,

            CreatePack,
            ManagePack,
            DeletePack,

            CreateCard,
            ManageCard,
            DeleteCard,

            CreateSeries,
            ManageSeries,
            DeleteSeries,

            CreateRarity,
            ManageRarity,
            DeleteRarity,

            CreateMutation,
            ManageMutation,
            DeleteMutation,

            CreateObjective,
            ManageObjective,
            DeleteObjective,

            CreateRole,
            ManageRole,

            UploadFile,
            ManageUser,
        };
    }
}