
namespace MazeAlgorithmTester
{
    public class Fingerprint
    {
        public WifiFingerprint Instance = new WifiFingerprint();

        public Point Center = new Point();

        public double Radius;

        public int Color;

        public List<int> Color4f = new List<int>();

        public bool IsRemoved;

        public List<string> KeySet()
        {
            List<string> keys = new List<string>();

            foreach (var macAndValue in Instance.MacsAndValues)
            {
                keys.Add(macAndValue.Mac);
            }

            return keys;
        }

        public int Get(string mac)
        {
            foreach (var macAndValue in Instance.MacsAndValues)
            {
                if (mac == macAndValue.Mac)
                    return macAndValue.Signal;
            }

            return Int32.MaxValue;
        }

        internal bool ContainsKey(string mac)
        {
            foreach (var macAndValue in Instance.MacsAndValues)
            {
                if (macAndValue.Mac == mac)
                    return true;
            }

            return false;
        }
    }

    public class WifiFingerprint
    {
        public List<MacAndValue> MacsAndValues = new List<MacAndValue>();
    }

    public class MacAndValue
    {
        public string Mac;
        public int Signal;

        public MacAndValue(string mac, int signal)
        {
            Mac = mac;
            Signal = signal;
        }
    }

    public class Point
    {
        public double X;
        public double Y;
    }
}
