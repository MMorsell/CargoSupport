namespace CargoSupport.Constants
{
    /// <summary>
    /// Application static string to get orders and routes easy
    /// </summary>
    public static class PinApi
    {
        /// <summary>
        /// Api "Get" order call
        /// </summary>
        /// <param name="id">Order to retrieve</param>
        /// <returns>Concatenated api call that requests the order with id <paramref name="id"/></returns>
        public static string GetOrder(int id)
        {
            return $"https://ica.pindeliver.com/api/v1_2/Order/get/{id}";
        }

        /// <summary>
        /// Api "Get" route call
        /// </summary>
        /// <param name="id">Route to retrieve</param>
        /// <returns>Concatenated api call that requests the route with id <paramref name="id"/></returns>
        public static string GetRoute(int id)
        {
            return $"https://ica.pindeliver.com/api/v1_2/Route/get/{id}";
        }
    }
}