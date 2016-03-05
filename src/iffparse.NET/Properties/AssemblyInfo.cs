using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Allgemeine Informationen über eine Assembly werden über die folgenden 
// Attribute gesteuert. Ändern Sie diese Attributwerte, um die Informationen zu ändern,
// die mit einer Assembly verknüpft sind.

[assembly: AssemblyTitle("iffparse.NET")]
[assembly: AssemblyDescription("interchange file format parse library")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Andreas Beck")]
[assembly: AssemblyProduct("iffparse.NET")]
[assembly: AssemblyCopyright("Copyright © Andreas Beck 2012")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Versionsinformationen für eine Assembly bestehen aus den folgenden vier Werten:
//
//      Hauptversion
//      Nebenversion 
//      Buildnummer
//      Revision
//
// Sie können alle Werte angeben oder die standardmäßigen Build- und Revisionsnummern 
// übernehmen, indem Sie "*" eingeben:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.1.*")]
[assembly: AssemblyFileVersion("0.1.0.0")]

[assembly: CLSCompliant(true)]

// for unit tests
[assembly: InternalsVisibleTo( "ListTest" )] 
