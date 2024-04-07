using Newtonsoft.Json.Linq;

namespace MazeAlgorithmTester
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Fingerprint> fingerprints = new List<Fingerprint>();

            Locator locator = new Locator();

            JsonParseFingerprints(fingerprints, "radio_map.json");

            var firstFinger = fingerprints[0];
            fingerprints.RemoveAt(0);

            var point = locator.GetLocation(fingerprints, firstFinger);

            Console.WriteLine(firstFinger.Center.X + " " + firstFinger.Center.Y);
            Console.WriteLine(point.X + " " + point.Y);
        }

        private static void JsonParseFingerprints(List<Fingerprint> fingerprints, string jsonFilePath)
        {
            try
            {
                // Read the JSON file
                string jsonText = File.ReadAllText(jsonFilePath);

                // Parse the JSON
                JArray jsonArray = JArray.Parse(jsonText);

                // Iterate through each object in the array
                foreach (JObject jsonObject in jsonArray)
                {
                    // Extract CLASSNAME and INSTANCE properties
                    string className = (string)jsonObject["CLASSNAME"];
                    JObject instance = (JObject)jsonObject["INSTANCE"];

                    // Extract INSTANCE properties
                    JObject wifiFingerprint = (JObject)instance["mWiFiFingerprint"];
                    JObject center = (JObject)instance["mCenter"];
                    double radius = (double)instance["mRadius"];
                    int color = (int)instance["mColor"];
                    JArray color4f = (JArray)instance["mColor4f"];
                    bool isRemoved = (bool)instance["mIsRemoved"];


                    Fingerprint fingerprint = new Fingerprint() { };
                    double.TryParse(center["x"].ToString(), out double centerX);
                    double.TryParse(center["y"].ToString(), out double centerY);
                    fingerprint.Center.X = centerX;
                    fingerprint.Center.Y = centerY;
                    fingerprint.Radius = radius;
                    fingerprint.Color = color;
                    fingerprint.IsRemoved = isRemoved;


                    //Console.WriteLine("WiFi Fingerprint:");
                    foreach (var item in wifiFingerprint)
                    {
                        MacAndValue macAndValue = new MacAndValue(item.Key, (int)item.Value);
                        fingerprint.Instance.MacsAndValues.Add(macAndValue);
                    }

                    foreach (int item in color4f)
                    {
                        fingerprint.Color4f.Add(item);
                    }

                    fingerprints.Add(fingerprint);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
