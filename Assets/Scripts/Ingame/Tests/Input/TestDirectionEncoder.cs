using FMLib.Structs;
using NUnit.Framework;
using Rogue.Ingame.Input;

namespace Rogue.Ingame.Tests.Input
{
    public class TestDirectionEncoder
    {
        [TestCase(0.01f, 1f, ExpectedResult = 1)]
        [TestCase(-0.01f, 1f, ExpectedResult = 1)]
        [TestCase(0f, -1f, ExpectedResult = 31)]
        [TestCase(1f, 0f, ExpectedResult = 16)]
        [TestCase(-1f, 0f, ExpectedResult = 46)]
        [TestCase(0f, 0.2f, ExpectedResult = 0)]
        [TestCase(0f, 0.4f, ExpectedResult = 1)]
        public int TestEncode(float x, float z)
        {
            return DirectionEncoder.Encode(new VectorXZ(x, z));
        }


        [TestCase(0, 0f, 0f)]
        [TestCase(1, 0f, 1f)]
        [TestCase(16, 1f, 0f)]
        [TestCase(31, 0f, -1f)]
        [TestCase(46, -1f, 0f)]
        public void TestDecode(int value, float x, float z)
        {
            var result = DirectionEncoder.Decode(value);
            Assert.AreEqual(x, result.x, 0.001f);
            Assert.AreEqual(z, result.z, 0.001f);
        }
    }
}