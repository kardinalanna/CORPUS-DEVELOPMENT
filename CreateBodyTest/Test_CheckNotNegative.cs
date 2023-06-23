
using BasicTemplate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SpaceClaim.Api.V23.Scripting.Commands;
//using Assert = NUnit.Framework.Assert;

namespace BasicTemplateTest
{
    public class Tests
    {
        [TestClass]
        public class ClassInitialize
        {
            CreateBlockCapsule createBlockCapsule = new CreateBlockCapsule();


            [TestMethod]
            public void CheckNotNegative()
            {
                double expectResult = 3;
                Assert.AreEqual(createBlockCapsule.CheckNotNegative(3), expectResult);
            }

            [TestMethod]
            public void CheckNotNegative_double()
            {
                double expectResult = 0.3;
                Assert.AreEqual(createBlockCapsule.CheckNotNegative(0.3), expectResult);
            }

            [TestMethod]
            public void CheckNotNegativeException_double()
            {
                try
                {
                    createBlockCapsule.CheckNotNegative(-0.008);
                }
                catch (Exception ioex)
                {
                    return;
                }
                Assert.Fail("Expected Exception was not thrown");
            }
        }
    }
}