using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Proj4CSharp;
using System.Windows.Forms;

namespace SGIS
{
    class PrjReader
    {
        // reads .prj files
        public static IProjection read(string filename)
        {
            // reads all text in file
            string input = File.ReadAllText(filename);
            string proj4 = null;

            OSGeo.OSR.SpatialReference oSRS = new OSGeo.OSR.SpatialReference("");
            string[] ESRI = {input};
            // converts text to spatial reference
            oSRS.ImportFromESRI(ESRI);
            // converts spatial reference to proj4-representation
            oSRS.ExportToProj4(out proj4);

            // returns proj4 srs object
            return Proj4CSharp.Proj4CSharp.ProjectionFactory(proj4);
        }
    }
}
