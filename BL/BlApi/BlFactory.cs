namespace BlApi
{
    public static class BlFactory
    {
        public static BlApi.IBL GetBL()
        {
            return BL.BL.Instance;
        }
    }
}
