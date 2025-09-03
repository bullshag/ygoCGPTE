using UnityEngine;

namespace UnityClient
{
    [CreateAssetMenu(fileName = "ServerConfig", menuName = "Config/Server Config")]
    public class ServerConfig : ScriptableObject
    {
        [Header("Database hosts")]
        public string host = "76.134.86.9";
        public string kimHost = "10.0.0.30";

        [Header("API")]
        public string apiBaseUrl = "https://localhost:5001";
    }
}
