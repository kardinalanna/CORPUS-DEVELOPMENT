using CreateBody;
using Newtonsoft.Json;
using SpaceClaim;
using SpaceClaim.Api.V23.Geometry;
//using HelpMethods;

namespace TestCreateBloc
{
    [TestClass]
    public class ParserMainBox_test
    {
        CreateBlockCapsule createBlockCapsule = new CreateBlockCapsule();
        HelpMethods helpMethods = new HelpMethods();

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
            Assert.IsNotNull(createBlockCapsule.mainBoxSize);
            Assert.IsNotNull(createBlockCapsule.mainBoxLocation);
            Assert.IsTrue(createBlockCapsule.mainBoxSize.Count != 0);
            Assert.IsTrue(createBlockCapsule.mainBoxBounds.Count != 0);
        }

        [TestMethod]
        public void test_correct_fiels()
        {
            var myData = new
            {
                mainBoxSize = new double[] { 0.373, 0.2082, 0.160 },
                mainBoxLocation = new double[] { 0.0015, 0, 0.08 },
            };
            var data = JsonConvert.SerializeObject(myData);
            var parametrs = JsonConvert.DeserializeObject(data);

            createBlockCapsule.ParserMainBox(parametrs);

            Assert.AreEqual(helpMethods.equalsListDouble(createBlockCapsule.mainBoxSize, new List<double>() { 0.373, 0.2082, 0.160 }), true);
            Assert.IsTrue(helpMethods.equalsVector(createBlockCapsule.mainBoxLocation, Vector.Create(0.0015, 0, 0.08)));
        }

        [TestMethod]
        public void test_mainBoxSize_not_specified()
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
        public void test_mainBoxLocation_not_specified()
        {
            var myData = new
            {
                mainBoxSize = new double[] { 0.373, 0.2082, 0.160 },
            };
            var data = JsonConvert.SerializeObject(myData);
            var parametrs = JsonConvert.DeserializeObject(data);
            Vector expect = Vector.Create(0, 0, 0);
            createBlockCapsule.ParserMainBox(parametrs);
            Assert.AreEqual(createBlockCapsule.mainBoxLocation, expect);
        }

        [DataTestMethod]
        [DataRow(new double[] { 0.373, 0.2082 }, new double[] { 0.0015, 0, 0.08 })]
        [DataRow(new double[] { 0.373 }, new double[] { 0.0015, 0, 0.08 })]
        [DataRow(new double[] { }, new double[] { 0.0015, 0, 0.08 })]
        [DataRow(new double[] { 0.373, 0.2082, 0.09, 0.98 }, new double[] { 0.0015, 0, 0.08 })]
        public void test_mainBoxSize_have_not_3_paramatrs(double[] size, double[] loc)
        {
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
            }
            catch (Exception ioex)
            {
                Assert.IsTrue(ioex is Exception);
                return;
            }
            Assert.Fail("Expected was not thrown");
        }


        [DataTestMethod]
        [DataRow(new double[] { 0.373, 0.2082, 0 }, new double[] { 0.0015, 0 })]
        [DataRow(new double[] { 0.373, 0, 0 }, new double[] { 0.0015 })]
        [DataRow(new double[] { 8, 9.88, 0.45 }, new double[] { })]
        [DataRow(new double[] { 0.373, 0.2082, 0.09 }, new double[] { 0.0015, 0, 0.08, 0.98 })]
        public void test_mainBoxLocation_have_not_3_paramatrs(double[] size, double[] loc)
        {
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
            }
            catch (Exception ioex)
            {
                Assert.IsTrue(ioex is Exception);
                return;
            }
            Assert.Fail("Expected was not thrown");
        }


        [DataTestMethod]
        [DataRow(new double[] { 10, 12, 20 }, new double[] { 0, 0, 0 }, new double[] { 5, -5, 6, -6, 10, -10 })]
        [DataRow(new double[] { 0.10, 0.12, 0.20 }, new double[] { 0, 0, 0 }, new double[] { 0.5, -0.5, 0.6, -0.6, 0.10, -0.10 })]
        [DataRow(new double[] { 0.10, 0.12, 0.20 }, new double[] { 0.2, 0.2, 0.2 }, new double[] { 0.7, -0.3, 0.8, -0.4, 0.12, -0.8 })]
        [DataRow(new double[] { 0.10, 0.12, 0.20 }, new double[] { 0.2, 0.2, 0 }, new double[] { 0.7, -0.3, 0.8, -0.4, 0.10, -0.10 })]
        [DataRow(new double[] { 0.10, 0.12, 0.20 }, new double[] { -0.2, -0.2, 0}, new double[] { 0.3, -0.7, 0.4, -0.8, 0.10, -0.10 })]
        public void tets_mainBoxBounds(double[] size, double[] loc, double[] res)
        {
            var myData = new
            {
                mainBoxSize = size,
                mainBoxLocation = loc,
            };
            var data = JsonConvert.SerializeObject(myData);
            var parametrs = JsonConvert.DeserializeObject(data);
            createBlockCapsule.ParserMainBox(parametrs);
            Assert.IsTrue(helpMethods.equalsListDouble(createBlockCapsule.mainBoxBounds, res.ToList()));
        }
    }
}