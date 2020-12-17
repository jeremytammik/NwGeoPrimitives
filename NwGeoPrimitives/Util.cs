using Autodesk.Navisworks.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NwGeoPrimitives
{
  class Util
  {
    #region Formatting
    /// <summary>
    /// Return an English plural suffix for the given
    /// number of items, i.e. 's' for zero or more
    /// than one, and nothing for exactly one.
    /// </summary>
    public static string PluralSuffix( int n )
    {
      return 1 == n ? "" : "s";
    }

    /// <summary>
    /// Return an English plural suffix 'ies' or
    /// 'y' for the given number of items.
    /// </summary>
    public static string PluralSuffixY( int n )
    {
      return 1 == n ? "y" : "ies";
    }

    /// <summary>
    /// Return a dot (full stop) for zero
    /// or a colon for more than zero.
    /// </summary>
    public static string DotOrColon( int n )
    {
      return 0 < n ? ":" : ".";
    }

    /// <summary>
    /// Return a string for a real number
    /// formatted to two decimal places.
    /// </summary>
    public static string RealString( double a )
    {
      return a.ToString( "0.##" );
    }

    /// <summary>
    /// Return a string for a UV point
    /// or vector with its coordinates
    /// formatted to two decimal places.
    /// </summary>
    public static string PointString(
      Point2D p,
      bool onlySpaceSeparator = false )
    {
      string format_string = onlySpaceSeparator
        ? "{0} {1}"
        : "({0},{1})";

      return string.Format( format_string,
        RealString( p.X ),
        RealString( p.Y ) );
    }

    /// <summary>
    /// Return a string for an XYZ point
    /// or vector with its coordinates
    /// formatted to two decimal places.
    /// </summary>
    public static string PointString(
      Point3D p,
      bool onlySpaceSeparator = false )
    {
      string format_string = onlySpaceSeparator
        ? "{0} {1} {2}"
        : "({0},{1},{2})";

      return string.Format( format_string,
        RealString( p.X ),
        RealString( p.Y ),
        RealString( p.Z ) );
    }

    /// <summary>
    /// Return a string for this bounding box
    /// with its coordinates formatted to two
    /// decimal places.
    /// </summary>
    public static string BoundingBoxString(
      BoundingBox3D bb,
      bool onlySpaceSeparator = false )
    {
      string format_string = onlySpaceSeparator
        ? "{0} {1}"
        : "({0},{1})";

      return string.Format( format_string,
        PointString( bb.Min, onlySpaceSeparator ),
        PointString( bb.Max, onlySpaceSeparator ) );
    }
    #endregion // Formatting
  }
}
