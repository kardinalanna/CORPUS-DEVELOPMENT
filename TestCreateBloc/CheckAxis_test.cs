using CreateBody;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace TestCreateBloc
{
    [TestClass]
    public class CheckAxis_test
    {
        HelpMethods helpMethods = new HelpMethods();
        CreateBlockCapsule createBlockCapsule = new CreateBlockCapsule();


        [DataTestMethod]
        [DataRow("x", 1)]
        [DataRow("y", 2)]
        [DataRow("z", 3)]
        [DataRow("X", 1)]
        [DataRow("Y", 2)]
        [DataRow("Z", 3)]
        public void test_CheckAxis_correct(string c, int resAxis)
        {
            Assert.AreEqual(createBlockCapsule.CheckAxis(c), resAxis);
        }

        [DataTestMethod]
        [DataRow("o", 1)]
        [DataRow("lala", 2)]
        [DataRow("", 3)]
        [DataRow("-10", 1)]
        [DataRow("L", 2)]
        [DataRow("LALA", 3)]
        public void test_CheckAxis_exeption(string c, int resAxis)
        {
            try
            {
                createBlockCapsule.CheckAxis(c);
            }
            catch (Exception ioex)
            {
                Assert.AreEqual(ioex.Message, "Incorrect axis of symmetry for holes. Valid are x, y, z, but specified " + c);
                return;
            }
            Assert.Fail("Expected was not thrown");;
        }
    }
}
