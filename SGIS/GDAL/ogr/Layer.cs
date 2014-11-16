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

public class Layer : IDisposable {
  private HandleRef swigCPtr;
  protected bool swigCMemOwn;
  protected object swigParentRef;
  
  protected static object ThisOwn_true() { return null; }
  protected object ThisOwn_false() { return this; }

  public Layer(IntPtr cPtr, bool cMemoryOwn, object parent) {
    swigCMemOwn = cMemoryOwn;
    swigParentRef = parent;
    swigCPtr = new HandleRef(this, cPtr);
  }

  public static HandleRef getCPtr(Layer obj) {
    return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
  }
  public static HandleRef getCPtrAndDisown(Layer obj, object parent) {
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
  public static HandleRef getCPtrAndSetReference(Layer obj, object parent) {
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

  public virtual void Dispose() {
  lock(this) {
      if(swigCPtr.Handle != IntPtr.Zero && swigCMemOwn) {
        swigCMemOwn = false;
        throw new MethodAccessException("C++ destructor does not have public access");
      }
      swigCPtr = new HandleRef(null, IntPtr.Zero);
      swigParentRef = null;
      GC.SuppressFinalize(this);
    }
  }

  public int GetRefCount() {
    int ret = OgrPINVOKE.Layer_GetRefCount(swigCPtr);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void SetSpatialFilter(Geometry filter) {
    OgrPINVOKE.Layer_SetSpatialFilter__SWIG_0(swigCPtr, Geometry.getCPtr(filter));
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetSpatialFilterRect(double minx, double miny, double maxx, double maxy) {
    OgrPINVOKE.Layer_SetSpatialFilterRect__SWIG_0(swigCPtr, minx, miny, maxx, maxy);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetSpatialFilter(int iGeomField, Geometry filter) {
    OgrPINVOKE.Layer_SetSpatialFilter__SWIG_1(swigCPtr, iGeomField, Geometry.getCPtr(filter));
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetSpatialFilterRect(int iGeomField, double minx, double miny, double maxx, double maxy) {
    OgrPINVOKE.Layer_SetSpatialFilterRect__SWIG_1(swigCPtr, iGeomField, minx, miny, maxx, maxy);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

  public Geometry GetSpatialFilter() {
    IntPtr cPtr = OgrPINVOKE.Layer_GetSpatialFilter(swigCPtr);
    Geometry ret = (cPtr == IntPtr.Zero) ? null : new Geometry(cPtr, false, ThisOwn_false());
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int SetAttributeFilter(string filter_string) {
    int ret = OgrPINVOKE.Layer_SetAttributeFilter(swigCPtr, Ogr.StringToUtf8Bytes(filter_string));
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void ResetReading() {
    OgrPINVOKE.Layer_ResetReading(swigCPtr);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

  public string GetName() {
    string ret = OgrPINVOKE.Layer_GetName(swigCPtr);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public wkbGeometryType GetGeomType() {
    wkbGeometryType ret = (wkbGeometryType)OgrPINVOKE.Layer_GetGeomType(swigCPtr);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public string GetGeometryColumn() {
    string ret = OgrPINVOKE.Layer_GetGeometryColumn(swigCPtr);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public string GetFIDColumn() {
    string ret = OgrPINVOKE.Layer_GetFIDColumn(swigCPtr);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public Feature GetFeature(int fid) {
    IntPtr cPtr = OgrPINVOKE.Layer_GetFeature(swigCPtr, fid);
    Feature ret = (cPtr == IntPtr.Zero) ? null : new Feature(cPtr, true, ThisOwn_true());
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public Feature GetNextFeature() {
    IntPtr cPtr = OgrPINVOKE.Layer_GetNextFeature(swigCPtr);
    Feature ret = (cPtr == IntPtr.Zero) ? null : new Feature(cPtr, true, ThisOwn_true());
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int SetNextByIndex(int new_index) {
    int ret = OgrPINVOKE.Layer_SetNextByIndex(swigCPtr, new_index);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int SetFeature(Feature feature) {
    int ret = OgrPINVOKE.Layer_SetFeature(swigCPtr, Feature.getCPtr(feature));
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int CreateFeature(Feature feature) {
    int ret = OgrPINVOKE.Layer_CreateFeature(swigCPtr, Feature.getCPtr(feature));
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int DeleteFeature(int fid) {
    int ret = OgrPINVOKE.Layer_DeleteFeature(swigCPtr, fid);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int SyncToDisk() {
    int ret = OgrPINVOKE.Layer_SyncToDisk(swigCPtr);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public FeatureDefn GetLayerDefn() {
    IntPtr cPtr = OgrPINVOKE.Layer_GetLayerDefn(swigCPtr);
    FeatureDefn ret = (cPtr == IntPtr.Zero) ? null : new FeatureDefn(cPtr, false, ThisOwn_false());
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int GetFeatureCount(int force) {
    int ret = OgrPINVOKE.Layer_GetFeatureCount(swigCPtr, force);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int GetExtent(Envelope extent, int force) {
    int ret = OgrPINVOKE.Layer_GetExtent(swigCPtr, Envelope.getCPtr(extent), force);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public bool TestCapability(string cap) {
    bool ret = OgrPINVOKE.Layer_TestCapability(swigCPtr, cap);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int CreateField(FieldDefn field_def, int approx_ok) {
    int ret = OgrPINVOKE.Layer_CreateField(swigCPtr, FieldDefn.getCPtr(field_def), approx_ok);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int DeleteField(int iField) {
    int ret = OgrPINVOKE.Layer_DeleteField(swigCPtr, iField);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int ReorderField(int iOldFieldPos, int iNewFieldPos) {
    int ret = OgrPINVOKE.Layer_ReorderField(swigCPtr, iOldFieldPos, iNewFieldPos);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int ReorderFields(int nList, int[] pList) {
    int ret = OgrPINVOKE.Layer_ReorderFields(swigCPtr, nList, pList);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int AlterFieldDefn(int iField, FieldDefn field_def, int nFlags) {
    int ret = OgrPINVOKE.Layer_AlterFieldDefn(swigCPtr, iField, FieldDefn.getCPtr(field_def), nFlags);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int CreateGeomField(GeomFieldDefn field_def, int approx_ok) {
    int ret = OgrPINVOKE.Layer_CreateGeomField(swigCPtr, GeomFieldDefn.getCPtr(field_def), approx_ok);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int StartTransaction() {
    int ret = OgrPINVOKE.Layer_StartTransaction(swigCPtr);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int CommitTransaction() {
    int ret = OgrPINVOKE.Layer_CommitTransaction(swigCPtr);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int RollbackTransaction() {
    int ret = OgrPINVOKE.Layer_RollbackTransaction(swigCPtr);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int FindFieldIndex(string pszFieldName, int bExactMatch) {
    int ret = OgrPINVOKE.Layer_FindFieldIndex(swigCPtr, pszFieldName, bExactMatch);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public OSGeo.OSR.SpatialReference GetSpatialRef() {
    IntPtr cPtr = OgrPINVOKE.Layer_GetSpatialRef(swigCPtr);
    OSGeo.OSR.SpatialReference ret = (cPtr == IntPtr.Zero) ? null : new OSGeo.OSR.SpatialReference(cPtr, true, ThisOwn_true());
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public long GetFeaturesRead() {
    long res = OgrPINVOKE.Layer_GetFeaturesRead(swigCPtr);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return res;
}

  public int SetIgnoredFields(string[] options) {
    int ret = OgrPINVOKE.Layer_SetIgnoredFields(swigCPtr, (options != null)? new OgrPINVOKE.StringListMarshal(options)._ar : null);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int Intersection(Layer method_layer, Layer result_layer, string[] options, Ogr.GDALProgressFuncDelegate callback, string callback_data) {
    int ret = OgrPINVOKE.Layer_Intersection(swigCPtr, Layer.getCPtr(method_layer), Layer.getCPtr(result_layer), (options != null)? new OgrPINVOKE.StringListMarshal(options)._ar : null, callback, callback_data);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int Union(Layer method_layer, Layer result_layer, string[] options, Ogr.GDALProgressFuncDelegate callback, string callback_data) {
    int ret = OgrPINVOKE.Layer_Union(swigCPtr, Layer.getCPtr(method_layer), Layer.getCPtr(result_layer), (options != null)? new OgrPINVOKE.StringListMarshal(options)._ar : null, callback, callback_data);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int SymDifference(Layer method_layer, Layer result_layer, string[] options, Ogr.GDALProgressFuncDelegate callback, string callback_data) {
    int ret = OgrPINVOKE.Layer_SymDifference(swigCPtr, Layer.getCPtr(method_layer), Layer.getCPtr(result_layer), (options != null)? new OgrPINVOKE.StringListMarshal(options)._ar : null, callback, callback_data);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int Identity(Layer method_layer, Layer result_layer, string[] options, Ogr.GDALProgressFuncDelegate callback, string callback_data) {
    int ret = OgrPINVOKE.Layer_Identity(swigCPtr, Layer.getCPtr(method_layer), Layer.getCPtr(result_layer), (options != null)? new OgrPINVOKE.StringListMarshal(options)._ar : null, callback, callback_data);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int Update(Layer method_layer, Layer result_layer, string[] options, Ogr.GDALProgressFuncDelegate callback, string callback_data) {
    int ret = OgrPINVOKE.Layer_Update(swigCPtr, Layer.getCPtr(method_layer), Layer.getCPtr(result_layer), (options != null)? new OgrPINVOKE.StringListMarshal(options)._ar : null, callback, callback_data);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int Clip(Layer method_layer, Layer result_layer, string[] options, Ogr.GDALProgressFuncDelegate callback, string callback_data) {
    int ret = OgrPINVOKE.Layer_Clip(swigCPtr, Layer.getCPtr(method_layer), Layer.getCPtr(result_layer), (options != null)? new OgrPINVOKE.StringListMarshal(options)._ar : null, callback, callback_data);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int Erase(Layer method_layer, Layer result_layer, string[] options, Ogr.GDALProgressFuncDelegate callback, string callback_data) {
    int ret = OgrPINVOKE.Layer_Erase(swigCPtr, Layer.getCPtr(method_layer), Layer.getCPtr(result_layer), (options != null)? new OgrPINVOKE.StringListMarshal(options)._ar : null, callback, callback_data);
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public StyleTable GetStyleTable() {
    IntPtr cPtr = OgrPINVOKE.Layer_GetStyleTable(swigCPtr);
    StyleTable ret = (cPtr == IntPtr.Zero) ? null : new StyleTable(cPtr, false, ThisOwn_false());
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void SetStyleTable(StyleTable table) {
    OgrPINVOKE.Layer_SetStyleTable(swigCPtr, StyleTable.getCPtr(table));
    if (OgrPINVOKE.SWIGPendingException.Pending) throw OgrPINVOKE.SWIGPendingException.Retrieve();
  }

}

}
