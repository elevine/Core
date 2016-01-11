// include Fake lib
#r @"tools\FAKE\tools\FakeLib.dll"

open System.IO
open Fake

// Properties
let artifactsDir = @"./artifacts/"
let artifactsBuildDir = @"./artifacts/build/"
let artifactsNuGetDir = @"./artifacts/nuget/"
let androidProject = @"./src/Castle.Core-Android/Castle.Core-Android.csproj"
let androidNuspec = @"./buildscripts/Castle.Core-Android.nuspec"
let projects =  [androidProject]

// Targets

Target "Clean" (fun _ ->
    trace "Cleanup..."
    
    CleanDirs [artifactsDir]
)

Target "Build" (fun _ ->
   trace "Building..."
   
   MSBuildRelease artifactsBuildDir "Build" projects
   |> Log "AppBuild-Output: "
)

Target "PackDroid" (fun _ ->
    trace "Packaging for Xamarin.Android ..."

    CreateDir artifactsNuGetDir
    
    NuGet (fun p -> 
        {p with
            Authors = ["Castle Project Contributors"]
            Project = "Castle.Core"                              
            OutputPath = artifactsNuGetDir
            WorkingDir = artifactsBuildDir
            Version = "3.3.3-droid"}) 
            androidNuspec
)

Target "Publish" (fun _ -> 
    let path = Path.Combine(artifactsNuGetDir, "*.nupkg")
    PublishArtifact path
)

// Dependencies
"Clean"
  ==> "Build"
  ==> "PackDroid"
  ==> "Publish"

// start build
RunTargetOrDefault "Publish"