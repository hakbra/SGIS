/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 1.3.31
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */

namespace OSGeo.OGR {

using System;
using System.Runtime.InteropServices;

public class Feature : IDisposable {
  private HandleRef swigCPtr;
  protected bool swigCMemOwn;
  protected object swigParentRef;
  
  protected static object ThisOwn_true() { return null; }
  protected object ThisOwn_false() { return this; }

  public Feature(IntPtr cPtr, bool cMemoryOwn, object parent) {
    swigCMemOwn = cMemoryOwn;
    swigParentRef = parent;
    swigCPtr = new HandleRef(this, cPtr);
  }

  public static HandleRef getCPtr(Feature obj) {
    return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
  }
  public static HandleRef getCPtrAndDisown(Feature obj, object parent) {
    if (obj != null)
    {
      obj.swigCMemOwn = false;
      obj.swigParentRef = parent;
      return obj.swigCPtr;
    }
    else
    {
      return new HandleRef(null, IntPtr.Zero);
    }
  }
  public static HandleRef getCPtrAndSetReference(Feature obj, object parent) {
    if (obj != null)
    {
      obj.swigParentRef = parent;
      return obj.swigCPtr;
    }
    else
    {
      return new HandleRef(null, IntPtr.Zero);
    }
  }

  ~Feature() {
    Dispose();
  }

  public virtual void Dispose() {
  lock(this) {
      if(swigCPtr.Handle != IntPtr.Zero && swigCMemOwn) {
        swigCMemOwn = false;
        OgrPINVOKE.delete_Feature(swigCPtr);
      }
      swigCPtr = new HandleRef(null, IntPtr.Zero);
      swigParentRef = null;
      GC.SuppressFinalize(this);
    }
  }

  public Feature(FeatureDefn feature_def) : this(OgrPINVOKE.new_Feature(FeatureDefn.getCPtr(feature_def)), true, null) {
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

  public FeatureDefn GetDefnRef() {
    IntPtr cPtr = OgrPINVOKE.Feature_GetDefnRef(swigCPtr);
    FeatureDefn ret = (cPtr == IntPtr.Zero) ? null : new FeatureDefn(cPtr, false, ThisOwn_false());
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int SetGeometry(Geometry geom) {
    int ret = OgrPINVOKE.Feature_SetGeometry(swigCPtr, Geometry.getCPtr(geom));
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int SetGeometryDirectly(Geometry geom) {
    int ret = OgrPINVOKE.Feature_SetGeometryDirectly(swigCPtr, Geometry.getCPtrAndDisown(geom, ThisOwn_false()));
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public Geometry GetGeometryRef() {
    IntPtr cPtr = OgrPINVOKE.Feature_GetGeometryRef(swigCPtr);
    Geometry ret = (cPtr == IntPtr.Zero) ? null : new Geometry(cPtr, false, ThisOwn_false());
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int SetGeomField(int iField, Geometry geom) {
    int ret = OgrPINVOKE.Feature_SetGeomField__SWIG_0(swigCPtr, iField, Geometry.getCPtr(geom));
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int SetGeomField(string name, Geometry geom) {
    int ret = OgrPINVOKE.Feature_SetGeomField__SWIG_1(swigCPtr, name, Geometry.getCPtr(geom));
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int SetGeomFieldDirectly(int iField, Geometry geom) {
    int ret = OgrPINVOKE.Feature_SetGeomFieldDirectly__SWIG_0(swigCPtr, iField, Geometry.getCPtrAndDisown(geom, ThisOwn_false()));
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int SetGeomFieldDirectly(string name, Geometry geom) {
    int ret = OgrPINVOKE.Feature_SetGeomFieldDirectly__SWIG_1(swigCPtr, name, Geometry.getCPtrAndDisown(geom, ThisOwn_false()));
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public Geometry GetGeomFieldRef(int iField) {
    IntPtr cPtr = OgrPINVOKE.Feature_GetGeomFieldRef__SWIG_0(swigCPtr, iField);
    Geometry ret = (cPtr == IntPtr.Zero) ? null : new Geometry(cPtr, false, ThisOwn_false());
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public Geometry GetGeomFieldRef(string name) {
    IntPtr cPtr = OgrPINVOKE.Feature_GetGeomFieldRef__SWIG_1(swigCPtr, name);
    Geometry ret = (cPtr == IntPtr.Zero) ? null : new Geometry(cPtr, false, ThisOwn_false());
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public Feature Clone() {
    IntPtr cPtr = OgrPINVOKE.Feature_Clone(swigCPtr);
    Feature ret = (cPtr == IntPtr.Zero) ? null : new Feature(cPtr, true, ThisOwn_true());
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public bool Equal(Feature feature) {
    bool ret = OgrPINVOKE.Feature_Equal(swigCPtr, Feature.getCPtr(feature));
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int GetFieldCount() {
    int ret = OgrPINVOKE.Feature_GetFieldCount(swigCPtr);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public FieldDefn GetFieldDefnRef(int id) {
    IntPtr cPtr = OgrPINVOKE.Feature_GetFieldDefnRef__SWIG_0(swigCPtr, id);
    FieldDefn ret = (cPtr == IntPtr.Zero) ? null : new FieldDefn(cPtr, false, ThisOwn_false());
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public FieldDefn GetFieldDefnRef(string name) {
    IntPtr cPtr = OgrPINVOKE.Feature_GetFieldDefnRef__SWIG_1(swigCPtr, name);
    FieldDefn ret = (cPtr == IntPtr.Zero) ? null : new FieldDefn(cPtr, false, ThisOwn_false());
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int GetGeomFieldCount() {
    int ret = OgrPINVOKE.Feature_GetGeomFieldCount(swigCPtr);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public GeomFieldDefn GetGeomFieldDefnRef(int id) {
    IntPtr cPtr = OgrPINVOKE.Feature_GetGeomFieldDefnRef__SWIG_0(swigCPtr, id);
    GeomFieldDefn ret = (cPtr == IntPtr.Zero) ? null : new GeomFieldDefn(cPtr, false, ThisOwn_false());
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public GeomFieldDefn GetGeomFieldDefnRef(string name) {
    IntPtr cPtr = OgrPINVOKE.Feature_GetGeomFieldDefnRef__SWIG_1(swigCPtr, name);
    GeomFieldDefn ret = (cPtr == IntPtr.Zero) ? null : new GeomFieldDefn(cPtr, false, ThisOwn_false());
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public string GetFieldAsString(int id) {
        /* %typemap(csout) (const char *utf8_path) */
        IntPtr cPtr = OgrPINVOKE.Feature_GetFieldAsString__SWIG_0(swigCPtr, id);
        string ret = Ogr.Utf8BytesToString(cPtr);
        
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
        return ret;
}

  public string GetFieldAsString(string name) {
        /* %typemap(csout) (const char *utf8_path) */
        IntPtr cPtr = OgrPINVOKE.Feature_GetFieldAsString__SWIG_1(swigCPtr, name);
        string ret = Ogr.Utf8BytesToString(cPtr);
        
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
        return ret;
}

  public int GetFieldAsInteger(int id) {
    int ret = OgrPINVOKE.Feature_GetFieldAsInteger__SWIG_0(swigCPtr, id);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int GetFieldAsInteger(string name) {
    int ret = OgrPINVOKE.Feature_GetFieldAsInteger__SWIG_1(swigCPtr, name);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public double GetFieldAsDouble(int id) {
    double ret = OgrPINVOKE.Feature_GetFieldAsDouble__SWIG_0(swigCPtr, id);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public double GetFieldAsDouble(string name) {
    double ret = OgrPINVOKE.Feature_GetFieldAsDouble__SWIG_1(swigCPtr, name);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void GetFieldAsDateTime(int id, out int pnYear, out int pnMonth, out int pnDay, out int pnHour, out int pnMinute, out int pnSecond, out int pnTZFlag) {
    OgrPINVOKE.Feature_GetFieldAsDateTime(swigCPtr, id, out pnYear, out pnMonth, out pnDay, out pnHour, out pnMinute, out pnSecond, out pnTZFlag);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

  public int[] GetFieldAsIntegerList(int id, out int count) {
        /* %typemap(csout) int *intList */
        IntPtr cPtr = OgrPINVOKE.Feature_GetFieldAsIntegerList(swigCPtr, id, out count);
        int[] ret = new int[count];
        if (count > 0) {       
	        System.Runtime.InteropServices.Marshal.Copy(cPtr, ret, 0, count);
        }
        
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
        return ret;
}

  public double[] GetFieldAsDoubleList(int id, out int count) {
        /* %typemap(csout) int *intList */
        IntPtr cPtr = OgrPINVOKE.Feature_GetFieldAsDoubleList(swigCPtr, id, out count);
        double[] ret = new double[count];
        if (count > 0) {       
	        System.Runtime.InteropServices.Marshal.Copy(cPtr, ret, 0, count);
        }
        
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
        return ret;
}

  public string[] GetFieldAsStringList(int id) {
        /* %typemap(csout) char**options */
        IntPtr cPtr = OgrPINVOKE.Feature_GetFieldAsStringList(swigCPtr, id);
        IntPtr objPtr;
        int count = 0;
        if (cPtr != IntPtr.Zero) {
            while (Marshal.ReadIntPtr(cPtr, count*IntPtr.Size) != IntPtr.Zero)
                ++count;
        }
        string[] ret = new string[count];
        if (count > 0) {       
	        for(int cx = 0; cx < count; cx++) {
                objPtr = System.Runtime.InteropServices.Marshal.ReadIntPtr(cPtr, cx * System.Runtime.InteropServices.Marshal.SizeOf(typeof(IntPtr)));
                ret[cx]= (objPtr == IntPtr.Zero) ? null : System.Runtime.InteropServices.Marshal.PtrToStringAnsi(objPtr);
            }
        }
        
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
        return ret;
}

  public bool IsFieldSet(int id) {
    bool ret = OgrPINVOKE.Feature_IsFieldSet__SWIG_0(swigCPtr, id);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public bool IsFieldSet(string name) {
    bool ret = OgrPINVOKE.Feature_IsFieldSet__SWIG_1(swigCPtr, name);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int GetFieldIndex(string name) {
    int ret = OgrPINVOKE.Feature_GetFieldIndex(swigCPtr, name);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int GetGeomFieldIndex(string name) {
    int ret = OgrPINVOKE.Feature_GetGeomFieldIndex(swigCPtr, name);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int GetFID() {
    int ret = OgrPINVOKE.Feature_GetFID(swigCPtr);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int SetFID(int fid) {
    int ret = OgrPINVOKE.Feature_SetFID(swigCPtr, fid);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void DumpReadable() {
    OgrPINVOKE.Feature_DumpReadable(swigCPtr);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

  public void UnsetField(int id) {
    OgrPINVOKE.Feature_UnsetField__SWIG_0(swigCPtr, id);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

  public void UnsetField(string name) {
    OgrPINVOKE.Feature_UnsetField__SWIG_1(swigCPtr, name);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetField(int id, string value) {
    OgrPINVOKE.Feature_SetField__SWIG_0(swigCPtr, id, Ogr.StringToUtf8Bytes(value));
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetField(string name, string value) {
    OgrPINVOKE.Feature_SetField__SWIG_1(swigCPtr, name, Ogr.StringToUtf8Bytes(value));
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetField(int id, int value) {
    OgrPINVOKE.Feature_SetField__SWIG_2(swigCPtr, id, value);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetField(string name, int value) {
    OgrPINVOKE.Feature_SetField__SWIG_3(swigCPtr, name, value);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetField(int id, double value) {
    OgrPINVOKE.Feature_SetField__SWIG_4(swigCPtr, id, value);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetField(string name, double value) {
    OgrPINVOKE.Feature_SetField__SWIG_5(swigCPtr, name, value);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetField(int id, int year, int month, int day, int hour, int minute, int second, int tzflag) {
    OgrPINVOKE.Feature_SetField__SWIG_6(swigCPtr, id, year, month, day, hour, minute, second, tzflag);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetField(string name, int year, int month, int day, int hour, int minute, int second, int tzflag) {
    OgrPINVOKE.Feature_SetField__SWIG_7(swigCPtr, name, year, month, day, hour, minute, second, tzflag);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetFieldIntegerList(int id, int nList, int[] pList) {
    OgrPINVOKE.Feature_SetFieldIntegerList(swigCPtr, id, nList, pList);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetFieldDoubleList(int id, int nList, double[] pList) {
    OgrPINVOKE.Feature_SetFieldDoubleList(swigCPtr, id, nList, pList);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetFieldStringList(int id, string[] pList) {
    OgrPINVOKE.Feature_SetFieldStringList(swigCPtr, id, (pList != null)? new OgrPINVOKE.StringListMarshal(pList)._ar : null);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetFieldBinaryFromHexString(int id, string pszValue) {
    OgrPINVOKE.Feature_SetFieldBinaryFromHexString__SWIG_0(swigCPtr, id, pszValue);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetFieldBinaryFromHexString(string name, string pszValue) {
    OgrPINVOKE.Feature_SetFieldBinaryFromHexString__SWIG_1(swigCPtr, name, pszValue);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

  public int SetFrom(Feature other, int forgiving) {
    int ret = OgrPINVOKE.Feature_SetFrom(swigCPtr, Feature.getCPtr(other), forgiving);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int SetFromWithMap(Feature other, int forgiving, int nList, int[] pList) {
    int ret = OgrPINVOKE.Feature_SetFromWithMap(swigCPtr, Feature.getCPtr(other), forgiving, nList, pList);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public string GetStyleString() {
    string ret = OgrPINVOKE.Feature_GetStyleString(swigCPtr);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void SetStyleString(string the_string) {
    OgrPINVOKE.Feature_SetStyleString(swigCPtr, the_string);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

  public FieldType GetFieldType(int id) {
    FieldType ret = (FieldType)OgrPINVOKE.Feature_GetFieldType__SWIG_0(swigCPtr, id);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public FieldType GetFieldType(string name) {
    FieldType ret = (FieldType)OgrPINVOKE.Feature_GetFieldType__SWIG_1(swigCPtr, name);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

}

}