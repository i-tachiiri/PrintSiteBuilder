using MathGenerator.DataTransferObject;

namespace MathGenerator.Services
{
    public class SampleGetter
    {
        public List<List<QuestionObject>> GetSamplePairs(List<List<QuestionObject>> values, int sampleSize = 1000)
        {
            if (values.Count <= sampleSize) return values;
            var random = new Random(0);
            var sampledValues = values.ToList();
            for (int i = 0; i < sampleSize; i++)
            {
                int j = random.Next(i, sampledValues.Count);
                (sampledValues[i], sampledValues[j]) = (sampledValues[j], sampledValues[i]);
            }
            return sampledValues.Take(sampleSize).ToList();
        }
    }
}
