#Habanero.Testability v1.3-13_02_2012
require 'rake'
require 'albacore'

#______________________________________________________________________________
#---------------------------------SETTINGS-------------------------------------

# set up the build script folder so we can pull in shared rake scripts.
# This should be the same for most projects, but if your project is a level
# deeper in the repo you will need to add another ..
bs = File.dirname(__FILE__)
bs = File.join(bs, "..") if bs.index("branches") != nil
bs = File.join(bs, "../../../HabaneroCommunity/BuildScripts")
$buildscriptpath = File.expand_path(bs)
$:.unshift($buildscriptpath) unless
    $:.include?(bs) || $:.include?($buildscriptpath)

if (bs.index("branches") == nil)	
	nuget_version = 'Trunk'
	nuget_version_id = '9.9.999'
	
	$nuget_habanero_version	= nuget_version
	$nuget_smooth_version =	nuget_version
	
	$nuget_publish_version = nuget_version
	$nuget_publish_version_id = nuget_version_id
else
	$nuget_habanero_version	= 'v2.6'
	$nuget_smooth_version =	'v1.6-13_02_2012'
	
	$nuget_publish_version = 'v1.3-13_02_2012'
	$nuget_publish_version_id = '1.3'
end	
#------------------------build settings--------------------------
require 'rake-settings.rb'

msbuild_settings = {
  :properties => {:configuration => :release},
  :targets => [:clean, :rebuild],
  :verbosity => :quiet,
  #:use => :net35  ;uncomment to use .net 3.5 - default is 4.0
}

#------------------------dependency settings---------------------
#------------------------project settings------------------------
$solution = "source/Habanero.Testability - 2010.sln"

#______________________________________________________________________________
#---------------------------------TASKS----------------------------------------

desc "Runs the build all task"
task :default => [:build_all_nuget]

desc "Rakes habanero+smooth, builds Testability"
task :build_all_nuget => [:installNugetPackages, :build, :nuget]

desc "Builds Testability, including tests"
task :build => [:clean, :msbuild, :test]

desc "Pushes Testability to Nuget"
task :nuget => [:publishTestabilityNugetPackage, 
				:publishTestabilityHelpersNugetPackage, 
				:publishTestabilityTestersNugetPackage ]
#------------------------build Faces  --------------------

desc "Cleans the bin folder"
task :clean do
	puts cyan("Cleaning bin folder")
	FileUtils.rm_rf 'bin'
end

desc "Builds the solution with msbuild"
msbuild :msbuild do |msb| 
	puts cyan("Building #{$solution} with msbuild")
	msb.update_attributes msbuild_settings
	msb.solution = $solution
end

desc "Runs the tests"
nunit :test do |nunit|
	puts cyan("Running tests")
	nunit.assemblies 'bin\Habanero.Testability.Tests.dll',
					 'bin\Habanero.Testability.Testers.Tests.dll'
end

desc "Install nuget packages"
getnugetpackages :installNugetPackages do |ip|
    ip.package_names = ["Habanero.Base.#{$nuget_habanero_version}",  
						"Habanero.BO.#{$nuget_habanero_version}",  
						"Habanero.Smooth.#{$nuget_smooth_version}"]
end

desc "Publish the Habanero.Testability nuget package"
pushnugetpackages :publishTestabilityNugetPackage do |package|
  package.InputFileWithPath = "bin/Habanero.Testability.dll"
  package.Nugetid = "Habanero.Testability.#{$nuget_publish_version}"
  package.Version = $nuget_publish_version_id
  package.Description = "Testability"
end

desc "Publish the Habanero.Testability.Helpers nuget package"
pushnugetpackages :publishTestabilityHelpersNugetPackage do |package|
  package.InputFileWithPath = "bin/Habanero.Testability.Helpers.dll"
  package.Nugetid = "Habanero.Testability.Helpers.#{$nuget_publish_version}"
  package.Version = $nuget_publish_version_id
  package.Description = "Habanero.Testability.Helpers"
end

desc "Publish the Habanero.Testability.Testers nuget package"
pushnugetpackages :publishTestabilityTestersNugetPackage do |package|
  package.InputFileWithPath = "bin/Habanero.Testability.Testers.dll"
  package.Nugetid = "Habanero.Testability.Testers.#{$nuget_publish_version}"
  package.Version = $nuget_publish_version_id
  package.Description = "Habanero.Testability.Testers"
end