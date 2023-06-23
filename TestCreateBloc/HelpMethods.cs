using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceClaim.Api.V23.Geometry;

namespace TestCreateBloc
{
    public class HelpMethods
    {
        public bool equalsVector(Vector v1, Vector v2)
        {
            if (v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z)
                return true;
            return false;
        }
        public bool equalsListDouble(List<double> v1, List<double> v2)
        {
            if (v1.Count != v2.Count)
                return false;
            else
            {
                for (int i = v1.Count - 1; i > 0; i--)
                {
                    if (v1[i] != v2[i])
                        return false;
                }
            }
            return true;
        }
    }
}
