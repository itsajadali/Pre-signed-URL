using System.Text;

namespace StorageService.Utilities
{
    public static class Extensions
    {
        public static string BuildQueryString(this Dictionary<string, string> queryStringParameters)
        {
            if (queryStringParameters == null || queryStringParameters.Count == 0)
                return string.Empty;


            StringBuilder queryString = new();

            queryString.Append('?');

            foreach (var parameter in queryStringParameters)
            {
                if (string.IsNullOrEmpty(parameter.Value))
                    continue;

                queryString.Append(parameter.Key)
                           .Append('=')
                           .Append(Uri.EscapeDataString(parameter.Value))
                           .Append('&');
            }

            if (queryString[^1] == '&')
                queryString.Length--;

            return queryString.ToString();
        }
    }
}
