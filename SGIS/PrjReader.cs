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
        public static IProjection read(string filename)
        {
            string input = File.ReadAllText(filename);
            string proj4 = null;

            OSGeo.OSR.SpatialReference oSRS = new OSGeo.OSR.SpatialReference("");
            string[] ESRI = {input};
            oSRS.ImportFromESRI(ESRI);
            oSRS.ExportToProj4(out proj4);

            return Proj4CSharp.Proj4CSharp.ProjectionFactory(proj4);
        }
    }
}
