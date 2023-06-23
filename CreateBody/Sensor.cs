using SpaceClaim.Api.V23.Geometry;
using System;
using System.Collections.Generic;

namespace CreateBody
{
    public class Sensor
    {
        //Default parameters fo sensor
        string name;
        List<double> size;
        double load;
        Vector location;
        double angle = 0;
        int axis = 0;
        string shape;

        public Sensor(string name, double load, List<double> size, Vector location, string shape)
        {
            this.name = name;
            this.load = load;
            this.size = size;
            this.location = location;
            this.shape= shape;

        }
        public void setRotation(double angle, int axic)
        {
            this.axis= axic;
            this.angle = angle;
        }

        public String getName()
        {
            return name;
        }

        public string getShape()
        {
            return shape;
        }

        public List<double> getSize()
        {
            return size;
        }

        public Vector getLocation()
        {
            return location;
        }
        
        public double getAngle()
        {
            return angle;
        }

        public int getAxis()
        {
            return axis;
        }

        public double getLoad()
        {
            return load;
        }      
    }
}
