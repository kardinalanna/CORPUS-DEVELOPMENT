using CreateBody;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceClaim.Api.V23.Geometry;

namespace TestCreateBloc
{
    [TestClass]
    public class MountPoint_test
    {
        HelpMethods helpMethods = new HelpMethods();
        CreateBlockCapsule createBlockCapsule = new CreateBlockCapsule();


        [TestMethod]
        public void test_MountPointClass_initialize()
        {
            Vector vector = Vector.Create(0, 0, 0);
            MountPoint mountPoint = new MountPoint(2, 4, vector);
            Assert.IsNotNull(mountPoint);
            Assert.IsNotNull(mountPoint.getRadius());
            Assert.AreEqual(mountPoint.getRadius(), 2);
            Assert.IsNotNull(mountPoint.getLength());
            Assert.AreEqual(mountPoint.getLength(), 4);
            Assert.IsNotNull(mountPoint.getLocation());
            Assert.IsTrue(helpMethods.equalsVector(mountPoint.getLocation(), vector));
            Assert.IsNotNull(mountPoint.getPointCount());
            Assert.AreEqual(mountPoint.getPointCount(), 1);
            Assert.IsNotNull(mountPoint.getAngle());
            Assert.AreEqual(mountPoint.getAngle(), 0);
            Assert.IsNotNull(mountPoint.getAxis());
            Assert.AreEqual(mountPoint.getAxis(), 0);
            Assert.IsNotNull(mountPoint.getPointSimetricAxis());
            Assert.AreEqual(mountPoint.getPointSimetricAxis(), 0);
            Assert.IsNotNull(mountPoint.getSimPointLocation());
            Assert.IsTrue(mountPoint.getSimPointLocation().Count == 0);
        }


        [TestMethod]
        public void test_addSimPointLocation_getSimPointLocation()
        {
            Vector vector = Vector.Create(2, 2, 2);
            MountPoint mountPoint = new MountPoint(2, 4, vector);
            Assert.AreEqual(mountPoint.getRadius(), 2);
            Assert.AreEqual(mountPoint.getLength(), 4);
            Assert.IsTrue(helpMethods.equalsVector(mountPoint.getLocation(), vector));

            List<Vector> vectorListRes = new List<Vector> {
             Vector.Create(1, 0, 0),
             Vector.Create(0, 1, 0),
             Vector.Create(0, 0, 1),
             Vector.Create(1, 2, 0),
             Vector.Create(0.001, 0.002, 0.2),
             Vector.Create(-30, -0.1, 0)
            };
            foreach (Vector v in vectorListRes)
                mountPoint.addSimPointLocation(v);

            Assert.AreEqual(mountPoint.getSimPointLocation().Count, vectorListRes.Count);

            for (int i = vectorListRes.Count - 1; i >= 0; i--)
            {
                Assert.IsTrue(helpMethods.equalsVector(mountPoint.getSimPointLocation()[i], vectorListRes[i]));
            }
            Assert.AreEqual(mountPoint.getPointCount(), 1);
            Assert.AreEqual(mountPoint.getAngle(), 0);
            Assert.AreEqual(mountPoint.getAxis(), 0);
            Assert.AreEqual(mountPoint.getPointSimetricAxis(), 0);
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(4)]
        public void test_setCountOfPoint_getPointCount(int count)
        {
            Vector vector = Vector.Create(2, 2, 2);
            MountPoint mountPoint = new MountPoint(2, 4, vector);
            Assert.AreEqual(mountPoint.getRadius(), 2);
            Assert.AreEqual(mountPoint.getLength(), 4);
            Assert.IsTrue(helpMethods.equalsVector(mountPoint.getLocation(), vector));
            Assert.AreEqual(mountPoint.getPointCount(), 1);
            mountPoint.setCountOfPoint(count);
            Assert.AreEqual(mountPoint.getPointCount(), count);
            Assert.AreEqual(mountPoint.getAngle(), 0);
            Assert.AreEqual(mountPoint.getAxis(), 0);
            Assert.AreEqual(mountPoint.getPointSimetricAxis(), 0);
        }

        [DataTestMethod]
        [DataRow(1, 90)]
        [DataRow(2, 140)]
        [DataRow(0, 90.78)]
        [DataRow(3, 90.78)]
        public void test_setRotation_getAxis_getAngle(int axis, double angle)
        {
            Vector vector = Vector.Create(2, 2, 2);
            MountPoint mountPoint = new MountPoint(2, 4, vector);
            Assert.AreEqual(mountPoint.getRadius(), 2);
            Assert.AreEqual(mountPoint.getLength(), 4);
            Assert.IsTrue(helpMethods.equalsVector(mountPoint.getLocation(), vector));
            Assert.AreEqual(mountPoint.getPointCount(), 1);
            Assert.AreEqual(mountPoint.getAngle(), 0);
            Assert.AreEqual(mountPoint.getAxis(), 0);
            Assert.AreEqual(mountPoint.getPointSimetricAxis(), 0);
            mountPoint.setRotation(angle, axis);
            Assert.AreEqual(mountPoint.getAngle(), angle);
            Assert.AreEqual(mountPoint.getAxis(), axis);
        }



        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(0)]
        [DataRow(3)]
        public void test_setPointSimetricAxis_getPointSimetricAxis(int axis)
        {
            Vector vector = Vector.Create(2, 2, 2);
            MountPoint mountPoint = new MountPoint(2, 4, vector);
            Assert.AreEqual(mountPoint.getPointSimetricAxis(), 0);
            mountPoint.setPointSimetricAxis(axis);
            Assert.AreEqual(mountPoint.getPointSimetricAxis(), axis);
            Assert.AreEqual(mountPoint.getRadius(), 2);
            Assert.AreEqual(mountPoint.getLength(), 4);
            Assert.IsTrue(helpMethods.equalsVector(mountPoint.getLocation(), vector));
            Assert.AreEqual(mountPoint.getPointCount(), 1);
            Assert.AreEqual(mountPoint.getAngle(), 0);
            Assert.AreEqual(mountPoint.getAxis(), 0);
        }
    }
}
