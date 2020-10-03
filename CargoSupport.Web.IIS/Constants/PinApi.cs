namespace CargoSupport.Constants
{
    public static class PinApi
    {
        public static string GetOrder(int id)
        {
            return $"https://ica.pindeliver.com/api/v1_2/Order/get/{id}";
        }

        public static string GetRoute(int id)
        {
            return $"https://ica.pindeliver.com/api/v1_2/Route/get/{id}";
        }
    }
}