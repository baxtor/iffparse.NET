#r @"src/packages/FAKE/tools/FakeLib.dll"
open Fake
open System.IO

// Properties
let product = "iffparse.NET"
let authors = [ "Andreas Beck" ]
let copyright = "Copyright © 2012-2017 Andreas Beck"
let company = ""
let description = "iffparse.NET is a port of the AMIGA iffparse.library for .NET"

let buildDir = "./build/"
let testDir = "./tests/"

//targets
open Fake.RestorePackageHelper
Target "RestorePackages" (fun _ -> 
     "./src/iffparse.NET.sln"
     |> RestoreMSSolutionPackages (fun p ->
         { p with
             OutputPath = "./src/packages"
             Retries = 4 })
 )

Target "Clean" <| fun _ ->
    DeleteDir buildDir
    
Target "Build" <| fun _ ->

    !!"src/iffparse.NET.sln"
    |> MSBuildRelease "" "Rebuild"
    |> ignore

let Exec command args =
    let result = Shell.Exec(command, args)
    if result <> 0 then failwithf "%s exited with error %d" command result

Target "BuildMono" <| fun _ ->

//    !!"src/iffparse.NET.sln"
//    |> MSBuild "" "Rebuild" [("Configuration","Release Mono")]
//    |> ignore
    Exec "xbuild" "./src/iffparse.NET.sln /t:Build /tv:12.0 /v:m  /p:RestorePackages='False' /p:Configuration='Release' /logger:Fake.MsBuildLogger+ErrorLogger,'./src/packages/FAKE/tools/FakeLib.dll'"

Target "BuildRelease" DoNothing

let count label glob =
    let (fileCount, lineCount) =
        !! glob
        |> Seq.map (fun path ->
            File.ReadLines(path) |> Seq.length)
        |> Seq.fold (fun (fileCount, lineCount) lineNum -> (fileCount+1, lineCount + lineNum)) (0, 0)
    printfn "%s - File Count: %i, Line Count: %i." label fileCount lineCount

Target "RunStatistics" (fun _ ->
    count "F# Source" "src/**/*.fs"
    count "C# Source" "src/**/*.cs"
    count "F# Test" "tests/**/*.fs"
    count "C# Test" "tests/**/*.cs"
)

//--------------------------------------------------------------------------------
// Help 
//--------------------------------------------------------------------------------

Target "Help" <| fun _ ->
    List.iter printfn [
      "usage:"
      "build [target]"
      ""
      " Targets for building:"
      " * Build      Builds"
      " * Nuget      Create and optionally publish nugets packages"
      " * RunTests   Runs tests"
      " * MultiNodeTests  Runs the slower multiple node specifications"
      " * All        Builds, run tests, creates and optionally publish nuget packages"
      ""
      " Other Targets"
      " * Help       Display this help" 
      " * HelpNuget  Display help about creating and pushing nuget packages" 
      " * HelpDocs   Display help about creating and pushing API docs"
      " * HelpMultiNodeTests  Display help about running the multiple node specifications"
      ""]

Target "HelpNuget" <| fun _ ->
    List.iter printfn [
      "usage: "
      "build Nuget [nugetkey=<key> [nugetpublishurl=<url>]] "
      "            [symbolskey=<key> symbolspublishurl=<url>] "
      "            [nugetprerelease=<prefix>]"
      ""
      "Arguments for Nuget target:"
      "   nugetprerelease=<prefix>   Creates a pre-release package."
      "                              The version will be version-prefix<date>"
      "                              Example: nugetprerelease=dev =>"
      "                                       0.6.3-dev1408191917"
      ""
      "In order to publish a nuget package, keys must be specified."
      "If a key is not specified the nuget packages will only be created on disk"
      "After a build you can find them in bin/nuget"
      ""
      "For pushing nuget packages to nuget.org and symbols to symbolsource.org"
      "you need to specify nugetkey=<key>"
      "   build Nuget nugetKey=<key for nuget.org>"
      ""
      "For pushing the ordinary nuget packages to another place than nuget.org specify the url"
      "  nugetkey=<key>  nugetpublishurl=<url>  "
      ""
      "For pushing symbols packages specify:"
      "  symbolskey=<key>  symbolspublishurl=<url> "
      ""
      "Examples:"
      "  build Nuget                      Build nuget packages to the bin/nuget folder"
      ""
      "  build Nuget nugetprerelease=dev  Build pre-release nuget packages"
      ""
      "  build Nuget nugetkey=123         Build and publish to nuget.org and symbolsource.org"
      ""
      "  build Nuget nugetprerelease=dev nugetkey=123 nugetpublishurl=http://abc"
      "              symbolskey=456 symbolspublishurl=http://xyz"
      "                                   Build and publish pre-release nuget packages to http://abc"
      "                                   and symbols packages to http://xyz"
      ""]

// build dependencies
//"Clean" ==> "AssemblyInfo" ==> "RestorePackages" ==> "Build" ==> "CopyOutput" ==> "BuildRelease"
// build dependencies
"Clean" ==> "RestorePackages" ==> "RunStatistics" ==> "Build" ==> "BuildRelease"
"Clean" ==> "RestorePackages" ==> "RunStatistics" ==> "BuildMono" ==> "BuildRelease"

Target "All" DoNothing
"BuildRelease" ==> "All"
//"RunTests" ==> "All"
//"MultiNodeTests" ==> "All"
//"NBench" ==> "All"
//"Nuget" ==> "All"

RunTargetOrDefault "All"