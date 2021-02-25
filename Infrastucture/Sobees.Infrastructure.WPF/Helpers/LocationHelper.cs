using System;
//using LocationApiLib;
using Sobees.Tools.Logging;

namespace Sobees.Infrastructure.Helpers
{
  public class LocationProvider //: ILocationEvents
  {
    //// Private

    //#region REPORT_TYPE enum

    //public enum REPORT_TYPE
    //{
    //  Error,
    //  LatLong,
    //  CivicAddress
    //}

    //#endregion

    //private readonly LocationClass loc;
    //public CIVIC_ADDRESS CivicAddress;
    //public LAT_LONG LatLong;
    //private Guid civicaddressType = new Guid(0xC0B19F70, 0x4ADF, 0x445d, 0x87, 0xF2, 0xCA, 0xD8, 0xFD, 0x71, 0x17, 0x92);
    //private bool fReporting;

    //private Guid latlongType = new Guid(0x7FED806D, 0x0EF8, 0x4f07, 0x80, 0xAC, 0x36, 0xA0, 0xBE, 0xAE, 0x31, 0x34);

    //public LocationProvider()
    //{
    //  // Initialize
    //  loc = new LocationClass();

    //  LocationChangedHandler = null;
    //  StatusChangedHandler = null;

    //  fReporting = false;
    //}

    //public IntPtr hWnd { get; set; }
    //public DateTime Timestamp { get; set; }

    //#region ILocationEvents Members

    //public void OnLocationChanged(ref Guid reportType, ILocationReport pLocationReport)
    //{
    //  if (reportType == latlongType)
    //  {
    //    // Save Latitude, Longitude, and Time
    //    LatLong.Latitude = ((ILatLongReport) loc.GetReport(ref latlongType)).GetLatitude();
    //    LatLong.Longitude = ((ILatLongReport) loc.GetReport(ref latlongType)).GetLongitude();
    //    _SYSTEMTIME time = ((ILatLongReport) loc.GetReport(ref latlongType)).GetTimestamp();
    //    Timestamp = new DateTime(time.wYear, time.wMonth, time.wDay, time.wHour, time.wMinute, time.wSecond);

    //    if (LocationChangedHandler != null)
    //      LocationChangedHandler(this, new EventArgs());
    //  }
    //}

    //public void OnStatusChanged(ref Guid reportType, LOCATION_REPORT_STATUS newStatus)
    //{
    //  if (reportType == latlongType || reportType == civicaddressType)
    //  {
    //    // Do something...
    //    if (StatusChangedHandler != null)
    //      StatusChangedHandler(this, new EventArgs());
    //  }
    //}

    //#endregion

    //public event EventHandler LocationChangedHandler;
    //public event EventHandler StatusChangedHandler;

    //public bool IsReporting()
    //{
    //  return fReporting;
    //}

    //public bool IsLatLongAvailable()
    //{
    //  bool ret = false;
    //  LOCATION_REPORT_STATUS status;

    //  status = loc.GetReportStatus(ref latlongType);

    //  switch (status)
    //  {
    //      // REPORT_ACCESS_DENIED is counted as available
    //      // since RequestPermission can change the status to REPORT_RUNNING
    //    case LOCATION_REPORT_STATUS.REPORT_ACCESS_DENIED:
    //    case LOCATION_REPORT_STATUS.REPORT_INITIALIZING:
    //    case LOCATION_REPORT_STATUS.REPORT_RUNNING:
    //      ret = true;
    //      break;

    //    default:
    //      ret = false;
    //      break;
    //  }

    //  return (ret);
    //}

    //public REPORT_TYPE GetCurrentLocation(bool requestPermission)
    //{
    //  REPORT_TYPE ret = REPORT_TYPE.Error;
    //  LOCATION_REPORT_STATUS status;

    //  status = loc.GetReportStatus(ref latlongType);

    //  if (status == LOCATION_REPORT_STATUS.REPORT_RUNNING)
    //  {
    //    LatLong.Latitude = ((ILatLongReport) loc.GetReport(ref latlongType)).GetLatitude();
    //    LatLong.Longitude = ((ILatLongReport) loc.GetReport(ref latlongType)).GetLongitude();
    //    _SYSTEMTIME time = ((ILatLongReport) loc.GetReport(ref latlongType)).GetTimestamp();
    //    Timestamp = new DateTime(time.wYear, time.wMonth, time.wDay, time.wHour, time.wMinute, time.wSecond);

    //    ret = REPORT_TYPE.LatLong;
    //  }
    //  else
    //  {
    //    status = loc.GetReportStatus(ref civicaddressType);

    //    if (status == LOCATION_REPORT_STATUS.REPORT_RUNNING)
    //    {
    //      CivicAddress.AddressLine1 = ((ICivicAddressReport) loc.GetReport(ref civicaddressType)).GetAddressLine1();
    //      CivicAddress.AddressLine2 = ((ICivicAddressReport) loc.GetReport(ref civicaddressType)).GetAddressLine2();
    //      CivicAddress.City = ((ICivicAddressReport) loc.GetReport(ref civicaddressType)).GetCity();
    //      CivicAddress.StateProvince = ((ICivicAddressReport) loc.GetReport(ref civicaddressType)).GetStateProvince();
    //      CivicAddress.PostalCode = ((ICivicAddressReport) loc.GetReport(ref civicaddressType)).GetPostalCode();
    //      CivicAddress.CountryRegion = ((ICivicAddressReport) loc.GetReport(ref civicaddressType)).GetCountryRegion();
    //      _SYSTEMTIME time = ((ICivicAddressReport) loc.GetReport(ref civicaddressType)).GetTimestamp();
    //      Timestamp = new DateTime(time.wYear, time.wMonth, time.wDay, time.wHour, time.wMinute, time.wSecond);

    //      ret = REPORT_TYPE.CivicAddress;
    //    }
    //  }

    //  return ret;
    //}

    //public void StartLatLongReport()
    //{
    //  LOCATION_REPORT_STATUS status = loc.GetReportStatus(ref latlongType);

    //  if (status == LOCATION_REPORT_STATUS.REPORT_ACCESS_DENIED)
    //  {
    //    //loc.RequestPermissions(hWnd, ref latlongType, 1, 1);
    //  }

    //  try
    //  {
    //    loc.RegisterForReport(this, ref latlongType, 1000);
    //  }
    //  catch (Exception ex)
    //  {
    //    TraceHelper.Trace(this,
    //                      ex.Message.Contains("The service is already registered.")
    //                        ? "The service is already registered."
    //                        : ex.Message);
    //  }
    //  fReporting = true;
    //}

    //public void StopLatLongReporting()
    //{
    //  loc.UnregisterForReport(ref latlongType);
    //  fReporting = false;
    //}

    //#region Nested type: CIVIC_ADDRESS

    //public struct CIVIC_ADDRESS
    //{
    //  public string AddressLine1 { get; internal set; }
    //  public string AddressLine2 { get; internal set; }
    //  public string City { get; internal set; }
    //  public string StateProvince { get; internal set; }
    //  public string PostalCode { get; internal set; }
    //  public string CountryRegion { get; internal set; }
    //}

    //#endregion

    //#region Nested type: LAT_LONG

    //public struct LAT_LONG
    //{
    //  public double Latitude { get; internal set; }
    //  public double Longitude { get; internal set; }
    //}

    //#endregion
  }

  //public class LocationHelper
  //{
  //  private static bool _loadproviderthrowsexception;
  //  private static LatLongReport _provider;
  //  private static Location loc { get; set; }

  //  public static Location GetLocation()
  //  {
  //    if (loc == null)
  //    {
  //      var l = new Location();

  //      //Try grabbing the Win7 location
  //      try
  //      {
  //        if (_loadproviderthrowsexception)
  //        {
  //          return null;
  //        }

  //        if (_provider == null)
  //        {
  //          _provider = new LatLongReport();


  //          l.Latitude = _provider.GetLatitude();
  //          l.Longitude = _provider.GetLongitude();
  //          loc = l;
  //          return l;
  //        }
  //      }
  //      catch
  //        (Exception
  //          ex)
  //      {
  //        _loadproviderthrowsexception = true;
  //        return null;
  //      }
  //    }
  //    else
  //      return loc;

  //    return null;
  //  }
  //}

  #region Nested type: Location

  public class Location
  {
    public double Latitude { get; set; }
    public double Longitude { get; set; }
  }

  #endregion
}