using CreateBody;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCreateBloc
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
