
namespace MazeAlgorithmTester
{
    public class Locator
    {
        private static int MIN_RSS_TO_COUNT = -75;
        private static int NEIGHBOUR_MIN_SCORE = -1;
        private static int RSS_OFFSET = 100;

        public PointF GetLocation(List<Fingerprint> fingerprintsDataSet, Fingerprint fingerprint)
        {
            double x = 0, y = 0;
            float weight;
            float weightSum = 0;

            PointF point = new PointF();

            List<Fingerprint> fingerprints = GetMarksWithSameAps2(fingerprintsDataSet, fingerprint);

            foreach (var mark in fingerprints)
            {
                float distance = Dissimilarity(fingerprint, mark);

                weight = 1 / distance;

                x += weight * mark.Center.X;
                y += weight * mark.Center.Y;
                weightSum += weight;
            }

            point.Set(x / weightSum, y / weightSum);

            return point;
        }

        public float Dissimilarity(Fingerprint actual, Fingerprint reference)
        {
            float difference = 0.0f;
            int distanceSq = 0;
            int bssidLevelDiff;

            if (actual == null || reference == null) return float.MaxValue;

            // Calculate Dissimilarity between signal strengths
            foreach (var mac in actual.KeySet())
            {
                if (reference.ContainsKey(mac))
                {
                    bssidLevelDiff = actual.Get(mac) - reference.Get(mac);
                }
                else
                {
                    bssidLevelDiff = actual.Get(mac) + RSS_OFFSET;
                }

                distanceSq += bssidLevelDiff * bssidLevelDiff;
            }

            foreach (var mac in reference.KeySet())
            {
                if (!actual.ContainsKey(mac))
                {
                    bssidLevelDiff = reference.Get(mac) + RSS_OFFSET;
                    distanceSq += bssidLevelDiff * bssidLevelDiff;
                }
            }

            difference = (float)Math.Sqrt(distanceSq);
            // division by zero handling:
            if (difference == 0.0f) difference = float.MinValue;
            return difference;
        }

        private List<Fingerprint> GetMarksWithSameAps2(List<Fingerprint> fingerprintsDataSet,
            Fingerprint fingerprint)
        {
            List<KeyValuePair<int, List<Fingerprint>>> fingerprintsByScore =
                new List<KeyValuePair<int, List<Fingerprint>>>();

            foreach (Fingerprint f in fingerprintsDataSet)
            {
                var score = CalculateScore(fingerprint, f);

                if (score > NEIGHBOUR_MIN_SCORE)
                {
                    List<Fingerprint> list = GetListByScore(fingerprintsByScore, score);
                    if (list == null)
                    {
                        list = new List<Fingerprint>();
                        fingerprintsByScore.Add(new KeyValuePair<int, List<Fingerprint>>(score, list));
                    }
                    list.Add(f);
                }
            }

            List<Fingerprint> bestFingerprints = new List<Fingerprint>();
            foreach (var goodFingerprints in fingerprintsByScore)
            {
                bestFingerprints.AddRange(goodFingerprints.Value);
                if (bestFingerprints.Count > 0) break;
            }

            return bestFingerprints;
        }

        private List<Fingerprint> GetListByScore(List<KeyValuePair<int, List<Fingerprint>>> fingerprintsByScore, int score)
        {
            List<Fingerprint> fingerprints = new List<Fingerprint>();

            foreach (var f in fingerprintsByScore)
            {
                if (f.Key == score)
                    fingerprints.AddRange(f.Value);
            }

            return fingerprints;
        }

        private int CalculateScore(Fingerprint fingerprint, Fingerprint f)
        {
            HashSet<string> fingerprintAps = GetApsWithMinRSS(fingerprint, MIN_RSS_TO_COUNT);
            HashSet<string> refFpAps = GetApsWithMinRSS(f, MIN_RSS_TO_COUNT);

            HashSet<string> intersection = new HashSet<string>(fingerprintAps); // use the copy constructor
            intersection = RetainAll(intersection, refFpAps);

            RemoveWhere(fingerprintAps, intersection);
            RemoveWhere(refFpAps, intersection);

            return intersection.Count() * 2 - fingerprintAps.Count() - refFpAps.Count();
        }

        private HashSet<string> RetainAll(HashSet<string> a, HashSet<string> b)
        {
            HashSet<string> retainedA = new HashSet<string>();

            foreach (var macA in a)
            {
                foreach (var macB in b)
                {
                    if (macA == macB)
                    {
                        retainedA.Add(macA);
                        break;
                    }
                }
            }
            return retainedA;
        }

        private void RemoveWhere(HashSet<string> a, HashSet<string> b)
        {
            foreach (var macB in b)
            {
                foreach (var macA in a)
                {
                    if (macA == macB)
                    {
                        a.Remove(macA);
                        break;
                    }
                }
            }
        }

        private HashSet<string> GetApsWithMinRSS(Fingerprint fingerprint, int minRSS)
        {
            HashSet<string> strongAps = new HashSet<string>();
            HashSet<string> weakAps = new HashSet<string>();

            if (fingerprint == null)
                return strongAps;

            foreach (string mac in fingerprint.KeySet())
            {
                if (fingerprint.Get(mac) > minRSS)
                {
                    strongAps.Add(mac);
                }
                else
                {
                    weakAps.Add(mac);
                }
            }

            // If not enough strong Aps, use weak as well
            if (strongAps.Count() < 3)
            {
                foreach (string mac in weakAps)
                {
                    strongAps.Add(mac);
                }
            }
            return strongAps;
        }

        public class PointF
        {
            double x, y;

            public void Set(double x, double y)
            {
                this.x = x;
                this.y = y;
            }
        }
    }
}
