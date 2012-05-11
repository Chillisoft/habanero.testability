#Habanero.Testability
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

#------------------------build settings--------------------------
require 'rake-settings.rb'

msbuild_settings = {
  :properties => {:configuration => :release},
  :targets => [:clean, :rebuild],
  :verbosity => :quiet,
  #:use => :net35  ;uncomment to use .net 3.5 - default is 4.0
}

#------------------------dependency settings---------------------
$habanero_version = 'branches/v2.6_2011-08-24'
require 'rake-habanero.rb'

$smooth_version = 'branches/v1.5_2011-08-24'
require 'rake-smooth.rb'

#------------------------project settings------------------------
$basepath = 'http://delicious:8080/svn/habanero/HabaneroCommunity/Habanero.Testability/branches/v2.5_2011-08-24'
$solution = "source/Habanero.Testability - 2010.sln"

#______________________________________________________________________________
#---------------------------------TASKS----------------------------------------

desc "Runs the build all task"
task :default => [:build_all_nuget]

desc "Rakes habanero+smooth, builds Testability"
task :build_all => [:create_temp, :rake_habanero, :rake_smooth, :build, :delete_temp]

desc "Rakes habanero+smooth, builds Testability"
task :build_all_nuget => [:create_temp, :installNugetPackages, :build, :nuget, :delete_temp]

desc "Builds Testability, including tests"
task :build => [:clean, :updatelib, :msbuild, :test]

desc "Pushes Testability to Nuget"
task :nuget => [:publishTestabilityNugetPackage, :publishTestabilityHelpersNugetPackage, :publishTestabilityTestersNugetPackage ]
#------------------------build Faces  --------------------

desc "Cleans the bin folder"
task :clean do
	puts cyan("Cleaning bin folder")
	FileUtils.rm_rf 'bin'
end

svn :update_lib_from_svn do |s|
	s.parameters "update lib"
end

task :updatelib => :update_lib_from_svn do 
	puts cyan("Updating lib")
	FileUtils.cp Dir.glob('temp/bin/Habanero.Base.dll'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.Base.pdb'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.Base.xml'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.BO.dll'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.BO.pdb'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.BO.xml'), 'lib'
	
	FileUtils.cp Dir.glob('temp/bin/Habanero.Smooth.dll'), 'lib'	
	FileUtils.cp Dir.glob('temp/bin/Habanero.Smooth.pdb'), 'lib'	
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
	nunit.assemblies 'bin\Habanero.Testability.Tests.dll','bin\Habanero.Testability.Testers.Tests.dll'
end

svn :commitlib do |s|
	#puts cyan("Commiting lib")
	#s.parameters "ci lib -m autocheckin"
end

desc "Install nuget packages"
getnugetpackages :installNugetPackages do |ip|
    ip.package_names = ["Habanero.Base.V2.6_2011-08-24",  "Habanero.BO.V2.6_2011-08-24",  "Habanero.Smooth.v1.5_2011-08-24"]
end

desc "Publish the Habanero.Testability nuget package"
pushnugetpackages :publishTestabilityNugetPackage do |package|
  package.InputFileWithPath = "bin/Habanero.Testability.dll"
  package.Nugetid = "Habanero.Testability.v2.5_2011-08-24"
  package.Version = "2.5"
  package.Description = "Testability"
end

desc "Publish the Habanero.Testability.Helpers nuget package"
pushnugetpackages :publishTestabilityHelpersNugetPackage do |package|
  package.InputFileWithPath = "bin/Habanero.Testability.Helpers.dll"
  package.Nugetid = "Habanero.Testability.Helpers.v2.5_2011-08-24"
  package.Version = "2.5"
  package.Description = "Habanero.Testability.Helpers"
end

desc "Publish the Habanero.Testability.Testers nuget package"
pushnugetpackages :publishTestabilityTestersNugetPackage do |package|
  package.InputFileWithPath = "bin/Habanero.Testability.Testers.dll"
  package.Nugetid = "Habanero.Testability.Testers.v2.5_2011-08-24"
  package.Version = "2.5"
  package.Description = "Habanero.Testability.Testers"
end