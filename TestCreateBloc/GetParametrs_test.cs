//using CreateBody;
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using SpaceClaim.Api.V23.Geometry;

//namespace TestCreateBloc
//{
//    [TestClass]
//    public class GetParametrs_test
//    {
//        CreateBlockCapsule createBlockCapsule = new CreateBlockCapsule();
        
//        [TestMethod]
//        public void test_GetParametrs_Initialaize()
//        {
//            HelpMethods helpMethods = new HelpMethods();
//            string file = "\\parametrs_for_test.json";

//            //var parametrs = new
//            //{

//            //mainBoxSize = new double[] {10, 10, 10 },
//            //mainBoxLocation = new double[] { 0.0, 0, 0 },

//            //sensors = new[] {
//            //        new {
//            //            name = "Name",
//            //            size = new double[]{ 1, 2, 3,},
//            //            location = new double[]{ 0, 2, 0},
//            //            shape = "box",
//            //        },
//            //                            new {
//            //            name = "Name",
//            //            size = new double[]{ 1, 2, 3,},
//            //            location = new double[]{ -0.5, -0.5, -1},
//            //            shape = "box",
//            //        }
//            //    },
//            //    mountPoints = new[] {
//            //        new {
//            //            radius = 0.002,
//            //            location = new double[] { -1, -2, 0 },
//            //            length = 0.003,
//            //            count =  new object[] { 1 },
//            //            rotation = new object[] { 90, "x"}
//            //        }, 
//            //        new
//            //        {
//            //            radius = 0.002,
//            //            location = new double[] { 2, 0.9, 0.16 },
//            //            length = 0.03,
//            //            count =  new object[] { 2, "y" },
//            //            rotation = new object[] { 90, "x"}
//            //        }
//            //    },

//            //    restrictors = new[] {
//            //        new {
//            //            size = new double[] { 10, 5, 5 },
//            //            location = new double[] { 0, 0, 2.5 },
//            //        },
//            //        new {
//            //            size = new double[] { 2, 2, 10 },
//            //            location = new double[] { -1, -2, 0 },
//            //        }
//            //    }
//            //};
//            Assert.IsTrue(helpMethods.equalsVector(createBlockCapsule.mainBoxLocation, Vector.Create(0,0,0)));
//            Assert.AreEqual(createBlockCapsule.mainBoxSize.Count, 0);
//            Assert.AreEqual(createBlockCapsule.mainBoxBounds.Count, 0);
//            Assert.AreEqual(createBlockCapsule.mountPoints.Count, 0);
//            Assert.AreEqual(createBlockCapsule.sensorList.Count, 0);
//            Assert.AreEqual(createBlockCapsule.restrictorsList.Count, 0);
//            Assert.AreEqual(createBlockCapsule.material, "Plastic, PP (homopolymer, low flow)");
//            Assert.AreEqual(createBlockCapsule.number, 1);

//            createBlockCapsule.GetParametrs(file);
//            Assert.IsTrue(helpMethods.equalsVector(createBlockCapsule.mainBoxLocation, Vector.Create(0, 0, 0)));
//            Assert.AreEqual(createBlockCapsule.mountPoints.Count, 2);
//            Assert.AreEqual(createBlockCapsule.sensorList.Count, 2);
//            Assert.AreEqual(createBlockCapsule.restrictorsList.Count, 2);


//        }
//    }
//}
