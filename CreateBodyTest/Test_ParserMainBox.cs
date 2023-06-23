using BasicTemplate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SpaceClaim.Api.V23.Scripting.Commands;
//using Assert = NUnit.Framework.Assert;
//[assembly: TestDataSourceDiscovery(TestDataSourceDiscoveryOption.DuringExecution)]

namespace BasicTemplateTest
{
    public class Test_ParserMainBox
    {
        [TestClass]
        public class ClassInitialize
        {
            CreateBlockCapsule createBlockCapsule = new CreateBlockCapsule();
            //static dynamic parametrs = new Dictionary<string, List<double>>() { { "mainBoxSize", new List<double>() { 0.373, 0.2082, 0.160 } }, { "mainBoxLocation", new List<double>() { 0.0015, 0, 0.08 } } };


            //[DataTestMethod]
            //[DynamicData(nameof(getParametrs))]
            [TestMethod]
            public void FilesNotNull()
            {
                var myData = new
                {
                    mainBoxSize = new double[] { 0.373, 0.2082, 0.160 },
                    mainBoxLocation = new double[] { 0.0015, 0, 0.08 },
                };
                var data = JsonConvert.SerializeObject(myData);
                var parametrs = JsonConvert.DeserializeObject(data);

                createBlockCapsule.ParserMainBox(parametrs);
                Assert.IsTrue(createBlockCapsule.mainBoxSize.Count != 0);
                Assert.IsTrue(createBlockCapsule.mainBoxLocation != null);
                Assert.IsTrue(createBlockCapsule.mainBoxBounds.Count != 0);
            }

            [TestMethod]
            public void mainBoxSize_not_specified()
            {
                var myData = new
                {
                    mainBoxLocation = new double[] { 0.0015, 0, 0.08 },
                };
                var data = JsonConvert.SerializeObject(myData);
                var parametrs = JsonConvert.DeserializeObject(data);
                try
                {
                    createBlockCapsule.ParserMainBox(parametrs);
                }
                catch (Exception ioex)
                {
                    Assert.IsTrue(ioex is Exception);
                    return;
                }
                Assert.Fail("Expected Exception was not thrown");
            }

            [TestMethod]
            public void mainBoxLocation_not_specified()
            {
                var myData = new
                {
                    mainBoxLocation = new double[] { 0.0015, 0, 0.08 },
                };
                var data = JsonConvert.SerializeObject(myData);
                var parametrs = JsonConvert.DeserializeObject(data);
                try
                {
                    createBlockCapsule.ParserMainBox(parametrs);
                }
                catch (Exception ioex)
                {
                    Assert.IsTrue(ioex is Exception);
                    return;
                }
                Assert.Fail("Expected Exception was not thrown");
            }

            [DataTestMethod]
            [DynamicData(nameof(getParametrs), DynamicDataSourceType.Method)]
            public void mainBoxSize_have_not_3_paramatrs(double[] size, double[] loc)
            {
                Assert.IsTrue(size != null);
                var myData = new
                {
                    mainBoxSize = size,
                    mainBoxLocation = loc,
                };
                var data = JsonConvert.SerializeObject(myData);
                var parametrs = JsonConvert.DeserializeObject(data);
                try
                {
                    createBlockCapsule.ParserMainBox(parametrs);
                    Assert.Fail("Expected was not thrown");
                }
                catch (Exception ioex)
                {
                    Assert.IsTrue(ioex is Exception);
                    return;
                }
            }


            private static IEnumerable<object?[]> getParametrs()
            {
                yield return new object?[] { new double[] { 0.373, 0.2082, 0 }, new double[] { 0.0015, 0, 0.08 } };
                yield return new object?[] { new double[] { 0.373 }, new double[] { 0.0015, 0, 0.08 } };
                yield return new object?[] { new double[] { }, new double[] { 0.0015, 0, 0.08 } };
                yield return new object?[] { new double[] { 0.373, 0.2082, 0.09, 0.98 }, new double[] { 0.0015, 0, 0.08 } };

            }
        }
    }
}
