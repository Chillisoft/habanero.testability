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
bs = File.join(bs, "../HabaneroCommunity/BuildScripts")
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
	$nuget_habanero_version	= 'v2.6-13_02_2012'
	$nuget_smooth_version =	'v1.6-13_02_2012'
	
	$nuget_publish_version = 'v1.3-13_02_2012'
	$nuget_publish_version_id = '1.3'
end	

$binaries_baselocation = "bin"
$nuget_baselocation = "nugetArtifacts"
$app_version ='9.9.9.999'
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
$major_version = ''
$minor_version = ''
$patch_version = ''

#______________________________________________________________________________
#---------------------------------TASKS----------------------------------------

desc "Runs the build all task"
task :default, [:major, :minor, :patch] => [:build_all_nuget]

desc "Rakes habanero+smooth, builds Testability"
task :build_all_nuget, [:major, :minor, :patch]  => [:installNugetPackages, :build, :nuget]

desc "Builds Testability, including tests"
task :build, [:major, :minor, :patch]  => [:clean, :setupversion, :set_assembly_version, :msbuild, :copy_to_nuget, :test]

desc "Pushes Testability to Nuget"
task :nuget => [:publishTestabilityNugetPackage, 
				:publishTestabilityHelpersNugetPackage, 
				:publishTestabilityTestersNugetPackage ]
				
#------------------------Setup Versions---------
desc "Setup Versions"
task :setupversion,:major ,:minor,:patch do |t, args|
	puts cyan("Setup Versions")
	args.with_defaults(:major => "0")
	args.with_defaults(:minor => "0")
	args.with_defaults(:patch => "0000")
	$major_version = "#{args[:major]}"
	$minor_version = "#{args[:minor]}"
	$patch_version = "#{args[:patch]}"
	$app_version = "#{$major_version}.#{$minor_version}.#{$patch_version}.0"
	puts cyan("Assembly Version #{$app_version}")	
end

task :set_assembly_version do
	puts green("Setting Shared AssemblyVersion to: #{$app_version}")
	file_path = "source/Common/AssemblyInfoShared.cs"
	outdata = File.open(file_path).read.gsub(/"9.9.9.999"/, "\"#{$app_version}\"")
	File.open(file_path, 'w') do |out|
		out << outdata
	end	
end
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


def copy_nuget_files_to location
	FileUtils.cp "#{$binaries_baselocation}/Habanero.Testability.dll", location
	FileUtils.cp "#{$binaries_baselocation}/Habanero.Testability.Helpers.dll", location
	FileUtils.cp "#{$binaries_baselocation}/Habanero.Testability.Testers.dll", location
end

task :copy_to_nuget do
	puts cyan("Copying files to the nuget folder")	
	copy_nuget_files_to $nuget_baselocation
end

desc "Install nuget packages"
getnugetpackages :installNugetPackages do |ip|
    ip.package_names = ["Habanero.Base.#{$nuget_habanero_version}",  
						"Habanero.BO.#{$nuget_habanero_version}",  
						"Habanero.Smooth.#{$nuget_smooth_version}",
						"nunit.trunk"]
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