
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
