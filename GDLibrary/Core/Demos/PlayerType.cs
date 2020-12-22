namespace GDLibrary
{
    public enum PlayerType : sbyte
    {
        Sniper,
        Medic,
        Builder,
        AirSupport,
        Unassigned
    }

    public enum SubscriptionType : sbyte
    {
        Movies = 1,
        Sports = 2,
        Kids = 4,
        Documentaries = 8,
        News = 16,
        None = 0
    }

    /*
    //PlayerType.Medic
    Movies =>   0000 0001
    News =>     0001 0000
    Kids =>     0000 0100
    ----------------------
                0001 0101 (21)

    What does the int (21) correspond to? Does this person have News setup?

                0001 0101 (Jane/John)
                0001 0000 (News)
   ------------------------------
                0001 0000 != 0 => News is enabled

    setUpAccount(SubscriptionType.Movies | SubscriptionType.News | SubscriptionType.Kids);
    setUpAccount(SubscriptionType.Sports)
    setUpAccount(SubscriptionType.None);

    public void setUpAccount(SubscriptionType type)
    {
    }
    */
}